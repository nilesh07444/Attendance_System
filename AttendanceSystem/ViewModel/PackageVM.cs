using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class PackageVM
    {
        public long PackageId { get; set; }
        [Display(Name = "Package Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string PackageName { get; set; }

        [Display(Name = "Package Amount *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public decimal Amount { get; set; }

        [Display(Name = "Package Description *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string PackageDescription { get; set; }

        [Display(Name = "Access Days *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int AccessDays { get; set; }

        public string PackageImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Package Image")]
        public HttpPostedFileBase PackageImageFile { get; set; }

        [Display(Name = "No of SMS *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int NoOfSMS { get; set; }

        [Display(Name = "No of Employee *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int NoOfEmployee { get; set; }

        [Display(Name = "Package Color Code *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string PackageColorCode { get; set; }

        [Display(Name = "Package Font Icon *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string PackageFontIcon { get; set; }

    }
}