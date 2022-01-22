using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class AttendanceVM
    {
        public long AttendanceId { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        [Display(Name = "Employee Name")]
        public string Name { get; set; }
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; }
        [Display(Name = "Day Type")]
        public double DayType { get; set; }
        public string DayTypeText { get; set; }
        [Display(Name = "Extra Hours")]
        public decimal ExtraHours { get; set; }
        [Display(Name = "Today Work detail")]
        public string TodayWorkDetail { get; set; }
        [Display(Name = "Tomorrow Work Detail")]
        public string TomorrowWorkDetail { get; set; }
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        [Display(Name = "In Location")]
        public string LocationFrom { get; set; }
        [Display(Name = "Out Location")]
        public string OutLocationFrom { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
        public string StatusText { get; set; }
        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "In Date Time")]
        public DateTime InDateTime { get; set; }
        [Display(Name = "Out Date Time")]
        public DateTime? OutDateTime { get; set; }
        public decimal? ExtraPerHourPrice { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public decimal? InLatitude { get; set; }
        public decimal? InLongitude { get; set; }
        public decimal? OutLatitude { get; set; }
        public decimal? OutLongitude { get; set; }
        [Display(Name = "No Of Hours Worked")]
        public decimal NoOfHoursWorked { get; set; }
        [Display(Name = "Worked Hours Amount")]
        public decimal WorkedHoursAmount { get; set; }
        [Display(Name = "No Of Unit Worked")]
        public decimal? NoOfUnitWorked { get; set; }
        [Display(Name = "Worked Unit Amount")]
        public decimal WorkedUnitAmount { get; set; }

        public string EmployeeCode { get; set; }
        public decimal PerCategoryPrice { get; set; }

        // Additional fields
        public string EmployeeDesignation { get; set; }
        public string BgColor { get; set; }
    }

    public class AttendanceFilterVM
    {
        public AttendanceFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? AttendanceStatus { get; set; }
        public int? UserId { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> CalenderMonth { get; set; }
        public List<AttendanceVM> AttendanceList { get; set; }
    }

    public class AttendanceAPIFilterVM
    {
        public AttendanceAPIFilterVM()
        {
            Year = CommonMethod.CurrentIndianDateTime().Year;
            AttendanceStatus = (int)Helper.AttendanceStatus.Pending;
        }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public int? AttendanceStatus { get; set; }
        public int? UserId { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> CalenderMonth { get; set; }
        public List<AttendanceVM> AttendanceList { get; set; }
    }

    public class InTimeRequestVM
    {
        public string InLocationFrom { get; set; }
        public decimal InLatitude { get; set; }
        public decimal InLongitude { get; set; }
    }

    public class OutTimeRequestVM
    {
        public string OutLocationFrom { get; set; }
        public decimal OutLatitude { get; set; }
        public decimal OutLongitude { get; set; }
        public decimal ExtraHours { get; set; }
        public string TodayWorkDetail { get; set; }
        public string TomorrowWorkDetail { get; set; }
        public string Remarks { get; set; }
        public double DayType { get; set; }
        public decimal NoOfHoursWorked { get; set; }
        public decimal NoOfUnitWorked { get; set; }
    }

    public class WorkerAttendanceRequestVM
    {
        public int AttendanceType { get; set; }
        public long EmployeeId { get; set; }
        public decimal ExtraHours { get; set; }
        public decimal NoOfHoursWorked { get; set; }
        public decimal NoOfUnitWorked { get; set; }
        public long SiteId { get; set; }
        public decimal? TodaySalary { get; set; }

        public string LocationFrom { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
    public class WorkerAttendanceFilterVM
    {
        public WorkerAttendanceFilterVM()
        {
            AttendanceDate = CommonMethod.CurrentIndianDateTime().Date;
        }
        public DateTime AttendanceDate { get; set; }
        public int SiteId { get; set; }
        public long? EmployeeId { get; set; }

        public List<WorkerAttendanceVM> AttendanceList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
    }

    public class WorkerAttendanceReportListFilterVM
    {
        public WorkerAttendanceReportListFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int SiteId { get; set; }
        public long? EmployeeId { get; set; }
        public long? EmploymentCategory { get; set; }
        public long? WorkerHeadId { get; set; }

        public List<WorkerAttendanceReportVM> AttendanceList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> EmploymentCategoryList { get; set; }
        public List<SelectListItem> WorkerHeadList { get; set; }
    }

    public class WorkerAttendanceVM
    {
        public long AttendanceId { get; set; }
        public long CompanyId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string WorkerTypeName { get; set; }
        public string Name { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string ProfilePicture { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public bool IsMorning { get; set; }
        public string IsMorningText { get; set; }
        public bool IsAfternoon { get; set; }
        public string IsAfternoonText { get; set; }
        public bool IsEvening { get; set; }
        public string IsEveningText { get; set; }
        public string SiteName { get; set; }
        public string BgColor { get; set; } 
    }

    public class WorkerAttendanceReportVM
    {
        public long AttendanceId { get; set; }
        public long CompanyId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string WorkerTypeName { get; set; }
        public string Name { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string ProfilePicture { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public bool IsMorning { get; set; }
        public string IsMorningText { get; set; }
        public bool IsAfternoon { get; set; }
        public string IsAfternoonText { get; set; }
        public bool IsEvening { get; set; }
        public string IsEveningText { get; set; }
        public string SiteName { get; set; }
        public string BgColor { get; set; }
        public decimal CalcTodaySalary { get; set; }
        public decimal ActTodaySalary { get; set; }
        public decimal TotalTodaySalary { get; set; }
        public decimal SalaryGiven { get; set; }
        public decimal MonthlySalary { get; set; }
        public decimal PerCategoryPrice { get; set; }
        public bool IsClosed { get; set; }
        public string MorningAttendanceBy { get; set; }
        public DateTime? MorningAttendanceDate { get; set; }
        public decimal? MorningLatitude { get; set; }
        public decimal? MorningLongitude { get; set; }
        public string MorningLocationFrom { get; set; }
        public string AfternoonAttendanceBy { get; set; }
        public DateTime? AfternoonAttendanceDate { get; set; }
        public decimal? AfternoonLatitude { get; set; }
        public decimal? AfternoonLongitude { get; set; }
        public string AfternoonLocationFrom { get; set; }
        public string EveningAttendanceBy { get; set; }
        public DateTime? EveningAttendanceDate { get; set; }
        public decimal? EveningLatitude { get; set; }
        public decimal? EveningLongitude { get; set; }
        public string EveningLocationFrom { get; set; }
        public decimal? ExtraHours { get; set; }
        public decimal? NoOfHoursWorked { get; set; }
        public decimal? NoOfUnitWorked { get; set; }

        public long? WorkerHeadId { get; set; }
        public string WorkerHeadName { get; set; }
    }

    public class WorkerAttendanceReportFilterVM
    {
        public WorkerAttendanceReportFilterVM()
        {
            StartDate = CommonMethod.CurrentIndianDateTime();
            EndDate = CommonMethod.CurrentIndianDateTime();
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? SiteId { get; set; }
        public long? EmployeeId { get; set; }
    }

    public class AddWorkerAttendanceVM
    {
        public AddWorkerAttendanceVM()
        {
            TotalPendingSalary = 0;
            RemainingBalance = 0;
            SalaryGiven = 0;
            NoOfUnitWorked = 0;
            NoOfUnitWorkedAmount = 0;
            NoOfHoursWorked = 0;
            NoOfHoursWorkedAmount = 0;
        }
        public long AttendanceId { get; set; }
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; }
        [Display(Name = "Attendance Type")]
        public int AttendanceType { get; set; }
        public long EmployeeId { get; set; }
        public long EmploymentCategoryId { get; set; }
        [Display(Name = "Employment Category")]
        public string EmploymentCategoryText { get; set; }
        [Display(Name = "Worker Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Worker Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Extra Hours")]
        public decimal ExtraHours { get; set; }
        [Display(Name = "Extra Hours Amount")]
        public decimal ExtraHoursAmount { get; set; }
        [Display(Name = "No of Hours Worked")]
        public decimal NoOfHoursWorked { get; set; }
        [Display(Name = "No of Hours Worked Amount")]
        public decimal NoOfHoursWorkedAmount { get; set; }
        [Display(Name = "No of Unit Worked")]
        public decimal NoOfUnitWorked { get; set; }
        [Display(Name = "No of Unit Worked Amount")]
        public decimal NoOfUnitWorkedAmount { get; set; }
        public long SiteId { get; set; }
        [Display(Name = "Site")]
        public string SiteName { get; set; }
        [Display(Name = "Today Salary")]
        public decimal? TodaySalary { get; set; }
        [Display(Name = "Salary Given")]
        public decimal SalaryGiven { get; set; }
        public string LocationFrom { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        [Display(Name = "Pending Salary")]
        public decimal PendingSalary { get; set; }
        public List<SelectListItem> WorkerAttendanceTypeList { get; set; }
        [Display(Name = "Monthly Salary")]
        public decimal? MonthlySalary { get; set; }
        public decimal PerCategoryPrice { get; set; }
        [Display(Name = "Extra Per Hour Price")]
        public decimal? ExtraPerHourPrice { get; set; }
        [Display(Name = "Morning")]
        public bool IsMorning { get; set; }
        public string IsMorningText { get; set; }
        [Display(Name = "Afternoon")]
        public bool IsAfternoon { get; set; }
        public string IsAfternoonText { get; set; }
        [Display(Name = "Evening")]
        public bool IsEvening { get; set; }
        public string IsEveningText { get; set; }
        [Display(Name = "Total Pending Salary")]
        public decimal TotalPendingSalary { get; set; }
        [Display(Name = "Remaining Balance")]
        public decimal RemainingBalance { get; set; }

        public decimal NoOfFreeLeavePerMonth { get; set; }
    }

    public class WorkerAttendanceViewVM
    {
        public long AttendanceId { get; set; }
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; }
        public string EmployeeCode { get; set; }
        public long EmployeeId { get; set; }
        [Display(Name = "Worker Name")]
        public string EmployeeName { get; set; }
        public long EmploymentCategory { get; set; }
        [Display(Name = "Employment Category")]
        public string EmploymentCategoryText { get; set; }
        public bool IsMorning { get; set; }
        [Display(Name = "Morning")]
        public string IsMorningText { get; set; }
        public bool IsAfternoon { get; set; }
        [Display(Name = "Afternoon")]
        public string IsAfternoonText { get; set; }
        public bool IsEvening { get; set; }
        [Display(Name = "Evening")]
        public string IsEveningText { get; set; }
        [Display(Name = "Morning Site")]
        public string MorningSite { get; set; }
        public long? MorningAttendanceBy { get; set; }
        [Display(Name = "Morning Attendance By")]
        public string MorningAttendanceByName { get; set; }
        [Display(Name = "Morning Attendance Date")]
        public DateTime? MorningAttendanceDate { get; set; }
        [Display(Name = "Morning Latitude")]
        public decimal? MorningLatitude { get; set; }
        [Display(Name = "Morning Longitude")]
        public decimal? MorningLongitude { get; set; }
        [Display(Name = "Morning Location")]
        public string MorningLocationFrom { get; set; }
        [Display(Name = "Afternoon Site")]
        public string AfternoonSite { get; set; }
        public long? AfternoonAttendanceBy { get; set; }
        [Display(Name = "Afternoon Attendance By")]
        public string AfternoonAttendanceByName { get; set; }
        [Display(Name = "Afternoon Attendance Date")]
        public DateTime? AfternoonAttendanceDate { get; set; }
        [Display(Name = "Afternoon Latitude")]
        public decimal? AfternoonLatitude { get; set; }
        [Display(Name = "Afternoon longitude")]
        public decimal? AfternoonLongitude { get; set; }
        [Display(Name = "Afternoon Location From")]
        public string AfternoonLocationFrom { get; set; }
        [Display(Name = "Evening Site")]
        public string EveningSite { get; set; }
        public long? EveningAttendanceBy { get; set; }
        [Display(Name = "Evening Attendance By")]
        public string EveningAttendanceByName { get; set; }
        [Display(Name = "Evening Attendance Date")]
        public DateTime? EveningAttendanceDate { get; set; }
        [Display(Name = "Evening Latitude")]
        public decimal? EveningLatitude { get; set; }
        [Display(Name = "Evening Longitude")]
        public decimal? EveningLongitude { get; set; }
        [Display(Name = "Evening location From")]
        public string EveningLocationFrom { get; set; }
        [Display(Name = "Extra Hours")]
        public decimal? ExtraHours { get; set; }
        [Display(Name = "No Of Hours Worked")]
        public decimal? NoOfHoursWorked { get; set; }
        [Display(Name = "No Of Unit Worked")]
        public decimal? NoOfUnitWorked { get; set; }

        // 
        [Display(Name = "Worked Hours Amount")]
        public decimal WorkedHoursAmount { get; set; }

        [Display(Name = "Worked Unit Amount")]
        public decimal WorkedUnitAmount { get; set; }

        public decimal? MonthlySalaryPrice { get; set; }
        public decimal? PerCategoryPrice { get; set; }
        public decimal? ExtraPerHourPrice { get; set; }
    }

    public class AfternoonAttendanceEmployeeVM
    {
        public long EmployeeId { get; set; }
    }

    public class AfternoonAttendanceRequestVM
    {
        public long SiteId { get; set; }
        public string LocationFrom { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public List<AfternoonAttendanceEmployeeVM> EmployeeList { get; set; }
    }

}