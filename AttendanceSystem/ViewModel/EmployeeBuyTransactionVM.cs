using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class EmployeeBuyTransactionVM
    {
        public long EmployeeBuyTransactionId { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        [Required, Display(Name = "No Of Employee")]
        public int NoOfEmpToBuy { get; set; }
        [Required, Display(Name = "Amount Per Employee")]
        public decimal AmountPerEmp { get; set; }
        [Required, Display(Name = "Total Amount")]
        public decimal TotalPaidAmount { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public string PaymentGatewayTransactionId { get; set; }
        [Required, Display(Name = "Days")]
        public int RemainingDays { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class EmployeeBuyTransactionFilterVM
    {
        public EmployeeBuyTransactionFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? CompanyId { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public List<EmployeeBuyTransactionVM> RenewList { get; set; }
    }
}