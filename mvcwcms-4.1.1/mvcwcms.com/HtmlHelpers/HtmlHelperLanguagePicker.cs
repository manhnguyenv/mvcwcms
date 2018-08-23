using MVCwCMS;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace MVCwCMS.HtmlHelpers
{
    public static class HtmlHelperLanguagePicker
    {
        public static IHtmlString GetLanguagePicker(this HtmlHelper htmlHelper, FrontEndCmsPage model, string className = "")
        {
            StringBuilder result = new StringBuilder();

            Languages languages = new Languages();
            Language language = languages.GetLanguageByCode(model.LanguageCode);
            GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();
            result.AppendLine("<ul class=\"nav " + className + "\">");
            result.AppendLine("    <li class=\"dropdown\">");
            result.AppendLine("        <a class=\"dropdown-toggle\" data-toggle=\"dropdown\" href=\"#\"><i class=\"fa fa-flag\"></i> " + language.LanguageNameOriginal + " <i class=\"fa fa-caret-down\"></i></a>");
            result.AppendLine("        <ul class=\"dropdown-menu dropdown-user\">");
            foreach (Language l in languages.GetAllLanguages(isActive: true))
            {
                result.AppendLine("            <li><a href=\"/" + (l.LanguageCode == globalConfiguration.DefaultLanguageCode ? "" : l.LanguageCode) + "\">" + l.LanguageNameOriginal + "</a></li>");
            }
            result.AppendLine("        </ul>");
            result.AppendLine("    </li>");
            result.AppendLine("</ul>");

            return htmlHelper.Raw(result.ToString());
        }
    }
}
