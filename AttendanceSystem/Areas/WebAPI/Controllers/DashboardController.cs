using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
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

                tbl_Employee emp = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();

                DateTime startDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                List<tbl_Attendance> attendanceData = _db.tbl_Attendance.Where(x => x.UserId == employeeId
                                                    && x.Status == (int)AttendanceStatus.Accept && x.IsActive
                                                    && !x.IsDeleted && x.AttendanceDate >= startDate && x.AttendanceDate <= endDate).ToList();

                if (attendanceData != null && attendanceData.Count > 0)
                {
                    if (emp.EmploymentCategory == (int)EmploymentCategory.MonthlyBased || emp.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                    {
                        dashboardCountVM.TotalAttendance = attendanceData.Count;
                    }
                    else if (emp.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                    {
                        dashboardCountVM.TotalAttendance = Convert.ToDecimal(attendanceData.Where(x => x.NoOfHoursWorked > 0).Sum(x => x.NoOfHoursWorked));
                    }
                    else if (emp.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                    {
                        dashboardCountVM.TotalAttendance = Convert.ToDecimal(attendanceData.Where(x => x.NoOfUnitWorked > 0).Sum(x => x.NoOfUnitWorked));
                    }
                }

                var listDays = (from hd in _db.tbl_Holiday
                                where hd.CompanyId == companyId.ToString()
                                && hd.IsActive && !hd.IsDeleted
                                 && hd.StartDate >= startDate && hd.StartDate <= endDate
                                select new
                                {
                                    StartDate = hd.StartDate,
                                    EndDate = hd.EndDate
                                }).ToList();
                dashboardCountVM.thisMonthHoliday = listDays.Select(x => (x.EndDate - x.StartDate).TotalDays + 1).Sum();

                dashboardCountVM.LeavePendingForApprove = _db.tbl_Leave.Where(x => x.UserId == employeeId && !x.IsDeleted && x.LeaveStatus == (int)LeaveStatus.Pending).Count();

                int LastMonth = CommonMethod.CurrentIndianDateTime().Month == 1 ? 12 : CommonMethod.CurrentIndianDateTime().Month - 1;
                int year = CommonMethod.CurrentIndianDateTime().Month == 1 ? CommonMethod.CurrentIndianDateTime().Year - 1 : CommonMethod.CurrentIndianDateTime().Year;
                tbl_EmployeeRating employeeRatingObject = _db.tbl_EmployeeRating.Where(x => x.EmployeeId == employeeId && x.RateMonth == LastMonth && x.RateYear == year).FirstOrDefault();
                dashboardCountVM.LastMonthRating = (employeeRatingObject != null ? (new decimal[] { employeeRatingObject.BehaviourRate, employeeRatingObject.RegularityRate, employeeRatingObject.WorkRate }).Average().ToString("#.##") : "0") + "/10";



                DateTime now = CommonMethod.CurrentIndianDateTime();
                var monthStartDate = new DateTime(now.Year, now.Month, 1);
                var monthEndDate = startDate.AddMonths(1).AddDays(-1);

                var transaction = (from ep in _db.tbl_EmployeePayment
                                   where ep.UserId == employeeId && !ep.IsDeleted
                                   && ep.PaymentDate >= monthStartDate
                                   && ep.PaymentDate <= monthEndDate
                                   && ep.PaymentType != (int)EmployeePaymentType.Extra
                                   select new
                                   {
                                       Amount = (ep.CreditAmount.HasValue ? ep.CreditAmount.Value : 0) - (ep.DebitAmount.HasValue ? ep.DebitAmount.Value : 0)
                                   }).ToList();

                dashboardCountVM.PendingSalary = transaction.Sum(x => x.Amount);

                response.IsError = false;
                dashboardCountVM.AttendancePendingForApprove = _db.tbl_Attendance.Where(x => x.Status == (int)AttendanceStatus.Pending && x.UserId == employeeId && !x.IsDeleted).Count();
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
