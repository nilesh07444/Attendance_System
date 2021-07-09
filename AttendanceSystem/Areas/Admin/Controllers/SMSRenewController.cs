using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class SMSRenewController : Controller
    {
        AttendanceSystemEntities _db;
        public SMSRenewController()
        {
            _db = new AttendanceSystemEntities();
        }
        // GET: Admin/SMSRenew
        public ActionResult Index()
        {
            List<CompanySMSPackRenewVM> companySMSPackRenewVM = new List<CompanySMSPackRenewVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                companySMSPackRenewVM = (from cp in _db.tbl_CompanySMSPackRenew
                                         where cp.CompanyId == companyId
                                         select new CompanySMSPackRenewVM
                                         {
                                             CompanySMSPackRenewId = cp.CompanySMSPackRenewId,
                                             CompanyId = cp.CompanyId,
                                             SMSPackageId = cp.SMSPackageId,
                                             SMSPackageName = cp.SMSPackageName,
                                             RenewDate = cp.RenewDate,
                                             AccessDays = cp.AccessDays,
                                             PackageExpiryDate = cp.PackageExpiryDate,
                                         }).OrderByDescending(x => x.CompanySMSPackRenewId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companySMSPackRenewVM);
        }
        public ActionResult Buy()
        {
            List<SMSPackageVM> lstSMSPackages = new List<SMSPackageVM>();
            try
            {
                lstSMSPackages = (from sms in _db.tbl_SMSPackage
                                  where !sms.IsDeleted && sms.IsActive
                                  select new SMSPackageVM
                                  {
                                      SMSPackageId = sms.SMSPackageId,
                                      PackageName = sms.PackageName,
                                      PackageImage = sms.PackageImage,
                                      PackageAmount = sms.PackageAmount,
                                      Description = sms.Description,
                                      AccessDays = sms.AccessDays,
                                      NoOfSMS = sms.NoOfSMS,
                                      PackageColorCode = sms.PackageColorCode,
                                      PackageFontIcon = sms.PackageFontIcon
                                  }).OrderByDescending(x => x.SMSPackageId).ToList();

                ViewData["lstSMSPackages"] = lstSMSPackages;

            }
            catch (Exception ex)
            {
            }

            return View();
        }


        [HttpPost]
        public JsonResult BuySelectedsmsPackage(PackageBuyVM packageBuyVM)
        {
            string message = string.Empty;
            bool isSuccess = true;

            try
            {
                if (packageBuyVM != null && packageBuyVM.PackageId != 0)
                {

                    // Get selected package detail
                    tbl_SMSPackage objPackage = _db.tbl_SMSPackage.Where(x => x.SMSPackageId == packageBuyVM.PackageId).FirstOrDefault();

                    if (objPackage != null)
                    {
                        tbl_CompanySMSPackRenew objCompanySMSPackRenew = new tbl_CompanySMSPackRenew();
                        objCompanySMSPackRenew.CompanyId = clsAdminSession.CompanyId;
                        objCompanySMSPackRenew.SMSPackageId = objPackage.SMSPackageId;
                        objCompanySMSPackRenew.SMSPackageName = objPackage.PackageName;
                        objCompanySMSPackRenew.RenewDate = CommonMethod.CurrentIndianDateTime();
                        objCompanySMSPackRenew.PackageAmount = objPackage.PackageAmount;
                        objCompanySMSPackRenew.AccessDays = objPackage.AccessDays;
                        objCompanySMSPackRenew.PackageExpiryDate = CommonMethod.CurrentIndianDateTime().AddDays(objPackage.AccessDays);
                        objCompanySMSPackRenew.NoOfSMS = objPackage.NoOfSMS;
                        objCompanySMSPackRenew.RemainingSMS = objPackage.NoOfSMS;
                        objCompanySMSPackRenew.CreatedBy = clsAdminSession.UserID;
                        objCompanySMSPackRenew.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objCompanySMSPackRenew.ModifiedBy = clsAdminSession.UserID;
                        objCompanySMSPackRenew.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_CompanySMSPackRenew.Add(objCompanySMSPackRenew);
                        _db.SaveChanges();

                        isSuccess = true;
                    }
                }
                else
                {
                    message = "Request data is not valid.";
                    isSuccess = false;
                }

            }
            catch (Exception ex)
            {
                isSuccess = false;
                message = ex.Message.ToString();
            }

            return Json(new { IsSuccess = isSuccess, Message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View(long id)
        {
            CompanySMSPackRenewVM companySMSPackRenewVM = new CompanySMSPackRenewVM();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                companySMSPackRenewVM = (from cp in _db.tbl_CompanySMSPackRenew
                                         where cp.CompanyId == companyId
                                         select new CompanySMSPackRenewVM
                                         {
                                             CompanySMSPackRenewId = cp.CompanySMSPackRenewId,
                                             CompanyId = cp.CompanyId,
                                             SMSPackageId = cp.SMSPackageId,
                                             SMSPackageName = cp.SMSPackageName,
                                             RenewDate = cp.RenewDate,
                                             AccessDays = cp.AccessDays,
                                             PackageExpiryDate = cp.PackageExpiryDate,
                                             PackageAmount = cp.PackageAmount,
                                             NoOfSMS = cp.NoOfSMS,
                                             RemainingSMS = cp.RemainingSMS
                                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companySMSPackRenewVM);
        }
    }
}