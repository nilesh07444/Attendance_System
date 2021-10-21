using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class LunchBreakController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public LunchBreakController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index(DateTime? startDate = null, DateTime? endDate = null, string employeeCode = null)
        {
            EmployeeLunchBreakFilterVM lunchbreakFilterVM = new EmployeeLunchBreakFilterVM();

            if (startDate.HasValue && endDate.HasValue)
            {
                lunchbreakFilterVM.StartDate = startDate.Value;
                lunchbreakFilterVM.EndDate = endDate.Value;
            }

            if (!string.IsNullOrEmpty(employeeCode))
            {
                lunchbreakFilterVM.EmployeeCode = employeeCode;
            }

            try
            {

                long companyId = clsAdminSession.CompanyId;
                   
                lunchbreakFilterVM.EmployeeLunchBreakList = (from lunch in _db.tbl_EmployeeLunchBreak
                                                             join emp in _db.tbl_Employee on lunch.EmployeeId equals emp.EmployeeId 
                                                             join role in _db.mst_AdminRole on emp.AdminRoleId equals role.AdminRoleId
                                                             where !emp.IsDeleted && emp.CompanyId == companyId
                                                             && DbFunctions.TruncateTime(lunch.StartDateTime) >= DbFunctions.TruncateTime(lunchbreakFilterVM.StartDate)
                                                             && DbFunctions.TruncateTime(lunch.StartDateTime) <= DbFunctions.TruncateTime(lunchbreakFilterVM.EndDate)
                                                             && (!string.IsNullOrEmpty(lunchbreakFilterVM.EmployeeCode) ? emp.EmployeeCode == lunchbreakFilterVM.EmployeeCode : true)
                                                             select new EmployeeLunchBreakVM
                                                             {
                                                                 EmployeeLunchBreakId = lunch.EmployeeLunchBreakId,
                                                                 EmployeeId = lunch.EmployeeId,
                                                                 EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                                 EmployeeCode = emp.EmployeeCode,
                                                                 StartDateTime = lunch.StartDateTime,
                                                                 EndDateTime = lunch.EndDateTime,
                                                                 EmployeeRole = role.AdminRoleName
                                                             }).OrderByDescending(x => x.StartDateTime).ToList();
                  
            }
            catch (Exception ex)
            {
            }

            return View(lunchbreakFilterVM);
        }
         
    }
}