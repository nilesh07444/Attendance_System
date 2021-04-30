using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI.ViewModel
{
    public class LoginRequestVM
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }

    public class AuthenticateRequestVM
    {
        public int EmployeeId { get; set; }
        public string LocationFrom { get; set; }
    }
}