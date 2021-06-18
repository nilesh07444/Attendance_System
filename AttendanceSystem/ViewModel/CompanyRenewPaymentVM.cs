using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class CompanyRenewPaymentVM
    {
        public long CompanyRegistrationPaymentId { get; set; }
        public long CompanyId { get; set; }
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
        public string PaymentFor { get; set; }
        public string PaymentGatewayResponseId { get; set; }
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        [Display(Name = "Access Days")]
        public int AccessDays { get; set; }
        public long PackageId { get; set; }
        [Display(Name = "Package Name")]
        public string PackageName { get; set; }
        [Display(Name = "No of Employee")]
        public int NoOfEmployee { get; set; }
        [Display(Name = "No Of SMS")]
        public int NoOfSMS { get; set; }
        [Display(Name = "Buy No Of Employee")]
        public int BuyNoOfEmployee { get; set; }
        [Display(Name = "Renew Date")]
        public DateTime CreatedDate { get; set; }
    }

    public class PackageBuyVM
    {
        public long PackageId { get; set; }
    }

    public class CompanyRenewFilterVM
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<CompanyRenewPaymentVM> CompanyRenewList { get; set; }
    }
}