using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Threading;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public class OutputNewsController : Controller
    {
        // GET: Atom10
        public ActionResult Atom10(string langCode)
        {
            return new Atom10Result() { Feed = GetSyndicationFeed(langCode) };
        }

        // GET: Rss
        public ActionResult Rss(string langCode)
        {
            return new RssResult() { Feed = GetSyndicationFeed(langCode) };
        }

        private SyndicationFeed GetSyndicationFeed(string langCode)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langCode);

            SyndicationFeed result = new SyndicationFeed();

            NewsConfiguration newsConfiguration = new NewsConfigurations().GetNewsConfiguration();
            if (newsConfiguration.IsNewsActive && newsConfiguration.NewsPageId.IsNotNull())
            {
                CmsPages cmsPages = new CmsPages();
                CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(newsConfiguration.NewsPageId, langCode);

                News news = new News();

                List<SingleNews> newsList = news.GetNews(
                                                    langCode,
                                                    isActive: true,
                                                    isCategoryActive: true
                                                ).OrderByDescending(i => i.NewsDate).Take(10).ToList();

                if (newsList.IsNotNull())
                {
                    GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

                    result = new SyndicationFeed(
                        Resources.Strings_News.NewsFeedTitle.Replace("{$SiteName}", globalConfiguration.SiteName),
                        Resources.Strings_News.NewsFeedDescription.Replace("{$SiteName}", globalConfiguration.SiteName),
                        Request.Url,
                        Guid.NewGuid().ToString(),
                        DateTime.Now);

                    List<SyndicationItem> items = new List<SyndicationItem>();

                    SyndicationItem item;
                    foreach (SingleNews singleNews in newsList)
                    {
                        item = new SyndicationItem(
                            singleNews.NewsTitle,
                            singleNews.NewsContent.StripHtml().TrimToMaxLength(255, "..."),
                            new Uri(Request.Url.Scheme + "://" + Request.Url.Authority + cmsPageActionlink.Url + singleNews.NewsId + "-" + singleNews.NewsTitle.ToFriendlyUrlDashedString() + "/"),
                            singleNews.NewsId.ToString(),
                            singleNews.NewsDate);
                        items.Add(item);
                    }

                    result.Items = items;
                }
            }

            return result;
        }
    }
}