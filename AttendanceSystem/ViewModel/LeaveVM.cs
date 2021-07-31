using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class LeaveVM
    {
        public long LeaveId { get; set; }
        public long UserId { get; set; }
        [Display(Name = "Employee Name")]
        public string UserName { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "No Of Days")]
        public decimal NoOfDays { get; set; }
        [Display(Name = "Leave Status")]
        public int LeaveStatus { get; set; }
        public string LeaveStatusText { get; set; }
        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }
        public bool IsDeleted { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime CreatedDate { get; set; }

        public string ContactNo { get; set; }
    }
    public class LeaveFilterVM
    {
        public LeaveFilterVM()
        {
            StartMonth = CommonMethod.CurrentIndianDateTime().Month;
            EndMonth = CommonMethod.CurrentIndianDateTime().Month;
            Year = CommonMethod.CurrentIndianDateTime().Year;
        }
        public int? UserRole { get; set; }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public int? LeaveStatus { get; set; }
        public List<LeaveVM> LeaveList { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public List<SelectListItem> CalenderMonth { get; set; }
    }
}