﻿using System;
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

        [Display(Name = "Contact Person Name *")]
        [Required(ErrorMessage ="This field is required")]
        public string CompanyPersonName { get; set; }
        
        [Display(Name = "Feedback *")]
        [Required(ErrorMessage = "This field is required")]
        public string FeedbackText { get; set; }

        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}