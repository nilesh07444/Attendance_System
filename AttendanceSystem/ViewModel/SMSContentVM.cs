using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class SMSContentVM
    {
        public int SMSContentId { get; set; }
        
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "SMS Title *")]
        public string SMSTitle { get; set; }
        
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "SMS Description *")]
        public string SMSDescription { get; set; }
        
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Sequence Number *")]
        public int? SeqNo { get; set; }
        
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
    }
}