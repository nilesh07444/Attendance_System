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
        [Required, Display(Name = "Material Category Name")]
        public string MaterialCategoryName { get; set; }

        [Required, Display(Name = "Material Category Description")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}