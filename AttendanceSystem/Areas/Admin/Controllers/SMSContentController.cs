using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class SMSContentController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public SMSContentController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index()
        {
            List<SMSContentVM> lstSMS = (from s in _db.tbl_SMSContent
                                         select new SMSContentVM
                                         {
                                             SMSContentId = s.SMSContentId,
                                             SMSTitle = s.SMSTitle,
                                             SMSDescription = s.SMSDescription,
                                             SeqNo = s.SeqNo,
                                             //Remarks = ""
                                         }).OrderBy(x => x.SeqNo).ToList();
            return View(lstSMS);
        }
         
        public ActionResult Edit(int Id)
        {
            SMSContentVM objSMS = (from s in _db.tbl_SMSContent
                                   where s.SMSContentId == Id
                                   select new SMSContentVM
                                   {
                                       SMSContentId = s.SMSContentId,
                                       SMSTitle = s.SMSTitle,
                                       SMSDescription = s.SMSDescription,
                                       SeqNo = s.SeqNo,
                                       //Remarks = ""
                                   }).FirstOrDefault();
            return View(objSMS);
        }

        [HttpPost]
        public ActionResult Edit(SMSContentVM smsVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = clsAdminSession.UserID;

                    tbl_SMSContent objSMSContent = _db.tbl_SMSContent.Where(x => x.SMSContentId == smsVM.SMSContentId).FirstOrDefault();
                    objSMSContent.SMSTitle = smsVM.SMSTitle;
                    objSMSContent.SMSDescription = smsVM.SMSDescription;
                    objSMSContent.SeqNo = smsVM.SeqNo;
                    //objSMSContent.Remarks = smsVM.Remarks;

                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return View(smsVM);
        }

    }
}