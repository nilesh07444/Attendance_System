using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class FeedbackVM
    {

        public long FeedbackId { get; set; }
        [Required, Display(Name = "Company")]
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Required, Display(Name = "Feedback Type")]
        public int FeedbackType { get; set; }
        public string FeedbackTypeText { get; set; }
        public int FeedbackStatus { get; set; }
        public string FeedbackStatusText { get; set; }
        [Required, Display(Name = "Feedback")]
        public string FeedbackText { get; set; }
        [Required, Display(Name = "Remark")]
        public string Remarks { get; set; }
        [Display(Name = "Super Admin Feedback")]
        public string SuperAdminFeedbackText { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        [Display(Name = "Feedback Date")]
        public DateTime CreatedDate { get; set; }
        public List<SelectListItem> FeedBackTypeList { get; set; }
        public List<SelectListItem> FeedBackStatusList { get; set; }
    }

    public class FeedBackFilterVM
    {
        public int? FeedbackType { get; set; }
        public int? FeedbackStatus { get; set; }
        public List<SelectListItem> FeedBackTypeList { get; set; }
        public List<SelectListItem> FeedBackStatusList { get; set; }

        public List<FeedbackVM> FeedBackList { get; set; }
    }
}