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
        public ResponseDataModel<DashboardCountVM> DashboardCount(long employeeId)
        {
            ResponseDataModel<DashboardCountVM> response = new ResponseDataModel<DashboardCountVM>();
            DashboardCountVM dashboardCountVM = new DashboardCountVM();

            try
            {
                tbl_Employee data = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();

                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                dashboardCountVM.TotalAttendance = _db.tbl_Attendance.Where(x => x.UserId == employeeId
                                                    && x.Status == (int)AttendanceStatus.Accept && x.IsActive
                                                    && !x.IsDeleted && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).Count();

                dashboardCountVM.thisMonthHoliday = _db.tbl_Holiday.Where(x => x.CompanyId == data.CompanyId.ToString()
                                                    && x.IsActive && !x.IsDeleted).Select(X => (X.EndDate - X.StartDate).TotalDays).Sum();

                int currentMonthWorkingDays = CommonMethod.WeekDaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                dashboardCountVM.TotalAbsent = currentMonthWorkingDays - dashboardCountVM.TotalAttendance - (int.Parse(dashboardCountVM.thisMonthHoliday.ToString()));

                dashboardCountVM.LeavePendingForApprove = _db.tbl_Leave.Where(x => x.UserId == employeeId && !x.IsDeleted && x.LeaveStatus == (int)LeaveStatus.Pending).Count();
                dashboardCountVM.LastMonthRating = "7/10";
                dashboardCountVM.PendingSalary = 15000;
                response.IsError = true;
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
