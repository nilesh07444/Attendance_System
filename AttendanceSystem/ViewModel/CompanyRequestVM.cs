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
        public long CompanyTypeId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Prefix")]
        public string Prefix { get; set; }
        [Display(Name = "First Name")]
        public string Firstname { get; set; }
        [Display(Name = "Last Name")]
        public string Lastname { get; set; }
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Email Id")]
        public string EmailId { get; set; }
        [Display(Name = "Mobile No")]
        public string MobileNo { get; set; }
        [Display(Name = "Alternate Mobile No")]
        public string AlternateMobileNo { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "State")]
        public string State { get; set; }
        [Display(Name = "Aadhar Card No")]
        public string AadharCardNo { get; set; }
        [Display(Name = "GST No")]
        public string GSTNo { get; set; }

        [Display(Name = "Pan Card No")]
        [RegularExpression("^([A-Za-z]){5}([0-9]){4}([A-Za-z]){1}$", ErrorMessage = "Invalid PAN No")]
        public string PanCardNo { get; set; }
        [Display(Name = "Pan Card Photo")]
        public string PanCardPhoto { get; set; }
        [Display(Name = "Aadhar Card Photo")]
        public string AadharCardPhoto { get; set; }
        [Display(Name = "GST Photo")]
        public string GSTPhoto { get; set; }
        [Display(Name = "Company Photo")]
        public string CompanyPhoto { get; set; }
        [Display(Name = "Cancel Cheque Photo")]
        public string CancellationChequePhoto { get; set; }
        [Display(Name = "Request Status")]
        public int RequestStatus { get; set; }
        [Display(Name = "Reject Reason")]
        public string RejectReason { get; set; }
        public long? CompanyId { get; set; }
        public int FreeAccessDays { get; set; }
        public List<SelectListItem> CompanyTypeList { get; set; }
        public HttpPostedFileBase PanCardPhotoFile { get; set; }
        public HttpPostedFileBase AadharCardPhotoFile { get; set; }
        public HttpPostedFileBase GSTPhotoFile { get; set; }
        public HttpPostedFileBase CompanyPhotoFile { get; set; }
        public HttpPostedFileBase CancellationChequePhotoFile { get; set; }
        public string CompanyTypeText { get; set; }
    }

    public class CompanyRequestFilterVM
    {
        public int RequestStatus { get; set; }
        public List<CompanyRequestVM> companyRequest { get; set; }
    }
}