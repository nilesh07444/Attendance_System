using AttendanceSystem.Models;
using AttendanceSystem.Helper;
using MyMobileApp.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static AttendanceSystem.Models.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        public DashboardController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ChangePassword()
        {
            ChangePasswordVM CPVM = new ChangePasswordVM();
            CPVM.UserName = clsAdminSession.UserName;
            return View(CPVM);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordVM changePasswordVM)
        {
            try
            {
                string curPwd = CommonMethod.Encrypt(changePasswordVM.CurrentPassword, psSult);
                tbl_AdminUser data = _db.tbl_AdminUser.Where(x => x.UserName == clsAdminSession.UserName && x.Password == curPwd).FirstOrDefault();

                if (data != null)
                {
                    string newPwd = CommonMethod.Encrypt(changePasswordVM.NewPassword, psSult);
                    data.Password = newPwd;
                    _db.SaveChanges();
                }
                else
                {
                    ViewBag.ErrorMessage = ErrorMessage.InvalidPassword;
                    return View("ChangePassword", changePasswordVM);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
                return RedirectToAction("ChangePassword", "Dashboard");
            }
            return RedirectToAction("Index");
        }
    }
}