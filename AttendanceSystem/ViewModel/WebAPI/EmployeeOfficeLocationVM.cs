using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class EmployeeOfficeLocationVM
    {
        public long EmployeeOfficeLocationId { get; set; }
        public long OfficeLocationId { get; set; }
        public long EmployeeId { get; set; }

        // Additional
        public string EmployeeName { get; set; }
        public string OfficeLocationName { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public decimal? RadiousInMeter { get; set; }

        public bool IsAssigned { get; set; }

    }
}