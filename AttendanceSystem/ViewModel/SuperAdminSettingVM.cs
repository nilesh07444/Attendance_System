using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class SuperAdminSettingVM
    {
        public long SettingId { get; set; }
        [Required, Display(Name = "Account Free Access Days")]
        public int AccountFreeAccessDays { get; set; }
        [Required, Display(Name = "Amount Per Employee Buy")]
        public decimal AmountPerEmp { get; set; }
        [Required, Display(Name = "Account Package Buy GST (%)")]
        public decimal AccountPackageBuyGSTPer { get; set; }
        [Required, Display(Name = "SMS Package Buy GST (%)")]
        public decimal SMSPackageBuyGSTPer { get; set; }
        [Required, Display(Name = "Employee Buy GST (%)")]
        public decimal EmployeeBuyGSTPer { get; set; }

        [Required, Display(Name = "SMTP Host")]
        public string SMTPHost { get; set; }
        [Required, Display(Name = "SMTP Port")]
        public Nullable<int> SMTPPort { get; set; }
        [Required, Display(Name = "SMTP Email")]
        public string SMTPEmail { get; set; }
        [Required, Display(Name = "SMTP Password")]
        public string SMTPPassword { get; set; }
        [Required, Display(Name = "SMTP Enable SSL")]
        public bool? SMTPEnableSSL { get; set; }
        [Required, Display(Name = "SMTP From Email Id")]
        public string SMTPFromEmailId { get; set; }
        [Required, Display(Name = "Super Admin Email Id")]
        public string SuperAdminEmailId { get; set; }
        [Required, Display(Name = "Super Admin Mobile No")]
        public string SuperAdminMobileNo { get; set; }

        [Required, Display(Name = "Is Stripe Live Mode ?")]
        public bool? IsStripeLiveMode { get; set; }

        [Required, Display(Name = "Stripe Sandbox Mode API Key")]
        public string StripeSandboxModeAPIKey { get; set; }

        [Required, Display(Name = "Stripe Live Mode API Key")]
        public string StripeLiveModeAPIKey { get; set; }

        [Display(Name = "Service Image")]
        public HttpPostedFileBase ServiceImageFile { get; set; }

        [Display(Name = "Home Image")]
        public HttpPostedFileBase HomeImageFile { get; set; }

        [Display(Name = "Home Image 2")]
        public HttpPostedFileBase HomeImageFile2 { get; set; }

        public string HomeImage { get; set; }
        public string HomeImage2 { get; set; }
        public string ServiceImage { get; set; }
    }
}