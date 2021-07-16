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
        // GET: Admin/SMSRenew
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
            EmployeeBuyTransactionVM employeeBuyTransactionVM = new EmployeeBuyTransactionVM();
            tbl_CompanyRenewPayment companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId && DateTime.Today >= x.StartDate && DateTime.Today < x.EndDate).FirstOrDefault();
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
        public ActionResult Buy(EmployeeBuyTransactionVM employeeBuyTransactionVM)
        {
            string message = string.Empty;
            bool isSuccess = true;

            try
            {
                if (employeeBuyTransactionVM != null)
                {

                    // Get selected package detail
                    tbl_CompanyRenewPayment companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId && DateTime.Today >= x.StartDate && DateTime.Today < x.EndDate).FirstOrDefault();

                    tbl_EmployeeBuyTransaction employeeBuyTransaction = new tbl_EmployeeBuyTransaction();
                    employeeBuyTransaction.CompanyId = employeeBuyTransactionVM.CompanyId;
                    employeeBuyTransaction.NoOfEmpToBuy = employeeBuyTransactionVM.NoOfEmpToBuy;
                    employeeBuyTransaction.AmountPerEmp = employeeBuyTransactionVM.AmountPerEmp;
                    employeeBuyTransaction.TotalPaidAmount = employeeBuyTransactionVM.TotalPaidAmount;
                    employeeBuyTransaction.ExpiryDate = companyPackage.EndDate;
                    employeeBuyTransaction.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    employeeBuyTransaction.CreatedBy = loggedInUserId;
                    employeeBuyTransaction.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    employeeBuyTransaction.ModifiedBy = loggedInUserId;
                    _db.tbl_EmployeeBuyTransaction.Add(employeeBuyTransaction);
                    _db.SaveChanges();

                    companyPackage.BuyNoOfEmployee = companyPackage.BuyNoOfEmployee + employeeBuyTransaction.NoOfEmpToBuy;
                    _db.SaveChanges();
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

            return RedirectToAction("index");
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
    }
}