using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class DashboardVM
    {
        public int? SMSLeft { get; set; }
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
        public bool AllowForWorker { get; set; }
        public bool AllowForEmployee { get; set; }


        public long TotalCustomer { get; set; }
        public long PendingCompanyRequest { get; set; }
        public long AccountPackage { get; set; }
        public long SMSPackage { get; set; }
        public long FeedBack_QueryPending { get; set; }
        public long TotalClientRegistration { get; set; }
        public long TotalClientForConstruction { get; set; }
        public long TotalClientForOffice { get; set; }
        public long CurrentPackageId { get; set; }
        public long CurrentSMSPackageId { get; set; }
        public int NoOfEmployeeAllowed { get; set; }

        public int NoOfTodayBirthday { get; set; }
        public int NoOfTodayAnniversary { get; set; }

    }
}