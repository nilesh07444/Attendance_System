using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class ResetPasswordVM
    {
        public int EmployeeId { get; set; }
        public string CurrentPassWord { get; set; }
        public string NewPassWord { get; set; }
    }
}