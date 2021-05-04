using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class CompanySMSPackRenewVM
    {
        public long CompanySMSPackRenewId { get; set; }
        public long CompanyId { get; set; }
        public long SMSPackageId { get; set; }
        public string SMSPackageName { get; set; }
        public DateTime RenewDate { get; set; }
        public int AccessDays { get; set; }
        public DateTime PackageExpiryDate { get; set; }

    }
}