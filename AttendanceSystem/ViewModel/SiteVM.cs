using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class SiteVM
    {
        public long SiteId { get; set; }
        [Required, Display(Name = "Site Name")]
        public string SiteName { get; set; }

        [Required, Display(Name = "Site Description")]
        public string SiteDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}