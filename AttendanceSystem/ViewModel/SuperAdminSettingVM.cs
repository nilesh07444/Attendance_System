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
    }
}