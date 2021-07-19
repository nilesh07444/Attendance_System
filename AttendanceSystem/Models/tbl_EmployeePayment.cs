//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AttendanceSystem.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class tbl_EmployeePayment
    {
        public long EmployeePaymentId { get; set; }
        public long CompanyId { get; set; }
        public long UserId { get; set; }
        public Nullable<long> AttendanceId { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public System.DateTime PaymentDate { get; set; }
        public Nullable<int> PaymentType { get; set; }
        public string CreditOrDebitText { get; set; }
        public Nullable<decimal> CreditAmount { get; set; }
        public Nullable<decimal> DebitAmount { get; set; }
        public string Remarks { get; set; }
        public string Status { get; set; }
        public string ProcessStatusText { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public long CreatedBy { get; set; }
        public Nullable<System.DateTime> ModifiedDate { get; set; }
        public Nullable<long> ModifiedBy { get; set; }
        public Nullable<long> FinancialYearId { get; set; }
    }
}
