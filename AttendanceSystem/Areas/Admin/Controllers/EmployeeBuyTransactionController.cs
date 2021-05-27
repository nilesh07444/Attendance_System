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
    public class EmployeeBuyTransactionController : Controller
    {
        AttendanceSystemEntities _db;
        long companyId;
        long loggedInUserId;
        public EmployeeBuyTransactionController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
            loggedInUserId = clsAdminSession.UserID;
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
                    employeeBuyTransaction.CreatedDate = DateTime.UtcNow;
                    employeeBuyTransaction.CreatedBy = loggedInUserId;
                    employeeBuyTransaction.ModifiedDate = DateTime.UtcNow;
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
    }
}