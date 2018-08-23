using HtmlAgilityPack;
using MvcPaging;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Mvc.Html;
using System.Web.Routing;
using System.Web.WebPages;

namespace MVCwCMS.Helpers
{
    public class BootstrapMenuItem
    {
        public enum Targets
        {
            Blank,
            Parent,
            Self,
            Top
        }

        public BootstrapMenuItem(string title, string href, Targets target = Targets.Self, string icon = "")
        {
            Title = title;
            Href = href;
            Target = target;
            Icon = icon;
        }

        public string Title { get; set; }
        public string Href { get; set; }
        public Targets Target { get; set; }
        public string Icon { get; set; }
    }

    public static class BootstrapHelper
    {
        /// <summary>
        /// Returns a dropdown button with the specified menu items.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="menuItems"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapActions<TModel>(this HtmlHelper<TModel> htmlHelper, List<BootstrapMenuItem> menuItems)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"btn-group\">");
            sb.AppendLine("<button type=\"button\" class=\"btn btn-info dropdown-toggle\" data-toggle=\"dropdown\"><i class=\"fa fa-bolt\"></i> " + Resources.Strings.Actions + " <span class=\"caret\"></span></button>");
            if (menuItems.Count > 0)
            {
                sb.AppendLine("<ul class=\"dropdown-menu pull-right\" role=\"menu\">");
                foreach (BootstrapMenuItem mi in menuItems)
                {
                    sb.AppendLine("<li><a href=\"" + mi.Href + "\" target=\"_" + mi.Target.ToString().ToLower() + "\"><i class=\"fa " + mi.Icon + " fa-fw\"></i> " + mi.Title + "</a></li>");
                }
                sb.AppendLine("</ul>");
            }
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns an apply filter button.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapApplyFilter<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new MvcHtmlString("<button type=\"submit\" class=\"btn btn-primary\"><i class=\"fa fa-filter\"></i> " + Resources.Strings.ApplyFilter + "</button>");
        }

        /// <summary>
        /// Writes a form tag to the response
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="actionName"></param>
        /// <param name="controllerName"></param>
        /// <param name="formMethod"></param>
        /// <param name="className"></param>
        /// <param name="isDirtyFormMonitored"></param>
        /// <returns></returns>
        public static MvcForm BootstrapBeginForm<TModel>(this HtmlHelper<TModel> htmlHelper, string actionName = null, string controllerName = null, FormMethod formMethod = FormMethod.Post, string className = "", bool isDirtyFormMonitored = true, bool isAutoCompleteOn = false, bool isBlockUiActive = true)
        {
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            if (isDirtyFormMonitored)
            {
                className += " are-you-sure";
            }
            if (isBlockUiActive)
            {
                className += " block-ui";
            }

            return htmlHelper.BeginForm(actionName, controllerName, formMethod, new { @class = className, @novalidate = "novalidate", @role = "form", @autocomplete = (isAutoCompleteOn ? "on" : "off") });
        }

        /// <summary>
        /// Returns an add button. It will be displayed only if the Add PermissionCode is assigned to the page.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="hrefValue"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapButtonAdd<TModel>(this HtmlHelper<TModel> htmlHelper, string hrefValue)
        {
            AdminPages adminPages = new AdminPages();
            AdminPage adminPage = adminPages.GetPageByCurrentAction();
            if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Add))
            {
                return new MvcHtmlString("<a href=\"" + hrefValue + "\" class=\"btn btn-success\"><i class=\"fa fa-plus\"></i> " + Resources.Strings.AddNewItem + "</a>");
            }
            else
            {
                return new MvcHtmlString("");
            }

        }

        /// <summary>
        /// Returns a delete button suitable for the GridView. It will be displayed only if the Delete PermissionCode is assigned to the page.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="hrefValue"></param>
        /// <param name="promptedValue"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapButtonDelete<TModel>(this HtmlHelper<TModel> htmlHelper, string formActionValue, object idValue, string promptedValue)
        {
            AdminPages adminPages = new AdminPages();
            AdminPage adminPage = adminPages.GetPageByCurrentAction();
            if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Delete))
            {
                return new MvcHtmlString("<button type=\"submit\" data-action=\"" + formActionValue + "\" data-id=\"" + idValue + "\" title=\"" + Resources.Strings.DeleteItem + "\" class=\"btn-a action-delete\" data-action-delete-item=\"" + promptedValue + "\"><i class=\"fa fa-trash-o\"></i></button>");
            }
            else
            {
                return new MvcHtmlString("");
            }
        }

        /// <summary>
        /// Returns an edit button suitable for the GridView. It will be displayed only if the Edit PermissionCode is assigned to the page.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="hrefValue"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapButtonEdit<TModel>(this HtmlHelper<TModel> htmlHelper, string hrefValue)
        {
            AdminPages adminPages = new AdminPages();
            AdminPage adminPage = adminPages.GetPageByCurrentAction();
            if (adminPages.IsPermissionGranted(adminPage.PageId, PermissionCode.Edit))
            {
                return new MvcHtmlString("<a href=\"" + hrefValue + "\" title=\"" + Resources.Strings.EditItem + "\" ><i class=\"fa fa-pencil\"></i></a>");
            }
            else
            {
                return new MvcHtmlString("");
            }

        }

        /// <summary>
        /// Returns a button that onclick will redirect to the specified URL.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="redirectUrl"></param>
        /// <param name="redirectText"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapButtonRedirect<TModel>(this HtmlHelper<TModel> htmlHelper, string redirectUrl, string redirectText)
        {
            bool isModal = HttpContext.Current.Request["IsModal"].ConvertTo<bool>(false, true);

            if (isModal)
            {
                return new MvcHtmlString("<button type=\"button\" class=\"btn btn-default btn-modal-close\"><i class=\"fa fa-sign-out\"></i> " + Resources.Strings.Close + "</button>");
            }
            else
            {
                return new MvcHtmlString("<button type=\"button\" class=\"btn btn-info redirect\" data-redirect-url=\"" + redirectUrl + "\"><i class=\"fa fa-level-up\"></i> " + redirectText + "</button>");
            }
        }

        /// <summary>
        /// Returns a CheckBox element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapCheckBoxFor<TModel>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, bool>> expression, string className = "", string labelTooltip = "", bool isLabelVisible = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            sb.AppendLine("<div class=\"checkbox\">");
            string label = string.Empty;
            if (isLabelVisible)
            {
                if (labelTooltip.IsNotEmptyOrWhiteSpace())
                {
                    label = htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString();
                }
                else
                {
                    label = htmlHelper.LabelFor(expression).ToString();
                }
            }
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.CheckBoxFor(expression, new { @class = className }).ToString() + label);
            sb.AppendLine("</div>");
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a DatePicker element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapDatePickerFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string className = "", string labelTooltip = "")
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (labelTooltip.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            sb.AppendLine("<div class=\"input-group date date-picker\" data-date-format=\"" + globalConfiguration.DateFormat.ToUpper() + "\" >");
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control" + className, @placeholder = placeholder }).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("<span class=\"input-group-addon btn\"><i class=\"fa fa-calendar\"></i></span></div></div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a DateTimePicker element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapDateTimePickerFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string className = "", string labelTooltip = "")
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (labelTooltip.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            sb.AppendLine("<div class=\"input-group date datetime-picker\" data-date-format=\"" + globalConfiguration.DateFormat.ToUpper() + "\" >");
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control" + className, @placeholder = placeholder }).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("<span class=\"input-group-addon btn\"><i class=\"fa fa-calendar\"></i> <i class=\"fa fa-clock-o\"></i></span></div></div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a DropDownList element containing images and that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="optionLabel"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <param name="addNewHref"></param>
        /// <param name="onChangeUpdateField"></param>
        /// <param name="updateUrl"></param>
        /// <param name="updateValue"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="autoFocus"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapDropDownImageListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel = "...", string className = "", string labelTooltip = "", string addNewHref = "", string onChangeUpdateField = "", string updateUrl = "", string extraUpdateUrlParameters = "", string updateValue = "", bool isReadOnly = false, bool autoFocus = false)
        {
            return BootstrapDropDownListFor(htmlHelper, expression, selectList, optionLabel, "SelectFilePathPicker " + className, labelTooltip, addNewHref, onChangeUpdateField, updateUrl, extraUpdateUrlParameters, updateValue, isReadOnly, autoFocus);
        }

        /// <summary>
        /// Returns a DropDownList element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="optionLabel"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <param name="addNewHref"></param>
        /// <param name="onChangeUpdateField"></param>
        /// <param name="updateUrl"></param>
        /// <param name="extraUpdateUrlParameters"></param>
        /// <param name="updateValue"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="autoFocus"></param>
        /// <param name="isLabelVisible"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string optionLabel = "...", string className = "", string labelTooltip = "", string addNewHref = "", string onChangeUpdateField = "", string updateUrl = "", string extraUpdateUrlParameters = "", string updateValue = "", bool isReadOnly = false, bool autoFocus = false, bool isLabelVisible = true)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("<div class=\"form-group\">");

            if (isLabelVisible)
            {
                if (labelTooltip.IsNotEmptyOrWhiteSpace())
                {
                    sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
                }
                else
                {
                    sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
                }
            }

            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            if (autoFocus)
            {
                className += " auto-focus";
            }

            var htmlAttributes = new Dictionary<string, object>();

            if (onChangeUpdateField.IsNotEmptyOrWhiteSpace())
            {
                htmlAttributes.Add("data-on-change-update-field", onChangeUpdateField);
            }

            if (updateUrl.IsNotEmptyOrWhiteSpace())
            {
                htmlAttributes.Add("data-update-url", updateUrl);
            }

            if (extraUpdateUrlParameters.IsNotEmptyOrWhiteSpace())
            {
                htmlAttributes.Add("data-extra-update-url-parameters", extraUpdateUrlParameters);
            }

            if (updateValue.IsNotEmptyOrWhiteSpace())
            {
                htmlAttributes.Add("data-update-value", updateValue);
            }

            if (addNewHref.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine("<div class=\"input-group\">");

                htmlAttributes.Add("class", "selectpicker" + className);
                htmlAttributes.Add("data-style", "btn btn-default form-control");
                htmlAttributes.Add("data-width", "100%");
            }
            else
            {
                htmlAttributes.Add("class", "form-control selectpicker" + className);
            }

            if (isReadOnly)
            {
                htmlAttributes.Add("disabled", "disabled");
                htmlAttributes.Add("id", (expression.Body as MemberExpression).Member.Name + "_disabled");
                sb.AppendLine(htmlHelper.HiddenFor(expression).ToString());
            }

            sb.AppendLine(htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes).ToString());

            if (addNewHref.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine("<span class=\"input-group-btn\"><a href=\"" + addNewHref + "?IsModal=true\" class=\"btn btn-default btn-modal\"><i class=\"fa fa-plus\" ></i></a></span>");
                sb.AppendLine("</div>");
            }

            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString BootstrapExitModal<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new MvcHtmlString("<button type=\"button\" class=\"btn btn-default btn-modal-close\"><i class=\"fa fa-sign-out\"></i> " + Resources.Strings.Close + "</button>");
        }

        /// <summary>
        /// Returns a File selector element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="resourceType"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapFilePathFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string resourceType = "", string className = "", string labelTooltip = "")
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (labelTooltip.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            sb.AppendLine("<div class=\"input-group\">");
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control ckfinder-file-textbox" + className, @placeholder = placeholder, @readonly = "readonly" }).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("<span class=\"input-group-addon btn ckfinder-file-remove\" data-ckfinder-file=\"" + (expression.Body as MemberExpression).Member.Name + "\"><i class=\"fa fa-times\"></i></span>");
            sb.AppendLine("<span class=\"input-group-addon btn ckfinder-file\" data-ckfinder-resourcetype=\"" + resourceType + "\" data-ckfinder-file=\"" + (expression.Body as MemberExpression).Member.Name + "\"><i class=\"fa fa-picture-o\"></i></span>");
            sb.AppendLine("</div></div>");
            sb.AppendLine("<img id=\"ckfinder-img-preview-" + (expression.Body as MemberExpression).Member.Name + "\" class=\"img-responsive hidden\" />");
            sb.AppendLine("<div id=\"ckfinder-swf-preview-" + (expression.Body as MemberExpression).Member.Name + "\" class=\"hidden\"></div>");
            sb.AppendLine("<a id=\"ckfinder-file-preview-" + (expression.Body as MemberExpression).Member.Name + "\" class=\"hidden\" target=\"_blank\"><i class=\"fa fa-external-link fa-3x\"></i></a>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a tick icon if isTicked is true.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="isTicked"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapIsTicked<TModel>(this HtmlHelper<TModel> htmlHelper, bool? isTicked)
        {
            return new MvcHtmlString(isTicked.ConvertTo<bool>(false, true) ? "<i class=\"fa fa-check\"></i>" : "");
        }

        /// <summary>
        /// Returns a File selector element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="resourceType"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapMultiFilePathFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string resourceType = "", string className = "", string labelTooltip = "")
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (labelTooltip.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            sb.AppendLine("<div class=\"input-group\">");
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control tokenfield-multifilepath" + className, @placeholder = placeholder }).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("<span class=\"input-group-addon btn ckfinder-file-multiple\" data-ckfinder-resourcetype=\"" + resourceType + "\" data-ckfinder-file=\"" + (expression.Body as MemberExpression).Member.Name + "\"><i class=\"fa fa-picture-o\"></i></span>");
            sb.AppendLine("</div></div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a ListBox element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <param name="addNewHref"></param>
        /// <param name="isReadOnly"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapListBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string className = "", string labelTooltip = "", string addNewHref = "", bool isReadOnly = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (labelTooltip.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            sb.AppendLine("<br />");
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            if (isReadOnly)
            {
                className += " selectpicker-readonly";
            }
            if (addNewHref.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine("<div class=\"input-group\">");
                sb.AppendLine(htmlHelper.ListBoxFor(expression, selectList, new { @class = "selectpicker" + className, data_style = "btn btn-default form-control", data_width = "100%" }).ToString());
                sb.AppendLine("<span class=\"input-group-btn\"><a href=\"" + addNewHref + "?IsModal=true\" class=\"btn btn-default btn-modal\"><i class=\"fa fa-plus\" ></i></a></span>");
                sb.AppendLine("</div>");
            }
            else
            {
                sb.AppendLine(htmlHelper.ListBoxFor(expression, selectList, new { @class = "form-control selectpicker" + className }).ToString());
            }
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a Password element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapPasswordFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string className = "", string labelTooltip = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (labelTooltip.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.PasswordFor(expression, new { @class = "form-control" + className, @placeholder = placeholder }).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a Radio Button element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="value"></param>
        /// <param name="index"></param>
        /// <param name="isChecked"></param>
        /// <param name="className"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapRadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object value, int index, bool isChecked, string className = "", bool isReadOnly = false, string label = "")
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (isReadOnly)
            {
                className = "radio-button-readonly " + className;
            }
            if (isChecked)
            {
                sb.AppendLine(htmlHelper.RadioButtonFor(expression, value, new { @id = (expression.Body as MemberExpression).Member.Name + "_" + index, @checked = "checked", @class = className }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.RadioButtonFor(expression, value, new { @id = (expression.Body as MemberExpression).Member.Name + "_" + index, @class = className }).ToString());
            }
            if (label.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, label).ToString());
            }
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a reset filter button.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapResetFilter<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new MvcHtmlString("<button type=\"button\" class=\"btn btn-default reset\"><i class=\"fa fa-undo\"></i> " + Resources.Strings.ResetFilter + "</button>");
        }

        /// <summary>
        /// Returns a reset form button.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapResetForm<TModel>(this HtmlHelper<TModel> htmlHelper)
        {
            return new MvcHtmlString("<button type=\"button\" class=\"btn btn-default reset-form\"><i class=\"fa fa-undo\"></i> " + Resources.Strings.ResetForm + "</button>");
        }

        /// <summary>
        /// Returns a submit button.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="className"></param>
        /// <param name="isConfirmActive"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapSubmit<TModel>(this HtmlHelper<TModel> htmlHelper, string className = "btn-success", bool isConfirmActive = false)
        {
            return new MvcHtmlString("<button type=\"submit\" class=\"btn " + className + (isConfirmActive ? " submit-confirm" : "") + "\"><i class=\"fa fa-share fa-rotate-180\"></i> " + Resources.Strings.Submit + "</button>");
        }

        /// <summary>
        /// Returns a TextArea element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="className"></param>
        /// <param name="rows"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapTextAreaFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string className = "", int rows = 2, string labelTooltip = "", bool isReadOnly = false, bool isLabelVisible = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (isLabelVisible)
            {
                if (labelTooltip.IsNotEmptyOrWhiteSpace())
                {
                    sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
                }
                else
                {
                    sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
                }
            }
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            if (isReadOnly)
            {
                sb.AppendLine(htmlHelper.TextAreaFor(expression, new { @class = "form-control" + className, @placeholder = placeholder, @rows = rows, @readonly = "readonly" }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.TextAreaFor(expression, new { @class = "form-control" + className, @placeholder = placeholder, @rows = rows }).ToString());
            }
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a TextBox element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="inputGroupAddon"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapTextBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string className = "", string labelTooltip = "", bool isReadOnly = false, string inputGroupAddon = "", bool autoFocus = false, bool isLabelVisible = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (isLabelVisible)
            {
                if (labelTooltip.IsNotEmptyOrWhiteSpace())
                {
                    sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
                }
                else
                {
                    sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
                }
            }
            if (inputGroupAddon.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine("<div class=\"input-group\"><span class=\"input-group-addon\">" + inputGroupAddon + "</span>");
            }
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            if (autoFocus)
            {
                className += " auto-focus";
            }
            if (isReadOnly)
            {
                sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control" + className, @placeholder = placeholder, @readonly = "readonly" }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control" + className, @placeholder = placeholder }).ToString());
            }
            if (inputGroupAddon.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine("</div>");
            }
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("</div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a TimePicker element that is represented by the specified expression.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="className"></param>
        /// <param name="labelTooltip"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapTimePickerFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string className = "", string labelTooltip = "")
        {
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<div class=\"form-group\">");
            if (labelTooltip.IsNotEmptyOrWhiteSpace())
            {
                sb.AppendLine(htmlHelper.LabelFor(expression, new { @data_toggle = "tooltip", @data_placement = "top", @title = labelTooltip }).ToString());
            }
            else
            {
                sb.AppendLine(htmlHelper.LabelFor(expression).ToString());
            }
            sb.AppendLine("<div class=\"input-group date time-picker\" data-date-format=\"" + globalConfiguration.DateFormat.ToUpper() + "\" >");
            string placeholder = HttpUtility.HtmlDecode(htmlHelper.DisplayNameFor(expression).ToHtmlString());
            if (className.IsNotEmptyOrWhiteSpace())
            {
                className = " " + className;
            }
            sb.AppendLine(htmlHelper.TextBoxFor(expression, new { @class = "form-control" + className, @placeholder = placeholder }).ToString());
            sb.AppendLine(htmlHelper.ValidationMessageFor(expression).ConvertTo<string>("", true));
            sb.AppendLine("<span class=\"input-group-addon btn\"><i class=\"fa fa-clock-o\"></i></span></div></div>");
            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns an unordered list of success messages or/and an unordered list of error messages
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString BootstrapValidationSummary<TModel>(this HtmlHelper<TModel> htmlHelper, bool IsErrorItemMarkerVisible = true, bool IsSuccessItemMarkerVisible = true)
        {
            StringBuilder sb = new StringBuilder();

            //Builds the success list
            List<string> keys = htmlHelper.ViewData.Keys.Where(k => k.StartsWith("SuccessMessage")).ToList();
            if (keys.Count > 0)
            {
                string successListUnstyled = "";
                if (!IsSuccessItemMarkerVisible)
                {
                    successListUnstyled = " class=\"list-unstyled\"";
                }
                sb.AppendLine("<div class=\"validation-summary-success alert alert-success\"><ul" + successListUnstyled + ">");
                foreach (string key in keys)
                {
                    sb.AppendLine("<li>" + htmlHelper.ViewData[key] + "</li>");
                }
                sb.AppendLine("</ul></div>");
            }

            //Builds the error list
            string errorListUnstyled = "";
            if (!IsErrorItemMarkerVisible)
            {
                errorListUnstyled = " child-list-unstyled";
            }
            sb.Append(HttpUtility.HtmlDecode(htmlHelper.ValidationSummary("", new { @class = "alert alert-danger" + errorListUnstyled }).ToString()));

            return new MvcHtmlString(sb.ToString());
        }

        /// <summary>
        /// Returns a Pager object.
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="pageSize">Sets the size of a page.</param>
        /// <param name="currentPage">Sets the current page.</param>
        /// <param name="totalItemCount">Sets the number of items.</param>
        /// <param name="action">Sets an alternative action for the pager that is different from the current action.</param>
        /// <param name="alwaysAddFirstPageNumber">If true it will add the page number for page 1 too.</param>
        /// <param name="displayFirstAndLastPage">If true displays first and last navigation pages.</param>
        /// <param name="displayTemplate">When set, the internal HTML rendering is bypassed and a DisplayTemplate view with the given name is rendered instead. Note that the DisplayTemplate must have a model of type PaginationModel.</param>
        /// <param name="maxNrOfPages">Sets the maximum number of pages to show.</param>
        /// <param name="pageRouteValueKey">Sets the page routeValue key for pagination links.</param>
        /// <param name="routeValues">Adds route value parameters that are added to the page url's.</param>
        /// <param name="setFirstPageText">Sets a custom text for the first page.</param>
        /// <param name="setFirstPageTitle">Sets a custom text for title attribute of the first page link.</param>
        /// <param name="setLastPageText">Sets a custom text for the last page.</param>
        /// <param name="setLastPageTitle">Sets a custom text for title attribute of the last page link.</param>
        /// <param name="setNextPageText">Sets a custom text for the next page.</param>
        /// <param name="setNextPageTitle">Sets a custom text for title attribute of the next page link.</param>
        /// <param name="setPreviousPageText">Sets a custom text for the previous page.</param>
        /// <param name="setPreviousPageTitle">Sets a custom text for title attribute of the previous page link.</param>
        /// <param name="useItemCountAsPageCount">The totalItemCount parameter is (ab)used for the total number of pages instead of the total number of items to facilitate backends that return the total number of pages instead of the total number of items.</param>
        /// <returns></returns>
        public static Pager BootstrapPager(this HtmlHelper htmlHelper,
            int pageSize,
            int currentPage,
            int totalItemCount,
            string action = null,
            bool alwaysAddFirstPageNumber = true,
            bool displayFirstAndLastPage = false,
            string displayTemplate = "MvcPagingBootstrap3",
            int? maxNrOfPages = 3,
            string pageRouteValueKey = "p",
            RouteValueDictionary routeValues = null,
            string setFirstPageText = "«",
            string setFirstPageTitle = null,
            string setLastPageText = "»",
            string setLastPageTitle = null,
            string setNextPageText = "›",
            string setNextPageTitle = null,
            string setPreviousPageText = "‹",
            string setPreviousPageTitle = null,
            bool useItemCountAsPageCount = false)
        {
            Pager result = htmlHelper.Pager(pageSize, currentPage, totalItemCount);

            if (alwaysAddFirstPageNumber)
                result.Options(o => o.AlwaysAddFirstPageNumber());

            if (displayFirstAndLastPage)
                result.Options(o => o.DisplayFirstAndLastPage());

            if (displayTemplate.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.DisplayTemplate(displayTemplate));

            if (maxNrOfPages.IsNotNull())
                result.Options(o => o.MaxNrOfPages((int)maxNrOfPages));

            if (pageRouteValueKey.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.PageRouteValueKey(pageRouteValueKey));

            if (routeValues.IsNotNull())
                result.Options(o => o.RouteValues(routeValues));

            if (setFirstPageText.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetFirstPageText(setFirstPageText));

            if (setFirstPageTitle.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetFirstPageTitle(setFirstPageTitle));

            if (setLastPageText.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetLastPageText(setLastPageText));

            if (setLastPageTitle.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetLastPageTitle(setLastPageTitle));

            if (setNextPageText.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetNextPageText(setNextPageText));

            if (setNextPageTitle.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetNextPageTitle(setNextPageTitle));

            if (setPreviousPageText.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetPreviousPageText(setPreviousPageText));

            if (setPreviousPageTitle.IsNotEmptyOrWhiteSpace())
                result.Options(o => o.SetPreviousPageTitle(setPreviousPageTitle));

            return result;
        }

        public static IPagedList<T> ToBootstrapPagedList<T>(this IEnumerable<T> source, int? pageIndex, int pageSize, int? totalCount = null)
        {
            int totalPages = source.Count() / pageSize;

            int actualPageIndex = pageIndex.HasValue ? pageIndex.Value - 1 : 0;
            if (actualPageIndex < 0)
            {
                actualPageIndex = 0;
            }
            if (actualPageIndex > totalPages)
            {
                actualPageIndex = totalPages;
            }

            return source.ToPagedList<T>(actualPageIndex, pageSize, totalCount);
        }
    }
}