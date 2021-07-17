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
    public class SMSLogController : Controller
    {
        AttendanceSystemEntities _db;
        // GET: Admin/SMSLog
        public SMSLogController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index(DateTime? startDate = null, DateTime? endDate = null, long? employeeId = null)
        {
            SMSLogFilterVM smsLogFilterVM = new SMSLogFilterVM();
            long companyId = clsAdminSession.CompanyId;
            if (employeeId.HasValue)
            {
                smsLogFilterVM.EmployeeId = employeeId.Value;
            }
            if (startDate.HasValue && endDate.HasValue)
            {
                smsLogFilterVM.StartDate = startDate.Value;
                smsLogFilterVM.EndDate = endDate.Value;
            }
            DateTime applyEndDate = smsLogFilterVM.EndDate.AddDays(1);
            smsLogFilterVM.SMSLogList = (from sms in _db.tbl_SMSLog
                                         join emp in _db.tbl_Employee on sms.EmployeeId equals emp.EmployeeId into outeremp
                                         from empr in outeremp.DefaultIfEmpty()

                                         where sms.CompanyId == companyId
                                           && sms.CreatedDate >= smsLogFilterVM.StartDate && sms.CreatedDate < applyEndDate
                                           && (smsLogFilterVM.EmployeeId.HasValue ? sms.EmployeeId == smsLogFilterVM.EmployeeId.Value : true)
                                         select new SMSLogVM
                                         {
                                             SMSLogId = sms.SMSLogId,
                                             MobileNo = sms.MobileNo,
                                             Message = sms.Message,
                                             CompanyId = sms.CompanyId,
                                             EmployeeId = sms.EmployeeId,
                                             EmployeeCode = sms.EmployeeCode,
                                             EmployeeName = empr != null ? empr.Prefix + " " + empr.FirstName + " " + empr.LastName : string.Empty,
                                             CreatedDate = sms.CreatedDate
                                         }).OrderByDescending(x => x.SMSLogId).ToList();

            smsLogFilterVM.EmployeeList = GetEmployeeList();
            return View(smsLogFilterVM);
        }

        private List<SelectListItem> GetEmployeeList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        orderby emp.EmployeeId
                                        select new SelectListItem
                                        {
                                            Text = emp.Prefix + " " + emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }
    }
}