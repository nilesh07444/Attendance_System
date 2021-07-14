using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class SalesReportVM
    {
        public DateTime BuyDate { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string PackageType { get; set; }
        public string PackageName { get; set; }
        public decimal PackageAmount { get; set; }
        public decimal GST { get; set; }
        public decimal GSTAmount { get; set; }
        public decimal FinalAmount { get; set; }
        public int TotalFreeEmployee { get; set; }
        public int FreeAccessDays { get; set; }
        public long TotalFreeSMS { get; set; }
    }

    public class SalesReportFilterVM
    {
        public SalesReportFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
            ReportType = 0;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? CompanyId { get; set; }
        public long ReportType { get; set; }
        public List<SelectListItem> CompanyList { get; set; }
        public List<SelectListItem> ReportTypeList { get; set; }
        public List<SalesReportVM> SalesReportList { get; set; }
    }
}