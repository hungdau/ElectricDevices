using BotDetect.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using TechZone.Common;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.App_Start;
using TechZone.Web.Infrastructure.Core;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private IProductService _productService;
        private IOrderService _orderService;
        private IMappingService _mappingService;

        public AccountController(ApplicationUserManager userManager,
            ApplicationSignInManager signInManager,
            IOrderService orderService,
            IProductService productService,
            IMappingService mappingService)
        {
            UserManager = userManager;
            SignInManager = signInManager;
            _productService = productService;
            _orderService = orderService;
            _mappingService = mappingService;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public AccountController()
        {

        }
        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = _userManager.Find(model.UserName, model.Password);
                if (user != null)
                {
                    IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
                    authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                    ClaimsIdentity identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationProperties props = new AuthenticationProperties();
                    props.IsPersistent = model.RememberMe;
                    authenticationManager.SignIn(props, identity);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng.");
                }
            }
            return View(model);
        }


        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [CaptchaValidation("CaptchaCode", "registerCaptcha", "Mã xác nhận không đúng")]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userByEmail = await _userManager.FindByEmailAsync(model.Email);
                if (userByEmail != null)
                {
                    ModelState.AddModelError("email", "Email đã tồn tại");
                    return View(model);
                }
                var userByUserName = await _userManager.FindByNameAsync(model.UserName);
                if (userByUserName != null)
                {
                    ModelState.AddModelError("email", "Tài khoản đã tồn tại");
                    return View(model);
                }
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    EmailConfirmed = true,
                    BirthDay = DateTime.Now,
                    FullName = model.FullName,
                    PhoneNumber = model.PhoneNumber,
                    Address = model.Address

                };

                await _userManager.CreateAsync(user, model.Password);


                var adminUser = await _userManager.FindByEmailAsync(model.Email);
                if (adminUser != null)
                    await _userManager.AddToRolesAsync(adminUser.Id, new string[] { "User" });

                ViewData["SuccessMsg"] = "Đăng ký thành công";
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            Session[CommonConstants.SessionCart] = new List<ShoppingCartViewModel>();
            IAuthenticationManager authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Index()
        {
            var userId = User.Identity.GetUserId();
            var order = _orderService.GetListOrderByUserId(userId);

            List<CartOrder> listCart = new List<CartOrder>();

            if (order != null)
            {
                foreach (var item in order)
                {
                    if (item.PaymentStatus != null)
                    {
                        var orderDetail = _orderService.GetAllOrderDetail(item.ID);
                        var orderDetailVm = _mappingService.Mapper.Map<IEnumerable<OrderDetail>, IEnumerable<OrderDetailViewModel>>(orderDetail);

                        var cart = new List<ShoppingCartViewModel>();

                        foreach (var item1 in orderDetailVm)
                        {
                            if (item1.IsOrder)
                            {
                                var product = _productService.GetById(item1.ProductID);
                                cart.Add(new ShoppingCartViewModel
                                {
                                    ProductId = item1.ProductID,
                                    Product = _mappingService.Mapper.Map<Product, ProductViewModel>(product),
                                    Quantity = item1.Quantity
                                });
                            }
                        }
                        listCart.Add(new CartOrder
                        {
                            Id = "Tech" + item.ID,
                            OrderDate = item.CreatedDate,
                            Status = item.Status,
                            Cart = cart
                        });
                    }
                }
            }
            return View(listCart.AsEnumerable());
        }
    }
}