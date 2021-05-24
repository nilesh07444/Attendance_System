using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class HolidayVM
    {
        public long HolidayId { get; set; }
        [Required, Display(Name = "Start Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime StartDate { get; set; }

        [Required, Display(Name = "End Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime EndDate { get; set; }

        [Required, Display(Name = "Holiday Reason")]
        public string HolidayReason { get; set; }
        [Display(Name = "Remark")]
        public string Remark { get; set; }
        public string CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class HolidayFilterVM
    {
        public HolidayFilterVM()
        {
            StartMonth = DateTime.Now.Month;
            EndMonth = DateTime.Now.Month;
            Year = DateTime.Now.Year;
        }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public List<HolidayVM> HolidayList { get; set; }
        public List<SelectListItem> CalenderMonth { get; set; }
    }
}