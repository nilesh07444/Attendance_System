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
    
    public partial class tbl_Employee
    {
        public long EmployeeId { get; set; }
        public long CompanyId { get; set; }
        public int AdminRoleId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobile { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Designation { get; set; }
        public Nullable<System.DateTime> Dob { get; set; }
        public Nullable<System.DateTime> DateOfJoin { get; set; }
        public string BloodGroup { get; set; }
        public string WorkingTime { get; set; }
        public string AdharCardNo { get; set; }
        public Nullable<System.DateTime> DateOfIdCardExpiry { get; set; }
        public string Remarks { get; set; }
        public string ProfilePicture { get; set; }
        public int EmploymentCategory { get; set; }
        public decimal PerCategoryPrice { get; set; }
        public Nullable<decimal> MonthlySalaryPrice { get; set; }
        public Nullable<decimal> ExtraPerHourPrice { get; set; }
        public bool IsLeaveForward { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsFingerprintEnabled { get; set; }
        public bool IsFingerprintSaved { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<long> UpdatedBy { get; set; }
        public Nullable<System.DateTime> UpdatedDate { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
    }
}
