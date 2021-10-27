using System;
using System.Collections.Generic;

namespace AttendanceSystem
{
    public class EmployeeLunchBreakVM
    {
        public long EmployeeLunchBreakId { get; set; }
        public long EmployeeId { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string StartLunchLocationFrom { get; set; }
        public decimal? StartLunchLatitude { get; set; }
        public decimal? StartLunchLongitude { get; set; }
        public string EndLunchLocationFrom { get; set; }
        public decimal? EndLunchLatitude { get; set; }
        public decimal? EndLunchLongitude { get; set; }
        public int? LunchBreakNo { get; set; }

        // Additional
        public long? AttendaceId { get; set; }
        public DateTime? AttendaceDate { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeRole { get; set; }

    }

    public class EmployeeLunchBreakFilterVM
    {
        public EmployeeLunchBreakFilterVM()
        {
            StartDate = CommonMethod.CurrentIndianDateTime();
            EndDate = StartDate;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string EmployeeCode { get; set; }
        public List<EmployeeLunchBreakVM> EmployeeLunchBreakList { get; set; }

    }

    public class LunchBreakLocationVM
    {
        public string LocationFrom { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }

    public class LunchBreakStatusVM
    {
        public long EmployeeId { get; set; }
        public long? AttendanceId { get; set; }
        public long? LunchBreakId { get; set; }
        public bool IsLunchBreakRunning { get; set; }
    }

}