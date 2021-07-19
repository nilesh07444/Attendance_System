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
                        // Get setting for GST Per
                        tbl_Setting objSetting = _db.tbl_Setting.FirstOrDefault();

                        string invoiceNo = GenerateSMSRenewInvoiceNo();
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
                        objCompanySMSPackRenew.InvoiceNo = invoiceNo;
                        objCompanySMSPackRenew.GSTPer = (objSetting != null && objSetting.SMSPackageBuyGSTPer != null ? objSetting.SMSPackageBuyGSTPer.Value : 0);
                        objCompanySMSPackRenew.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objCompanySMSPackRenew.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objCompanySMSPackRenew.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objCompanySMSPackRenew.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_CompanySMSPackRenew.Add(objCompanySMSPackRenew);
                        _db.SaveChanges();

                        isSuccess = true;
                        message = ErrorMessage.SMSPackageBuySuccessfully;
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

        public ActionResult RenewList(DateTime? startDate, DateTime? endDate, long? companyId)
        {
            CompanySMSRenewFilterVM companySMSRenewFilterVM = new CompanySMSRenewFilterVM();
            if (companyId.HasValue)
            {
                companySMSRenewFilterVM.CompanyId = companyId;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                companySMSRenewFilterVM.StartDate = startDate.Value;
                companySMSRenewFilterVM.EndDate = endDate.Value;
            }


            try
            {
                companySMSRenewFilterVM.RenewList = (from cp in _db.tbl_CompanySMSPackRenew
                                                     join cmp in _db.tbl_Company on cp.CompanyId equals cmp.CompanyId
                                                     where cp.RenewDate >= companySMSRenewFilterVM.StartDate && cp.RenewDate <= companySMSRenewFilterVM.EndDate
                                                      && (companySMSRenewFilterVM.CompanyId.HasValue ? cp.CompanyId == companySMSRenewFilterVM.CompanyId.Value : true)
                                                     select new CompanySMSPackRenewVM
                                                     {
                                                         CompanySMSPackRenewId = cp.CompanySMSPackRenewId,
                                                         CompanyId = cp.CompanyId,
                                                         CompanyName = cmp.CompanyName,
                                                         SMSPackageId = cp.SMSPackageId,
                                                         SMSPackageName = cp.SMSPackageName,
                                                         RenewDate = cp.RenewDate,
                                                         AccessDays = cp.AccessDays,
                                                         PackageExpiryDate = cp.PackageExpiryDate,
                                                         NoOfSMS = cp.NoOfSMS
                                                     }).OrderByDescending(x => x.CompanySMSPackRenewId).ToList();

                companySMSRenewFilterVM.CompanyList = GetCompanyList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companySMSRenewFilterVM);
        }
        private List<SelectListItem> GetCompanyList()
        {
            List<SelectListItem> lst = (from cmp in _db.tbl_Company
                                        orderby cmp.CompanyId
                                        select new SelectListItem
                                        {
                                            Text = cmp.CompanyName + " (" + cmp.CompanyCode + ")",
                                            Value = cmp.CompanyId.ToString()
                                        }).ToList();
            return lst;
        }

        public string GenerateSMSRenewInvoiceNo()
        {
            string invoiceNo = string.Empty;
            long newNo = 0;
            tbl_InvoiceLastDocNo lastDocNoObj = _db.tbl_InvoiceLastDocNo.Where(x => x.PackageType == (int)SalesReportType.SMS).FirstOrDefault();

            if (lastDocNoObj != null)
            {
                newNo = lastDocNoObj.LastDocNo + 1;
            }
            else
            {
                newNo = 1;
            }
            string numberInStringFormat = String.Format("{0:0000}", newNo);
            string yearString = CommonMethod.InvoiceFinancialYear();
            invoiceNo = ErrorMessage.InvoicePrefix + ErrorMessage.InvoiceNoSaperator + yearString + ErrorMessage.InvoiceNoSaperator + ErrorMessage.SMSInvoiceNoPrefix + ErrorMessage.InvoiceNoSaperator + numberInStringFormat;

            if (lastDocNoObj == null)
            {
                lastDocNoObj = new tbl_InvoiceLastDocNo();
                lastDocNoObj.PackageType = (int)SalesReportType.SMS;
                lastDocNoObj.LastDocNo = newNo;
                _db.tbl_InvoiceLastDocNo.Add(lastDocNoObj);
                _db.SaveChanges();
            }
            else
            {
                lastDocNoObj.LastDocNo = newNo;
                _db.SaveChanges();
            }
            return invoiceNo;
        }
    }
}