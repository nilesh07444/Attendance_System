using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class CompanyRequestVM
    {

        public long CompanyRequestId { get; set; }

        
        [Display(Name = "Company Type")]
        [Required(ErrorMessage = "Company Type is required")]
        public long? CompanyTypeId { get; set; }

        [MaxLength(2)]
        [Required(ErrorMessage = "Company Name is required")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Prefix")]
        [Required(ErrorMessage = "Prefix is required")]
        public string Prefix { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required")]
        public string Firstname { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required")]
        public string Lastname { get; set; }
        
        [Display(Name = "Email Id")]
        public string EmailId { get; set; }
         
        [Display(Name = "Mobile No")]
        [Required(ErrorMessage = "Mobile No is required")]
        public string MobileNo { get; set; }

        [Display(Name = "Alternate Mobile No")]
        public string AlternateMobileNo { get; set; }

        [Display(Name = "City")]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Display(Name = "State")]
        [Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Display(Name = "Aadhar Card No")]
        [Required(ErrorMessage = "Aadhar Card No is required")]
        public string AadharCardNo { get; set; }

        [Display(Name = "GST No")]
        public string GSTNo { get; set; }

        [Display(Name = "Pan Card No")]
        [Required(ErrorMessage = "Pan Card No is required")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN No")]
        public string PanCardNo { get; set; }

        [Display(Name = "Pan Card Photo")]
        public HttpPostedFileBase PanCardPhotoFile { get; set; }
         
        [Display(Name = "Aadhar Card Photo")]
        public HttpPostedFileBase AadharCardPhotoFile { get; set; }

        [Display(Name = "GST Photo")]
        public HttpPostedFileBase GSTPhotoFile { get; set; }

        [Display(Name = "Company Photo")]
        public HttpPostedFileBase CompanyPhotoFile { get; set; }

        [Display(Name = "Cancel Cheque Photo")]
        public HttpPostedFileBase CancellationChequePhotoFile { get; set; }

        //
        public int RequestStatus { get; set; }
        public long? CompanyId { get; set; }
        public int FreeAccessDays { get; set; }
        public List<SelectListItem> CompanyTypeList { get; set; }

        public string CompanyTypeText { get; set; }
        public string RejectReason { get; set; }

        public string PanCardPhoto { get; set; }
        public string AadharCardPhoto { get; set; }
        public string GSTPhoto { get; set; }
        public string CompanyPhoto { get; set; }
        public string CancellationChequePhoto { get; set; }

        [Display(Name = "Company Code")]
        public string CompanyCode{ get; set; }
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

        [Display(Name = "Company Photo")]
        public HttpPostedFileBase CompanyPhotoFile { get; set; }

        [Display(Name = "Cancel Cheque Photo")]
        public HttpPostedFileBase CancellationChequePhotoFile { get; set; }

        public long CompanyId { get; set; }
        public int FreeAccessDays { get; set; }
        public string CompanyTypeText { get; set; }

        public string GSTPhoto { get; set; }
        public string CompanyPhoto { get; set; }
        public string CancellationChequePhoto { get; set; }

        [Display(Name = "Company Code")]
        public string CompanyCode { get; set; }
    }
}