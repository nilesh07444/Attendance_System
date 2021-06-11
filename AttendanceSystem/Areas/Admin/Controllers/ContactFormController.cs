using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class ContactFormController : Controller
    {
        AttendanceSystemEntities _db;
        // GET: Admin/ContactForm
        public ContactFormController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<ContactFormVM> contactFormVM = new List<ContactFormVM>();
            try
            {
                contactFormVM = (from cn in _db.tbl_ContactForm
                                 select new ContactFormVM
                                 {
                                     Firstname = cn.FirstName,
                                     Lastname = cn.LastName,
                                     MobileNo = cn.MobileNo,
                                     EmailId = cn.EmailId,
                                     Message = cn.Message,
                                     CreatedDate = cn.CreatedDate,
                                     ContactFormId = cn.ContactFormId
                                 }).OrderByDescending(x => x.ContactFormId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(contactFormVM);
        }
    }
}