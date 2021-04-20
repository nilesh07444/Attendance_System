using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class CompanyRequestVM
    {
        public long CompanyRequestId { get; set; }
        public long CompanyTypeId { get; set; }
        public string CompanyName { get; set; }
        public string Prefix { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobileNo { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string AadharCardNo { get; set; }
        public string GSTNo { get; set; }
        public string PanCardNo { get; set; }
        public string PanCardPhoto { get; set; }
        public string AadharCardPhoto { get; set; }
        public string GSTPhoto { get; set; }
        public string CompanyPhoto { get; set; }
        public string CancellationChequePhoto { get; set; }
        public string RequestStatus { get; set; }
        public string RejectReason { get; set; }
        public long? CompanyId { get; set; }
        public decimal RegistrationFee { get; set; }
        public int FreeAccessDays { get; set; }
        public bool IsDeleted { get; set; }
        public List<SelectListItem> CompanyTypeList { get; set; }
        public HttpPostedFileBase PanCardPhotoFile { get; set; }
        public HttpPostedFileBase AadharCardPhotoFile { get; set; }
        public HttpPostedFileBase GSTPhotoFile { get; set; }
        public HttpPostedFileBase CompanyPhotoFile { get; set; }
        public HttpPostedFileBase CancellationChequePhotoFile { get; set; }

    }
}