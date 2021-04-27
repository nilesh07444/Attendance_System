using System;
using System.Collections.Generic;
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
        public string Name { get; set; }
        public DateTime AttendanceDate { get; set; }
        public string DayType { get; set; }
        public decimal ExtraHours { get; set; }
        public string TodayWorkDetail { get; set; }
        public string TomorrowWorkDetail { get; set; }
        public string Remarks { get; set; }
        public string LocationFrom { get; set; }
        public int Status { get; set; }
        public string RejectReason { get; set; }
        public bool IsActive { get; set; }
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
}