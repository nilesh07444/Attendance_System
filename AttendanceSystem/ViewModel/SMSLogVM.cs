using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class SMSLogVM
    {
        public long SMSLogId { get; set; }
        public string MobileNo { get; set; }
        public string Message { get; set; }
        public long? CompanyId { get; set; }
        public long? EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class SMSLogFilterVM
    {
        public SMSLogFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SMSLogVM> SMSLogList { get; set; }
        public long? EmployeeId { get; set; }
    }
}