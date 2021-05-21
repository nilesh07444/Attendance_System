using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class TestimonialVM
    {
        public long TestimonialId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPersonName { get; set; }
        public string FeedbackText { get; set; }
        public bool IsActive { get; set; }
    }
}