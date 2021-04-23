using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class CompanyRenewPaymentVM
    {
        public long CompanyRegistrationPaymentId { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public decimal Amount { get; set; }
        public string PaymentFor { get; set; }
        public string PaymentGatewayResponseId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AccessDays { get; set; }
        public long PackageId { get; set; }
        public string PackageName { get; set; }

    }
}