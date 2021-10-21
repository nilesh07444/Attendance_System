using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class CompanyAdminSettingVM
    {
        [Required, Display(Name = "No Of Lunch Break Allowed Per Day")]
        public int? NoOfLunchBreakAllowed { get; set; }
    }
}