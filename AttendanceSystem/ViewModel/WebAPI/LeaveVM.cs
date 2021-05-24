using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class LeaveVM
    {
        public long? LeaveId { get; set; }
        public long EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveStatus { get; set; }
        public decimal NoOfDays { get; set; }
        public string LeaveReason { get; set; }
        public string RejectReason { get; set; }
        public string CancelledReason { get; set; }
    }

    public class LeaveFilterVM
    {
        public LeaveFilterVM()
        {
            StartMonth = DateTime.Now.Month;
            EndMonth = DateTime.Now.Month;
            Year = DateTime.Now.Year;
        }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public int? LeaveStatus { get; set; }
    }
}