 using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; 
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem
{
    public class HomeSliderVM
    {
        public long HomeSliderId { get; set; }

        [Display(Name = "Slider Image")]
        public HttpPostedFileBase SliderImageFile { get; set; }

        [Display(Name = "Slider Type")]
        public int SliderType { get; set; }

        public bool IsActive { get; set; }

        // Additional fields
        public string SliderImageName { get; set; }
        public List<SelectListItem> SliderTypeList { get; set; }
    }
}