//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AttendanceSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_WorkerAttendance
    {
        public long WorkerAttendanceId { get; set; }
        public long EmployeeId { get; set; }
        public System.DateTime AttendanceDate { get; set; }
        public bool IsMorning { get; set; }
        public Nullable<long> MorningSiteId { get; set; }
        public Nullable<long> MorningAttendanceBy { get; set; }
        public Nullable<System.DateTime> MorningAttendanceDate { get; set; }
        public Nullable<decimal> MorningLatitude { get; set; }
        public Nullable<decimal> MorningLongitude { get; set; }
        public string MorningLocationFrom { get; set; }
        public bool IsAfternoon { get; set; }
        public Nullable<long> AfternoonSiteId { get; set; }
        public Nullable<long> AfternoonAttendanceBy { get; set; }
        public Nullable<System.DateTime> AfternoonAttendanceDate { get; set; }
        public Nullable<decimal> AfternoonLatitude { get; set; }
        public Nullable<decimal> AfternoonLongitude { get; set; }
        public string AfternoonLocationFrom { get; set; }
        public bool IsEvening { get; set; }
        public Nullable<long> EveningSiteId { get; set; }
        public Nullable<long> EveningAttendanceBy { get; set; }
        public Nullable<System.DateTime> EveningAttendanceDate { get; set; }
        public Nullable<decimal> EveningLatitude { get; set; }
        public Nullable<decimal> EveningLongitude { get; set; }
        public string EveningLocationFrom { get; set; }
        public Nullable<decimal> ExtraHours { get; set; }
        public Nullable<decimal> NoOfHoursWorked { get; set; }
        public Nullable<int> NoOfUnitWorked { get; set; }
        public bool IsClosed { get; set; }
        public decimal TodaySalary { get; set; }
        public decimal SalaryGiven { get; set; }
    }
}
