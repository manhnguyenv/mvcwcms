jQuery(document).ready(function ($) {

    $.storage = new $.store();

    CollapseExpandMenu();
    $(".menu-collapse").click(function () {
        if ($.storage.get("IsMenuCollapsed") === "false") {
            $.storage.set("IsMenuCollapsed", "true");
        } else {
            $.storage.set("IsMenuCollapsed", "false");
        }
        CollapseExpandMenu();
    });
    function CollapseExpandMenu() {
        if ($.storage.get("IsMenuCollapsed") === "false") {
            $(".navbar-static-side").addClass("navbar-collapsed");
            $("#page-wrapper").addClass("page-wrapper-collapsed");
            $("#side-menu").hide();
        } else {
            $(".navbar-static-side").removeClass("navbar-collapsed");
            $("#page-wrapper").removeClass("page-wrapper-collapsed");
            $("#side-menu").show();
        }
    }
    $(".navbar-toggle").click(function () {
        $(".menu-collapse-container").hide();
        $.storage.set("IsMenuCollapsed", "true");
        CollapseExpandMenu();
    });

    $(".confirm-action").click(function () {
        return confirm(toConfirmPressOK);
    });

    $(".auto-select").mousedown(function (e) {
        e.preventDefault();
        this.select();
    });

    $(".footable").footable({
        breakpoints: {
            phone: 768,
            tablet: 1024
        }
    });
    $(".footable-expand-all").click(function () {
        $(".footable").trigger("footable_expand_all");
    });
    $(".footable-collapse-all").click(function () {
        $(".footable").trigger("footable_collapse_all");
    });

    $(".are-you-sure").each(function () {
        $(this).areYouSure({ "message": youHaveUnsavedChanges });
        //$(this).areYouSure( {"message": youHaveUnsavedChanges} ).on("dirty.areYouSure", function () {
        //    $("button[type=submit]").addClass("submit-confirm");
        //});
    });
    if (isRefresh === "true") {
        $(".are-you-sure").each(function () {
            $(this).addClass("dirty").on("clean.areYouSure", function () {
                $(this).addClass("dirty");
            });
        });
    }

    $(".auto-focus").focus();

    $('select[data-on-change-update-field!=""][data-on-change-update-field]').each(function () {
        DataOnChangeUpdateField(this);
    }).change(function () {
        DataOnChangeUpdateField(this);
    });
    function DataOnChangeUpdateField(changedObj) {
        var selectedValue = $(changedObj).val();
        var targetField = $("#" + $(changedObj).data("on-change-update-field"));
        var updateUrl = targetField.data("update-url");
        var extraUpdateUrlParameters = targetField.data("extra-update-url-parameters");
        if (typeof extraUpdateUrlParameters === "undefined") {
            extraUpdateUrlParameters = "";
        } else {
            extraUpdateUrlParameters = "&" + extraUpdateUrlParameters;
        }
        var updateValue = targetField.data("update-value");

        targetField.html($("<option></option>").val("").html("..."));
        targetField.selectpicker("refresh");
        if (selectedValue != "") {
            CreateBlockUI();
            $.ajax({
                cache: false,
                type: "POST",
                url: updateUrl,
                data: "id=" + selectedValue + extraUpdateUrlParameters,
                success: function (data) {
                    $.each(data, function (id, option) {
                        targetField.append($("<option></option>").val(option.id).html(option.name));
                    });
                    targetField.selectpicker("val", updateValue);
                    targetField.selectpicker("refresh");
                    $.unblockUI();
                },
                error: function (xhr, ajaxOptions, thrownError) {
                    alert("Error");
                    $.unblockUI();
                }
            });
        }
    };

    $("form.block-ui").submit(function () {
        if ($(this).valid()) {
            CreateBlockUI();
        }
    });

    $("#side-menu").metisMenu();

    $(window).bind("load resize", function () {
        if ($(this).width() < 768) {
            $('div.sidebar-collapse').addClass('collapse')
        } else {
            $('div.sidebar-collapse').removeClass('collapse')
        }
    });

    $(".selectpicker").selectpicker({
        //selectedTextFormat: "count>5",
        liveSearch: true,
        noneSelectedText: "..."
    });
    $(".selectpicker-readonly").each(function () {
        $(this).next().click(function (event) {
            event.stopPropagation();
        });
    });

    $(".radio-button-readonly").each(function () {
        $(this).click(function (event) {
            return false;
        });
    });

    $("select.SelectFilePathPicker").each(function () {
        var selectTarget = $(this);
        selectTarget.find("option").each(function () {
            if ($(this).val() !== "") {
                var temp = $.string($(this).text().toLowerCase());
                if (temp.endsWith(".swf")) {
                    $(this).data("content", '<div style="position:absolute;width:100%;height:70px;"></div><object type="application/x-shockwave-flash" data="' + $(this).text() + '" width="100%" height="70"><param name="wmode" value="transparent"></object> ID: ' + $(this).val());
                } else if (temp.endsWith(".jpg") || temp.endsWith(".jpeg") || temp.endsWith(".gif") || temp.endsWith(".png")) {
                    $(this).data("content", '<img src="' + $(this).text() + '" height="70" title="' + $(this).text() + '" /> ID: ' + $(this).val());
                } else {
                    $(this).data("content", '<a href="' + $(this).text() + '" title="' + $(this).text() + '">Download</a> ID: ' + $(this).val());
                }
            };
        });
        setTimeout(function () {
            selectTarget.selectpicker("refresh");    
        }, 500);
    });

    $(".btn-modal-close").each(function () {
        var actionUrl = new Url($(this).closest("form").attr("action"));
        actionUrl.query.IsModal = "true";
        $(this).closest("form").attr("action", actionUrl);
    });

    $(".btn-modal-close").on("click", function (e) {
        var actionUrl = new Url(window.parent.$("#refresh-modal-form").attr("action"));
        actionUrl.query.IsRefresh = "true";
        window.parent.$("#refresh-modal-form").attr("action", actionUrl);
        window.parent.$(".are-you-sure").each(function () {
            $(this).areYouSure({ "silent": true });
        });
        window.parent.$("#refresh-modal-form")[0].submit(); //Use [0] to avoid JQuery firing the client validation script

        return false;
    });

    $(".btn-modal").on("click", function (e) {
        //Assign the ID to the current form
        $(this).closest("form").attr("id", "refresh-modal-form");

        var src = $(this).attr("href");
        var modalIframeId = "modal-" + (new Date()).getTime();
        var htmlIframe = '<div class="modal fade" id="' + modalIframeId + '" tabindex="-1" role="dialog" aria-labelledby="" aria-hidden="true"><div class="modal-dialog"><div class="modal-content"><div class="modal-body">' +
                         '<iframe width="100%" height="100%" frameborder="0" scrolling="yes" allowtransparency="true" src="' + src + '"></iframe>' +
                         '</div></div></div></div>';
        $("body").prepend(htmlIframe);

        $("#" + modalIframeId).on("show.bs.modal", function () {
            $(this).find(".modal-dialog").css({
                width: "70%",
                height: "70%",
                "padding": "0"
            });
            $(this).find(".modal-content").css({
                height: "100%",
                "border-radius": "0",
                "padding": "0"
            });
            $(this).find(".modal-body").css({
                width: "auto",
                height: "100%",
                "padding": "0"
            });
        })

        $("#" + modalIframeId).modal({
            show: true,
            keyboard: false,
            backdrop: "static"
        });

        return false;
    });

    $(".digits-mask").inputmask({
        mask: "9",
        repeat: "*",
        greedy: false
    });

    $(".digits-3-mask").inputmask({
        mask: "9",
        repeat: 3,
        greedy: false
    });

    $(".percentage-mask").inputmask("integer", {
        allowPlus: false,
        allowMinus: false,
        min: 0,
        max: 100
    });

    $(".date-mask").inputmask(dateFormat); //If the inputmask fails most probably is because the jquery.inputmask.date.extensions.js file is missing the alias for the current dateFormat specified in _LayoutBackEnd.cshtml

    $(".date-picker").datetimepicker({
        locale: adminLanguageCode,
        format: dateFormat.toUpperCase(),
        useCurrent: false
    });
    $(".time-picker").datetimepicker({
        locale: adminLanguageCode,
        format: timeFormat.replace("tt", "A"),
        useCurrent: false
    });
    $(".datetime-picker").datetimepicker({
        locale: adminLanguageCode,
        format: dateFormat.toUpperCase() + " " + timeFormat.replace("tt", "A"),
        useCurrent: false
    });

    $("#NewSubfolder").click(function () {
        var selectedFolder = ckFinderApi.getSelectedFolder();
        ckFinderApi.openInputDialog("", typeTheNewFolderName + ": ", "", function (value) {
            if (value != "") {
                selectedFolder.createNewFolder(value);
            }
        });
    });

    $("#DeleteFolder").click(function () {
        var selectedFolder = ckFinderApi.getSelectedFolder();
        if (selectedFolder.parent != null) {
            selectedFolder.getFiles(function (files) {
                var hasFiles = (files.length > 0);
                if (selectedFolder.hasChildren || hasFiles) {
                    ckFinderApi.openMsgDialog("", selectedFolderCannotBeDeletedNotEmpty.replace("{$SelectedFolderName}", selectedFolder.name));
                } else {
                    ckFinderApi.openConfirmDialog("", pressOkToDeleteSelectedFolder.replace("{$SelectedFolderName}", selectedFolder.name), function (value) {
                        selectedFolder.remove();
                    });
                }
            }, true);
        } else {
            ckFinderApi.openMsgDialog("", selectedFolderCannotBeDeletedRoot.replace("{$SelectedFolderName}", selectedFolder.name));
        }
    });

    $("#RenameFolder").click(function () {
        var selectedFolder = ckFinderApi.getSelectedFolder();

        if (selectedFolder.parent != null) {
            selectedFolder.getFiles(function (files) {
                var hasFiles = (files.length > 0);
                if (selectedFolder.hasChildren || hasFiles) {
                    ckFinderApi.openMsgDialog("", selectedFolderCannotBeRenamedNotEmpty.replace("{$SelectedFolderName}", selectedFolder.name));
                } else {
                    ckFinderApi.openInputDialog("", typeTheNewFolderName + ": ", selectedFolder.name, function (value) {
                        if (value != "") {
                            selectedFolder.rename(value);
                        }
                    });
                }
            }, true);
        } else {
            ckFinderApi.openMsgDialog("", selectedFolderCannotBeRenamedRoot.replace("{$SelectedFolderName}", selectedFolder.name));
        }
    });

    $("#DeleteFile").click(function () {
        if (IsSessionActive("/Admin/") && IsPageBrowseAuthorized("/Admin/FileManagerIsFileUsed")) {
            var selectedFile = ckFinderApi.getSelectedFile();
            if (selectedFile != null) {
                var fileUrl = selectedFile.getUrl();
                if (!IsFileUsed(fileUrl)) {
                    ckFinderApi.openConfirmDialog("", pressOkToDeleteSelectedFile.replace("{$SelectedFileName}", selectedFile.name), function (value) {
                        selectedFile.remove();
                    });
                } else {
                    ckFinderApi.openMsgDialog("", selectedFileCannotBeDeletedUsed.replace("{$SelectedFileName}", selectedFile.name));
                }
            } else {
                ckFinderApi.openMsgDialog("", noFileSelected);
            }
        }
    });

    $("#RenameFile").click(function () {
        if (IsSessionActive("/Admin/") && IsPageBrowseAuthorized("/Admin/FileManagerIsFileUsed")) {
            var selectedFile = ckFinderApi.getSelectedFile();
            if (selectedFile != null) {
                var fileUrl = selectedFile.getUrl();
                if (!IsFileUsed(fileUrl)) {
                    ckFinderApi.openInputDialog("", typeTheNewFileName + ": ", selectedFile.name, function (value) {
                        if (value != "") {
                            selectedFile.rename(value);
                        }
                    });
                } else {
                    ckFinderApi.openMsgDialog("", selectedFileCannotBeRenamedUsed.replace("{$SelectedFileName}", selectedFile.name));
                }
            } else {
                ckFinderApi.openMsgDialog("", noFileSelected);
            }
        }
    });

    $(".filepath-preview").each(function (index) {
        $(this).attr("id", "filepath-preview-" + index);
        swfobject.embedSWF($(this).attr("title"), "filepath-preview-" + index, "100%", "70", "9.0.0", false, false, { wmode: "transparent" }, false, false);
    });

    $(".ckfinder-file-textbox").each(function () {
        var name = $(this).attr("name");
        if ($(this).val() !== "") {
            var temp = $.string($(this).val().toLowerCase());
            if (temp.endsWith(".swf")) {
                swfobject.embedSWF($(this).val(), "ckfinder-swf-preview-" + name, "100%", "100%", "9.0.0", false, false, { wmode: "transparent" }, false, false);
                $("#ckfinder-swf-preview-" + name).removeClass("hidden");
            } else if (temp.endsWith(".jpg") || temp.endsWith(".jpeg") || temp.endsWith(".gif") || temp.endsWith(".png")) {
                $("#ckfinder-img-preview-" + name).attr("src", $(this).val()).removeClass("hidden");
            } else {
                $("#ckfinder-file-preview-" + name).attr("href", $(this).val()).removeClass("hidden");
            }
        }
    })

    $(".ckfinder-standalone").each(function (index, value) {
        var finder = new CKFinder();
        finder.basePath = "/ckfinder/";
        finder.height = 600;
        finder.language = adminLanguageCode;
        finder.resourceType = adminResourceType;
        finder.selectMultiple = false;
        finder.callback = function (api) {
            api.disableFileContextMenuOption("deleteFile", false);
            api.disableFileContextMenuOption("renameFile", false);
            api.disableFolderContextMenuOption("removeFolder", false);
            api.disableFolderContextMenuOption("renameFolder", false);
        };
        ckFinderApi = finder.appendTo(value);
    });

    //Usage: <button type="button" class="btn ckfinder-file" data-ckfinder-file="MyTextBox">...</button>
    $(".ckfinder-file").on("click", function () {
        var finder = new CKFinder();
        finder.basePath = "/ckfinder/";
        finder.height = 600;
        finder.language = adminLanguageCode;
        finder.resourceType = $(this).attr("data-ckfinder-resourcetype");
        finder.selectActionData = $(this).attr("data-ckfinder-file");
        finder.selectMultiple = false;
        finder.selectActionFunction = function (fileUrl, data, allFiles) {
            $("#ckfinder-swf-preview-" + data["selectActionData"]).addClass("hidden");
            $("#ckfinder-img-preview-" + data["selectActionData"]).addClass("hidden");
            $("#ckfinder-file-preview-" + data["selectActionData"]).addClass("hidden");
            $("#" + data["selectActionData"]).val(fileUrl);
            var temp = $.string(fileUrl.toLowerCase());
            if (temp.endsWith(".swf")) {
                swfobject.embedSWF(fileUrl, "ckfinder-swf-preview-" + data["selectActionData"], "100%", "100%", "9.0.0", false, false, {wmode:"transparent"}, false, false);
                $("#ckfinder-swf-preview-" + data["selectActionData"]).removeClass("hidden");
            } else if (temp.endsWith(".jpg") || temp.endsWith(".jpeg") || temp.endsWith(".gif") || temp.endsWith(".png")) {
                $("#ckfinder-img-preview-" + data["selectActionData"]).attr("src", fileUrl).removeClass("hidden");
            } else {
                $("#ckfinder-file-preview-" + data["selectActionData"]).attr("href", fileUrl).removeClass("hidden");
            }
        };
        finder.callback = function (api) {
            api.disableFileContextMenuOption("deleteFile", false);
            api.disableFileContextMenuOption("renameFile", false);
            api.disableFolderContextMenuOption("removeFolder", false);
            api.disableFolderContextMenuOption("renameFolder", false);
        };
        finder.popup();
    });
    $(".ckfinder-file-remove").on("click", function () {
        var selectActionData = $(this).attr("data-ckfinder-file");
        $("#" + selectActionData).val("");
        $("#ckfinder-swf-preview-" + selectActionData).addClass("hidden");
        $("#ckfinder-img-preview-" + selectActionData).addClass("hidden");
        $("#ckfinder-file-preview-" + selectActionData).addClass("hidden");
    });

    $(".tokenfield-youtubevideo").on('tokenfield:createdtoken', function (e) {
        var tempOriginal = $.string(e.attrs.value);
        var tempLowerCase = $.string(e.attrs.value.toLowerCase());
        var videoId = "";
        if (tempLowerCase.include("youtube.com")) {
            videoId = tempOriginal.toQueryParams().v;
        } else if (tempLowerCase.include("youtu.be")){
            videoId = tempOriginal.gsub("https://youtu.be/", "").str;
        }
        if (videoId !== "") {
            $(e.relatedTarget).tooltip({
                html: true,
                title: '<img src="http:/' + '/img.youtube.com/vi/' + videoId + '/0.jpg" height="70" alt="Preview" title="' + tempOriginal + '" /></a>',
                container: e.relatedTarget,
                placement: "bottom"
            });
        }
    }).tokenfield();

    $(".tokenfield-multifilepath").on('tokenfield:createdtoken', function (e) {
        var tokenTitle;
        var temp = $.string(e.attrs.value.toLowerCase());
        if (temp.endsWith(".swf")) {
            tokenTitle = "<object width='100%' height='100%' data='" + e.attrs.value + "' type='application/x-shockwave-flash'><param name='wmode' value='transparent'></object>";
        } else if (temp.endsWith(".gif") || temp.endsWith(".jpg") || temp.endsWith(".jpeg") || temp.endsWith(".png")) {
            var thumbImage = e.attrs.value;
            if (temp.include("files/")) {
                thumbImage = thumbImage.replace("Files/", "_thumbs/Files/");
            } else if (temp.include("images/")) {
                thumbImage = thumbImage.replace("Images/", "_thumbs/Images/");
            }
            tokenTitle = "<img src='" + thumbImage + "'/>";
        } else {
            tokenTitle = "";
        }
        if (tokenTitle !== "") {
            $(e.relatedTarget).tooltip({
                html: true,
                title: tokenTitle,
                container: e.relatedTarget,
                placement: "bottom"
            });
        }
    }).on('tokenfield:initialize', function (e) {
        $(".tokenfield-multifilepath").each(function () {
            $(this).parent().find(".token-input").hide();
        });
    }).tokenfield();
    
    //Usage: <button type="button" class="btn ckfinder-file-multiple" data-ckfinder-file="MyTextBox">...</button>
    $(".ckfinder-file-multiple").on("click", function () {
        var finder = new CKFinder();
        finder.basePath = "/ckfinder/";
        finder.height = 600;
        finder.language = adminLanguageCode;
        finder.selectActionData = $(this).attr("data-ckfinder-file");
        finder.resourceType = $(this).data("ckfinder-resourcetype");
        finder.selectMultiple = true;
        finder.selectActionFunction = function (fileUrl, data, allFiles) {
            var existingTokens = $("#" + data["selectActionData"]).tokenfield('getTokens');
            var isContained;
            for (var i = 0; i < allFiles.length; i++) {
                isContained = false;
                $.each(existingTokens, function(index, token) {
                    if (token.value === allFiles[i].url) {
                        isContained = true;
                    }
                });
                if (!isContained) {
                    $("#" + data["selectActionData"]).tokenfield("createToken", allFiles[i].url);
                }
            }
        };
        finder.callback = function (api) {
            api.disableFileContextMenuOption("deleteFile", false);
            api.disableFileContextMenuOption("deleteFiles", false);
            api.disableFileContextMenuOption("renameFile", false);
            api.disableFolderContextMenuOption("removeFolder", false);
            api.disableFolderContextMenuOption("renameFolder", false);

            var toolId = api.addTool(helpSelectMultipleFiles);
            api.showTool(toolId);
        };
        finder.popup();
    });

    tinymce.init({
        selector: ".tinymce-editor",
        height: 600,
        image_advtab: true,
        forced_root_block: "",
        valid_elements: "+*[*]",
        content_css: "/bundles/frontend-css", //"/bundles/frontend-css?v=" + (new Date()).getTime(),
        language: adminLanguageCode.replace('-','_'),
        convert_urls: false,
        las_seconds: 15,
        las_keyName: "LocalAutoSave",
        las_callback: function () {
            //var content = this.content; //content saved
            //var time = this.time; //time on save action
        },
        plugins: [
            "advlist autolink lists link image charmap print preview hr anchor pagebreak",
            "searchreplace wordcount visualblocks visualchars code fullscreen",
            "insertdatetime media nonbreaking save table contextmenu directionality",
            "emoticons template paste textcolor localautosave codemirror cmsmodules template"
        ],
        codemirror: {
            indentOnInit: true,
            path: "CodeMirror",
            config: {
                lineNumbers: true,
                tabSize: 4,
                indentWithTabs: false,
                indentUnit: 4,
                lineWrapping: false
            }
        },
        setup: function(ed){
            ed.on("blur", function () {
                $("#" + ed.id).val(tinyMCE.activeEditor.getContent());
            });
        },
        toolbar1: "localautosave | bold italic | alignleft aligncenter alignright alignjustify | bullist numlist outdent indent | link image media | cmsmodules | template | fullscreen",
        cmsmodules_url: "/Admin/GetCmsModules/",
        templates: "/Admin/GetContentTemplates/",
        file_browser_callback: function (field_name, url, type, win) {
            var resourceType;
            if (adminResourceType === "") {
                switch (type) {
                    case "image":
                        resourceType = "Images";
                        break;
                    case "media":
                        resourceType = "Videos";
                        break;
                    default:
                        resourceType = "Files";
                        break;
                }
            } else {
                resourceType = adminResourceType;
            }
            tinymce.activeEditor.windowManager.open({
                title: "File manager",
                url: "/ckfinder/ckfinder.html?type=" + resourceType + "&action=js&func=SetTinyMceFileUrl&langCode=" + adminLanguageCode,
                width: 950,
                height: 600
            }, {
                oninsert: function (url) {
                    var fieldElm = win.document.getElementById(field_name);

                    //Update the field value
                    fieldElm.value = url;

                    //Fire the onchange event to auto fill width and height
                    if ("createEvent" in document) {
                        var evt = document.createEvent("HTMLEvents");
                        evt.initEvent("change", false, true);
                        fieldElm.dispatchEvent(evt);
                    } else {
                        fieldElm.fireEvent("onchange");
                    }

                    //Close the popup
                    tinymce.activeEditor.windowManager.close();
                }
            });
        }
    });

    $(".set-segment").click(function () {
        var segmentFieldValue = $("#PageName").val().toLowerCase().replace(/ /g, "-");
        if ($("#Segment").val().trim() != "") {
            if (confirm(segmentNotEmptyPressOkToOverwrite)) {
                $("#Segment").val(segmentFieldValue);
            }
        } else {
            $("#Segment").val(segmentFieldValue);
        }
    });

    if ($(".OnChangePageTemplateId").length) {
        function onChangePageTemplateId() {
            if ($(".OnChangePageTemplateId").val() == "") {
                $("#Segment").attr("readonly", "readonly");
                $("#Segment").val("");
                $("#SegmentBtn").attr("disabled", "disabled");
                $("#Url").removeAttr("readonly");
            } else {
                $("#Url").attr("readonly", "readonly");
                $("#Url").val("");
                $("#Segment").removeAttr("readonly");
                $("#SegmentBtn").removeAttr("disabled");
            }
        }
        onChangePageTemplateId();
        $(".OnChangePageTemplateId").change(function () {
            onChangePageTemplateId();
        });
    }

    $(".CMSPagesTree").treeview({
        animated: "fast",
        collapsed: true,
        control: ".CMSPagesTreeControl",
        persist: "cookie",
        cookieId: "CMSPagesTree" //Unique name
    })

    $(".AdminPagesTree").treeview({
        animated: "fast",
        collapsed: true,
        control: ".AdminPagesTreeControl",
        persist: "cookie",
        cookieId: "AdminPagesTree" //Unique name
    })

    $(".page-jump").click(function (e) {
        window.location.href = $(this).attr("data-page-jump-url");
        return false;
    });

    $(".insert-at-cursor-onclick").click(function () {
        var textArea = $("#" + $(this).data("insert-at-cursor-textarea-id"));
        var insertedValue = $(this).data("insert-at-cursor-value");
        textArea.insertAtCaret(insertedValue);
        $(this).val("");
    });

    $(".insert-at-cursor-onchange").change(function () {
        var textArea = $("#" + $(this).data("insert-at-cursor-textarea-id"));
        textArea.insertAtCaret($(this).val());
        $(this).val("");
    });

    //Attaches a tooltip message to the invalid fields. It requires a CSS setting for .field-validation-error
    $(".form-group").on("focus", ".input-validation-error", function (event) {
        var errElement = $(this).next().find(":last-child");
        $(this).tooltip("destroy");
        $(this).tooltip({
            title: errElement.html(),
            container: "body"
        });
    });

    $('label[data-toggle="tooltip"]').each(function (index, value) {
        $(value).html('<span data-toggle="tooltip" title="' + $(value).attr("title") + '">' + $(value).text() + '</span>');
        $(value).removeAttr("title");
        $(value).removeAttr("data-toggle");
    });
    $('img[data-toggle="tooltip"],span[data-toggle="tooltip"],a[data-toggle="tooltip"],i[data-toggle="tooltip"],button[data-toggle="tooltip"],input[data-toggle="tooltip"]').tooltip({
        container: "body",
        html: true
    });

    $(".reset").click(function () {
        window.location.href = window.location.href.split('?')[0] + "?reset=true";
        return false;
    });

    $(".action-delete").click(function () {
        if (confirm(confirmDeleteItem.replace("{$ItemName}", $(this).data("action-delete-item")))) {
            $(this).closest("form")
                   .attr("action", $(this).data("action"))
                   .prepend('<input type="hidden" name="deleteId" value="' + $(this).data("id") + '" />');
            return true;
        } else {
            return false;
        }
    });

    $(".action-post-id").click(function () {
        $(this).closest("form")
                .attr("action", $(this).data("action"))
                .prepend('<input type="hidden" name="postId" value="' + $(this).data("id") + '" />');
        return true;
    });

    $(".action-post").click(function () {
        $(this).closest("form")
                .attr("action", $(this).data("action"));
        return true;
    });

    $(".action-post-confirm").click(function () {
        if (confirm(toConfirmSubmitPressOK)) {
            $(this).closest("form")
                    .attr("action", $(this).data("action"));
            return true;
        } else {
            return false;
        }
    });

    $(".reset-form").click(function () {
        window.location.href = window.location.href;
        return false;
    });

    $(".redirect").click(function () {
        window.location.href = $(this).attr("data-redirect-url");
        return false;
    });

    $("form").on("click", ".submit-confirm", function () {
        if (confirm(toConfirmSubmitPressOK)) {
            $(this).closest("form").submit();
        }
        return false;
    });

    if ($("form").length && typeof $("form").data("validator") !== "undefined") {
        $("form").data("validator").settings.showErrors = function (map, errors) {
            this.defaultShowErrors();
            if (errors.length) {
                $(".validation-summary-errors").removeClass("alert-success").addClass("alert-danger");
                $(".validation-summary-success").hide();
            }
        }
    }

    $("select.toggle-sql-authentication-type").change(function () {
        ToggleSqlAuthenticationType(this);
    });
    $("select.toggle-sql-authentication-type").each(function () {
        ToggleSqlAuthenticationType(this);
    });
    function ToggleSqlAuthenticationType(trigger) {
        var selectedValue = $(trigger).val();
        if (selectedValue === "IntegratedWindowsAuthentication") {
            //IntegratedWindowsAuthentication
            $("#CurrentWindowsUser").prop("disabled", false).closest(".form-group").show();
            $("#SqlUsername").prop("disabled", true).closest(".form-group").hide();
            $("#SqlPassword").prop("disabled", true).closest(".form-group").hide();
        } else {
            //SqlServerAccount
            $("#CurrentWindowsUser").prop("disabled", true).closest(".form-group").hide();
            $("#SqlUsername").prop("disabled", false).closest(".form-group").show();
            $("#SqlPassword").prop("disabled", false).closest(".form-group").show();
        }
    };
    
    $("select.toggle-admin-language-code").change(function () {
        $("#IsChangeAdminLanguageCode").val("true");
        $(this).closest("form")[0].submit();
    });

    $(".toggle-ignore-db-exists-warning").change(function () {
        if (!$(this).prop("checked")) {
            $(".toggle-reset-db-if-does-exists").prop("checked", false);
        }
    });

    $(".toggle-reset-db-if-does-exists").change(function () {
        if ($(this).prop("checked")) {
            $(".toggle-ignore-db-exists-warning").prop("checked", true);
        }
    });
});

$.validator.setDefaults({
    ignore: [] //Enables validation for hidden fields - http://stackoverflow.com/questions/8466643/jquery-validate-enable-validation-for-hidden-fields
});

//This function is invoked by SWFObject once the <object> has been created
var callback = function (e){
    //Only execute if SWFObject embed was successful
    if (!e.success || !e.ref) {
        return false;
    };
    function swfLoadEvent(fn) {
        //Ensure fn is a valid function
        if (typeof fn !== "function") { return false; }
        //This timeout ensures we don't try to access PercentLoaded too soon
        var initialTimeout = setTimeout(function () {
            //Ensure Flash Player's PercentLoaded method is available and returns a value
            if (typeof e.ref.PercentLoaded !== "undefined" && e.ref.PercentLoaded()) {
                //Set up a timer to periodically check value of PercentLoaded
                var loadCheckInterval = setInterval(function () {
                    //Once value == 100 (fully loaded) we can do whatever we want
                    if (e.ref.PercentLoaded() === 100) {
                        //Execute function
                        fn();
                        //Clear timer
                        clearInterval(loadCheckInterval);
                    }
                }, 1500);
            }
        }, 200);
    };
    swfLoadEvent(function(){
        //Put your code here
        alert("The SWF has finished loading!");
    });
};

function CreateBlockUI() {
    var blockUIMessage = '<img src="/content/backend/images/loader.gif" width="126" height="22" />';
    $.blockUI({
        message: blockUIMessage,
        css: { 
            top:  ($(window).height() - 126) /2 + 'px', 
            left: ($(window).width() - 126) /2 + 'px', 
            width: '126px' 
        }
    });
}

var ckFinderApi;

function SetTinyMceFileUrl(fileUrl, data, allFiles) {
    top.tinymce.activeEditor.windowManager.getParams().oninsert(fileUrl);
}

AddAntiForgeryTokenByFormField = function (data) {
    data.__RequestVerificationToken = $('#__AjaxAntiForgeryForm input[name=__RequestVerificationToken]').val();
    return data;
};

AddAntiForgeryTokenByField = function (data) {
    data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
    return data;
};

function IsSessionActive(redirectToPath) {
    var isSessionActive = false;
    var AntiForgeryTokenData;
    $.ajax({
        url: "/Admin/IsSessionActive/",
        type: "POST",
        data: AddAntiForgeryTokenByFormField({ AntiForgeryTokenResult: AntiForgeryTokenData }),
        cache: false,
        async: false,
        success: function (result) {
            isSessionActive = (result.toString().toLowerCase() === "true");
        }
    });
    if (!isSessionActive) {
        alert(sessionExpiredLoginAgain);
        window.location.href = redirectToPath;
    }
    return isSessionActive;
}

function IsPageBrowseAuthorized(pageUrl) {
    var isPageUrlAuthorized = false;
    var AntiForgeryTokenData;
    var pageAction = pageUrl.split("/")[2];
    $.ajax({
        url: "/Admin/IsPageBrowseAuthorized/" + pageAction + "/",
        type: "POST",
        data: AddAntiForgeryTokenByFormField({ AntiForgeryTokenResult: AntiForgeryTokenData }),
        cache: false,
        async: false,
        success: function (result) {
            isPageUrlAuthorized = (result.toString().toLowerCase() === "true");
        }
    });
    if (!isPageUrlAuthorized) {
        alert(notAuthorizedToBrowsePage + ": " + pageAction);
    }
    return isPageUrlAuthorized;
}

function IsFileUsed(fileUrl) {
    var isFileUsed = false;
    var AntiForgeryTokenData;
    $.ajax({
        url: "/Admin/FileManagerIsFileUsed?f=" + encodeURI(fileUrl),
        type: "POST",
        data: AddAntiForgeryTokenByFormField({ AntiForgeryTokenResult: AntiForgeryTokenData }),
        cache: false,
        async: false,
        success: function (result) {
            isFileUsed = (result.toString().toLowerCase() === "true");
        }
    });
    return isFileUsed;
}