using System;
using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.ViewModel
{
    public class EmployeeBuyTransactionVM
    {
        public long EmployeeBuyTransactionId { get; set; }
        public long CompanyId { get; set; }
        [Required, Display(Name = "No Of Employee")]
        public int NoOfEmpToBuy { get; set; }
        [Required, Display(Name = "Amount Per Employee")]
        public decimal AmountPerEmp { get; set; }
        [Required, Display(Name = "Total Amount")]
        public decimal TotalPaidAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
        [Required, Display(Name = "Days")]
        public int RemainingDays { get; set; }
        public string ErrorMessage { get; set; }
    }

}