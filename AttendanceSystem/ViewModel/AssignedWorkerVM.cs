using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class AssignedWorkerVM
    {
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public DateTime Date { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public bool IsClosed { get; set; }
        public bool IsMorning { get; set; }
        public string IsMorningText { get; set; }
        public bool IsAfternoon { get; set; }
        public string IsAfternoonText { get; set; }
        public bool IsEvening { get; set; }
        public string IsEveningText { get; set; }
        public long? WorkerAttendanceId { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public string WorkerTypeName { get; set; } 
    }

    public class AssignedWorkerFilterVM
    {
        public AssignedWorkerFilterVM()
        {
            Date = CommonMethod.CurrentIndianDateTime().Date;
        }
        public int? SiteId { get; set; }
        public DateTime Date { get; set; }
        public long? EmployeeId { get; set; }
        public List<AssignedWorkerVM> AssignedWorkerList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public int AttendanceType { get; set; }
        public List<SelectListItem> AttendanceTypeList { get; set; }
    }
}