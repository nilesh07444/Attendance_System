using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using static AttendanceSystem.ViewModel.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class SettingController : Controller
    {        
        private readonly AttendanceSystemEntities _db;
        string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        public SettingController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            SuperAdminSettingVM objSASetting = new SuperAdminSettingVM();
            CompanyAdminSettingVM objCASetting = new CompanyAdminSettingVM();

            int loggedUserRoleId = clsAdminSession.RoleID;

            try
            {
                tbl_Setting setting = _db.tbl_Setting.FirstOrDefault();
                if (setting != null)
                {
                    if (loggedUserRoleId == (int)AdminRoles.SuperAdmin)
                    {
                        // Super Admin Setting
                        objSASetting.SiteCompanyFreeAccessDays = (int)setting.SiteCompanyFreeAccessDays;
                        objSASetting.OfficeCompanyFreeAccessDays = (int)setting.OfficeCompanyFreeAccessDays;
                        objSASetting.AmountPerEmp = (decimal)setting.AmountPerEmp;

                        ViewData["objSASetting"] = objSASetting;
                    }
                    else
                    {
                        // Company Admin Setting


                        ViewData["objCASetting"] = objCASetting;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }

        public ActionResult Edit()
        {
            int loggedUserRoleId = clsAdminSession.RoleID;

            if (loggedUserRoleId == (int)AdminRoles.SuperAdmin)
            {
                SuperAdminSettingVM objSASetting = (from s in _db.tbl_Setting
                                                    select new SuperAdminSettingVM
                                                    {
                                                        SettingId = s.SettingId,
                                                        SiteCompanyFreeAccessDays = (int)s.SiteCompanyFreeAccessDays,
                                                        OfficeCompanyFreeAccessDays = (int)s.OfficeCompanyFreeAccessDays,
                                                        AmountPerEmp = (decimal)s.AmountPerEmp
                                                    }).FirstOrDefault();

                return View("~/Areas/Admin/Views/Setting/EditSettingSA.cshtml", objSASetting);
            }
            else
            {
                CompanyAdminSettingVM objCASetting = (from s in _db.tbl_Setting
                                                      select new CompanyAdminSettingVM
                                                      {
                                                          SettingId = s.SettingId,
                                                      }).FirstOrDefault();

                return View("~/Areas/Admin/Views/Setting/EditSettingCA.cshtml", objCASetting);
            }
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

        [HttpPost]
        public ActionResult EditSuperAdminSetting(SuperAdminSettingVM settingVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    tbl_Setting objSetting = _db.tbl_Setting.FirstOrDefault();
                    objSetting.SiteCompanyFreeAccessDays = settingVM.SiteCompanyFreeAccessDays;
                    objSetting.OfficeCompanyFreeAccessDays = settingVM.OfficeCompanyFreeAccessDays;
                    objSetting.AmountPerEmp = settingVM.AmountPerEmp;
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(settingVM);
        }

    }
}