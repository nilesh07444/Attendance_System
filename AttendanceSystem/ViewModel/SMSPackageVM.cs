using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class SMSPackageVM
    {
        public long SMSPackageId { get; set; }
        [Required, Display(Name = "Package Name")]
        public string PackageName { get; set; }

        [Required, Display(Name = "Package Amount")]
        public decimal PackageAmount { get; set; }
       
        [Required, Display(Name = "Acess Days")]
        public int AccessDays { get; set; }
        [Required, Display(Name = "No Of SMS")]
        public int NoOfSMS { get; set; }
        [Required, Display(Name = "Color Code")]
        public string PackageColorCode { get; set; }
        [Required, Display(Name = "Font Icon")]
        public string PackageFontIcon { get; set; }
        public string PackageImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Package Image")]
        public HttpPostedFileBase PackageImageFile { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
    }
}