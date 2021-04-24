using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class HolidayVM
    {
        public long HolidayId { get; set; }
        [Required, Display(Name = "Holiday Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime HolidayDate { get; set; }
        [Required, Display(Name = "Holiday Reason")]
        public string HolidayReason { get; set; }
        public string CompanyId { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class HolidayFilterVM
    {
        public HolidayFilterVM()
        {
            StartDate = new DateTime(DateTime.Now.Year, 1, 1);
            EndDate = new DateTime(DateTime.Now.Year, 1, 31);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<HolidayVM> HolidayList { get; set; }
    }
}