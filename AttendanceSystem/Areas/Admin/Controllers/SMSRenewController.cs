using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
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

        public ActionResult Index()
        {
            List<CompanySMSPackRenewVM> companySMSPackRenewVM = new List<CompanySMSPackRenewVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                tbl_Company company = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                clsAdminSession.CurrentSMSPackageId = company.CurrentSMSPackageId.HasValue ? company.CurrentSMSPackageId.Value : 0;

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
                                             NoOfSMS = cp.NoOfSMS
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

        public ActionResult View(long Id)
        {
            CompanySMSPackRenewVM companySMSPackRenewVM = new CompanySMSPackRenewVM();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                companySMSPackRenewVM = (from cp in _db.tbl_CompanySMSPackRenew
                                         where cp.CompanyId == companyId && cp.CompanySMSPackRenewId == Id
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

        public PartialViewResult CreateRazorPaymentOrder(decimal Amount, string description)
        {
            string RazorPayKey = string.Empty;
            string RazorPaySecretKey = string.Empty;
            string IsRazorPayTestMode = ConfigurationManager.AppSettings["IsRazorPayTestMode"];

            if (IsRazorPayTestMode == "true")
            {
                RazorPayKey = ConfigurationManager.AppSettings["RazorPayTestKey"];
                RazorPaySecretKey = ConfigurationManager.AppSettings["RazorPayTestSecretKey"];
            }
            else
            {
                RazorPayKey = ConfigurationManager.AppSettings["RazorPayLiveKey"];
                RazorPaySecretKey = ConfigurationManager.AppSettings["RazorPayLiveSecretKey"];
            }

            Dictionary<string, object> input = new Dictionary<string, object>();
            input.Add("amount", Amount * 100); // this amount should be same as transaction amount
            input.Add("currency", "INR");
            input.Add("receipt", "12121");
            input.Add("payment_capture", 1);

            RazorpayClient client = new RazorpayClient(RazorPayKey, RazorPaySecretKey);

            Razorpay.Api.Order order = client.Order.Create(input);
            ViewBag.OrderId = order["id"];
            ViewBag.Description = description;
            ViewBag.Amount = Amount * 100;
            ViewBag.Key = RazorPayKey;

            long loggedInUserId = clsAdminSession.UserID;
            tbl_AdminUser objAdminUser = _db.tbl_AdminUser.Where(x => x.AdminUserId == loggedInUserId).FirstOrDefault();

            ViewBag.FullName = objAdminUser.FirstName + " " + objAdminUser.LastName;
            ViewBag.EmailId = objAdminUser.EmailId;
            ViewBag.MobileNo = objAdminUser.MobileNo;

            return PartialView("~/Areas/Admin/Views/SMSRenew/_RazorPayPayment.cshtml");
        }

        public void DownloadInvoice(long id)
        {
            string response = "failed";
            try
            {
                InvoiceFieldsVM invoiceFieldsVM = (from pkg in _db.tbl_CompanySMSPackRenew
                                                   join cmp in _db.tbl_Company on pkg.CompanyId equals cmp.CompanyId
                                                   join ur in _db.tbl_AdminUser on cmp.CompanyCode equals ur.UserName
                                                   where pkg.CompanySMSPackRenewId == id
                                                   select new InvoiceFieldsVM
                                                   {
                                                       CompanyName = cmp.CompanyName,
                                                       InvoiceNo = pkg.InvoiceNo,
                                                       CustomerName = ur.FirstName + " " + ur.LastName,
                                                       InvoiceDatetime = pkg.CreatedDate,
                                                       Address = cmp.Address,
                                                       Phone = cmp.ContactNo,
                                                       GstNo = cmp.GSTNo,
                                                       PanNo = cmp.PanNo,
                                                       PackageName = pkg.SMSPackageName,
                                                       GSTRate = pkg.GSTPer.HasValue ? pkg.GSTPer.Value : 0,
                                                       TotalAmount = pkg.PackageAmount,
                                                       State = cmp.State
                                                   }).FirstOrDefault();

                invoiceFieldsVM.InvoiceDate = invoiceFieldsVM.InvoiceDatetime.ToString("dd MMM yyyy");
                invoiceFieldsVM.HsnCode = "9982";
                bool isCGSTSGSTApply = invoiceFieldsVM.State.ToLower() == ErrorMessage.Gujarat.ToLower();

                invoiceFieldsVM.Rate = Math.Round((invoiceFieldsVM.TotalAmount / (100 + invoiceFieldsVM.GSTRate)) * 100, 2);
                decimal gstAmount = Math.Round(invoiceFieldsVM.Rate * invoiceFieldsVM.GSTRate / 100, 2);
                invoiceFieldsVM.CGST = isCGSTSGSTApply ? Math.Round(gstAmount / 2, 2) : 0;
                invoiceFieldsVM.SGST = isCGSTSGSTApply ? Math.Round(gstAmount / 2, 2) : 0;
                invoiceFieldsVM.IGST = !isCGSTSGSTApply ? gstAmount : 0;
                invoiceFieldsVM.Qty = 1;

                string htmlContent = CommonMethod.GetInvoiceContent(invoiceFieldsVM);


                StringReader sr = new StringReader(htmlContent);
                Document pdfDoc1 = new Document(PageSize.A4, 10f, 10f, 10f, 0f);
                PdfWriter writer1 = PdfWriter.GetInstance(pdfDoc1, Response.OutputStream);
                pdfDoc1.Open();
                XMLWorkerHelper.GetInstance().ParseXHtml(writer1, pdfDoc1, sr);
                pdfDoc1.Close();
                Response.ContentType = "application/pdf";
                Response.AddHeader("content-disposition", "attachment;filename=" + invoiceFieldsVM.InvoiceNo + ".pdf");
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Write(pdfDoc1);
                Response.End();


                response = "success";
            }
            catch (Exception ex)
            { response = ex.Message; }
            //return response;
        }

    }
}