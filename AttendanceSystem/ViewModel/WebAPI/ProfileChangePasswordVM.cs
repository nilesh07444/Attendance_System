﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI
{
    public class ProfileChangePasswordVM
    {
        public string CurrentPassWord { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}