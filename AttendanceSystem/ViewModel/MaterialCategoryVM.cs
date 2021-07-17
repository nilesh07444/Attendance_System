using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class MaterialCategoryVM
    {
        public long MaterialCategoryId { get; set; }

        [Display(Name = "Material Category Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string MaterialCategoryName { get; set; }

        [Display(Name = "Material Category Description *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}