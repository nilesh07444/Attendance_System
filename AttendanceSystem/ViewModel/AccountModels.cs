﻿using System.ComponentModel.DataAnnotations;

namespace AttendanceSystem.ViewModel
{
    public class AccountModels
    {
        public class LoginVM
        {
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
            public string OTP { get; set; }
            public string TextOTP { get; set; } 
        }

        public class ChangePasswordVM
        {
            [Required]
            [Display(Name = "User name")]
            public string UserName { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Current Password")]
            public string CurrentPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }

        }
    }
}