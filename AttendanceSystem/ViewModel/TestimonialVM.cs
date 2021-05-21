using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class TestimonialVM
    {
        public long TestimonialId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Required, Display(Name = "Contact Person Name")]
        public string CompanyPersonName { get; set; }
        [Required, Display(Name = "Feedback")]
        public string FeedbackText { get; set; }
        public bool IsActive { get; set; }
    }
}