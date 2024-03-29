﻿using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static AttendanceSystem.ViewModel.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class SettingController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public string serviceImageDirectoryPath = "";
        public string homeImageDirectoryPath = "";
        public string heroImageDirectoryPath = "";
        string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();

        public SettingController()
        {
            _db = new AttendanceSystemEntities();
            serviceImageDirectoryPath = ErrorMessage.ServiceDirectoryPath;
            homeImageDirectoryPath = ErrorMessage.HomeDirectoryPath;
            heroImageDirectoryPath = ErrorMessage.HeroDirectoryPath;
        }

        public ActionResult Index()
        {
            SuperAdminSettingVM objSASetting = new SuperAdminSettingVM();
            CompanyAdminSettingVM objCASetting = new CompanyAdminSettingVM();

            int loggedUserRoleId = clsAdminSession.RoleID;

            try
            {
                if (loggedUserRoleId == (int)AdminRoles.SuperAdmin)
                {
                    tbl_Setting setting = _db.tbl_Setting.FirstOrDefault();
                    if (setting != null)
                    {

                        // Super Admin Setting
                        objSASetting.AccountFreeAccessDays = (int)setting.AccountFreeAccessDays;
                        objSASetting.AmountPerEmp = (decimal)setting.AmountPerEmp;
                        objSASetting.AccountPackageBuyGSTPer = (decimal)setting.AccountPackageBuyGSTPer;
                        objSASetting.SMSPackageBuyGSTPer = (decimal)setting.SMSPackageBuyGSTPer;
                        objSASetting.EmployeeBuyGSTPer = (decimal)setting.EmployeeBuyGSTPer;
                        objSASetting.AddVideoUrl = setting.AddVideoUrl;
                        objSASetting.AddVideoDescription = setting.AddVideoDescription;

                        objSASetting.SMTPHost = setting.SMTPHost;
                        objSASetting.SMTPPort = setting.SMTPPort;
                        objSASetting.SMTPEmail = setting.SMTPEmail;
                        objSASetting.SMTPPassword = setting.SMTPPassword;
                        objSASetting.SMTPEnableSSL = setting.SMTPEnableSSL;
                        objSASetting.SMTPFromEmailId = setting.SMTPFromEmailId;
                        objSASetting.SuperAdminEmailId = setting.SuperAdminEmailId;
                        objSASetting.SuperAdminMobileNo = setting.SuperAdminMobileNo;

                        objSASetting.ServiceImage = setting.ServiceImage;
                        objSASetting.HomeImage = setting.HomeImage;
                        objSASetting.HomeImage2 = setting.HomeImage2;

                        objSASetting.HeroAboutPageImageName = setting.HeroAboutPageImageName;
                        objSASetting.HeroContactPageImageName = setting.HeroContactPageImageName;
                        objSASetting.HeroTermsConditionPageImageName = setting.HeroTermsConditionPageImageName;
                        objSASetting.HeroFAQPageImageName = setting.HeroFAQPageImageName;
                        objSASetting.HeroServicePageImageName = setting.HeroServicePageImageName;
                        objSASetting.HeroPrivacyPolicyPageImageName = setting.HeroPrivacyPolicyPageImageName;
                        objSASetting.HeroHowToUsePageImageName = setting.HeroHowToUsePageImageName;
                        objSASetting.HeroCompanyRequestPageImageName = setting.HeroCompanyRequestPageImageName;

                        ViewData["objSASetting"] = objSASetting;

                    }
                }
                else
                {
                    // Company Admin Setting
                    long companyId = clsAdminSession.CompanyId;

                    tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    if (objCompany != null)
                    {
                        objCASetting.NoOfLunchBreakAllowed = objCompany.NoOfLunchBreakAllowed;
                        objCASetting.SiteLocationAccessPassword = objCompany.SiteLocationAccessPassword;
                        objCASetting.OfficeLocationAccessPassword = objCompany.OfficeLocationAccessPassword;
                        objCASetting.CompanyConversionType = objCompany.CompanyConversionType;
                    }

                    ViewData["objCASetting"] = objCASetting;
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
                                                        AccountFreeAccessDays = (int)s.AccountFreeAccessDays,
                                                        AmountPerEmp = (decimal)s.AmountPerEmp,
                                                        AccountPackageBuyGSTPer = (decimal)s.AccountPackageBuyGSTPer,
                                                        SMSPackageBuyGSTPer = (decimal)s.SMSPackageBuyGSTPer,
                                                        EmployeeBuyGSTPer = (decimal)s.EmployeeBuyGSTPer,
                                                        AddVideoUrl = s.AddVideoUrl,
                                                        AddVideoDescription = s.AddVideoDescription,

                                                        SMTPHost = s.SMTPHost,
                                                        SMTPPort = s.SMTPPort,
                                                        SMTPEmail = s.SMTPEmail,
                                                        SMTPPassword = s.SMTPPassword,
                                                        SMTPEnableSSL = s.SMTPEnableSSL,
                                                        SMTPFromEmailId = s.SMTPFromEmailId,

                                                        SuperAdminEmailId = s.SuperAdminEmailId,
                                                        SuperAdminMobileNo = s.SuperAdminMobileNo,

                                                        HomeImage = s.HomeImage,
                                                        HomeImage2 = s.HomeImage2,
                                                        ServiceImage = s.ServiceImage,

                                                        HeroAboutPageImageName = s.HeroAboutPageImageName,
                                                        HeroContactPageImageName = s.HeroContactPageImageName,
                                                        HeroTermsConditionPageImageName = s.HeroTermsConditionPageImageName,
                                                        HeroFAQPageImageName = s.HeroFAQPageImageName,
                                                        HeroServicePageImageName = s.HeroServicePageImageName,
                                                        HeroPrivacyPolicyPageImageName = s.HeroPrivacyPolicyPageImageName,
                                                        HeroHowToUsePageImageName = s.HeroHowToUsePageImageName,
                                                        HeroCompanyRequestPageImageName = s.HeroCompanyRequestPageImageName,

                                                    }).FirstOrDefault();

                return View("~/Areas/Admin/Views/Setting/EditSettingSA.cshtml", objSASetting);
            }
            else
            {
                CompanyAdminSettingVM objCASetting = new CompanyAdminSettingVM();
                long companyId = clsAdminSession.CompanyId;

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                if (objCompany != null)
                {
                    objCASetting.NoOfLunchBreakAllowed = objCompany.NoOfLunchBreakAllowed;
                    objCASetting.SiteLocationAccessPassword = objCompany.SiteLocationAccessPassword;
                    objCASetting.OfficeLocationAccessPassword = objCompany.OfficeLocationAccessPassword;
                    objCASetting.CompanyConversionType = objCompany.CompanyConversionType;
                }

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
        public JsonResult ValidateCurrentPassword(string currentPassword)
        {
            bool IsValid = false;
            string sentOTP = string.Empty;
            try
            {
                string curPwd = CommonMethod.Encrypt(currentPassword, psSult);
                tbl_AdminUser objUser = _db.tbl_AdminUser.Where(x => x.UserName == clsAdminSession.UserName && x.Password == curPwd).FirstOrDefault();
                if (objUser != null)
                {
                    sentOTP = SendChangePasswordOTP(objUser);
                    IsValid = true;
                }
            }
            catch (Exception ex)
            {
                IsValid = false;
            }

            return Json(new { IsValid = IsValid, OTP = sentOTP }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string ChangePassword(string NewPassword)
        {
            string message = string.Empty;
            try
            {
                tbl_AdminUser data = _db.tbl_AdminUser.Where(x => x.UserName == clsAdminSession.UserName).FirstOrDefault();

                if (data != null)
                {
                    string newPwd = CommonMethod.Encrypt(NewPassword, psSult);
                    data.Password = newPwd;
                    data.ModifiedBy = clsAdminSession.RoleID == (int)AdminRoles.SuperAdmin ? (int)PaymentGivenBy.SuperAdmin : (int)PaymentGivenBy.CompanyAdmin;
                    data.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.SaveChanges();

                    message = "success";
                }
                else
                {
                    message = "notfound";
                }
            }
            catch (Exception ex)
            {
                message = "exception";
            }
            return message;
        }

        [HttpPost]
        public ActionResult EditSuperAdminSetting(SuperAdminSettingVM settingVM,
            HttpPostedFileBase ServiceImageFile,
            HttpPostedFileBase HomeImageFile,
            HttpPostedFileBase HomeImageFile2,

            HttpPostedFileBase HeroAboutPageImageFile,
            HttpPostedFileBase HeroContactPageImageFile,
            HttpPostedFileBase HeroTermsConditionPageImageFile,
            HttpPostedFileBase HeroFAQPageImageFile,
            HttpPostedFileBase HeroServicePageImageFile,
            HttpPostedFileBase HeroPrivacyPolicyPageImageFile,
            HttpPostedFileBase HeroHowToUsePageImageFile,
            HttpPostedFileBase HeroCompanyRequestPageImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    // Get Setting record
                    tbl_Setting objSetting = _db.tbl_Setting.FirstOrDefault();

                    #region Upload HERO - About Page Image

                    string heroAboutPageFileName = objSetting.HeroAboutPageImageName;
                    string heroPath = Server.MapPath(heroImageDirectoryPath);

                    bool folderExists = Directory.Exists(heroPath);
                    if (!folderExists)
                        Directory.CreateDirectory(heroPath);

                    if (HeroAboutPageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroAboutPageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroAboutPageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroAboutPageFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroAboutPageImageFile.FileName);
                        HeroAboutPageImageFile.SaveAs(heroPath + heroAboutPageFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroAboutPageImageName))
                        {
                            ModelState.AddModelError("HeroAboutPageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload HERO - Contact Page Image

                    string heroContactPageFileName = objSetting.HeroContactPageImageName;

                    if (HeroContactPageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroContactPageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroContactPageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroContactPageFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroContactPageImageFile.FileName);
                        HeroContactPageImageFile.SaveAs(heroPath + heroContactPageFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroContactPageImageName))
                        {
                            ModelState.AddModelError("HeroContactPageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload HERO - Terms Condition Page Image

                    string heroTermsConditionPageFileName = objSetting.HeroTermsConditionPageImageName;

                    if (HeroTermsConditionPageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroTermsConditionPageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroTermsConditionPageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroTermsConditionPageFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroTermsConditionPageImageFile.FileName);
                        HeroTermsConditionPageImageFile.SaveAs(heroPath + heroTermsConditionPageFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroTermsConditionPageImageName))
                        {
                            ModelState.AddModelError("HeroTermsConditionPageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload HERO - FAQ Page Image

                    string heroFAQPageFileName = objSetting.HeroFAQPageImageName;

                    if (HeroFAQPageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroFAQPageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroFAQPageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroFAQPageFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroFAQPageImageFile.FileName);
                        HeroFAQPageImageFile.SaveAs(heroPath + heroFAQPageFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroFAQPageImageName))
                        {
                            ModelState.AddModelError("HeroFAQPageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload HERO - Service Page Image

                    string heroServicePageFileName = objSetting.HeroServicePageImageName;

                    if (HeroServicePageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroServicePageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroServicePageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroServicePageFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroServicePageImageFile.FileName);
                        HeroServicePageImageFile.SaveAs(heroPath + heroServicePageFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroServicePageImageName))
                        {
                            ModelState.AddModelError("HeroServicePageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload HERO - Privacy Policy Page Image

                    string heroPrivacyPolicyPageFileName = objSetting.HeroPrivacyPolicyPageImageName;

                    if (HeroPrivacyPolicyPageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroPrivacyPolicyPageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroPrivacyPolicyPageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroPrivacyPolicyPageFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroPrivacyPolicyPageImageFile.FileName);
                        HeroPrivacyPolicyPageImageFile.SaveAs(heroPath + heroPrivacyPolicyPageFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroPrivacyPolicyPageImageName))
                        {
                            ModelState.AddModelError("HeroPrivacyPolicyPageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload HERO - How to Use Page Image

                    string heroHowToUsePageFileName = objSetting.HeroHowToUsePageImageName;

                    if (HeroHowToUsePageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroHowToUsePageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroHowToUsePageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroHowToUsePageFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroHowToUsePageImageFile.FileName);
                        HeroHowToUsePageImageFile.SaveAs(heroPath + heroHowToUsePageFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroHowToUsePageImageName))
                        {
                            ModelState.AddModelError("HeroHowToUsePageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload HERO - Company Request

                    string heroCompanyRequestFileName = objSetting.HeroCompanyRequestPageImageName;

                    if (HeroCompanyRequestPageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HeroCompanyRequestPageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HeroCompanyRequestPageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        heroCompanyRequestFileName = Guid.NewGuid() + "-" + Path.GetFileName(HeroCompanyRequestPageImageFile.FileName);
                        HeroCompanyRequestPageImageFile.SaveAs(heroPath + heroCompanyRequestFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HeroCompanyRequestPageImageName))
                        {
                            ModelState.AddModelError("HeroCompanyRequestPageImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload Service Image

                    string servicefileName = objSetting.ServiceImage;
                    string servicePath = Server.MapPath(serviceImageDirectoryPath);

                    if (ServiceImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(ServiceImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("ServiceImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        servicefileName = Guid.NewGuid() + "-" + Path.GetFileName(ServiceImageFile.FileName);
                        ServiceImageFile.SaveAs(servicePath + servicefileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.ServiceImage))
                        {
                            ModelState.AddModelError("ServiceImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload Home Image

                    string homefileName = objSetting.HomeImage;
                    string homePath = Server.MapPath(homeImageDirectoryPath);

                    bool homefolderExists = Directory.Exists(homePath);
                    if (!homefolderExists)
                        Directory.CreateDirectory(homePath);

                    if (HomeImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HomeImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HomeImageFile", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        homefileName = Guid.NewGuid() + "-" + Path.GetFileName(HomeImageFile.FileName);
                        HomeImageFile.SaveAs(homePath + homefileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HomeImage))
                        {
                            ModelState.AddModelError("HomeImageFile", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    #region Upload Home Image 2

                    string homefileName2 = objSetting.HomeImage2;

                    if (HomeImageFile2 != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HomeImageFile2.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HomeImageFile2", ErrorMessage.SelectOnlyImage);
                            return View(settingVM);
                        }

                        // Save file in folder
                        homefileName2 = Guid.NewGuid() + "-" + Path.GetFileName(HomeImageFile2.FileName);
                        HomeImageFile2.SaveAs(homePath + homefileName2);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(objSetting.HomeImage2))
                        {
                            ModelState.AddModelError("HomeImageFile2", ErrorMessage.ImageRequired);
                            return View(settingVM);
                        }
                    }

                    #endregion

                    // Save data

                    objSetting.AccountFreeAccessDays = settingVM.AccountFreeAccessDays;
                    objSetting.AmountPerEmp = settingVM.AmountPerEmp;
                    objSetting.AccountPackageBuyGSTPer = settingVM.AccountPackageBuyGSTPer;
                    objSetting.SMSPackageBuyGSTPer = settingVM.SMSPackageBuyGSTPer;
                    objSetting.EmployeeBuyGSTPer = settingVM.EmployeeBuyGSTPer;
                    objSetting.AddVideoUrl = settingVM.AddVideoUrl;
                    objSetting.AddVideoDescription = settingVM.AddVideoDescription;

                    objSetting.SMTPHost = settingVM.SMTPHost;
                    objSetting.SMTPPort = settingVM.SMTPPort;
                    objSetting.SMTPEmail = settingVM.SMTPEmail;
                    objSetting.SMTPPassword = settingVM.SMTPPassword;
                    objSetting.SMTPEnableSSL = settingVM.SMTPEnableSSL;
                    objSetting.SMTPFromEmailId = settingVM.SMTPFromEmailId;
                    objSetting.SuperAdminEmailId = settingVM.SuperAdminEmailId;
                    objSetting.SuperAdminMobileNo = settingVM.SuperAdminMobileNo;

                    objSetting.ServiceImage = servicefileName;
                    objSetting.HomeImage = homefileName;
                    objSetting.HomeImage2 = homefileName2;

                    objSetting.HeroAboutPageImageName = heroAboutPageFileName;
                    objSetting.HeroContactPageImageName = heroContactPageFileName;
                    objSetting.HeroTermsConditionPageImageName = heroTermsConditionPageFileName;
                    objSetting.HeroFAQPageImageName = heroFAQPageFileName;
                    objSetting.HeroServicePageImageName = heroServicePageFileName;
                    objSetting.HeroPrivacyPolicyPageImageName = heroPrivacyPolicyPageFileName;
                    objSetting.HeroHowToUsePageImageName = heroHowToUsePageFileName;
                    objSetting.HeroCompanyRequestPageImageName = heroCompanyRequestFileName;

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
         
        [HttpPost]
        public ActionResult EditCompanyAdminSetting(CompanyAdminSettingVM settingVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    // Get Setting record
                    long companyId = clsAdminSession.CompanyId;

                    tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    if (objCompany != null)
                    {
                        objCompany.NoOfLunchBreakAllowed = settingVM.NoOfLunchBreakAllowed;
                        objCompany.SiteLocationAccessPassword = settingVM.SiteLocationAccessPassword;
                        objCompany.OfficeLocationAccessPassword = settingVM.OfficeLocationAccessPassword;
                        objCompany.CompanyConversionType = settingVM.CompanyConversionType;
                        _db.SaveChanges();
                    }

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
         
        private string SendChangePasswordOTP(tbl_AdminUser objUser)
        {
            string sentOTP = string.Empty;
            try
            {
                Random random = new Random();
                int num = random.Next(555555, 999999);

                int SmsId = (int)SMSType.ChangePasswordOTP;
                string msg = CommonMethod.GetSmsContent(SmsId);

                Regex regReplace = new Regex("{#var#}");
                msg = regReplace.Replace(msg, objUser.FirstName + " " + objUser.LastName, 1);
                msg = regReplace.Replace(msg, num.ToString(), 1);

                msg = msg.Replace("\r\n", "\n");

                if (clsAdminSession.RoleID == (int)AdminRoles.SuperAdmin)
                {
                    var json = CommonMethod.SendSMSWithoutLog(msg, objUser.MobileNo);
                    if (!json.Contains("invalidnumber"))
                    {
                        sentOTP = num.ToString();
                    }
                }
                else
                {
                    ResponseDataModel<string> json = CommonMethod.SendSMS(msg, objUser.MobileNo, clsAdminSession.CompanyId, (int)PaymentGivenBy.CompanyAdmin, "", (int)PaymentGivenBy.CompanyAdmin, clsAdminSession.IsTrialMode);
                    if (!json.Data.Contains("invalidnumber"))
                    {
                        sentOTP = num.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return sentOTP;
        }

    }
}