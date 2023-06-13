using AutoMapper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TechZone.Common;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Infrastructure.Core;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Controllers
{
    public class ProductController : Controller
    {
        private IProductService _productService;
        private IProductCategoryService _productCategoryService;
        private IMappingService _mappingService;

        public ProductController(IProductService productService,
            IProductCategoryService productCategoryService,
            IMappingService mappingService)
        {
            this._productService = productService;
            this._productCategoryService = productCategoryService;
            _mappingService = mappingService;
        }

        public ActionResult Detail(int productId)
        {
            var productModel = _productService.GetById(productId);
            var viewModel = _mappingService.Mapper.Map<Product, ProductViewModel>(productModel);
            var relatedProduct = _productService.GetReatedProducts(productId, 6);
            ViewBag.RelatedProducts = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(relatedProduct);
            if (!string.IsNullOrEmpty(viewModel.MoreImages))
            {
                List<string> listImages = new JavaScriptSerializer().Deserialize<List<string>>(viewModel.MoreImages);
                ViewBag.MoreImages = listImages;
            }

            ViewBag.Tags = _mappingService.Mapper.Map<IEnumerable<Tag>, IEnumerable<TagViewModel>>(_productService.GetListTagByProductId(productId));
            return View(viewModel);
        }

        public ActionResult Category(int id, int page = 1, string sort = "")
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var productModel = _productService.GetListProductByCategoryIdPaging(id, page, pageSize, sort, out totalRow);
            var productViewModel = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            var category = _productCategoryService.GetById(id);
            ViewBag.Category = _mappingService.Mapper.Map<ProductCategory, ProductCategoryViewModel>(category);
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Items = productViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            return View(paginationSet);
        }

        public ActionResult Search(string keyword, int page = 1, string sort = "")
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var productModel = _productService.Search(keyword, page, pageSize, sort, out totalRow);
            var productViewModel = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            ViewBag.Keyword = keyword;
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Items = productViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            return View(paginationSet);
        }

        public ActionResult ListByTag(string tagId, int page = 1)
        {
            int pageSize = int.Parse(ConfigHelper.GetByKey("PageSize"));
            int totalRow = 0;
            var productModel = _productService.GetListProductByTag(tagId, page, pageSize, out totalRow);
            var productViewModel = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(productModel);
            int totalPage = (int)Math.Ceiling((double)totalRow / pageSize);

            ViewBag.Tag = _mappingService.Mapper.Map<Tag, TagViewModel>(_productService.GetTag(tagId));
            var paginationSet = new PaginationSet<ProductViewModel>()
            {
                Items = productViewModel,
                MaxPage = int.Parse(ConfigHelper.GetByKey("MaxPage")),
                Page = page,
                TotalCount = totalRow,
                TotalPages = totalPage
            };

            return View(paginationSet);
        }

        public JsonResult GetListProductByName(string keyword)
        {
            var model = _productService.GetListProductByName(keyword);
            return Json(new
            {
                data = model
            }, JsonRequestBehavior.AllowGet);
        }
    }
}