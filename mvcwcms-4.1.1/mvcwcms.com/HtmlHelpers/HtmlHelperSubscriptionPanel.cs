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
    public static class HtmlHelperSubscriptionPanel
    {
        public static IHtmlString GetSubscriptionPanel(this HtmlHelper htmlHelper, FrontEndCmsPage model)
        {
            StringBuilder result = new StringBuilder();

            SubscriptionConfiguration subscriptionConfiguration = new SubscriptionConfigurations().GetSubscriptionConfiguration();
            if (subscriptionConfiguration.IsSubscriptionActive)
            {
                CmsPages cmsPages = new CmsPages();
                result.AppendLine("<ul class=\"nav navbar-nav navbar-right navbar-underline-hover\">");
                if (FrontEndSessions.CurrentSubscription.IsNull())
                {
                    if (subscriptionConfiguration.RegisterPageId.IsNotNull())
                    {
                        CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(subscriptionConfiguration.RegisterPageId, model.LanguageCode);
                        result.AppendLine("    <li><a href=\"" + cmsPageActionlink.Url + "\"><i class=\"fa fa-check-square-o\"></i> " + cmsPageActionlink.Title + "</a></li>");
                    }
                    if (subscriptionConfiguration.SignInPageId.IsNotNull())
                    {
                        CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(subscriptionConfiguration.SignInPageId, model.LanguageCode);
                        result.AppendLine("    <li><a href=\"" + cmsPageActionlink.Url + "\"><i class=\"fa fa-sign-in\"></i> " + cmsPageActionlink.Title + "</a></li>");
                    }
                }
                else
                {
                    result.AppendLine("    <li class=\"dropdown\">");
                    result.AppendLine("        <a href=\"#\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">" + FrontEndSessions.CurrentSubscription.FirstName + " <i class=\"fa fa-user\"></i> <b class=\"caret\"></b></a>");
                    result.AppendLine("        <ul class=\"dropdown-menu\">");
                    if (subscriptionConfiguration.ProfilePageId.IsNotNull())
                    {
                        CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(subscriptionConfiguration.ProfilePageId, model.LanguageCode);
                        result.AppendLine("            <li><a href=\"" + cmsPageActionlink.Url + "\"><i class=\"fa fa-cog\"></i> " + cmsPageActionlink.Title + "</a></li>");
                    }
                    if (subscriptionConfiguration.ChangePasswordPageId.IsNotNull())
                    {
                        CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(subscriptionConfiguration.ChangePasswordPageId, model.LanguageCode);
                        result.AppendLine("            <li><a href=\"" + cmsPageActionlink.Url + "\"><i class=\"fa fa-lock\"></i> " + cmsPageActionlink.Title + "</a></li>");
                    }
                    if (subscriptionConfiguration.SignInPageId.IsNotNull())
                    {
                        CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(subscriptionConfiguration.SignInPageId, model.LanguageCode);
                        result.AppendLine("            <li class=\"divider\"></li>");
                        result.AppendLine("            <li><a href=\"" + cmsPageActionlink.Url + "?a=sign-out\"><i class=\"fa fa-sign-out\"></i> " + MVCwCMS.Resources.Strings.SignOut + "</a></li>");
                    }
                    result.AppendLine("        </ul>");
                    result.AppendLine("    </li>");
                }
                result.AppendLine("</ul>");
            }

            return htmlHelper.Raw(result.ToString());
        }
    }
}