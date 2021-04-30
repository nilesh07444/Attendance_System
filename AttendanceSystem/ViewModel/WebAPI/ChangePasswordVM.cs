using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class ChangePasswordVM
    {
        public int EmployeeId { get; set; }
        public string PassWord { get; set; }
    }
}