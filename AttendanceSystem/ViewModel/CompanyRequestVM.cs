using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class CompanyRequestVM
    {

        public long CompanyRequestId { get; set; }


        [Required, Display(Name = "Company Type")]
        public long? CompanyTypeId { get; set; }

        [MinLength(2)]
        [Required, Display(Name = "Company Name")]
        [RegularExpression("^[a-zA-Z\\s]+", ErrorMessage = "special characters are not allowed.")]

        public string CompanyName { get; set; }

        [Required, Display(Name = "Company EmailId")]
        public string CompanyEmailId { get; set; }

        [Required, Display(Name = "Company ContactNo No")]
        public string CompanyContactNo { get; set; }

        [Display(Name = "Company Alternate Mobile No")]
        public string CompanyAlternateContactNo { get; set; }

        [Display(Name = "Company GST No")]
        public string CompanyGSTNo { get; set; }


        [Display(Name = "Company GST Photo")]
        public HttpPostedFileBase CompanyGSTPhotoFile { get; set; }
        public string CompanyGSTPhoto { get; set; }

        [Required, Display(Name = "Company Pan Card No")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN No")]
        public string CompanyPanNo { get; set; }

        [Display(Name = "Company Pan Card Photo")]
        public HttpPostedFileBase CompanyPanPhotoFile { get; set; }
        public string CompanyPanPhoto { get; set; }
        [Required, Display(Name = "Company Address")]
        public string CompanyAddress { get; set; }
        [Required, Display(Name = "Company Pincode")]
        public string CompanyPincode { get; set; }
        [Required, Display(Name = "Company City")]
        public string CompanyCity { get; set; }
        [Required, Display(Name = "Company State")]
        public string CompanyState { get; set; }

        [Display(Name = "Company Logo Image")]
        public HttpPostedFileBase CompanyLogoImageFile { get; set; }
        public string CompanyLogoImage { get; set; }

        [Display(Name = "Company Register Proof Image")]
        public HttpPostedFileBase CompanyRegisterProofImageFile { get; set; }
        public string CompanyRegisterProofImage { get; set; }
        [Required, Display(Name = "Company Description")]
        public string CompanyDescription { get; set; }
        [Display(Name = "Company website Url")]
        public string CompanyWebisteUrl { get; set; }

        [Required, Display(Name = "Prefix")]
        public string CompanyAdminPrefix { get; set; }

        [Required, Display(Name = "First Name")]
        public string CompanyAdminFirstName { get; set; }

        [Required, Display(Name = "Middle Name")]
        public string CompanyAdminMiddleName { get; set; }

        [Required, Display(Name = "Last Name")]
        public string CompanyAdminLastName { get; set; }
        [Required, Display(Name = "Company Admin Date Of Birth")]
        public DateTime CompanyAdminDOB { get; set; }

        [Display(Name = "Email Id")]
        public string CompanyAdminEmailId { get; set; }

        [Required, Display(Name = "Mobile No")]
        public string CompanyAdminMobileNo { get; set; }

        [Display(Name = "Alternate Mobile No")]
        public string CompanyAdminAlternateMobileNo { get; set; }

        [Required, Display(Name = "Designation")]
        public string CompanyAdminDesignation { get; set; }

        [Required, Display(Name = "Address")]
        public string CompanyAdminAddress { get; set; }

        [Required, Display(Name = "Pincode")]
        public string CompanyAdminPincode { get; set; }

        [Required, Display(Name = "City")]
        public string CompanyAdminCity { get; set; }

        [Required, Display(Name = "State")]
        public string CompanyAdminState { get; set; }

        [Required, Display(Name = "Aadhar Card No")]
        public string CompanyAdminAadharCardNo { get; set; }

        [Display(Name = "Profile Photo")]
        public HttpPostedFileBase CompanyAdminProfilePhotoFile { get; set; }
        public string CompanyAdminProfilePhoto { get; set; }

        [Display(Name = "Aadhar Card Photo")]
        public HttpPostedFileBase CompanyAdminAadharCardPhotoFile { get; set; }
        public string CompanyAdminAadharCardPhoto { get; set; }

        [Required,Display(Name = "Pan Card No")]
        public string CompanyAdminPanCardNo { get; set; }

        [Display(Name = "Pan Card Photo")]
        public HttpPostedFileBase CompanyAdminPanCardPhotoFile { get; set; }
        public string CompanyAdminPanCardPhoto { get; set; }
        [Display(Name = "Request Status")]
        public int RequestStatus { get; set; }
        public string RejectReason { get; set; }
        public long? CompanyId { get; set; }
        public int FreeAccessDays { get; set; }
        public List<SelectListItem> CompanyTypeList { get; set; }

        public string CompanyTypeText { get; set; }

        [Display(Name = "Company Code")]
        public string CompanyCode { get; set; }
        public string OTP { get; set; }
        [Display(Name = "I accept the terms and conditions.")]
        public bool IsAccept { get; set; }
    }

    public class CompanyRequestFilterVM
    {
        public int RequestStatus { get; set; }
        public List<CompanyRequestVM> companyRequest { get; set; }
    }

    public class RegisteredCompanyVM
    {

        [Display(Name = "Company Type")]
        [Required(ErrorMessage = "Company Type is required")]
        public long? CompanyTypeId { get; set; }


        [Required(ErrorMessage = "Company Name is required")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "State is required")]
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
    }
}