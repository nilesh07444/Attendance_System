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

        public ActionResult Index(DateTime? startDate = null, DateTime? endDate = null, string employeeCode = null, int? lunchStatus = null)
        {
            EmployeeLunchBreakFilterVM lunchbreakFilterVM = new EmployeeLunchBreakFilterVM();

            if (startDate.HasValue && endDate.HasValue)
            {
                lunchbreakFilterVM.StartDate = startDate.Value;
                lunchbreakFilterVM.EndDate = endDate.Value;
            }

            if (lunchStatus.HasValue)
            {
                lunchbreakFilterVM.LunchStatus = lunchStatus.Value;
            }

            if (!string.IsNullOrEmpty(employeeCode))
            {
                lunchbreakFilterVM.EmployeeCode = employeeCode;
            }

            try
            {

                long companyId = clsAdminSession.CompanyId;

                lunchbreakFilterVM.EmployeeLunchBreakList = (from lunch in _db.tbl_EmployeeLunchBreak
                                                             join att in _db.tbl_Attendance on lunch.AttendanceId equals att.AttendanceId
                                                             join emp in _db.tbl_Employee on lunch.EmployeeId equals emp.EmployeeId
                                                             join role in _db.mst_AdminRole on emp.AdminRoleId equals role.AdminRoleId
                                                             where !emp.IsDeleted && emp.CompanyId == companyId
                                                             && DbFunctions.TruncateTime(att.AttendanceDate) >= DbFunctions.TruncateTime(lunchbreakFilterVM.StartDate)
                                                             && DbFunctions.TruncateTime(att.AttendanceDate) <= DbFunctions.TruncateTime(lunchbreakFilterVM.EndDate)
                                                             && (!string.IsNullOrEmpty(lunchbreakFilterVM.EmployeeCode) ? emp.EmployeeCode == lunchbreakFilterVM.EmployeeCode : true)

                                                             && (lunchbreakFilterVM.LunchStatus.HasValue ?
                                                                (lunchbreakFilterVM.LunchStatus.Value == 1 ? lunch.EndDateTime == null : lunch.EndDateTime.HasValue)
                                                             : true)

                                                             select new EmployeeLunchBreakVM
                                                             {
                                                                 EmployeeLunchBreakId = lunch.EmployeeLunchBreakId,
                                                                 EmployeeId = lunch.EmployeeId,
                                                                 EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                                 EmployeeCode = emp.EmployeeCode,
                                                                 StartDateTime = lunch.StartDateTime,
                                                                 EndDateTime = lunch.EndDateTime,
                                                                 EmployeeRole = role.AdminRoleName,
                                                                 LunchBreakNo = lunch.LunchBreakNo,
                                                                 AttendaceDate = att.AttendanceDate,
                                                                 AttendaceId = att.AttendanceId
                                                             }).OrderBy(x => x.AttendaceDate).OrderBy(x => x.StartDateTime).ToList();

            }
            catch (Exception ex)
            {
            }

            return View(lunchbreakFilterVM);
        }

        public ActionResult View(long Id)
        {
            EmployeeLunchBreakVM objLunchBreakVM = new EmployeeLunchBreakVM();

            try
            {
                objLunchBreakVM = (from lunch in _db.tbl_EmployeeLunchBreak
                                   join att in _db.tbl_Attendance on lunch.AttendanceId equals att.AttendanceId
                                   join emp in _db.tbl_Employee on lunch.EmployeeId equals emp.EmployeeId
                                   join role in _db.mst_AdminRole on emp.AdminRoleId equals role.AdminRoleId
                                   where lunch.EmployeeLunchBreakId == Id
                                   select new EmployeeLunchBreakVM
                                   {
                                       EmployeeLunchBreakId = lunch.EmployeeLunchBreakId,
                                       EmployeeId = lunch.EmployeeId,
                                       EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                       EmployeeCode = emp.EmployeeCode,
                                       EmployeeRole = role.AdminRoleName,

                                       AttendaceDate = att.AttendanceDate,
                                       AttendaceId = att.AttendanceId,
                                       AttendaceInDate = att.InDateTime,
                                       AttendaceOutDate = att.OutDateTime,

                                       LunchBreakNo = lunch.LunchBreakNo,

                                       StartDateTime = lunch.StartDateTime,
                                       StartLunchLatitude = lunch.StartLunchLatitude,
                                       StartLunchLongitude = lunch.StartLunchLongitude,
                                       StartLunchLocationFrom = lunch.StartLunchLocationFrom,

                                       EndDateTime = lunch.EndDateTime,
                                       EndLunchLatitude = lunch.EndLunchLatitude,
                                       EndLunchLongitude = lunch.EndLunchLongitude,
                                       EndLunchLocationFrom = lunch.EndLunchLocationFrom

                                   }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return View(objLunchBreakVM);
        }

    }
}