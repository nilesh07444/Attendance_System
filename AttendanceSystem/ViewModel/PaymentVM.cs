using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class PaymentVM
    {
        public PaymentVM()
        {
            PaymentDate = CommonMethod.CurrentIndianDateTime();
            PendingSalary = 0;
        }
        public long EmployeePaymentId { get; set; }
        [Display(Name = "Payment Date *")]
        [Required(ErrorMessage = "This field is required")]
        public DateTime PaymentDate { get; set; }

        [Display(Name = "Select Employee")]
        [Required(ErrorMessage = "This field is required")]
        public long UserId { get; set; }
        public string UserName { get; set; }
        
        [Display(Name = "Credit Amount")]
        public decimal? CreditAmount { get; set; }
        
        [Display(Name = "Debit Amount")]
        public decimal? DebitAmount { get; set; }
        
        [Display(Name = "Payment Type")]
        [Required(ErrorMessage = "This field is required")]
        public int? PaymentType { get; set; }
          
        [Display(Name = "Remarks")]
        public string Remarks { get; set; }
         
        [Display(Name = "Pending Salary")]
        public decimal PendingSalary { get; set; }

        // Additional Field Name
        public int AdminRoleId { get; set; }
        public string EmployeeCode { get; set; }
        public string OTP { get; set; }
        public string PaymentTypeText { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> EmployeePaymentTypeList { get; set; }
        public string AdminRoleText { get; set; }
        public string AmountGivenBy { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        
    }

    public class PaymentFilterVM
    {
        public PaymentFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UserRole { get; set; }
        public List<PaymentVM> PaymentList { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
    }

    public class PaymentSummuryReportVM
    {
        public long EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public decimal PendingSalary { get; set; }
    }

    public class PaymentSummuryReportFilterVM
    {
        public PaymentSummuryReportFilterVM()
        {
            Month = CommonMethod.CurrentIndianDateTime().Month;
            Year = CommonMethod.CurrentIndianDateTime().Year;
        }
        public List<PaymentSummuryReportVM> PaymentSummuryReportList { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public long? EmployeeId { get; set; }
        public List<SelectListItem> CalenderMonth { get; set; }
    }

    public class PaymentReportFilterVM
    {
        public PaymentReportFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public long? EmployeeId { get; set; }
        public List<EmployeePaymentReportVM> PaymentReportList { get; set; }
    }

    public class EmployeePaymentReportVM
    {
        public DateTime Date { get; set; }
        public decimal Opening { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Closing { get; set; }
        public string Remark { get; set; }
    }
}