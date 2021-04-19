using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using static AttendanceSystem.ViewModel.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class SettingController : Controller
    {
        // GET: Admin/Setting
        private readonly AttendanceSystemEntities _db;
        string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        public SettingController()
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
                return RedirectToAction("ChangePassword", "Setting");
            }
            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult Edit()
        {
            return View();
        }

    }
}