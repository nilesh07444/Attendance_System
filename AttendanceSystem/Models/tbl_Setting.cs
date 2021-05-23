//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AttendanceSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_Setting
    {
        public long SettingId { get; set; }
        public Nullable<int> AccountFreeAccessDays { get; set; }
        public Nullable<decimal> AmountPerEmp { get; set; }
        public Nullable<decimal> AccountPackageBuyGSTPer { get; set; }
        public Nullable<decimal> SMSPackageBuyGSTPer { get; set; }
        public Nullable<decimal> EmployeeBuyGSTPer { get; set; }
        public string SMTPHost { get; set; }
        public Nullable<int> SMTPPort { get; set; }
        public string SMTPEmail { get; set; }
        public string SMTPPassword { get; set; }
        public Nullable<bool> SMTPEnableSSL { get; set; }
        public string SMTPFromEmailId { get; set; }
        public string SuperAdminEmailId { get; set; }
        public string SuperAdminMobileNo { get; set; }
        public string RazorPayKey { get; set; }
        public string RazorPaySecret { get; set; }
        public string ServiceImage { get; set; }
        public string HomeImage { get; set; }
    }
}
