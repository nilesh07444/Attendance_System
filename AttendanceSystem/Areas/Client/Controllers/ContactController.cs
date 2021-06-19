using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class ContactController : Controller
    {
        AttendanceSystemEntities _db; 

        public ContactController()
        {
            _db = new AttendanceSystemEntities(); 
        }
        
        public ActionResult Index()
        {
            try
            {
                var HeroImageName = _db.tbl_Setting.FirstOrDefault().HeroContactPageImageName;
                ViewBag.HeroUrl = ErrorMessage.HeroDirectoryPath + HeroImageName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }

        [HttpPost]
        public ActionResult SaveContactForm(string ContactDetail)
        {
            GeneralResponseVM response = new GeneralResponseVM();
            try
            {
                ContactFormVM contactDataVM = JsonConvert.DeserializeObject<ContactFormVM>(ContactDetail);

                tbl_ContactForm objContact = new tbl_ContactForm();
                objContact.FirstName = contactDataVM.Firstname;
                objContact.LastName = contactDataVM.Lastname;
                objContact.MobileNo = contactDataVM.MobileNo;
                objContact.EmailId = contactDataVM.EmailId;
                objContact.Message = contactDataVM.Message;
                objContact.CreatedDate = CommonMethod.CurrentIndianDateTime();
                _db.tbl_ContactForm.Add(objContact);
                _db.SaveChanges();
                response.IsError = false;  

            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.ErrorMessage = ex.Message.ToString();
            }
            return Json(response);
        }

    }
}