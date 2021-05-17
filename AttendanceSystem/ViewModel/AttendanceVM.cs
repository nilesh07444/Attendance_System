using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
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
        [Display(Name = "Extra Hours")]
        public decimal ExtraHours { get; set; }
        [Display(Name = "Today Work detail")]
        public string TodayWorkDetail { get; set; }
        [Display(Name = "Tomorrow Work Detail")]
        public string TomorrowWorkDetail { get; set; }
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public string LocationFrom { get; set; }
        [Display(Name = "Status")]
        public int Status { get; set; }
        public string StatusText { get; set; }
        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "In Time")]
        public TimeSpan InTime { get; set; }
        [Display(Name = "Out Time")]
        public TimeSpan OutTime { get; set; }
    }

    public class AttendanceFilterVM
    {
        public AttendanceFilterVM()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? AttendanceStatus { get; set; }
        public int? UserId { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
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
    }
}