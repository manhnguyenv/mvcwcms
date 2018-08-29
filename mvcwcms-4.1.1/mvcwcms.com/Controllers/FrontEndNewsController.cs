using MVCwCMS.Filters;
using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    [ChildActionOnly]
    public class FrontEndNewsController : FrontEndBaseController
    {
        [HttpGet]
        public ActionResult Index(FrontEndCmsPage page, int? p, string c, string d)
        {
            News news = new News();

            FrontEndNews frontEndNews;

            if (page.Parameter.IsEmptyOrWhiteSpace())
            {
                int? filterCategoryId = c.ConvertTo<int?>(null, true);
                string filterNewsDate = d;

                List<SingleNews> newsList = news.GetNews(
                                                page.LanguageCode,
                                                isActive: true,
                                                isCategoryActive: true,
                                                categoryId: filterCategoryId,
                                                newsDate: (filterNewsDate.IsNotEmptyOrWhiteSpace() ? ("1-" + d).ConvertTo<DateTime?>(null, true) : null)
                                            ).OrderByDescending(i => i.NewsDate).ToList();

                if (newsList.IsNotNull())
                {
                    frontEndNews = new FrontEndNews()
                    {
                        LanguageCode = page.LanguageCode,
                        NewsList = newsList.ToBootstrapPagedList(p, 5),
                        FilterCategoryId = filterCategoryId,
                        FilterNewsDate = filterNewsDate,
                        NewsId = null,
                        NewsDate = null,
                        UserName = null,
                        CategoryId = null,
                        CategoryName = null,
                        MainImageFilePath = null,
                        NewsTitle = null,
                        NewsContent = null
                    };
                }
                else
                {
                    frontEndNews = new FrontEndNews();
                }
            }
            else
            {
                if (page.Parameter.ToLower() == "rss")
                {
                    Server.TransferRequest("~/OutputNews/Rss/?langCode=" + page.LanguageCode);
                    return null;
                }
                else if (page.Parameter.ToLower() == "atom10")
                {
                    Server.TransferRequest("~/OutputNews/Atom10/?langCode=" + page.LanguageCode);
                    return null;
                }
                else
                {
                    SingleNews singleNews = news.GetSingleNews(page.Parameter.Split('-').FirstOrDefault().ConvertTo<int?>(null, true), page.LanguageCode);

                    if (singleNews.IsNotNull())
                    {
                        NewsComments newsComments = new NewsComments();

                        frontEndNews = new FrontEndNews()
                        {
                            LanguageCode = page.LanguageCode,
                            NewsList = null,
                            FilterCategoryId = null,
                            FilterNewsDate = null,
                            NewsId = singleNews.NewsId,
                            NewsDate = singleNews.NewsDate,
                            UserName = singleNews.UserName,
                            CategoryId = singleNews.CategoryId,
                            CategoryName = singleNews.CategoryName,
                            MainImageFilePath = singleNews.MainImageFilePath,
                            NewsTitle = singleNews.NewsTitle,
                            NewsContent = singleNews.NewsContent,
                            NewsCommentList = (newsComments.IsNotNull() ? newsComments.GetNewsComments(singleNews.NewsId, true) : null)
                        };
                    }
                    else
                    {
                        frontEndNews = null;
                    }
                }
            }

            return View(frontEndNews);
        }

        [HttpPost]
        [IsFrontEndChildActionRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult Index(FrontEndCmsPage page, FrontEndNews frontEndNews)
        {
            News news = new News();
            if (page.Parameter.IsNotEmptyOrWhiteSpace())
            {
                SingleNews singleNews = news.GetSingleNews(page.Parameter.Split('-').FirstOrDefault().ConvertTo<int?>(null, true), page.LanguageCode);

                if (singleNews.IsNotNull())
                {
                    NewsComments newsComments = new NewsComments();

                    frontEndNews.LanguageCode = page.LanguageCode;
                    frontEndNews.NewsList = null;
                    frontEndNews.FilterCategoryId = null;
                    frontEndNews.FilterNewsDate = null;
                    frontEndNews.NewsId = singleNews.NewsId;
                    frontEndNews.NewsDate = singleNews.NewsDate;
                    frontEndNews.UserName = singleNews.UserName;
                    frontEndNews.CategoryId = singleNews.CategoryId;
                    frontEndNews.CategoryName = singleNews.CategoryName;
                    frontEndNews.MainImageFilePath = singleNews.MainImageFilePath;
                    frontEndNews.NewsTitle = singleNews.NewsTitle;
                    frontEndNews.NewsContent = singleNews.NewsContent;
                    frontEndNews.NewsCommentList = (newsComments.IsNotNull() ? newsComments.GetNewsComments(singleNews.NewsId, true) : null);
                }
                else
                {
                    frontEndNews = null;
                }
            }

            if (ModelState.IsValidOrRefresh() && frontEndNews.IsNotNull())
            {
                NewsConfiguration newsConfiguration = new NewsConfigurations().GetNewsConfiguration();

                NewsComments newsComments = new NewsComments();
                int? result = newsComments.Add((int)frontEndNews.NewsId,
                                               newsConfiguration.IsCommentAutoApproved,
                                               frontEndNews.NewsComment,
                                               DateTime.Now,
                                               FrontEndSessions.CurrentSubscription.Email);
                switch (result)
                {
                    case 0:
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings_News.CommentAdded);
                        ViewData.IsFormVisible(false);

                        GlobalConfiguration globalConfiguration = new GlobalConfigurations().GetGlobalConfiguration();

                        CultureInfoHelper.ForceBackEndLanguage();

                        string subject = Resources.Strings_News.EmailSubject.Replace("{$Url}", globalConfiguration.DomainName.ToUrl());
                        string body = Resources.Strings_News.EmailBody
                                      .Replace("{$Url}", globalConfiguration.DomainName.ToUrl())
                                      .Replace("{$subscriptionEmail}", FrontEndSessions.CurrentSubscription.Email)
                                      .Replace("{$comment}", frontEndNews.NewsComment)
                                      .Replace("{$ipAddress}", Request.UserHostAddress);

                        CultureInfoHelper.RestoreFrontEndLanguage();

                        EmailHelper email = new EmailHelper(globalConfiguration.NotificationEmail, globalConfiguration.NotificationEmail, subject, body);
                        email.Send();
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError + " (Database)");
                        break;
                }
            }

            return View(frontEndNews);
        }

        [HttpGet]
        public ActionResult Summary(FrontEndCmsPage page)
        {
            News news = new News();

            NewsConfiguration newsConfiguration = new NewsConfigurations().GetNewsConfiguration();

            FrontEndNews frontEndNews = new FrontEndNews()
            {
                LanguageCode = page.LanguageCode,
                NewsList = news
                            .GetNews(page.LanguageCode, isActive: true, isCategoryActive: true)
                            .OrderByDescending(i => i.NewsDate)
                            .ToBootstrapPagedList(0, newsConfiguration.NumberOfNewsInSummary),
                FilterCategoryId = null,
                FilterNewsDate = null,
                NewsId = null,
                NewsDate = null,
                UserName = null,
                CategoryId = null,
                CategoryName = null,
                MainImageFilePath = null,
                NewsTitle = null,
                NewsContent = null
            };

            return View(frontEndNews);
        }
    }
}