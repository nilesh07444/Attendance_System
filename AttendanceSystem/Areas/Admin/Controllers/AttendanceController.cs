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
    public class AttendanceController : Controller
    {
        // GET: Admin/Attendance
        AttendanceSystemEntities _db;
        public AttendanceController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index(int? userId = null, int? attendanceStatus = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            AttendanceFilterVM attendanceFilterVM = new AttendanceFilterVM();
            try
            {
                if (userId.HasValue)
                    attendanceFilterVM.UserId = userId.Value;
                if (attendanceStatus.HasValue)
                    attendanceFilterVM.AttendanceStatus = attendanceStatus.Value;

                if (startDate.HasValue && endDate.HasValue)
                {
                    attendanceFilterVM.StartDate = startDate.Value;
                    attendanceFilterVM.EndDate = endDate.Value;
                }

                long companyId = clsAdminSession.CompanyId;

                attendanceFilterVM.AttendanceList = (from at in _db.tbl_Attendance
                                                     join emp in _db.tbl_Employee on at.UserId equals emp.EmployeeId
                                                     where !at.IsDeleted
                                                     && emp.CompanyId == companyId
                                                     && at.AttendanceDate >= attendanceFilterVM.StartDate && at.AttendanceDate <= attendanceFilterVM.EndDate
                                                     && (attendanceFilterVM.AttendanceStatus.HasValue ? at.Status == attendanceFilterVM.AttendanceStatus.Value : true)
                                                     && (attendanceFilterVM.UserId.HasValue ? emp.EmployeeId == attendanceFilterVM.UserId.Value : true)
                                                     select new AttendanceVM
                                                     {
                                                         AttendanceId = at.AttendanceId,
                                                         CompanyId = at.CompanyId,
                                                         UserId = at.UserId,
                                                         Name = emp.FirstName + " " + emp.LastName,
                                                         AttendanceDate = at.AttendanceDate,
                                                         DayType = at.DayType,
                                                         ExtraHours = at.ExtraHours,
                                                         TodayWorkDetail = at.TodayWorkDetail,
                                                         TomorrowWorkDetail = at.TomorrowWorkDetail,
                                                         Remarks = at.Remarks,
                                                         LocationFrom = at.LocationFrom,
                                                         Status = at.Status,
                                                         RejectReason = at.RejectReason
                                                     }).OrderByDescending(x => x.AttendanceDate).ToList();

                attendanceFilterVM.EmployeeList = GetEmployeeList();
            }
            catch (Exception ex)
            {
            }
            return View(attendanceFilterVM);
        }

        private List<SelectListItem> GetEmployeeList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName,
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }
    }
}