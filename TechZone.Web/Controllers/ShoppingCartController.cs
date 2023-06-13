using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TechZone.Common;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.App_Start;
using TechZone.Web.Infrastructure.Extensions;
using TechZone.Web.Mappings;
using TechZone.Web.Models;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace TechZone.Web.Controllers
{
    public class ShoppingCartController : Controller
    {
        private IProductService _productService;
        private IOrderService _orderService;
        private ApplicationUserManager _userManager;
        private IMappingService _mappingService;

        public ShoppingCartController(IOrderService orderService,
            IProductService productService,
            ApplicationUserManager userManager,
            IMappingService mappingService)
        {
            this._productService = productService;
            this._userManager = userManager;
            this._orderService = orderService;
            this._mappingService = mappingService;
        }

        // GET: ShoppingCart
        public ActionResult Index()
        {
            if (Session[CommonConstants.SessionCart] == null)
                Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            return View();
        }

        public ActionResult CheckOut()
        {
            if (Session[CommonConstants.SessionCart] == null)
            {
                return Redirect("/gio-hang.html");
            }
            return View();
        }

        public JsonResult GetUser()
        {
            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var user = _userManager.FindById(userId);
                return Json(new
                {
                    data = user,
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }

        public JsonResult CreateOrder(string orderViewModel)
        {
            var order = new JavaScriptSerializer().Deserialize<OrderViewModel>(orderViewModel);
            order.CreatedDate = DateTime.Now;
            var orderNew = new Order();
            //var oldOrder = new Order();
            var oldOrder = new List<Order>();

            orderNew.UpdateOrder(order);

            if (Request.IsAuthenticated)
            {
                orderNew.CustomerID = User.Identity.GetUserId();
                orderNew.CreatedBy = User.Identity.GetUserName();

                //oldOrder = _orderService.GetOrderByUserId(User.Identity.GetUserId());
                oldOrder = _orderService.GetListOrderByUserId(User.Identity.GetUserId()).ToList();
                //oldOrder.UpdateOrder(order);
                //oldOrder.CustomerID = User.Identity.GetUserId();
            }

            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            List<OrderDetail> orderDetails = new List<OrderDetail>();
            bool isEnough = true;
            foreach (var item in cart)
            {
                var detail = new OrderDetail();
                detail.ProductID = item.ProductId;
                detail.Quantity = item.Quantity;
                detail.Price = item.Product.Price;
                detail.OrderDate = DateTime.Now;
                detail.IsOrder = true;
                orderDetails.Add(detail);

                isEnough = _productService.SellProduct(item.ProductId, item.Quantity);
                if (!isEnough)
                    break;
            }
            if (isEnough)
            {
                if (oldOrder != null)
                {
                    //var orderDetail = _orderService.GetAllOrderDetail(oldOrder.ID);
                    //foreach (var item in orderDetail)
                    //{
                    //    item.IsOrder = true;
                    //    _orderService.UpdateOrderDetail(item);
                    //}
                    foreach (var item in oldOrder)
                    {
                        bool check = false;
                        var orderDetail = _orderService.GetAllOrderDetail(item.ID);
                        foreach (var item1 in orderDetail)
                        {
                            if (!item1.IsOrder)
                            {
                                _orderService.DeleteOrderDetail(item1.OrderID, item1.ProductID);
                                check = true;
                            }
                        }
                        if (!check)
                        {
                            item.PaymentStatus = "true";
                            _orderService.UpdateOrder(item);
                            _orderService.Save();
                        }
                    }
                }
                //else
                //{
                //    _orderService.Create(orderNew, orderDetails);
                //}
                orderNew.PaymentStatus = "true";
                _orderService.Create(orderNew, orderDetails);
                _orderService.Save();
                return Json(new
                {
                    status = true
                });
            }
            else
            {
                return Json(new
                {
                    status = false,
                    message = "Không đủ hàng."
                });
            }
        }

        public JsonResult GetAll()
        {
            if (Session[CommonConstants.SessionCart] == null)
                Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];

            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
                cart = new List<ShoppingCartViewModel>();
                var order = _orderService.GetOrderByUserId(userId);
                if (order != null)
                {
                    var orderDetail = _orderService.GetAllOrderDetail(order.ID);
                    var orderDetailVm = _mappingService.Mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailViewModel>>(orderDetail);
                    //var orderDetail = _orderService.GetAllOrderDetail(order.ID);

                    //var orderDetailVm = _mappingService.Mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailViewModel>>(orderDetail);

                    foreach (var item in orderDetailVm)
                    {
                        if (!item.IsOrder)
                        {
                            var product = _productService.GetById(item.ProductID);
                            cart.Add(new ShoppingCartViewModel
                            {
                                ProductId = item.ProductID,
                                Product = _mappingService.Mapper.Map<Product, ProductViewModel>(product),
                                Quantity = item.Quantity
                            });
                        }
                    }
                }
                Session[CommonConstants.SessionCart] = cart;
            }

            return Json(new
            {
                data = cart,
                status = true
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Add(int productId)
        {
            var cart = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            var product = _productService.GetById(productId);
            if (cart == null)
            {
                cart = new List<ShoppingCartViewModel>();
            }
            if (product.Quantity == 0)
            {
                return Json(new
                {
                    status = false,
                    message = "Sản phẩm này hiện đang hết hàng"
                });
            }

            if (cart.Any(x => x.ProductId == productId))
            {
                foreach (var item in cart)
                {
                    if (item.ProductId == productId)
                    {
                        item.Quantity += 1;
                    }
                }
            }
            else
            {
                ShoppingCartViewModel newItem = new ShoppingCartViewModel();
                newItem.ProductId = productId;
                newItem.Product = _mappingService.Mapper.Map<Product, ProductViewModel>(product);
                newItem.Quantity = 1;
                cart.Add(newItem);
            }

            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var order = _orderService.GetOrderByUserId(userId);
                if (order != null)
                {
                    var orderDetail = _orderService.GetAllOrderDetail(order.ID);

                    foreach (var item in cart)
                    {
                        var od = _orderService.GetOrderDetailByProductId(order.ID, item.ProductId);
                        if (od != null && !od.IsOrder)
                        {
                            od.Quantity = item.Quantity;
                            _orderService.UpdateOrderDetail(od);
                        }
                        else
                        {
                            var detail = new OrderDetail();
                            detail.OrderID = order.ID;
                            detail.ProductID = item.ProductId;
                            detail.Quantity = item.Quantity;
                            detail.Price = item.Product.Price;
                            detail.OrderDate = DateTime.Now;
                            _orderService.AddOrderDetail(detail);
                        }
                    }
                }
                else
                {
                    var orderNew = new Order();
                    orderNew.CustomerID = User.Identity.GetUserId();
                    orderNew.CreatedBy = User.Identity.GetUserName();
                    orderNew.CustomerName = User.Identity.GetUserName();
                    orderNew.CustomerAddress = "temp";
                    orderNew.CustomerEmail = "temp";
                    orderNew.CustomerMobile = "temp";
                    orderNew.CustomerMessage = "temp";

                    List<OrderDetail> orderDetail = new List<OrderDetail>();
                    foreach (var item in cart)
                    {
                        var detail = new OrderDetail();
                        detail.OrderID = orderNew.ID;
                        detail.ProductID = item.ProductId;
                        detail.Quantity = item.Quantity;
                        detail.Price = item.Product.Price;
                        detail.OrderDate = DateTime.Now;
                        orderDetail.Add(detail);
                    }
                    _orderService.Create(orderNew, orderDetail);
                }
                _orderService.Save();
            }

            Session[CommonConstants.SessionCart] = cart;
            return Json(new
            {
                status = true
            });
        }

        [HttpPost]
        public JsonResult DeleteItem(int productId)
        {
            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var order = _orderService.GetOrderByUserId(userId);
                _orderService.DeleteOrderDetail(order.ID, productId);
                _productService.Save();
            }

            var cartSession = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            if (cartSession != null)
            {
                cartSession.RemoveAll(x => x.ProductId == productId);
                Session[CommonConstants.SessionCart] = cartSession;
                return Json(new
                {
                    status = true
                });
            }
            return Json(new
            {
                status = false
            });
        }

        [HttpPost]
        public JsonResult Update(string cartData)
        {
            var cartViewModel = new JavaScriptSerializer().Deserialize<List<ShoppingCartViewModel>>(cartData);

            var cartSession = (List<ShoppingCartViewModel>)Session[CommonConstants.SessionCart];
            foreach (var item in cartSession)
            {
                foreach (var jitem in cartViewModel)
                {
                    if (item.ProductId == jitem.ProductId)
                    {
                        var product = _productService.GetById(jitem.ProductId);
                        if (product.Quantity < jitem.Quantity)
                        {
                            return Json(new
                            {
                                status = false,
                                quantity = product.Quantity
                            });
                        }
                        item.Quantity = jitem.Quantity;
                    }
                }
            }

            if (Request.IsAuthenticated)
            {
                var userId = User.Identity.GetUserId();
                var order = _orderService.GetOrderByUserId(userId);

                var orderDetail = _orderService.GetAllOrderDetail(order.ID);

                foreach (var item in cartSession)
                {
                    var od = _orderService.GetOrderDetailByProductId(order.ID, item.ProductId);
                    if (od != null && !od.IsOrder)
                    {
                        od.Quantity = item.Quantity;
                        _orderService.UpdateOrderDetail(od);
                    }
                }

                _orderService.Save();
            }

            Session[CommonConstants.SessionCart] = cartSession;
            return Json(new
            {
                status = true
            });
        }

        [HttpPost]
        public JsonResult DeleteAll()
        {
            Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            return Json(new
            {
                status = true
            });
        }
    }
}