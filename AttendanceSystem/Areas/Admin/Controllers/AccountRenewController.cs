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
    public class AccountRenewController : Controller
    {
        AttendanceSystemEntities _db;
        public AccountRenewController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index()
        {
            List<CompanyRenewPaymentVM> companyRenewPaymentVM = new List<CompanyRenewPaymentVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                tbl_Company company = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                clsAdminSession.CurrentAccountPackageId = company.CurrentPackageId.HasValue ? company.CurrentPackageId.Value : 0;

                companyRenewPaymentVM = (from cp in _db.tbl_CompanyRenewPayment
                                         where cp.CompanyId == companyId
                                         select new CompanyRenewPaymentVM
                                         {
                                             CompanyRegistrationPaymentId = cp.CompanyRegistrationPaymentId,
                                             CompanyId = cp.CompanyId,
                                             Amount = cp.Amount,
                                             PaymentFor = cp.PaymentFor,
                                             PaymentGatewayResponseId = cp.PaymentGatewayResponseId,
                                             StartDate = cp.StartDate,
                                             EndDate = cp.EndDate,
                                             AccessDays = cp.AccessDays,
                                             PackageId = cp.PackageId,
                                             PackageName = cp.PackageName,
                                             NoOfEmployee = cp.NoOfEmployee,
                                             NoOfSMS = cp.NoOfSMS,
                                             CreatedDate = cp.CreatedDate
                                         }).OrderByDescending(x => x.CompanyRegistrationPaymentId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRenewPaymentVM);
        }

        public ActionResult Buy()
        {
            List<PackageVM> lstAccountPackages = new List<PackageVM>();
            try
            {
                ViewBag.StripePublishKey = ConfigurationManager.AppSettings["stripePublishableKey"];
                lstAccountPackages = (from pck in _db.tbl_Package
                                      where !pck.IsDeleted && pck.IsActive
                                      select new PackageVM
                                      {
                                          PackageId = pck.PackageId,
                                          PackageName = pck.PackageName,
                                          Amount = pck.Amount,
                                          PackageDescription = pck.PackageDescription,
                                          AccessDays = pck.AccessDays,
                                          IsActive = pck.IsActive,
                                          PackageImage = pck.PackageImage,
                                          NoOfSMS = pck.NoOfSMS,
                                          NoOfEmployee = pck.NoOfEmployee,
                                          PackageColorCode = pck.PackageColorCode,
                                          PackageFontIcon = pck.PackageFontIcon
                                      }).OrderByDescending(x => x.PackageId).ToList();

                ViewData["lstAccountPackages"] = lstAccountPackages;

            }
            catch (Exception ex)
            {
            }

            return View();
        }

        [HttpPost]
        public JsonResult GetSelectedAccountPackageDetail(long Id)
        {
            PackageVM objPackage = new PackageVM();
            try
            {
                objPackage = (from pkg in _db.tbl_Package
                              where pkg.PackageId == Id && !pkg.IsDeleted
                              select new PackageVM
                              {
                                  PackageId = pkg.PackageId,
                                  PackageName = pkg.PackageName,
                                  Amount = pkg.Amount,
                                  PackageDescription = pkg.PackageDescription,
                                  AccessDays = pkg.AccessDays,
                                  PackageImage = pkg.PackageImage,
                                  IsActive = pkg.IsActive,
                                  NoOfSMS = pkg.NoOfSMS,
                                  NoOfEmployee = pkg.NoOfEmployee
                              }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return Json(objPackage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult BuySelectedAccountPackage(PackageBuyVM packageBuyVM)
        {
            string message = string.Empty;
            bool isSuccess = true;
            long companyId = clsAdminSession.CompanyId;
            DateTime today = CommonMethod.CurrentIndianDateTime();
            long loggedInUserId = (int)PaymentGivenBy.CompanyAdmin;
            try
            {
                if (packageBuyVM != null && packageBuyVM.PackageId != 0)
                {
                    // Get general setting detail
                    tbl_Setting objSetting = _db.tbl_Setting.FirstOrDefault();

                    // Get selected package detail
                    tbl_Package objPackage = _db.tbl_Package.Where(x => x.PackageId == packageBuyVM.PackageId).FirstOrDefault();

                    // Get last bought package of company
                    tbl_CompanyRenewPayment objLastBoughtPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId)
                                                                                                .OrderByDescending(x => x.CompanyRegistrationPaymentId).FirstOrDefault();

                    if (objPackage != null)
                    {
                        string invoiceNo = GenerateAccountRenewInvoiceNo();
                        tbl_CompanyRenewPayment objCompanyRenewPayment = new tbl_CompanyRenewPayment();
                        objCompanyRenewPayment.CompanyId = companyId;
                        objCompanyRenewPayment.PackageId = objPackage.PackageId;
                        objCompanyRenewPayment.PackageName = objPackage.PackageName;
                        objCompanyRenewPayment.StartDate = today;
                        objCompanyRenewPayment.EndDate = today.AddDays(objPackage.AccessDays);
                        objCompanyRenewPayment.Amount = objPackage.Amount;
                        objCompanyRenewPayment.GSTPer = (objSetting != null && objSetting.AccountPackageBuyGSTPer != null ? objSetting.AccountPackageBuyGSTPer.Value : 0);
                        objCompanyRenewPayment.AccessDays = objPackage.AccessDays;
                        objCompanyRenewPayment.NoOfEmployee = objPackage.NoOfEmployee;
                        objCompanyRenewPayment.NoOfSMS = objPackage.NoOfSMS;
                        objCompanyRenewPayment.PaymentFor = "Account Renew";
                        objCompanyRenewPayment.PaymentGatewayResponseId = "";
                        objCompanyRenewPayment.InvoiceNo = invoiceNo;
                        objCompanyRenewPayment.CreatedBy = loggedInUserId;
                        objCompanyRenewPayment.CreatedDate = today;
                        _db.tbl_CompanyRenewPayment.Add(objCompanyRenewPayment);
                        _db.SaveChanges();


                        tbl_Company companyObj = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                        if (companyObj.IsTrialMode)
                        {

                            tbl_CompanySMSPackRenew objCompanySMSPackRenew = new tbl_CompanySMSPackRenew();
                            objCompanySMSPackRenew.CompanyId = companyObj.CompanyId;
                            objCompanySMSPackRenew.SMSPackageId = objPackage.PackageId;
                            objCompanySMSPackRenew.SMSPackageName = objPackage.PackageName;
                            objCompanySMSPackRenew.RenewDate = today;
                            objCompanySMSPackRenew.PackageAmount = objPackage.Amount;
                            objCompanySMSPackRenew.AccessDays = objPackage.AccessDays;
                            objCompanySMSPackRenew.PackageExpiryDate = today.AddDays(objPackage.AccessDays);
                            objCompanySMSPackRenew.NoOfSMS = objPackage.NoOfSMS;
                            objCompanySMSPackRenew.RemainingSMS = objPackage.NoOfSMS;
                            objCompanySMSPackRenew.InvoiceNo = invoiceNo;
                            objCompanySMSPackRenew.GSTPer = (objSetting != null && objSetting.SMSPackageBuyGSTPer != null ? objSetting.SMSPackageBuyGSTPer.Value : 0);
                            objCompanySMSPackRenew.CreatedBy = loggedInUserId;
                            objCompanySMSPackRenew.CreatedDate = today;
                            objCompanySMSPackRenew.ModifiedBy = loggedInUserId;
                            objCompanySMSPackRenew.ModifiedDate = today;
                            _db.tbl_CompanySMSPackRenew.Add(objCompanySMSPackRenew);
                            _db.SaveChanges();


                            companyObj.IsTrialMode = false;
                            companyObj.AccountExpiryDate = objCompanyRenewPayment.EndDate;
                            companyObj.CurrentPackageId = objCompanyRenewPayment.CompanyRegistrationPaymentId;
                            companyObj.CurrentSMSPackageId = Convert.ToInt32(objCompanySMSPackRenew.CompanySMSPackRenewId);
                            _db.SaveChanges();

                            #region checkEmployee
                            ActiveDeactiveEmployeeFromPackage(companyObj.CompanyId, objCompanyRenewPayment.NoOfEmployee);
                            #endregion checkEmployee
                        }
                        else
                        {
                            if (objLastBoughtPackage == null || (objLastBoughtPackage != null && objLastBoughtPackage.EndDate < today))
                            {
                                tbl_CompanySMSPackRenew objCompanySMSPackRenew = new tbl_CompanySMSPackRenew();
                                objCompanySMSPackRenew.CompanyId = companyObj.CompanyId;
                                objCompanySMSPackRenew.SMSPackageId = objPackage.PackageId;
                                objCompanySMSPackRenew.SMSPackageName = objPackage.PackageName;
                                objCompanySMSPackRenew.RenewDate = today;
                                objCompanySMSPackRenew.PackageAmount = objPackage.Amount;
                                objCompanySMSPackRenew.AccessDays = objPackage.AccessDays;
                                objCompanySMSPackRenew.PackageExpiryDate = today.AddDays(objPackage.AccessDays);
                                objCompanySMSPackRenew.NoOfSMS = objPackage.NoOfSMS;
                                objCompanySMSPackRenew.RemainingSMS = objPackage.NoOfSMS;
                                objCompanySMSPackRenew.InvoiceNo = invoiceNo;
                                objCompanySMSPackRenew.GSTPer = (objSetting != null && objSetting.SMSPackageBuyGSTPer != null ? objSetting.SMSPackageBuyGSTPer.Value : 0);
                                objCompanySMSPackRenew.CreatedBy = loggedInUserId;
                                objCompanySMSPackRenew.CreatedDate = today;
                                objCompanySMSPackRenew.ModifiedBy = loggedInUserId;
                                objCompanySMSPackRenew.ModifiedDate = today;
                                _db.tbl_CompanySMSPackRenew.Add(objCompanySMSPackRenew);
                                _db.SaveChanges();

                                companyObj.AccountExpiryDate = objCompanyRenewPayment.EndDate;
                                companyObj.CurrentPackageId = objCompanyRenewPayment.CompanyRegistrationPaymentId;
                                companyObj.CurrentSMSPackageId = companyObj.CurrentSMSPackageId == null ? Convert.ToInt32(objCompanySMSPackRenew.CompanySMSPackRenewId) : companyObj.CurrentSMSPackageId;
                                _db.SaveChanges();

                                #region checkEmployee
                                ActiveDeactiveEmployeeFromPackage(companyObj.CompanyId, objCompanyRenewPayment.NoOfEmployee);
                                #endregion checkEmployee
                            }
                        }
                        isSuccess = true;
                        message = ErrorMessage.AccountPackageBuySuccessfully;
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
            CompanyRenewPaymentVM companyRenewPaymentVM = new CompanyRenewPaymentVM();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                companyRenewPaymentVM = (from cp in _db.tbl_CompanyRenewPayment
                                         join cm in _db.tbl_Company on cp.CompanyId equals cm.CompanyId
                                         where cp.CompanyId == companyId
                                         && cp.CompanyRegistrationPaymentId == id
                                         select new CompanyRenewPaymentVM
                                         {
                                             CompanyRegistrationPaymentId = cp.CompanyRegistrationPaymentId,
                                             CompanyId = cp.CompanyId,
                                             CompanyName = cm.CompanyName,
                                             Amount = cp.Amount,
                                             PaymentFor = cp.PaymentFor,
                                             PaymentGatewayResponseId = cp.PaymentGatewayResponseId,
                                             StartDate = cp.StartDate,
                                             EndDate = cp.EndDate,
                                             AccessDays = cp.AccessDays,
                                             PackageId = cp.PackageId,
                                             PackageName = cp.PackageName,
                                             NoOfEmployee = cp.NoOfEmployee,
                                             NoOfSMS = cp.NoOfSMS,
                                             CreatedDate = cp.CreatedDate,
                                             BuyNoOfEmployee = cp.BuyNoOfEmployee
                                         }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRenewPaymentVM);
        }

        public string GenerateAccountRenewInvoiceNo()
        {
            string invoiceNo = string.Empty;
            long newNo = 0;
            tbl_InvoiceLastDocNo lastDocNoObj = _db.tbl_InvoiceLastDocNo.Where(x => x.PackageType == (int)SalesReportType.Account).FirstOrDefault();

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
            invoiceNo = ErrorMessage.InvoicePrefix + ErrorMessage.InvoiceNoSaperator + yearString + ErrorMessage.InvoiceNoSaperator + ErrorMessage.AccountInvoiceNoPrefix + ErrorMessage.InvoiceNoSaperator + numberInStringFormat;

            if (lastDocNoObj == null)
            {
                lastDocNoObj = new tbl_InvoiceLastDocNo();
                lastDocNoObj.PackageType = (int)SalesReportType.Account;
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

            return PartialView("~/Areas/Admin/Views/AccountRenew/_RazorPayPayment.cshtml");
        }

        public void DownloadInvoice(long id)
        {
            string response = "failed";
            try
            {
                InvoiceFieldsVM invoiceFieldsVM = (from pkg in _db.tbl_CompanyRenewPayment
                                                   join cmp in _db.tbl_Company on pkg.CompanyId equals cmp.CompanyId
                                                   join ur in _db.tbl_AdminUser on cmp.CompanyCode equals ur.UserName
                                                   where pkg.CompanyRegistrationPaymentId == id
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
                                                       PackageName = pkg.PackageName,
                                                       GSTRate = pkg.GSTPer,
                                                       TotalAmount = pkg.Amount,
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

        private void ActiveDeactiveEmployeeFromPackage(long companyId, int allowedEmployees)
        {
            #region checkEmployee
            DateTime today = CommonMethod.CurrentIndianDateTime();
            long loggedInUserId = (int)PaymentGivenBy.CompanyAdmin;

            List<tbl_Employee> totalEmployeeList = _db.tbl_Employee.Where(x => x.CompanyId == companyId && !x.IsDeleted).ToList();
            int activeEmployee = totalEmployeeList.Where(x => x.IsActive).Count();

            if (allowedEmployees > activeEmployee)
            {
                int employeeToActive = allowedEmployees - activeEmployee;
                if (employeeToActive > 0)
                {
                    List<tbl_Employee> employeeListToBeActive = totalEmployeeList.Where(x => !x.IsActive).OrderBy(x => x.EmployeeId).Take(employeeToActive).ToList();
                    if (employeeListToBeActive.Count > 0)
                    {
                        employeeListToBeActive.ForEach(x =>
                        {
                            x.IsActive = true;
                            x.UpdatedBy = loggedInUserId;
                            x.UpdatedDate = today;
                            _db.SaveChanges();
                        });
                    }
                }
            }
            else
            {
                int employeeToDeActive = activeEmployee - allowedEmployees;
                if (employeeToDeActive > 0)
                {
                    List<tbl_Employee> employeeListToBeDeActive = totalEmployeeList.Where(x => x.IsActive).OrderBy(x => x.EmployeeId).Take(employeeToDeActive).ToList();
                    if (employeeListToBeDeActive.Count > 0)
                    {
                        employeeListToBeDeActive.ForEach(x =>
                        {
                            x.IsActive = false;
                            x.UpdatedBy = loggedInUserId;
                            x.UpdatedDate = today;
                            _db.SaveChanges();
                        });
                    }
                }
            }


            #endregion checkEmployee
        }
    }
}