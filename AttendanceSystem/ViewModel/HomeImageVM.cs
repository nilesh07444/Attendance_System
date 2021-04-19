﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class HomeImageVM
    {
        public long HomeImageId { get; set; }
        [Required, Display(Name = "Home Image For")]
        public int? HomeImageFor { get; set; }
        public string HomeImageName { get; set; }
        [Display(Name = "Heading Text 1")]
        public string HeadingText1 { get; set; }
        [Display(Name = "Heading Text 2")]
        public string HeadingText2 { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Home Image")]
        public HttpPostedFileBase HomeImageFile { get; set; }

        // Additional fields
        public string ImageUrl { get; set; }
        public List<SelectListItem> HomeImageForList { get; set; }
    }

    public class AdvertiseImageVM
    {
        public int AdvertiseImageId { get; set; }
        [Required, Display(Name = "Advertise Image For")]
        public int? ImageFor { get; set; }
        [Display(Name = "Advertise Image")]
        public HttpPostedFileBase AdvertiseImageFile { get; set; }

        public bool IsActive { get; set; }
        // Additional fields
        public string ImageUrl { get; set; }
        [Display(Name = "Slider Type")]
        public int SliderType { get; set; }
        public List<SelectListItem> SliderTypeList { get; set; }
        public List<SelectListItem> AdvertiseImageForList { get; set; }
    }
}