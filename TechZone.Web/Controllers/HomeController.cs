using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TechZone.Common;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Controllers
{
    public class HomeController : Controller
    {
        IProductCategoryService _productCategoryService;
        IProductService _productService;
        ICommonService _commonService;
        IMappingService _mappingService;

        public HomeController(IProductCategoryService productCategoryService,
            IProductService productService,
            ICommonService commonService,
            IMappingService mappingService)
        {
            _productCategoryService = productCategoryService;
            _commonService = commonService;
            _productService = productService;
            _mappingService = mappingService;
        }

        [OutputCache(Duration = 60, Location = System.Web.UI.OutputCacheLocation.Client)]
        public ActionResult Index()
        {
            var slideModel = _commonService.GetSlides();
            var slideView = _mappingService.Mapper.Map<IEnumerable<Slide>, IEnumerable<SlideViewModel>>(slideModel);
            var homeViewModel = new HomeViewModel();
            homeViewModel.Slides = slideView;

            var lastestProductModel = _productService.GetLastest(3);
            var topSaleProductModel = _productService.GetHotProduct(8);
            var mostViewProductModel = _productService.GetMostView(3);
            var lastestProductViewModel = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(lastestProductModel);
            var topSaleProductViewModel = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(topSaleProductModel);
            var mostViewProducViewtModel = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(mostViewProductModel);
            homeViewModel.LastestProducts = lastestProductViewModel;
            homeViewModel.TopSaleProducts = topSaleProductViewModel;
            homeViewModel.MostViewProduct = mostViewProducViewtModel;

            try
            {
                homeViewModel.Title = _commonService.GetSystemConfig(CommonConstants.HomeTitle).ValueString;
                homeViewModel.MetaKeyword = _commonService.GetSystemConfig(CommonConstants.HomeMetaKeyword).ValueString;
                homeViewModel.MetaDescription = _commonService.GetSystemConfig(CommonConstants.HomeMetaDescription).ValueString;
            }
            catch
            {

            }

            return View(homeViewModel);
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 3600)]
        public ActionResult Footer()
        {
            var footerModel = _commonService.GetFooter();
            var footerViewModel = _mappingService.Mapper.Map<Footer, FooterViewModel>(footerModel);
            return PartialView(footerViewModel);
        }

        [ChildActionOnly]
        public ActionResult Header()
        {
            return PartialView();
        }

        [ChildActionOnly]
        //[OutputCache(Duration = 3600)]
        public ActionResult Category()
        {
            var model = _productCategoryService.GetAll();
            var listProductCategoryViewModel = _mappingService.Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);
            return PartialView(listProductCategoryViewModel);
        }
    }
}