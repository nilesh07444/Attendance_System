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
        [Display(Name = "Site Location *")]
        [Required(ErrorMessage = "This field is required")]
        public string SiteName { get; set; }

        [Display(Name = "Site Description *")]
        [Required(ErrorMessage = "This field is required")]
        public string SiteDescription { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? RadiousInMeter { get; set; }

    }
}