using System.Web.Mvc;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Controllers
{
    public class PageController : Controller
    {
        private readonly IPageService _pageService;
        private readonly IMappingService _mappingService;

        public PageController(IPageService pageService, IMappingService mappingService)
        {
            this._pageService = pageService;
            this._mappingService = mappingService;
        }

        // GET: Page
        public ActionResult Index()
        {
            return View();
        }
    }
}