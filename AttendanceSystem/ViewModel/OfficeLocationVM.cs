using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class OfficeLocationVM
    {
        public long OfficeLocationId { get; set; } 

        [Display(Name = "Office Location Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string OfficeLocationName { get; set; }

        [Display(Name = "Office Location Description *")]
        [Required(ErrorMessage = "This field is required")]
        public string OfficeLocationDescription { get; set; }
        
        public bool IsActive { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? RadiousInMeter { get; set; }
    }

    public class OfficeLocationFilterVM
    {
        public int? IsLocationSet { get; set; }
        public List<OfficeLocationVM> OfficeLocationList { get; set; }
    }

}