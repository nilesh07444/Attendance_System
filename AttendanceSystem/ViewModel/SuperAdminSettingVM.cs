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
        [Required, Display(Name = "Free Access Days (Site Company)")]
        public int SiteCompanyFreeAccessDays { get; set; }
        [Required, Display(Name = "Free Access Days (Office Company)")]
        public int OfficeCompanyFreeAccessDays { get; set; }
        [Required, Display(Name = "Amount Per Employee Buy")]
        public decimal AmountPerEmp { get; set; }
    }
}