using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class LoginHistoryVM
    {

        public long LoginHistoryId { get; set; }
        public long EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime LoginDate { get; set; }
        public string LocationFrom { get; set; }
        public long? SiteId { get; set; }
    }

    public class LoginHistoryFilterVM
    {
        public int? EmployeeId { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<LoginHistoryVM> LoginHistoryList { get; set; }
    }
}