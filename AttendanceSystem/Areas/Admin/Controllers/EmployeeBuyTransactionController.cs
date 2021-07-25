using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using Razorpay.Api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class EmployeeBuyTransactionController : Controller
    {
        AttendanceSystemEntities _db;
        long companyId;
        long loggedInUserId;

        public EmployeeBuyTransactionController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
            loggedInUserId = (int)PaymentGivenBy.CompanyAdmin;
        }

        public ActionResult Index()
        {
            List<EmployeeBuyTransactionVM> employeeBuyTransactionVM = new List<EmployeeBuyTransactionVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                employeeBuyTransactionVM = (from eb in _db.tbl_EmployeeBuyTransaction
                                            where eb.CompanyId == companyId
                                            select new EmployeeBuyTransactionVM
                                            {
                                                EmployeeBuyTransactionId = eb.EmployeeBuyTransactionId,
                                                CompanyId = eb.CompanyId,
                                                NoOfEmpToBuy = eb.NoOfEmpToBuy,
                                                AmountPerEmp = eb.AmountPerEmp,
                                                TotalPaidAmount = eb.TotalPaidAmount,
                                                PaymentGatewayTransactionId = eb.PaymentGatewayTransactionId,
                                                ExpiryDate = eb.ExpiryDate,
                                                CreatedDate = eb.CreatedDate,
                                                RemainingDays = eb.Days
                                            }).OrderByDescending(x => x.EmployeeBuyTransactionId).ToList();

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(employeeBuyTransactionVM);
        }

        public ActionResult Buy()
        {
            DateTime today = CommonMethod.CurrentIndianDateTime();
            EmployeeBuyTransactionVM employeeBuyTransactionVM = new EmployeeBuyTransactionVM();
            tbl_CompanyRenewPayment companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId && today >= x.StartDate && today < x.EndDate).FirstOrDefault();
            if (companyPackage != null && !clsAdminSession.IsTrialMode)
            {
                employeeBuyTransactionVM.CompanyId = companyId;
                employeeBuyTransactionVM.AmountPerEmp = _db.tbl_Setting.FirstOrDefault().AmountPerEmp.Value;
                employeeBuyTransactionVM.RemainingDays = (int)(companyPackage.EndDate - DateTime.Today).TotalDays;
            }
            else
            {
                employeeBuyTransactionVM.ErrorMessage = ErrorMessage.PleaseBuyAccountPackage;
            }
            return View(employeeBuyTransactionVM);
        }

        [HttpPost]
        public JsonResult Buy(EmployeeBuyTransactionVM employeeBuyTransactionVM)
        {
            string message = string.Empty;
            bool isSuccess = true;
            DateTime today = CommonMethod.CurrentIndianDateTime();
            try
            {
                if (employeeBuyTransactionVM != null)
                {
                    // Get setting for GST Per
                    tbl_Setting objSetting = _db.tbl_Setting.FirstOrDefault();

                    // Get selected package detail
                    tbl_CompanyRenewPayment companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId && x.StartDate <= today && x.EndDate >= today
                    && (clsAdminSession.CurrentAccountPackageId > 0 ? x.CompanyRegistrationPaymentId == clsAdminSession.CurrentAccountPackageId : true)).FirstOrDefault();

                    string invoiceNo = GenerateEmployeeBuyInvoiceNo();

                    tbl_EmployeeBuyTransaction employeeBuyTransaction = new tbl_EmployeeBuyTransaction();
                    employeeBuyTransaction.CompanyId = clsAdminSession.CompanyId;
                    employeeBuyTransaction.NoOfEmpToBuy = employeeBuyTransactionVM.NoOfEmpToBuy;
                    employeeBuyTransaction.Days = employeeBuyTransactionVM.RemainingDays;
                    employeeBuyTransaction.GSTPer = (objSetting != null && objSetting.EmployeeBuyGSTPer != null ? objSetting.EmployeeBuyGSTPer : 0);
                    employeeBuyTransaction.AmountPerEmp = employeeBuyTransactionVM.AmountPerEmp;
                    employeeBuyTransaction.TotalPaidAmount = employeeBuyTransactionVM.TotalPaidAmount;
                    employeeBuyTransaction.ExpiryDate = companyPackage.EndDate;
                    employeeBuyTransaction.InvoiceNo = invoiceNo;
                    employeeBuyTransaction.CreatedDate = today;
                    employeeBuyTransaction.CreatedBy = loggedInUserId;
                    employeeBuyTransaction.ModifiedDate = today;
                    employeeBuyTransaction.ModifiedBy = loggedInUserId;
                    _db.tbl_EmployeeBuyTransaction.Add(employeeBuyTransaction);
                    _db.SaveChanges();

                    companyPackage.BuyNoOfEmployee = companyPackage.BuyNoOfEmployee + employeeBuyTransaction.NoOfEmpToBuy;
                    _db.SaveChanges();

                    List<tbl_Employee> totalEmployeeList = _db.tbl_Employee.Where(x => x.CompanyId == companyId && !x.IsDeleted).ToList();
                    int activeEmployee = totalEmployeeList.Where(x => x.IsActive).Count();
                    int allowedEmployees = companyPackage.NoOfEmployee + companyPackage.BuyNoOfEmployee;
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

                    isSuccess = true;
                    message = ErrorMessage.EmployeePackageBuySuccessfully;
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

        public ActionResult RenewList(DateTime? startDate, DateTime? endDate, long? companyId)
        {
            EmployeeBuyTransactionFilterVM employeeBuyTransactionFilterVM = new EmployeeBuyTransactionFilterVM();
            if (companyId.HasValue)
            {
                employeeBuyTransactionFilterVM.CompanyId = companyId;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                employeeBuyTransactionFilterVM.StartDate = startDate.Value;
                employeeBuyTransactionFilterVM.EndDate = endDate.Value;
            }

            try
            {
                employeeBuyTransactionFilterVM.RenewList = (from eb in _db.tbl_EmployeeBuyTransaction
                                                            join cmp in _db.tbl_Company on eb.CompanyId equals cmp.CompanyId
                                                            where eb.CreatedDate >= employeeBuyTransactionFilterVM.StartDate && eb.CreatedDate <= employeeBuyTransactionFilterVM.EndDate
                                                            && (employeeBuyTransactionFilterVM.CompanyId.HasValue ? eb.CompanyId == employeeBuyTransactionFilterVM.CompanyId.Value : true)
                                                            select new EmployeeBuyTransactionVM
                                                            {
                                                                EmployeeBuyTransactionId = eb.EmployeeBuyTransactionId,
                                                                CompanyId = eb.CompanyId,
                                                                CompanyName = cmp.CompanyName,
                                                                NoOfEmpToBuy = eb.NoOfEmpToBuy,
                                                                AmountPerEmp = eb.AmountPerEmp,
                                                                TotalPaidAmount = eb.TotalPaidAmount,
                                                                PaymentGatewayTransactionId = eb.PaymentGatewayTransactionId,
                                                                ExpiryDate = eb.ExpiryDate,
                                                                CreatedDate = eb.CreatedDate,
                                                            }).OrderBy(x => x.EmployeeBuyTransactionId).ToList();

                employeeBuyTransactionFilterVM.CompanyList = GetCompanyList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(employeeBuyTransactionFilterVM);
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

        public string GenerateEmployeeBuyInvoiceNo()
        {
            string invoiceNo = string.Empty;
            long newNo = 0;
            tbl_InvoiceLastDocNo lastDocNoObj = _db.tbl_InvoiceLastDocNo.Where(x => x.PackageType == (int)SalesReportType.Employee).FirstOrDefault();

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
            invoiceNo = ErrorMessage.InvoicePrefix + ErrorMessage.InvoiceNoSaperator + yearString + ErrorMessage.InvoiceNoSaperator + ErrorMessage.EmployeeInvoiceNoPrefix + ErrorMessage.InvoiceNoSaperator + numberInStringFormat;

            if (lastDocNoObj == null)
            {
                lastDocNoObj = new tbl_InvoiceLastDocNo();
                lastDocNoObj.PackageType = (int)SalesReportType.Employee;
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

            return PartialView("~/Areas/Admin/Views/EmployeeBuyTransaction/_RazorPayPayment.cshtml");
        }

    }
}