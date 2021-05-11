using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class PackageVM
    {
        public long PackageId { get; set; }
        [Required, Display(Name = "Package Name")]
        public string PackageName { get; set; }

        [Required, Display(Name = "Package Amount")]
        public decimal Amount { get; set; }
        [Required, Display(Name = "Package Description")]
        public string PackageDescription { get; set; }
        [Required, Display(Name = "Acess Days")]
        public int AccessDays { get; set; }
        public string PackageImage { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        [Display(Name = "Package Image")]
        public HttpPostedFileBase PackageImageFile { get; set; }
        [Display(Name = "No of SMS")]
        public int NoOfSMS { get; set; }
        [Display(Name = "No of Employee")]
        public int NoOfEmployee { get; set; }
    }
}