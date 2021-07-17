using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System;
using AttendanceSystem.Helper;

namespace AttendanceSystem.ViewModel
{
    public class EmployeeRatingVM
    {
        public long EmployeeRatingId { get; set; }

        [Display(Name = "Employee Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public long EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }

        [Display(Name = "Rate Month *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int RateMonth { get; set; }
        public string RateMonthText { get; set; }

        [Display(Name = "Rate Year *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int RateYear { get; set; }

        [Display(Name = "Behaviour Rate *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public decimal BehaviourRate { get; set; }

        [Display(Name = "Ragularity Rate *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public decimal RegularityRate { get; set; }

        [Display(Name = "Work Rate *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
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