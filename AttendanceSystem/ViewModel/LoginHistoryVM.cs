using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class LoginHistoryVM
    {

        public long LoginHistoryId { get; set; }
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LoginDate { get; set; }
        public string LocationFrom { get; set; }
        public long? SiteId { get; set; }
    }

    public class LoginHistoryFilterVM
    {
        public LoginHistoryFilterVM()
        {
            StartDate = CommonMethod.CurrentIndianDateTime();
            EndDate = CommonMethod.CurrentIndianDateTime();
        }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? EmployeeId { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<LoginHistoryVM> LoginHistoryList { get; set; }
    }
}