using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem
{
    public class DynamicContentVM
    {
        public long DynamicContentId { get; set; }
        
        [Display(Name = "Content Type *")]
        [Required(ErrorMessage ="This field is required")]
        public int DynamicContentType { get; set; }
         
        [Display(Name = "Content Title *")]
        [Required(ErrorMessage = "This field is required")]
        public string ContentTitle { get; set; }
         
        [Display(Name = "Content Description *")]
        [Required(ErrorMessage = "This field is required")]
        public string ContentDescription { get; set; }

        [Display(Name = "Sequence Number *")]
        [Required(ErrorMessage = "This field is required")]
        public int? SeqNo { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }

        //
        public List<SelectListItem> DynamicContentTypeList { get; set; }
    }
}
