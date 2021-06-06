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
        [Display(Name = "Package Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string PackageName { get; set; }

        [Display(Name = "Package Amount *")]
        [Required(ErrorMessage = "This field is required")]
        public decimal PackageAmount { get; set; }

        [Display(Name = "Access Days *")]
        [Required(ErrorMessage = "This field is required")]
        public int AccessDays { get; set; }

        [Display(Name = "No Of SMS *")]
        [Required(ErrorMessage = "This field is required")]
        public int NoOfSMS { get; set; }

        [Display(Name = "Color Code *")]
        [Required(ErrorMessage = "This field is required")]
        public string PackageColorCode { get; set; }

        [Display(Name = "Font Icon *")]
        [Required(ErrorMessage = "This field is required")]
        public string PackageFontIcon { get; set; }

        [Display(Name = "Package Image")]
        public HttpPostedFileBase PackageImageFile { get; set; }

        [Display(Name = "Description *")]
        [Required(ErrorMessage = "This field is required")]
        public string Description { get; set; }

        public string PackageImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

    }
}