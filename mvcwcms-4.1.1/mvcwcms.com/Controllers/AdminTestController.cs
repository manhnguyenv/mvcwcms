using AutoMapper;
using MVCwCMS.Filters;
using MVCwCMS.Models;
using MVCwCMS.Services;
using MVCwCMS.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MVCwCMS.Controllers
{
    public partial class AdminController : AdminBaseController
    {
        private readonly ICategoryService categoryService;
        private readonly IGadgetService gadgetService;

        public AdminController(ICategoryService categoryService, IGadgetService gadgetService)
        {
            this.categoryService = categoryService;
            this.gadgetService = gadgetService;
        }

        //  /Admin/Test
        [IsRestricted]
        public ActionResult Test(string category = null)
        {
            IEnumerable<CategoryViewModel> viewModelGadgets;
            IEnumerable<Category> categories;

            categories = categoryService.GetCategories(category).ToList();

            viewModelGadgets = Mapper.Map<IEnumerable<Category>, IEnumerable<CategoryViewModel>>(categories);
            return View(viewModelGadgets);
        }

        public ActionResult Filter(string category, string gadgetName)
        {
            IEnumerable<GadgetViewModel> viewModelGadgets;
            IEnumerable<Gadget> gadgets;

            gadgets = gadgetService.GetCategoryGadgets(category, gadgetName);

            viewModelGadgets = Mapper.Map<IEnumerable<Gadget>, IEnumerable<GadgetViewModel>>(gadgets);

            return View(viewModelGadgets);
        }

        [HttpPost]
        public ActionResult Create(GadgetFormViewModel newGadget)
        {
            if (newGadget != null && newGadget.File != null)
            {
                var gadget = Mapper.Map<GadgetFormViewModel, Gadget>(newGadget);
                gadgetService.CreateGadget(gadget);

                string gadgetPicture = System.IO.Path.GetFileName(newGadget.File.FileName);
                string path = System.IO.Path.Combine(Server.MapPath("~/images/"), gadgetPicture);
                newGadget.File.SaveAs(path);

                gadgetService.SaveGadget();
            }

            var category = categoryService.GetCategory(newGadget.GadgetCategory);
            return RedirectToAction("Test", new { category = category.Name });
        }
    }
}