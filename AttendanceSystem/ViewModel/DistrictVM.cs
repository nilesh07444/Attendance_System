using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem
{
    public class DistrictVM
    {
        public long DistrictId { get; set; }
        
        [Display(Name = "District Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string DistrictName { get; set; }
        
        [Display(Name = "State Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public long StateId { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
         
        // Additional
        public string StateName { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public bool IsActive { get; set; } 
    }

    public class DistrictFilterVM
    {
        public long? StateId { get; set; }
        public List<SelectListItem> StateList { get; set; }

        public List<DistrictVM> DistrictList { get; set; }
    }
}