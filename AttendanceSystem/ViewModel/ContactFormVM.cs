using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class ContactFormVM
    {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string MobileNo { get; set; }
        public string EmailId { get; set; }
        public string Message { get; set; }
    }
}