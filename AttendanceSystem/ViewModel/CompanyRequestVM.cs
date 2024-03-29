﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class CompanyRequestVM
    {
        public long CompanyRequestId { get; set; }

        [Display(Name = "Company Type *")]
        [Required(ErrorMessage = "This field is required")]
        public long? CompanyTypeId { get; set; }

        [MinLength(2)]
        [Required(ErrorMessage = "This field is required")]
        [Display(Name = "Company Name *")]
        [RegularExpression("^[a-zA-Z\\s]+", ErrorMessage = "special characters are not allowed.")]

        public string CompanyName { get; set; }

        [Display(Name = "Company Email Id *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyEmailId { get; set; }

        [Display(Name = "Company Contact No *")]
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Company Contact Number.")]
        public string CompanyContactNo { get; set; }

        [Display(Name = "Company Alternate Mobile No")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Company Alternate Mobile No.")]
        public string CompanyAlternateContactNo { get; set; }

        [Display(Name = "Company GST No")]
        public string CompanyGSTNo { get; set; }

        [Display(Name = "Company GST Photo")]
        public HttpPostedFileBase CompanyGSTPhotoFile { get; set; }
        public string CompanyGSTPhoto { get; set; }

        [Display(Name = "Company Pan Card No")]
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN No")]
        public string CompanyPanNo { get; set; }

        [Display(Name = "Company Pan Card Photo")]
        public HttpPostedFileBase CompanyPanPhotoFile { get; set; }
        public string CompanyPanPhoto { get; set; }

        [Display(Name = "Company Address *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAddress { get; set; }

        [Display(Name = "Company Pincode *")]
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "Invalid Pincode.")]
        public string CompanyPincode { get; set; }

        [Display(Name = "Company City *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyCity { get; set; }

        [Display(Name = "Company State *")]
        //[Required(ErrorMessage = "This field is required")]
        public string CompanyState { get; set; }

        [Display(Name = "Company District *")]
        public string CompanyDistrict { get; set; }

        [Display(Name = "State *")]
        //[Required(ErrorMessage = "This field is required")]
        public long? CompanyStateId { get; set; }

        [Display(Name = "District *")]
        public long? CompanyDistrictId { get; set; }

        [Display(Name = "Company Logo Image")]
        public HttpPostedFileBase CompanyLogoImageFile { get; set; }
        public string CompanyLogoImage { get; set; }

        [Display(Name = "Company Register Proof Image")]
        public HttpPostedFileBase CompanyRegisterProofImageFile { get; set; }
        public string CompanyRegisterProofImage { get; set; }

        [Display(Name = "Company Description *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyDescription { get; set; }

        [Display(Name = "Company website Url")]
        //[Url]
        public string CompanyWebisteUrl { get; set; }

        [Display(Name = "Company Conversion Type *")]
        //[Required(ErrorMessage = "This field is required")]
        public int? CompanyConversionType { get; set; }

        [Display(Name = "Prefix *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminPrefix { get; set; }

        [Display(Name = "First Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminFirstName { get; set; }

        [Display(Name = "Middle Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminMiddleName { get; set; }

        [Display(Name = "Last Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminLastName { get; set; }

        [Display(Name = "Date Of Birth *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminDOB { get; set; }

        [Display(Name = "Date Of Marriage Anniversary *")]
        //[Required(ErrorMessage = "This field is required")]
        public DateTime? CompanyAdminDateOfMarriageAnniversary { get; set; }

        [Display(Name = "Email Id")]
        public string CompanyAdminEmailId { get; set; }

        [Display(Name = "Mobile No *")]
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string CompanyAdminMobileNo { get; set; }

        [Display(Name = "Alternate Mobile No")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Alternate Mobile Number.")]
        public string CompanyAdminAlternateMobileNo { get; set; }

        [Display(Name = "Designation *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminDesignation { get; set; }

        [Display(Name = "Address *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminAddress { get; set; }

        [Display(Name = "Pincode *")]
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "Invalid Pincode.")]
        public string CompanyAdminPincode { get; set; }

        [Display(Name = "City *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyAdminCity { get; set; }

        [Display(Name = "State *")]
        //[Required(ErrorMessage = "This field is required")]
        public string CompanyAdminState { get; set; }

        [Display(Name = "District *")]
        public string CompanyAdminDistrict { get; set; }

        [Display(Name = "State *")]
        //[Required(ErrorMessage = "This field is required")]
        public long? CompanyAdminStateId { get; set; }

        [Display(Name = "District *")]
        public long? CompanyAdminDistrictId { get; set; }

        [Display(Name = "Aadhar Card No *")]
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression(@"^([0-9]{12})$", ErrorMessage = "Invalid Adhar Card No.")]
        public string CompanyAdminAadharCardNo { get; set; }

        [Display(Name = "Profile Photo")]
        public HttpPostedFileBase CompanyAdminProfilePhotoFile { get; set; }
        public string CompanyAdminProfilePhoto { get; set; }

        [Display(Name = "Aadhar Card Photo")]
        public HttpPostedFileBase CompanyAdminAadharCardPhotoFile { get; set; }
        public string CompanyAdminAadharCardPhoto { get; set; }

        [Display(Name = "Pan Card No *")]
        [Required(ErrorMessage = "This field is required")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN No")]
        public string CompanyAdminPanCardNo { get; set; }

        [Display(Name = "Pan Card Photo")]
        public HttpPostedFileBase CompanyAdminPanCardPhotoFile { get; set; }
        public string CompanyAdminPanCardPhoto { get; set; }

        [Display(Name = "Request Status *")]
        public int RequestStatus { get; set; }

        [Display(Name = "Reject Reason *")]
        public string RejectReason { get; set; }

        public long? CompanyId { get; set; }
        public int FreeAccessDays { get; set; }
        public List<SelectListItem> CompanyTypeList { get; set; }

        public string CompanyTypeText { get; set; }

        [Display(Name = "Company Code *")]
        public string CompanyCode { get; set; }
        public string OTP { get; set; }
        [Display(Name = "I accept the terms and conditions.")]
        public bool IsAccept { get; set; }

        // Additional
        public DateTime dtCompanyAdminDOB { get; set; }
        public string RequestStatusText { get; set; }
        [Display(Name = "Employee Code *")]
        public string EmployeeCode { get; set; }

        public DateTime? CreatedDate { get; set; }
        [Display(Name = "Trail Mode")]
        public bool IsTrialMode { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime? TrialExpiryDate { get; set; }
        [Display(Name = "Expiry Date")]
        public DateTime? AccountExpiryDate { get; set; }

        public List<SelectListItem> CompanyStateList { get; set; }
        public List<SelectListItem> CompanyDistrictList { get; set; }

        public List<SelectListItem> CompanyAdminStateList { get; set; }
        public List<SelectListItem> CompanyAdminDistrictList { get; set; }
    }

    public class CompanyRequestFilterVM
    {
        public int RequestStatus { get; set; }
        public List<CompanyRequestVM> companyRequest { get; set; }
    }

    public class RegisteredCompanyVM
    {

        [Display(Name = "Company Type *")]
        [Required(ErrorMessage = "This field is required")]
        public long? CompanyTypeId { get; set; }

        [Display(Name = "Company Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string CompanyName { get; set; }

        [Display(Name = "City *")]
        [Required(ErrorMessage = "This field is required")]
        public string City { get; set; }

        [Display(Name = "State *")]
        [Required(ErrorMessage = "This field is required")]
        public string State { get; set; }

        [Display(Name = "GST No")]
        public string GSTNo { get; set; }

        [Display(Name = "GST Photo")]
        public HttpPostedFileBase GSTPhotoFile { get; set; }

        [Display(Name = "Company Logo Image")]
        public HttpPostedFileBase CompanyLogoImageFile { get; set; }

        [Display(Name = "Cancel Cheque Photo")]
        public HttpPostedFileBase CancellationChequePhotoFile { get; set; }

        public long CompanyId { get; set; }
        public int FreeAccessDays { get; set; }
        public string CompanyTypeText { get; set; }

        public string GSTPhoto { get; set; }
        public string CompanyLogoImage { get; set; }
        public string CancellationChequePhoto { get; set; }

        [Display(Name = "Company Code")]
        public string CompanyCode { get; set; }

        public bool IsActive { get; set; }

        public bool IsTrialMode { get; set; }
        public DateTime? TrialExpiryDate { get; set; }
        public DateTime? AccountExpiryDate { get; set; }
        public string ContactNo { get; set; }
        public string AlternateContactNo { get; set; }
        public string District { get; set; }
        public string PanNo { get; set; }
        public string Description { get; set; }
        public string WebsiteUrl { get; set; }
        public string EmailId { get; set; }
        public string Address { get; set; }
        public string Pincode { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    public class CompanyRegisteredFilterVM
    {
        public long? CompanyTypeId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<RegisteredCompanyVM> RegisteredCompanyList { get; set; }
        public List<SelectListItem> CompanyTypeList { get; set; }
    }
}