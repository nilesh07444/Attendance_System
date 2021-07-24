using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class EmployeeFirgerprintVM
    {
        public long EmployeeId { get; set; }
        public string ISOCode { get; set; }
        public string BitmapCode { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}