using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class DashboardController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        public DashboardController()
        {
            _db = new AttendanceSystemEntities();
        }

        [Route("DashboardCount"), HttpGet]
        public ResponseDataModel<DashboardCountVM> DashboardCount()
        {
            ResponseDataModel<DashboardCountVM> response = new ResponseDataModel<DashboardCountVM>();
            DashboardCountVM dashboardCountVM = new DashboardCountVM();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                dashboardCountVM.TotalAttendance = _db.tbl_Attendance.Where(x => x.UserId == employeeId
                                                    && x.Status == (int)AttendanceStatus.Accept && x.IsActive
                                                    && !x.IsDeleted && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).Count();


                var listDays = (from hd in _db.tbl_Holiday
                                where hd.CompanyId == companyId.ToString()
                                && hd.IsActive && !hd.IsDeleted
                                select new
                                {
                                    StartDate = hd.StartDate,
                                    EndDate = hd.EndDate
                                }).ToList();
                dashboardCountVM.thisMonthHoliday = listDays.Select(x => (x.EndDate - x.StartDate).TotalDays).Sum();

                int currentMonthWorkingDays = CommonMethod.WeekDaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                dashboardCountVM.TotalAbsent = currentMonthWorkingDays - dashboardCountVM.TotalAttendance - (int.Parse(dashboardCountVM.thisMonthHoliday.ToString()));

                dashboardCountVM.LeavePendingForApprove = _db.tbl_Leave.Where(x => x.UserId == employeeId && !x.IsDeleted && x.LeaveStatus == (int)LeaveStatus.Pending).Count();
                dashboardCountVM.LastMonthRating = "7/10";
                dashboardCountVM.PendingSalary = 15000;
                response.IsError = false;
                response.Data = dashboardCountVM;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message.ToString());
            }

            return response;
        }
    }
}
