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

    }
    public class LeaveFilterVM
    {
        public LeaveFilterVM()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public int? UserRole { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? LeaveStatus { get; set; }
        public List<LeaveVM> LeaveList { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
    }
}