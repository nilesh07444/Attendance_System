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
    
    public partial class tbl_AdminUser
    {
        public long AdminUserId { get; set; }
        public int AdminUserRoleId { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string MIddleName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public System.DateTime DOB { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobileNo { get; set; }
        public string Designation { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AadharCardNo { get; set; }
        public string AadharCardPhoto { get; set; }
        public string PanCardNo { get; set; }
        public string PanCardPhoto { get; set; }
        public string ProfilePhoto { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
    }
}
