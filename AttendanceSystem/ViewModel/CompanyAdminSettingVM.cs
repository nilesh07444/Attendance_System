using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class CompanyAdminSettingVM
    {
        [Display(Name = "No Of Lunch Break Allowed Per Attendance *")]
        [Required(ErrorMessage = "This field is required")]
        public int? NoOfLunchBreakAllowed { get; set; }

        [Display(Name = "Site Location Access Password")]
        public string SiteLocationAccessPassword { get; set; }

        [Display(Name = "Office Location Access Password")]
        public string OfficeLocationAccessPassword { get; set; }

        [Display(Name = "Company Conversion Type *")]
        [Required(ErrorMessage = "This field is required")]
        public int? CompanyConversionType { get; set; }
        
    }
}
