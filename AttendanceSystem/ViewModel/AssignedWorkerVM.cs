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

    }

    public class AssignedWorkerFilterVM
    {
        public AssignedWorkerFilterVM()
        {
            Date = CommonMethod.CurrentIndianDateTime();
        }
        public int? SiteId { get; set; }
        public DateTime Date { get; set; }
        public long? EmployeeId { get; set; }
        public List<AssignedWorkerVM> AssignedWorkerList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
    }
}