using System;

namespace AttendanceSystem.ViewModel
{
    public class InvoiceFieldsVM
    {
        public string CompanyName { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public DateTime InvoiceDatetime { get; set; }
        public string InvoiceDate { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string GstNo { get; set; }
        public string PanNo { get; set; }
        public string PackageName { get; set; }
        public string HsnCode { get; set; }
        public decimal Qty { get; set; }
        public decimal Rate { get; set; }
        public decimal GSTRate { get; set; }
        public decimal CGST { get; set; }
        public decimal SGST { get; set; }
        public decimal IGST { get; set; }
        public decimal TotalAmount { get; set; }
    }
}