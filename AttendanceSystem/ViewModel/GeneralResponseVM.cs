using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class GeneralResponseVM
    {
        public bool IsError { get; set; } = false;
        public string ErrorMessage = "";
        public string RedirectUrl = "";
    }
}