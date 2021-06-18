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
            PaymentDate = DateTime.Now;
            PendingSalary = 0;
        }
        public long EmployeePaymentId { get; set; }
        [Required, Display(Name = "Payment Date")]
        public DateTime PaymentDate { get; set; }
        [Required, Display(Name = "Employee")]
        public long UserId { get; set; }
        public string UserName { get; set; }
        [Display(Name = "Credit Amount")]
        public decimal? CreditAmount { get; set; }
        [Display(Name = "Debit Amount")]
        public decimal? DebitAmount { get; set; }
        [Required, Display(Name = "Payment Type")]
        public int? PaymentType { get; set; }
        public string PaymentTypeText { get; set; }
        public List<SelectListItem> EmployeeList { get; set; }
        public List<SelectListItem> EmployeePaymentTypeList { get; set; }
        [ Display(Name = "Remarks")]
        public string Remarks { get; set; }
        public string OTP { get; set; }
        public string EmployeeCode { get; set; }
        [Display(Name = "Pending Salary")]
        public decimal PendingSalary { get; set; }
    }

    public class PaymentFilterVM
    {
        public PaymentFilterVM()
        {
            StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UserRole { get; set; }
        public List<PaymentVM> PaymentList { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
    }
}