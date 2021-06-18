using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class CompanySMSPackRenewVM
    {
        public long CompanySMSPackRenewId { get; set; }
        public long CompanyId { get; set; }
        public long SMSPackageId { get; set; }
        [Display(Name = "Package Name")]
        public string SMSPackageName { get; set; }
        [Display(Name = "Renew Date")]
        public DateTime RenewDate { get; set; }
        [Display(Name = "Access Days")]
        public int AccessDays { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime PackageExpiryDate { get; set; }
        [Display(Name = "Amount")]
        public decimal PackageAmount { get; set; }
        [Display(Name = "No Of SMS")]
        public int? NoOfSMS { get; set; }
        [Display(Name = "Remaining SMS")]
        public int? RemainingSMS { get; set; }
    }
}