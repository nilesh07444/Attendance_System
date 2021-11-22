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
    
    public partial class tbl_CompanyRequest
    {
        public long CompanyRequestId { get; set; }
        public long CompanyTypeId { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmailId { get; set; }
        public string CompanyContactNo { get; set; }
        public string CompanyAlternateContactNo { get; set; }
        public string CompanyGSTNo { get; set; }
        public string CompanyGSTPhoto { get; set; }
        public string CompanyPanNo { get; set; }
        public string CompanyPanPhoto { get; set; }
        public string CompanyAddress { get; set; }
        public string CompanyPincode { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyState { get; set; }
        public string CompanyDistrict { get; set; }
        public Nullable<long> CompanyStateId { get; set; }
        public Nullable<long> CompanyDistrictId { get; set; }
        public Nullable<long> StateId { get; set; }
        public Nullable<long> DistrictId { get; set; }
        public string CompanyLogoImage { get; set; }
        public string CompanyRegisterProofImage { get; set; }
        public string CompanyDescription { get; set; }
        public string CompanyWebisteUrl { get; set; }
        public string CompanyAdminPrefix { get; set; }
        public string CompanyAdminFirstName { get; set; }
        public string CompanyAdminMiddleName { get; set; }
        public string CompanyAdminLastName { get; set; }
        public System.DateTime CompanyAdminDOB { get; set; }
        public Nullable<System.DateTime> CompanyAdminDateOfMarriageAnniversary { get; set; }
        public string CompanyAdminEmailId { get; set; }
        public string CompanyAdminMobileNo { get; set; }
        public string CompanyAdminAlternateMobileNo { get; set; }
        public string CompanyAdminDesignation { get; set; }
        public string CompanyAdminAddress { get; set; }
        public string CompanyAdminPincode { get; set; }
        public string CompanyAdminCity { get; set; }
        public string CompanyAdminState { get; set; }
        public string CompanyAdminDistrict { get; set; }
        public Nullable<long> CompanyAdminStateId { get; set; }
        public Nullable<long> CompanyAdminDistrictId { get; set; }
        public string CompanyAdminProfilePhoto { get; set; }
        public string CompanyAdminAadharCardNo { get; set; }
        public string CompanyAdminAadharCardPhoto { get; set; }
        public string CompanyAdminPanCardPhoto { get; set; }
        public string CompanyAdminPanCardNo { get; set; }
        public int RequestStatus { get; set; }
        public string RejectReason { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public int FreeAccessDays { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<int> CompanyConversionType { get; set; }
    }
}
