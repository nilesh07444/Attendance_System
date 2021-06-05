using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class DashboardVM
    {
        public long PendingLeaves { get; set; }
        public long PendingAttendance { get; set; }
        public DateTime AccountExpiryDate { get; set; }
        public long ThisMonthHoliday { get; set; }

        public long Employee { get; set; }
        public long Supervisor { get; set; }
        public long Checker { get; set; }
        public long Payer { get; set; }
        public long Worker { get; set; }
        public bool IsOfficeCompany { get; set; }
        public string MonthName { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }



    }
}