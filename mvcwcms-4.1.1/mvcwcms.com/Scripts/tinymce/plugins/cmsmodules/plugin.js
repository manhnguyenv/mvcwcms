/**
 * cmsmodules plugin
 *
 * Author: Valerio Gentile - valgen@gmail.com
 * Released under LGPL License.
 *
 * Required setting parameter: cmsmodules_url - Represents the url of a server side page that returns a JSON list of Text/Value items
 *
 * To do: Add multilingual support
 *
 */

/**
 * Plugin that adds a dropdown select box to insert a cms module.
 */
tinymce.PluginManager.add('cmsmodules', function (editor, url) {

    // Adds a button that opens a window
    editor.addButton('cmsmodules', {
        text: 'M',
        tooltip: 'Insert CMS module',
        icon: false,
        onclick: function () {
            BuildCmsModulePlugin();
        }
    });

    // Adds a menu item to the tools menu
    editor.addMenuItem('cmsmodules', {
        text: 'Insert CMS module',
        context: 'insert',
        onclick: function () {
            BuildCmsModulePlugin();
        }
    });

    function BuildCmsModulePlugin() {
        var win
        // Open window
        win = editor.windowManager.open({
            title: 'CMS Modules',
            width: 600,
            height: 140,
            body: [
                {
                    type: 'ListBox',
                    name: 'cmsModuleValue',
                    values: buildCmsModuleList(),
                    onselect: onSelectPrimary
                }, {
                    type: 'ListBox',
                    name: 'cmsModuleValues',
                    values: [{ text: 'Choose...', value: '' }]
                }
            ],
            onsubmit: function (e) {
                // Insert content when the window form is submitted
                if (e.data.cmsModuleValues != null)
                    editor.insertContent(e.data.cmsModuleValues);
                else
                    editor.insertContent(e.data.cmsModuleValue);
            }
        });
        win.find('#cmsModuleValues')[0].hide();

        function buildCmsModuleList() {
            var cmsModuleListItems = [{ text: 'Choose...', value: '' }];

            if (!editor.settings.cmsmodules_url || editor.settings.cmsmodules_url.length === 0) {
                alert('No cmsmodules_url defined');
            } else {
                tinymce.util.XHR.send({
                    url: editor.settings.cmsmodules_url + "?" + new Date().getTime(),
                    success: function (text) {

                        var currentGrouping = "";
                        tinymce.each(tinymce.util.JSON.parse(text), function (cmsmodule) {

                            if (cmsmodule.Text.indexOf("->") > 0) {

                                var grouping = cmsmodule.Text.substring(0, cmsmodule.Text.indexOf("->"));
                                if (currentGrouping != grouping) {
                                    cmsModuleListItems.push({
                                        text: grouping,
                                        value: grouping
                                    });
                                    currentGrouping = grouping;
                                }
                            } else {
                                cmsModuleListItems.push({
                                    text: cmsmodule.Text,
                                    value: cmsmodule.Value
                                });
                            }

                        });
                    },
                    error: function (type, req, o) {
                        alert('Something went wrong in TinyMCE cmsmodules plugin.\nError type: ' + type + '\nError status: ' + req.status);
                    }
                });
            }

            return cmsModuleListItems;
        }

        function onSelectPrimary(e) {
            var selectedvalue = e.control.value();
            var valbox = win.find('#cmsModuleValues')[0];
            valbox.value(null);
            valbox.menu = null;
            valbox.hide();

            tinymce.util.XHR.send({
                url: editor.settings.cmsmodules_url + "?" + new Date().getTime(),
                success: function (text) {
                    var cmsModuleListItems = [];
                    var currentGrouping = "";
                    var visible = false;
                    tinymce.each(tinymce.util.JSON.parse(text), function (cmsmodule) {
                        if (cmsmodule.Text.indexOf("->") > 0) {
                            var grouping = cmsmodule.Text.substring(0, cmsmodule.Text.indexOf("->"));
                            if (selectedvalue == grouping) {
                                visible = true;
                                cmsModuleListItems.push({
                                    text: cmsmodule.Text.substr(cmsmodule.Text.indexOf("->") + 3),
                                    value: cmsmodule.Value
                                });
                            }
                        };

                    });
                    if (visible) {
                        var valbox = win.find('#cmsModuleValues')[0];
                        valbox.value('');
                        valbox.menu = null;
                        valbox.state.data.menu = valbox.settings.menu = cmsModuleListItems;
                        valbox.show();
                    }

                },
                error: function (type, req, o) {
                    alert('Something went wrong in TinyMCE cmsmodules plugin.\nError type: ' + type + '\nError status: ' + req.status);
                }
            });

        }
    }

});