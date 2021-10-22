using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class StateVM
    {
        public long StateId { get; set; }
        [Display(Name = "State Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string StateName { get; set; }
        public long CountryId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
      
}