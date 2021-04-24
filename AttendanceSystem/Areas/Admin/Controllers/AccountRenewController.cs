using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class AccountRenewController : Controller
    {
        AttendanceSystemEntities _db;
        public AccountRenewController()
        {
            _db = new AttendanceSystemEntities();
        }
        // GET: Admin/AccountRenew
        public ActionResult Index()
        {
            List<CompanyRenewPaymentVM> companyRenewPaymentVM = new List<CompanyRenewPaymentVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                companyRenewPaymentVM = (from cp in _db.tbl_CompanyRenewPayment
                                         join cm in _db.tbl_Company on cp.CompanyId equals cm.CompanyId
                                         where cp.CompanyId== companyId
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
                                             PackageName = cp.PackageName
                                         }).OrderByDescending(x => x.CompanyRegistrationPaymentId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRenewPaymentVM);
        }
    }
}