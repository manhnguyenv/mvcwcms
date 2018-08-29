using MVCwCMS.Helpers;
using MVCwCMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public class SitemapController : Controller
    {
        // GET: Sitemap
        public ActionResult Index()
        {
            StringBuilder sitemap = new StringBuilder();

            sitemap.AppendLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            sitemap.AppendLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\" xmlns:xhtml=\"http://www.w3.org/1999/xhtml\">");

            BuildSitemap(new CmsPages().GetAllPages(), new Languages().GetAllLanguages(isActive: true), new GlobalConfigurations().GetGlobalConfiguration(), null, ref sitemap);

            sitemap.AppendLine("</urlset>");

            return Content(sitemap.ToString(), "text/xml");
        }

        private static void BuildSitemap(List<CmsPage> cmsPageList, List<Language> sitemapLanguages, GlobalConfiguration globalConfiguration, int? pageParentId, ref StringBuilder sitemap)
        {
            if (cmsPageList.IsNotNull())
            {
                List<CmsPage> sitemapPages = (from page in cmsPageList
                                              where page.PageParentId == pageParentId
                                              && page.ShowInSitemap
                                              && page.IsActive
                                              select page).ToList();

                if (sitemapPages.IsNotNull() && sitemapPages.Count() > 0)
                {
                    foreach (CmsPage page in sitemapPages)
                    {
                        foreach (Language language in sitemapLanguages)
                        {
                            sitemap.AppendLine("  <url>");
                            sitemap.AppendLine("    <loc>" + globalConfiguration.DomainName.ToUrl() + (page.IsHomePage ? (language.LanguageCode != globalConfiguration.DefaultLanguageCode ? language.LanguageCode + "/" : "") : language.LanguageCode + "/" + page.FullSegment + "/") + "</loc>");
                            foreach (Language languageSubset in sitemapLanguages)
                            {
                                sitemap.AppendLine("    <xhtml:link rel=\"alternate\" hreflang=\"" + languageSubset.LanguageCode + "\" href=\"" + globalConfiguration.DomainName.ToUrl() + (page.IsHomePage ? (languageSubset.LanguageCode != globalConfiguration.DefaultLanguageCode ? languageSubset.LanguageCode + "/" : "") : languageSubset.LanguageCode + "/" + page.FullSegment + "/") + "\" />");
                            }
                            sitemap.AppendLine("  </url>");
                        }

                        BuildSitemap(cmsPageList, sitemapLanguages, globalConfiguration, page.PageId, ref sitemap);
                    }
                }
            }
        }
    }
}