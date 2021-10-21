using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class DistrictVM
    {
        public long DistrictId { get; set; }
        public string DistrictName { get; set; }
        public long StateId { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}