using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class PaymentVM
    {
        public long EmployeePaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public long UserId { get; set; }
        public decimal Amount { get; set; }
        public int PaymentType { get; set; }
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
    }
}