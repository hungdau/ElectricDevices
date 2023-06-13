using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Script.Serialization;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Infrastructure.Core;
using TechZone.Web.Infrastructure.Extensions;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Api
{
    [RoutePrefix("api/productcategory")]
    [Authorize]
    public class ProductCategoryController : ApiControllerBase
    {
        #region Initialize
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMappingService _mappingService;

        public ProductCategoryController(IErrorService errorService, IProductCategoryService productCategoryService, IMappingService mappingService)
            : base(errorService)
        {
            this._productCategoryService = productCategoryService;
            this._mappingService = mappingService;
        }

        #endregion

        [Route("getallparents")]
        [HttpGet]
        public IHttpActionResult GetAllParents()
        {
            var model = _productCategoryService.GetAll();
            var responseData = _mappingService.Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(model);
            return Ok(responseData);
        }

        [Route("getbyid/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            return Ok(GetProductCategory(id));
        }

        public ProductCategoryViewModel GetProductCategory(int id)
        {
            var model = _productCategoryService.GetById(id);
            return _mappingService.Mapper.Map<ProductCategory, ProductCategoryViewModel>(model);
        }

        [Route("getallProductCategory")]
        [HttpGet]
        public IHttpActionResult GetAllProductCategory(string keyword, int page, int pageSize = 20)
        {
            var model = _productCategoryService.GetAll(keyword);
            List<ProductCategory> data = new List<ProductCategory>();

            foreach (var item in model)
            {
                if (!item.ParentID.HasValue)
                {
                    data.Add(item);
                }
            }
            return GetAll(page, pageSize, data);
        }


        [Route("getallManufacturer/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetAllManufacturer(string keyword, int page, int id, int pageSize = 20)
        {
            var model = _productCategoryService.GetAll(keyword);
            List<ProductCategory> data = new List<ProductCategory>();

            foreach (var item in model)
            {
                if (item.ParentID.HasValue)
                {
                    if (item.ParentID.Value == id)
                    {
                        data.Add(item);
                    }
                }
            }
            return GetAll(page, pageSize, data);
        }

        private IHttpActionResult GetAll(int page, int pageSize, IEnumerable<ProductCategory> model)
        {
            int totalRow = model.Count();
            var query = model.OrderByDescending(x => x.CreatedDate).Skip((page) * pageSize).Take(pageSize);

            var responseData = _mappingService.Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(query);

            var paginationSet = new PaginationSet<ProductCategoryViewModel>()
            {
                Items = responseData,
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
            };
            return Ok(paginationSet);
        }

        private IHttpActionResult GetAll(int page, int pageSize, List<ProductCategory> data)
        {
            int totalRow = data.Count();
            var query = data.OrderByDescending(x => x.CreatedDate).Skip((page) * pageSize).Take(pageSize);

            var responseData = _mappingService.Mapper.Map<IEnumerable<ProductCategory>, IEnumerable<ProductCategoryViewModel>>(query);

            var paginationSet = new PaginationSet<ProductCategoryViewModel>()
            {
                Items = responseData,
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
            };
            return Ok(paginationSet);
        }

        [Route("create")]
        [HttpPost]
        [AllowAnonymous]
        public HttpResponseMessage Create(HttpRequestMessage request, ProductCategoryViewModel productCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var newProductCategory = new ProductCategory();
                    newProductCategory.UpdateProductCategory(productCategoryVm);
                    newProductCategory.CreatedDate = DateTime.Now;
                    newProductCategory.CreatedBy = User.Identity.Name;
                    _productCategoryService.Add(newProductCategory);
                    _productCategoryService.Save();

                    var responseData = _mappingService.Mapper.Map<ProductCategory, ProductCategoryViewModel>(newProductCategory);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductCategoryViewModel productCategoryVm)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var dbProductCategory = _productCategoryService.GetById(productCategoryVm.ID);

                    dbProductCategory.UpdateProductCategory(productCategoryVm);
                    dbProductCategory.UpdatedDate = DateTime.Now;
                    dbProductCategory.UpdatedBy = User.Identity.Name;

                    _productCategoryService.Update(dbProductCategory);
                    _productCategoryService.Save();

                    var responseData = _mappingService.Mapper.Map<ProductCategory, ProductCategoryViewModel>(dbProductCategory);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("delete")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var oldProductCategory = _productCategoryService.Delete(id);
                    _productCategoryService.Save();

                    var responseData = _mappingService.Mapper.Map<ProductCategory, ProductCategoryViewModel>(oldProductCategory);
                    response = request.CreateResponse(HttpStatusCode.OK, responseData);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedProductCategories)
        {
            return CreateHttpResponse(request, () =>
            {
                HttpResponseMessage response = null;
                if (!ModelState.IsValid)
                {
                    response = request.CreateResponse(HttpStatusCode.BadRequest, ModelState);
                }
                else
                {
                    var listProductCategory = new JavaScriptSerializer().Deserialize<List<int>>(checkedProductCategories);
                    foreach (var item in listProductCategory)
                    {
                        _productCategoryService.Delete(item);
                    }
                    _productCategoryService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }

                return response;
            });
        }
    }
}