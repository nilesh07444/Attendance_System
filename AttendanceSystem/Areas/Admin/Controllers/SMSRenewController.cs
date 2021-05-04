using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
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
    }
}