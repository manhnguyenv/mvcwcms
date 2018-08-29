using MVCwCMS.Filters;
using MVCwCMS.Helpers;
using MVCwCMS.Models;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        //  /Admin/News/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult News(BackEndNewsList backEndNewsList)
        {
            News news = new News();
            backEndNewsList.NewsList = news.GetNews(ConfigurationManager.AppSettings["AdminLanguageCode"],
                                                    newsTitle: backEndNewsList.NewsTitle,
                                                    isActive: backEndNewsList.IsActive,
                                                    categoryId: backEndNewsList.CategoryId,
                                                    newsDateFrom: backEndNewsList.NewsDateFrom,
                                                    newsDateTo: backEndNewsList.NewsDateTo);
            if (backEndNewsList.NewsList.IsNull() || backEndNewsList.NewsList.Count == 0)
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
            }
            else
            {
                NewsConfiguration newsConfiguration = new NewsConfigurations().GetNewsConfiguration();
                if (newsConfiguration.IsNewsActive && newsConfiguration.NewsPageId.IsNotNull())
                {
                    CmsPages cmsPages = new CmsPages();
                    CmsPageActionlink cmsPageActionlink = cmsPages.GetCmsPageActionlink(newsConfiguration.NewsPageId, ConfigurationManager.AppSettings["AdminLanguageCode"]);
                    backEndNewsList.FrontEndUrl = cmsPageActionlink.Url;
                }
            }

            return View(backEndNewsList);
        }

        // /Admin/NewsAddEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult NewsAddEdit(int? id)
        {
            BackEndNewsAddEdit backEndNewsAddEdit = new BackEndNewsAddEdit();

            News news = new News();
            SingleNews singleNews;

            BackEndNewsLanguagesAddEdit backEndNewsLanguagesAddEdit;

            List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
            if (allActiveLanguages.IsNotNull() && allActiveLanguages.Count > 0)
            {
                if (id.IsNotNull())
                {
                    List<SingleNews> backEndNewsList = news.GetNews((int)id);
                    if (backEndNewsList.IsNotNull() && backEndNewsList.Count > 0)
                    {
                        backEndNewsAddEdit.NewsId = id;
                        backEndNewsAddEdit.NewsDate = backEndNewsList[0].NewsDate.ToDateTimeString();
                        backEndNewsAddEdit.IsActive = backEndNewsList[0].IsActive;
                        backEndNewsAddEdit.CategoryId = backEndNewsList[0].CategoryId;
                        backEndNewsAddEdit.MainImageFilePath = backEndNewsList[0].MainImageFilePath;

                        foreach (Language language in allActiveLanguages)
                        {
                            backEndNewsLanguagesAddEdit = new BackEndNewsLanguagesAddEdit();
                            backEndNewsLanguagesAddEdit.LanguageCode = language.LanguageCode;
                            backEndNewsLanguagesAddEdit.LanguageName = language.LanguageName;

                            singleNews = news.GetSingleNews(id, language.LanguageCode);
                            if (singleNews.IsNotNull())
                            {
                                backEndNewsLanguagesAddEdit.NewsTitle = singleNews.NewsTitle;
                                backEndNewsLanguagesAddEdit.NewsContent = singleNews.NewsContent;
                            }

                            backEndNewsAddEdit.NewsLanguages.Add(backEndNewsLanguagesAddEdit);
                        }
                    }
                    else
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                        ViewData.IsFormVisible(false);
                    }
                }
                else
                {
                    foreach (Language language in allActiveLanguages)
                    {
                        backEndNewsLanguagesAddEdit = new BackEndNewsLanguagesAddEdit();
                        backEndNewsLanguagesAddEdit.LanguageCode = language.LanguageCode;
                        backEndNewsLanguagesAddEdit.LanguageName = language.LanguageName;

                        backEndNewsAddEdit.NewsLanguages.Add(backEndNewsLanguagesAddEdit);
                    }
                }
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
            }

            return View(backEndNewsAddEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult NewsAddEdit(BackEndNewsAddEdit backEndNewsAddEdit)
        {
            if (ModelState.IsValidOrRefresh())
            {
                News news = new News();
                int? result;
                bool isLoopSuccessful = true;
                int? lastInsertedId = null;
                int? currentId = backEndNewsAddEdit.NewsId;
                foreach (BackEndNewsLanguagesAddEdit backEndNewsLanguagesAddEdit in backEndNewsAddEdit.NewsLanguages)
                {
                    if (currentId.IsNull())
                    {
                        currentId = lastInsertedId;
                    }
                    result = news.AddEdit(currentId,
                                          backEndNewsAddEdit.NewsDate.ToDateTime(),
                                          BackEndSessions.CurrentUser.UserName,
                                          backEndNewsAddEdit.IsActive,
                                          backEndNewsAddEdit.CategoryId,
                                          backEndNewsAddEdit.MainImageFilePath,
                                          backEndNewsLanguagesAddEdit.LanguageCode,
                                          backEndNewsLanguagesAddEdit.NewsTitle,
                                          backEndNewsLanguagesAddEdit.NewsContent,
                                          out lastInsertedId);
                    switch (result)
                    {
                        case 0:
                            //success
                            break;

                        case 2:
                            isLoopSuccessful = false;
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                            ViewData.IsFormVisible(false);
                            break;

                        default:
                            isLoopSuccessful = false;
                            ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                            break;
                    }
                    if (!isLoopSuccessful)
                        break;
                }
                if (isLoopSuccessful)
                {
                    if (backEndNewsAddEdit.NewsId.IsNull())
                    {
                        ModelState.Clear();
                        backEndNewsAddEdit = new BackEndNewsAddEdit();
                        BackEndNewsLanguagesAddEdit backEndNewsLanguagesAddEdit;
                        List<Language> allActiveLanguages = new Languages().GetAllLanguages(isActive: true);
                        foreach (Language language in allActiveLanguages)
                        {
                            backEndNewsLanguagesAddEdit = new BackEndNewsLanguagesAddEdit();
                            backEndNewsLanguagesAddEdit.LanguageCode = language.LanguageCode;
                            backEndNewsLanguagesAddEdit.LanguageName = language.LanguageName;

                            backEndNewsAddEdit.NewsLanguages.Add(backEndNewsLanguagesAddEdit);
                        }

                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyAdded);
                    }
                    else
                    {
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    }
                }
            }

            return View(backEndNewsAddEdit);
        }

        //  /Admin/NewsDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult NewsDelete(int deleteId)
        {
            News news = new News();
            switch (news.Delete(deleteId))
            {
                case 0:
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyDeleted);
                    break;

                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    break;

                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemUsedSomewhereElse);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("News");
        }

        //  /Admin/NewsConfiguration/
        [HttpGet]
        [IsRestricted]
        public ActionResult NewsConfiguration()
        {
            BackEndNewsConfigurationEdit backEndNewsConfigurationEdit = new BackEndNewsConfigurationEdit();

            NewsConfiguration newsConfiguration = new NewsConfigurations().GetNewsConfiguration();
            if (newsConfiguration.IsNotNull())
            {
                backEndNewsConfigurationEdit.IsNewsActive = newsConfiguration.IsNewsActive;
                backEndNewsConfigurationEdit.NewsPageId = newsConfiguration.NewsPageId;
                backEndNewsConfigurationEdit.NumberOfNewsInSummary = newsConfiguration.NumberOfNewsInSummary;
                backEndNewsConfigurationEdit.IsCommentAutoApproved = newsConfiguration.IsCommentAutoApproved;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndNewsConfigurationEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult NewsConfiguration(BackEndNewsConfigurationEdit backEndNewsConfigurationEdit)
        {
            NewsConfigurations newsConfigurations = new NewsConfigurations();
            int? result = newsConfigurations.Edit(backEndNewsConfigurationEdit.IsNewsActive,
                                                          backEndNewsConfigurationEdit.NewsPageId,
                                                          backEndNewsConfigurationEdit.NumberOfNewsInSummary,
                                                          backEndNewsConfigurationEdit.IsCommentAutoApproved);
            switch (result)
            {
                case 0:

                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                    break;

                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    ViewData.IsFormVisible(false);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return View(backEndNewsConfigurationEdit);
        }

        //  /Admin/NewsComments/
        [HttpGet]
        [IsRestricted]
        [PersistQuerystring]
        [ImportModelStateFromTempData]
        public ActionResult NewsComments(BackEndNewsCommentsList backEndNewsCommentsList, int id)
        {
            backEndNewsCommentsList.NewsId = id;

            SingleNews singleNews = new News().GetSingleNews(id, ConfigurationManager.AppSettings["AdminLanguageCode"]);
            if (singleNews.IsNotNull())
            {
                backEndNewsCommentsList.NewsTitle = singleNews.NewsTitle;

                NewsComments newsComments = new NewsComments();
                backEndNewsCommentsList.NewsCommentList = newsComments.GetNewsComments(id, backEndNewsCommentsList.IsActive);
                if (backEndNewsCommentsList.NewsCommentList.IsNull() || backEndNewsCommentsList.NewsCommentList.Count == 0)
                {
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.NoDataFound);
                }
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndNewsCommentsList);
        }

        //  /Admin/NewsCommentsEdit/
        [HttpGet]
        [IsRestricted]
        public ActionResult NewsCommentsEdit(int id)
        {
            BackEndNewsCommentsEdit backEndNewsCommentsEdit = new BackEndNewsCommentsEdit();

            NewsComments newsComments = new NewsComments();
            NewsComment newsComment = newsComments.GetNewsComment(id);
            SingleNews singleNews = new News().GetSingleNews(newsComment.NewsId, ConfigurationManager.AppSettings["AdminLanguageCode"]);
            if (singleNews.IsNotNull() && newsComment.IsNotNull())
            {
                backEndNewsCommentsEdit.NewsId = newsComment.NewsId;
                backEndNewsCommentsEdit.NewsTitle = singleNews.NewsTitle;
                backEndNewsCommentsEdit.IsActive = newsComment.IsActive;
                backEndNewsCommentsEdit.Comment = newsComment.Comment;
            }
            else
            {
                ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                ViewData.IsFormVisible(false);
            }

            return View(backEndNewsCommentsEdit);
        }

        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        public ActionResult NewsCommentsEdit(BackEndNewsCommentsEdit backEndNewsCommentsEdit, int id)
        {
            if (ModelState.IsValidOrRefresh())
            {
                NewsComments newsComments = new NewsComments();
                int? result = newsComments.Edit(id, backEndNewsCommentsEdit.IsActive, backEndNewsCommentsEdit.Comment);
                switch (result)
                {
                    case 0:
                        ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyEdited);
                        break;

                    case 2:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                        ViewData.IsFormVisible(false);
                        break;

                    default:
                        ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                        break;
                }
            }

            return View(backEndNewsCommentsEdit);
        }

        //  /Admin/NewsCommentsDelete/
        [HttpPost]
        [IsRestricted]
        [ValidateAntiForgeryToken]
        [ExportModelStateToTempData]
        public ActionResult NewsCommentsDelete(int deleteId)
        {
            NewsComments newsComments = new NewsComments();

            NewsComment newsComment = newsComments.GetNewsComment(deleteId);
            int newsId = 0;
            if (newsComment.IsNotNull())
            {
                newsId = newsComment.NewsId;
            }

            switch (newsComments.Delete(deleteId))
            {
                case 0:
                    ModelState.AddResult(ViewData, ModelStateResult.Success, Resources.Strings.ItemSuccessfullyDeleted);
                    break;

                case 2:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemDoesNotExist);
                    break;

                case 3:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.ItemUsedSomewhereElse);
                    break;

                default:
                    ModelState.AddResult(ViewData, ModelStateResult.Error, Resources.Strings.UnexpectedError);
                    break;
            }

            return RedirectToAction("NewsComments", new { id = newsId });
        }
    }
}