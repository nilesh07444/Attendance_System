using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI.ViewModel
{
    public class LoginResponseVM
    {
        public int Status { get; set; }
        public string ErrorMessage { get; set; }
        public string access_token { get; set; }
    }
}