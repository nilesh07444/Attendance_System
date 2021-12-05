using System;
using System.Collections.Generic;

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class PaymentVM
    {
        public long EmployeePaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public long UserId { get; set; }
        public decimal? DebitAmount { get; set; }
        public int? PaymentType { get; set; }
        public string PaymentTypeText { get; set; }
        public string Remarks { get; set; }
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
        public long? EmployeeId { get; set; }
    }

    public class WorkerPaymentFilterVM
    {
        public WorkerPaymentFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<long> EmployeeIds { get; set; }
    }

    public class PaymentReportFilterVM
    {
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public long FinancialYearId { get; set; }
    }

    public class WorkerPaymentReportFilterVM
    {
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int Year { get; set; }
        public long EmployeeId { get; set; }
        public long FinancialYearId { get; set; }
    }

    public class PaymentReportVM
    {
        public DateTime Date { get; set; }
        public decimal Opening { get; set; }
        public decimal Credit { get; set; }
        public decimal Debit { get; set; }
        public decimal Closing { get; set; }
        public string Remark { get; set; }
    }

   
    
}