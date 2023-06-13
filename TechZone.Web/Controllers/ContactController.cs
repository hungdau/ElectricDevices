using AutoMapper;
using BotDetect.Web.Mvc;
using System.Web.Mvc;
using TechZone.Common;
using TechZone.Model.Models;
using TechZone.Service;
using TechZone.Web.Infrastructure.Extensions;
using TechZone.Web.Mappings;
using TechZone.Web.Models;

namespace TechZone.Web.Controllers
{
    public class ContactController : Controller
    {
        private IContactDetailService _contactDetailService;
        IFeedbackService _feedbackService;
        private IMappingService _mappingService;

        public ContactController(IContactDetailService contactDetailService, IFeedbackService feedbackService, IMappingService mappingService)
        {
            this._contactDetailService = contactDetailService;
            this._feedbackService = feedbackService;
            this._mappingService = mappingService;
        }

        // GET: Contact
        public ActionResult Index()
        {
            FeedbackViewModel viewModel = new FeedbackViewModel();
            viewModel.ContactDetail = GetDetail();
            return View(viewModel);
        }

        [HttpPost]
        [CaptchaValidation("CaptchaCode", "contactCaptcha", "Mã xác nhận không đúng")]
        public ActionResult SendFeedback(FeedbackViewModel feedbackViewModel)
        {
            if (ModelState.IsValid)
            {
                Feedback newFeedback = new Feedback();
                newFeedback.UpdateFeedback(feedbackViewModel);
                _feedbackService.Create(newFeedback);
                _feedbackService.Save();

                ViewData["SuccessMsg"] = "Gửi phản hồi thành công";

                string content = System.IO.File.ReadAllText(Server.MapPath("/Assets/client/template/contact_template.html"));
                content = content.Replace("{{Name}}", feedbackViewModel.Name);
                content = content.Replace("{{Email}}", feedbackViewModel.Email);
                content = content.Replace("{{Message}}", feedbackViewModel.Message);
                var adminEmail = ConfigHelper.GetByKey("AdminEmail");
                MailHelper.SendMail(adminEmail, "Thông tin liên hệ từ website", content);

                feedbackViewModel.Name = "";
                feedbackViewModel.Message = "";
                feedbackViewModel.Email = "";
            }
            feedbackViewModel.ContactDetail = GetDetail();

            return View("Index", feedbackViewModel);
        }

        private ContactDetailViewModel GetDetail()
        {
            var model = _contactDetailService.GetDefaultContact();
            var contactViewModel = _mappingService.Mapper.Map<ContactDetail, ContactDetailViewModel>(model);
            return contactViewModel;
        }
    }
}