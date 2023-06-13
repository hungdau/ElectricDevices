using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using TechZone.Common;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Infrastructure.Core;
using TechZone.Web.Infrastructure.Extensions;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Api
{
    [RoutePrefix("api/product")]
    [Authorize]
    public class ProductController : ApiControllerBase
    {
        #region Initialize

        private readonly IProductService _productService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IMappingService _mappingService;

        public ProductController(IErrorService errorService,
            IProductService productService,
            IProductCategoryService productCategoryService,
            IMappingService mappingService)
            : base(errorService)
        {
            this._productService = productService;
            this._productCategoryService = productCategoryService;
            this._mappingService = mappingService;
        }

        #endregion Initialize

        [Route("getallparents")]
        [HttpGet]
        public IHttpActionResult GetAllParents()
        {
            var model = _productService.GetAll();
            var responseData = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(model);
            return Ok(responseData);
        }

        [Route("getbyid/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetById(int id)
        {
            var model = _productService.GetById(id);
            var responseData = _mappingService.Mapper.Map<Product, ProductViewModel>(model);
            return Ok(responseData);
        }

        public ProductCategoryViewModel GetProductCategory(int id)
        {
            var model = _productCategoryService.GetById(id);
            return _mappingService.Mapper.Map<ProductCategory, ProductCategoryViewModel>(model);
        }

        [Route("getall")]
        [HttpGet]
        public IHttpActionResult GetAll(string keyword, int categoryId = 0, int page = 0, int pageSize = 20)
        {
            int totalRow = 0;
            IEnumerable<Product> model;
            if (categoryId == 0)
            {
                model = _productService.GetAll(keyword);
            }
            else
            {
                model = _productService.GetAll(keyword, categoryId);
            }

            totalRow = model.Count();
            var query = model.OrderByDescending(x => x.CreatedDate).Skip((page) * pageSize).Take(pageSize);

            var responseData = _mappingService.Mapper.Map<IEnumerable<Product>, IEnumerable<ProductViewModel>>(query);

            ProductCategoryViewModel manufacturer;

            foreach (var item in responseData)
            {
                manufacturer = new ProductCategoryViewModel();
                manufacturer = GetProductCategory(item.CategoryID);

                if (manufacturer.ParentID.HasValue)
                {
                    item.Manufacturer = manufacturer.Name;
                    item.ProductCategory = GetProductCategory(manufacturer.ParentID.Value);
                }
                else
                {
                    item.ProductCategory = manufacturer;
                }
            }

            var paginationSet = new PaginationSet<ProductViewModel>()
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
        public HttpResponseMessage Create(HttpRequestMessage request, ProductViewModel productVm)
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
                    var newProduct = new Product();
                    newProduct.UpdateProduct(productVm);
                    newProduct.CreatedDate = DateTime.Now;
                    newProduct.CreatedBy = User.Identity.Name;
                    _productService.Add(newProduct);
                    _productService.Save();

                    var responseData = _mappingService.Mapper.Map<Product, ProductViewModel>(newProduct);
                    response = request.CreateResponse(HttpStatusCode.Created, responseData);
                }

                return response;
            });
        }

        [Route("update")]
        [HttpPut]
        [AllowAnonymous]
        public HttpResponseMessage Update(HttpRequestMessage request, ProductViewModel productVm)
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
                    var dbProduct = _productService.GetById(productVm.ID);

                    dbProduct.UpdateProduct(productVm);
                    dbProduct.UpdatedDate = DateTime.Now;
                    dbProduct.UpdatedBy = User.Identity.Name;

                    _productService.Update(dbProduct);
                    _productService.Save();

                    var responseData = _mappingService.Mapper.Map<Product, ProductViewModel>(dbProduct);
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
                    var oldProduct = _productService.Delete(id);
                    _productService.Save();

                    var responseData = _mappingService.Mapper.Map<Product, ProductViewModel>(oldProduct);
                    response = request.CreateResponse(HttpStatusCode.OK, responseData);
                }

                return response;
            });
        }

        [Route("deletemulti")]
        [HttpDelete]
        [AllowAnonymous]
        public HttpResponseMessage DeleteMulti(HttpRequestMessage request, string checkedProducts)
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
                    var listProduct = new JavaScriptSerializer().Deserialize<List<int>>(checkedProducts);
                    foreach (var item in listProduct)
                    {
                        _productService.Delete(item);
                    }
                    _productService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, true);
                }

                return response;
            });
        }

        [HttpGet]
        [Route("exportXls")]
        public async Task<HttpResponseMessage> ExportXls(HttpRequestMessage request, string filter = null)
        {
            string fileName = string.Concat("Product_" + DateTime.Now.ToString("yyyyMMddhhmmsss") + ".xlsx");

            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string fullPath = Path.Combine(filePath, fileName);

            try
            {
                var data = _productService.GetListProduct(filter).ToList();
                await ReportHelper.GenerateXls(data, fullPath);
                if (File.Exists(fullPath))
                {
                    /*var response = new HttpResponseMessage(HttpStatusCode.OK);
                    var filestream = new FileStream(fullPath, FileMode.Open);
                    response.Content = new StreamContent(filestream);
                    response.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment");
                    response.Content.Headers.ContentDisposition.FileName = fileName;
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");

                    return response;*/
                    return request.CreateErrorResponse(HttpStatusCode.OK, Path.Combine(folderReport, fileName));
                }
                else
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Lỗi không tạo được tệp báo cáo.");
                }
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [HttpGet]
        [Route("exportPdf")]
        public async Task<HttpResponseMessage> ExportPdf(HttpRequestMessage request, int id)
        {
            string fileName = string.Concat("Product" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".pdf");
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fullPath = Path.Combine(filePath, fileName);
            try
            {
                var template = File.ReadAllText(HttpContext.Current.Server.MapPath("/Assets/admin/templates/product-detail.html"));
                var replaces = new Dictionary<string, string>();
                var product = _productService.GetById(id);

                replaces.Add("{{ProductName}}", product.Name);
                replaces.Add("{{Price}}", product.Price.ToString("N0"));
                replaces.Add("{{Description}}", product.Description);
                replaces.Add("{{Warranty}}", product.Warranty + " tháng");

                template = template.Parse(replaces);

                await ReportHelper.GeneratePdf(template, fullPath);
                return request.CreateErrorResponse(HttpStatusCode.OK, Path.Combine(folderReport, fileName));
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        [Route("import")]
        [HttpPost]
        public async Task<HttpResponseMessage> Import()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                return Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType, "Định dạng không được server hỗ trợ");
            }

            var root = HttpContext.Current.Server.MapPath("~/UploadedFiles/Excels");
            if (!Directory.Exists(root))
            {
                Directory.CreateDirectory(root);
            }

            var provider = new MultipartFormDataStreamProvider(root);
            var result = await Request.Content.ReadAsMultipartAsync(provider);

            //do stuff with files if you wish
            if (result.FormData["categoryId"] == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Bạn chưa chọn nhà sản xuất.");
            }

            //Upload files
            int addedCount = 0;
            int categoryId = 0;
            int.TryParse(result.FormData["categoryId"], out categoryId);
            foreach (MultipartFileData fileData in result.FileData)
            {
                if (string.IsNullOrEmpty(fileData.Headers.ContentDisposition.FileName))
                {
                    return Request.CreateResponse(HttpStatusCode.NotAcceptable, "Yêu cầu không đúng định dạng");
                }
                string fileName = fileData.Headers.ContentDisposition.FileName;
                if (fileName.StartsWith("\"") && fileName.EndsWith("\""))
                {
                    fileName = fileName.Trim('"');
                }
                if (fileName.Contains(@"/") || fileName.Contains(@"\"))
                {
                    fileName = Path.GetFileName(fileName);
                }

                var fullPath = Path.Combine(root, fileName);

                try
                {
                    File.Copy(fileData.LocalFileName, fullPath, true);
                }
                catch (IOException ex)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
                }

                //insert to DB
                var listProduct = this.ReadProductFromExcel(fullPath, categoryId);
                if (listProduct.Count > 0)
                {
                    foreach (var product in listProduct)
                    {
                        _productService.Add(product);
                        addedCount++;
                    }
                    _productService.Save();
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, "Đã nhập " + addedCount + " sản phẩm thành công.");
        }

        private List<Product> ReadProductFromExcel(string fullPath, int categoryId)
        {
            using (var package = new ExcelPackage(new FileInfo(fullPath)))
            {
                ExcelWorksheet workSheet = package.Workbook.Worksheets[1];
                List<Product> listProduct = new List<Product>();
                ProductViewModel productViewModel;
                Product product;

                decimal originalPrice = 0;
                decimal price = 0;
                decimal promotionPrice = 0;

                bool status = false;
                bool showHome = false;
                bool isHot = false;
                int warranty;
                int quantity;

                for (int i = workSheet.Dimension.Start.Row + 1; i <= workSheet.Dimension.End.Row; i++)
                {
                    productViewModel = new ProductViewModel();
                    product = new Product();

                    productViewModel.Name = workSheet.Cells[i, 1].Value.ToString();
                    productViewModel.Alias = StringHelper.ToUnsignString(productViewModel.Name);

                    if (!string.IsNullOrWhiteSpace(workSheet.Cells[i, 2].Value?.ToString()))
                    {
                        productViewModel.Description = workSheet.Cells[i, 2].Value.ToString();
                    }

                    if (!string.IsNullOrWhiteSpace(workSheet.Cells[i, 3].Value?.ToString())
                        && int.TryParse(workSheet.Cells[i, 3].Value.ToString(), out warranty))
                    {
                        productViewModel.Warranty = warranty;
                    }

                    decimal.TryParse(workSheet.Cells[i, 4].Value?.ToString().Replace(",", ""), out originalPrice);
                    productViewModel.OriginalPrice = originalPrice;

                    decimal.TryParse(workSheet.Cells[i, 5].Value?.ToString().Replace(",", ""), out price);
                    productViewModel.Price = price;

                    if (!string.IsNullOrWhiteSpace(workSheet.Cells[i, 6].Value?.ToString())
                        && decimal.TryParse(workSheet.Cells[i, 6].Value.ToString(), out promotionPrice))
                    {
                        productViewModel.PromotionPrice = promotionPrice;
                    }

                    int.TryParse(workSheet.Cells[i, 7].Value?.ToString().Replace(",", ""), out quantity);
                    productViewModel.Quantity = quantity;

                    if (!string.IsNullOrWhiteSpace(workSheet.Cells[i, 8].Value?.ToString()))
                    {
                        productViewModel.Content = workSheet.Cells[i, 8].Value.ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(workSheet.Cells[i, 9].Value?.ToString()))
                    {
                        productViewModel.MetaKeyword = workSheet.Cells[i, 9].Value.ToString();
                    }
                    if (!string.IsNullOrWhiteSpace(workSheet.Cells[i, 10].Value?.ToString()))
                    {
                        productViewModel.MetaDescription = workSheet.Cells[i, 10].Value.ToString();
                    }

                    bool.TryParse(workSheet.Cells[i, 11].Value.ToString(), out status);
                    productViewModel.Status = false;

                    bool.TryParse(workSheet.Cells[i, 12].Value.ToString(), out showHome);
                    productViewModel.HomeFlag = showHome;

                    bool.TryParse(workSheet.Cells[i, 13].Value.ToString(), out isHot);
                    productViewModel.HotFlag = isHot;

                    productViewModel.CategoryID = categoryId;
                    productViewModel.CreatedDate = DateTime.Now;
                    productViewModel.CreatedBy = User.Identity.Name;
                    product.UpdateProduct(productViewModel);
                    listProduct.Add(product);
                }
                return listProduct;
            }
        }
    }
}