using Microsoft.AspNet.Identity;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using TechZone.Common;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Infrastructure.Core;
using TechZone.Web.Infrastructure.Extensions;
using TechZone.Web.Mappings;
using TechZone.Web.Models;
using Product = TechZone.Model.Models.Product;

namespace TechZone.Web.Api
{
    [RoutePrefix("api/statistic")]
    public class StatisticController : ApiControllerBase
    {
        private readonly IStatisticService _statisticService;
        private readonly IOrderService _orderService;
        private readonly IProductService _productService;
        private readonly IMappingService _mappingService;

        public StatisticController(IErrorService errorService,
            IStatisticService statisticService,
            IOrderService orderService,
            IProductService productService,
            IMappingService mappingService) : base(errorService)
        {
            _statisticService = statisticService;
            _orderService = orderService;
            _productService = productService;
            _mappingService = mappingService;
        }

        [Route("getrevenuebydate")]
        [HttpGet]
        public HttpResponseMessage GetRevenueStatisticByDate(HttpRequestMessage request, string fromDate, string toDate)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _statisticService.GetRevenueStatisticByDate(fromDate, toDate);
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            });
        }

        [Route("getrevenuebymonth")]
        [HttpGet]
        public HttpResponseMessage GetRevenueStatisticByMonth(HttpRequestMessage request, string fromMonth, string toMonth)
        {
            return CreateHttpResponse(request, () =>
            {
                var model = _statisticService.GetRevenueStatisticByMonth(fromMonth, toMonth);
                HttpResponseMessage response = request.CreateResponse(HttpStatusCode.OK, model);
                return response;
            });
        }

        [Route("getorder")]
        [HttpGet]
        public IHttpActionResult GetOrder(string keyword, int page, int pageSize = 20)
        {
            var model = _orderService.GetAllOrder(keyword);
            int totalRow = model.Count();
            var query = model.OrderByDescending(x => x.CreatedDate).Skip((page) * pageSize).Take(pageSize);

            var responseData = _mappingService.Mapper.Map<IEnumerable<Order>, IEnumerable<OrderViewModel>>(query);

            var paginationSet = new PaginationSet<OrderViewModel>()
            {
                Items = responseData,
                Page = page,
                TotalCount = totalRow,
                TotalPages = (int)Math.Ceiling((decimal)totalRow / pageSize)
            };
            return Ok(paginationSet);
        }

        [HttpDelete]
        [Route("delete/order")]
        public HttpResponseMessage DeleteOrder(HttpRequestMessage request, int id)
        {
            var appGroup = _orderService.DeleteOrder(id);
            _orderService.Save();
            return request.CreateResponse(HttpStatusCode.OK, appGroup);
        }

        [Route("deletemulti/order")]
        [HttpDelete]
        public HttpResponseMessage DeleteMultiOrder(HttpRequestMessage request, string checkedList)
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
                    var listItem = new JavaScriptSerializer().Deserialize<List<int>>(checkedList);
                    foreach (var item in listItem)
                    {
                        _orderService.DeleteOrder(item);
                    }

                    _orderService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, listItem.Count);
                }

                return response;
            });
        }


        [Route("update")]
        [HttpPut]
        //public HttpResponseMessage UpdateOrderDetail(HttpRequestMessage request, OrderDetailViewModel orderDetailViewModel)
        public HttpResponseMessage UpdateOrderDetail(HttpRequestMessage request, OrderViewModel orderViewModel)
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

                    //var dbOrderDetail = _orderService.GetOrderDetailByProductId(orderDetailViewModel.OrderID, orderDetailViewModel.ProductID);
                    //UpdateOrder(orderDetailViewModel.OrderID);

                    //dbOrderDetail.UpdateOrderDetail(orderDetailViewModel);
                    //dbOrderDetail.DeliveryDate = DateTime.Now;

                    //_orderService.UpdateOrderDetail(dbOrderDetail);
                    var dbOrder = _orderService.GetOrderById(orderViewModel.ID);
                    dbOrder.Status = true;
                    _orderService.UpdateOrder(dbOrder);
                    _orderService.Save();

                    response = request.CreateResponse(HttpStatusCode.Created);
                }

                return response;
            });
        }

        //private void UpdateOrder(int id)
        //{
        //    var dbOrder = _orderService.GetOrderById(id);
        //    dbOrder.Status = true;
        //    var listOrderDetail = _orderService.GetAllOrderDetail(id);

        //    foreach (var item in listOrderDetail)
        //    {
        //        if (!item.IsDelivery)
        //        {
        //            dbOrder.Status = false;
        //            break;
        //        }
        //    }
        //    _orderService.UpdateOrder(dbOrder);
        //}

        [HttpDelete]
        [Route("delete/orderdetail")]
        public HttpResponseMessage DeleteOrderDetail(HttpRequestMessage request, int id)
        {
            var orderDetail = _orderService.DeleteOrderDetail(0, id);
            _orderService.Save();
            return request.CreateResponse(HttpStatusCode.OK, orderDetail);
        }

        [Route("deletemulti/orderdetail")]
        [HttpDelete]
        public HttpResponseMessage DeleteMultiOrderDetail(HttpRequestMessage request, string checkedList)
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
                    var listItem = new JavaScriptSerializer().Deserialize<List<int>>(checkedList);
                    foreach (var item in listItem)
                    {
                        _orderService.DeleteOrderDetail(0, item);
                    }

                    _orderService.Save();

                    response = request.CreateResponse(HttpStatusCode.OK, listItem.Count);
                }

                return response;
            });
        }

        [Route("getorderdetail/{id:int}")]
        [HttpGet]
        public IHttpActionResult GetOrderDetail(int id)
        {
            var model = _orderService.GetAllOrderDetail(id);

            IEnumerable<OrderDetailViewModel> responseData = _mappingService.Mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailViewModel>>(model);
            List<OrderDetailViewModel> data = new List<OrderDetailViewModel>();
            foreach (var item in responseData)
            {
                if (item.IsOrder)
                {
                    var product = _productService.GetById(item.ProductID);
                    item.Product = _mappingService.Mapper.Map<Product, ProductViewModel>(product);
                    data.Add(item);
                }
            }

            return Ok(data);
        }


        [HttpGet]
        [Route("exportPdf")]
        public async Task<HttpResponseMessage> ExportPdf(HttpRequestMessage request, int id)
        {
            string fileName = string.Concat("Order" + DateTime.Now.ToString("yyyyMMddhhmmssfff") + ".pdf");
            var folderReport = ConfigHelper.GetByKey("ReportFolder");
            string filePath = HttpContext.Current.Server.MapPath(folderReport);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
            string fullPath = Path.Combine(filePath, fileName);
            try
            {
                var template = File.ReadAllText(HttpContext.Current.Server.MapPath("/Assets/admin/templates/order-detail.html"));
                var replaces = new Dictionary<string, string>();
                var order = _orderService.GetOrderById(id);
                var orderDetail = _orderService.GetAllOrderDetail(id);
                string detail = "";
                decimal total = 0;
                if (orderDetail != null)
                {
                    var dem = 1;
                    foreach (var item in orderDetail)
                    {
                        var product = _productService.GetById(item.ProductID);
                        detail += "<tr>\r\n";
                        detail += "<td style=\"text-align:center\">" + dem + "</td>\r\n";
                        detail += "<td>" + product.Name + "</td>\r\n";
                        detail += "<td style=\"text-align:center\">" + item.Quantity + "</td>\r\n";
                        detail += "<td style=\"text-align:right\">" + item.Price.ToString("N0", new System.Globalization.CultureInfo("en-US")) + "</td>\r\n";
                        detail += "<td style=\"text-align:right\">" + (item.Price * item.Quantity).ToString("N0", new System.Globalization.CultureInfo("en-US")) + "</td>\r\n";
                        detail += "</tr>";

                        total += item.Price * item.Quantity;
                        dem++;
                    }
                }

                replaces.Add("{{ID}}", "HDTZ" + order.ID);
                replaces.Add("{{CreatedDate}}", order.CreatedDate.ToString());
                replaces.Add("{{CustomerName}}", order.CustomerName);
                replaces.Add("{{CustomerAddress}}", order.CustomerAddress);
                replaces.Add("{{CustomerMobile}}", order.CustomerMobile);
                replaces.Add("{{CustomerEmail}}", order.CustomerEmail);
                replaces.Add("{{Detail}}", detail);
                replaces.Add("{{TotalNumber}}", total.ToString("N0", new System.Globalization.CultureInfo("en-US")));
                replaces.Add("{{TotalText}}", ConvertNumberToWords((int)total));

                template = template.Parse(replaces);

                await ReportHelper.GeneratePdf(template, fullPath);
                return request.CreateErrorResponse(HttpStatusCode.OK, Path.Combine(folderReport, fileName));
            }
            catch (Exception ex)
            {
                return request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        private string ConvertNumberToWords(int number)
        {
            if (number == 0) return "không";

            string[] ones = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín", "mười", "mười một", "mười hai", "mười ba", "mười bốn", "mười lăm", "mười sáu", "mười bảy", "mười tám", "mười chín" };
            string[] suffixes = { "", "ngàn", "triệu ", "tỷ " };

            var words = new StringBuilder();
            var groups = new List<int>();

            if (number < 0)
            {
                words.Append("âm ");
                number = -number;
            }

            for (var i = 0; number > 0; i++)
            {
                groups.Add(number % 1000);
                number /= 1000;
            }

            for (var i = groups.Count - 1; i >= 0; i--)
            {
                var group = groups[i];
                if (group == 0) continue;

                var groupWords = new StringBuilder();

                var hundreds = group / 100;
                var tensUnits = group % 100;

                if (hundreds > 0)
                {
                    groupWords.AppendFormat("{0} trăm ", ones[hundreds]);
                }

                if (tensUnits > 0)
                {
                    if (tensUnits < 20)
                    {
                        groupWords.Append(ones[tensUnits]);
                    }
                    else
                    {
                        var tens = tensUnits / 10;
                        var units = tensUnits % 10;

                        if (units > 0)
                        {
                            groupWords.AppendFormat("{0} mươi {1}", ones[tens], ones[units]);
                        }
                        else
                        {
                            groupWords.AppendFormat("{0} mươi", ones[tens]);
                        }
                    }
                }

                groupWords.AppendFormat(" {0}", suffixes[i]);

                words.Append(groupWords);
            }

            return words.ToString().Trim();
        }
    }
}