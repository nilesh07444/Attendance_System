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
    
    public partial class tbl_EmployeeLunchBreak
    {
        public long EmployeeLunchBreakId { get; set; }
        public long EmployeeId { get; set; }
        public System.DateTime StartDateTime { get; set; }
        public Nullable<System.DateTime> EndDateTime { get; set; }
        public string StartLunchLocationFrom { get; set; }
        public Nullable<decimal> StartLunchLatitude { get; set; }
        public Nullable<decimal> StartLunchLongitude { get; set; }
        public string EndLunchLocationFrom { get; set; }
        public Nullable<decimal> EndLunchLatitude { get; set; }
        public Nullable<decimal> EndLunchLongitude { get; set; }
        public Nullable<long> AttendanceId { get; set; }
        public Nullable<int> LunchBreakNo { get; set; }
    }
}
