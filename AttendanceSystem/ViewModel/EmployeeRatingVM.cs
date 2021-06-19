using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;

namespace AttendanceSystem.ViewModel
{
    public class EmployeeRatingVM
    {
        public long EmployeeRatingId { get; set; }
        [Required, Display(Name = "Employee")]
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        [Required, Display(Name = "Rate Month")]
        public int RateMonth { get; set; }
        public string RateMonthText { get; set; }
        [Required, Display(Name = "Rate Year")]
        public int RateYear { get; set; }
        [Required, Display(Name = "Behaviour Rate")]
        public decimal BehaviourRate { get; set; }
        [Required, Display(Name = "Ragularity Rate")]
        public decimal RegularityRate { get; set; }
        [Required, Display(Name = "Work Rate")]
        public decimal WorkRate { get; set; }
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }

        public string AvgRate { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }

        public DateTime CreatedDate { get; set; }
    }

    public class EmployeeRatingFilterVM
    {
        public EmployeeRatingFilterVM()
        {
            StartMonth = CommonMethod.CurrentIndianDateTime().Month;
            EndMonth = CommonMethod.CurrentIndianDateTime().Month;
            Year = CommonMethod.CurrentIndianDateTime().Year;
        }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public int? UserRole { get; set; }
        public string EmployeeCode { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public List<EmployeeRatingVM> EmployeeRatingList { get; set; }
        public List<SelectListItem> CalenderMonth { get; set; }
    }

    public class CommonEmployeeRatingFilterVM
    {
        public CommonEmployeeRatingFilterVM()
        {
            StartMonth = CommonMethod.CurrentIndianDateTime().Month;
            EndMonth = CommonMethod.CurrentIndianDateTime().Month;
            Year = CommonMethod.CurrentIndianDateTime().Year;
        }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
    }
}