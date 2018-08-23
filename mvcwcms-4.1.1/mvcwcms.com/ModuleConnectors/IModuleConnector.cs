using MVCwCMS.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace MVCwCMS.ModuleConnectors
{
    public interface IModuleConnector
    {
        List<SelectListItem> GetSelectItemList();

        string GetContent(HtmlHelper htmlHelper, FrontEndCmsPage model, string id);

        bool IsFileUsed(string filePath);
    }
}
