using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class EmployeeFingerprintVM
    {
        public long EmployeeFingerprintId { get; set; }
        public long EmployeeId { get; set; }
        public string ISOCode { get; set; }
        public string BitmapCode { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class FingerprintTemplateVM
    {
        public string ISOCode { get; set; }
        public string BitmapCode { get; set; }
    }
    public class FingerprintSaveVM
    {
        public long EmployeeId { get; set; }
        public List<FingerprintTemplateVM> FingerprintTemplateList { get; set; }
    }

    public class EmployeeFingerprintDetailVM
    {
        public long EmployeeFingerprintId { get; set; }
        public long EmployeeId { get; set; }
        public string ISOCode { get; set; }
        public string BitmapCode { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeRole { get; set; }
    }

}