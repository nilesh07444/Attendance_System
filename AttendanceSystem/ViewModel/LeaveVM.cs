using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class LeaveVM
    {
        public long LeaveId { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal NoOfDays { get; set; }
        public int LeaveStatus { get; set; }
        public string RejectReason { get; set; }
        public string CancelledReason { get; set; }
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