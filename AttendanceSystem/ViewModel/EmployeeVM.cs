﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class EmployeeVM
    {
        public long EmployeeId { get; set; }
        public long CompanyId { get; set; }
        [Required, Display(Name = "Employee Role")]
        public int AdminRoleId { get; set; }
        public string AdminRoleText { get; set; }
        [Display(Name = "Prefix")]
        public string Prefix { get; set; }
        [Required, Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required, Display(Name = "Last Name")]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string Password { get; set; }
        [Required, Display(Name = "Mobile No")]
        public string MobileNo { get; set; }
        [Display(Name = "Alternate Mobile no")]
        public string AlternateMobile { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "City")]
        public string City { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Required, Display(Name = "Date of Birth")]
        public DateTime? Dob { get; set; }
        [Required, Display(Name = "Date of Join")]
        public DateTime? DateOfJoin { get; set; }
        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }
        [Display(Name = "Working TIme")]
        public string WorkingTime { get; set; }
        [Display(Name = "Aadhar card No")]
        public string AdharCardNo { get; set; }
        [Display(Name = "Date of Id card Expiry")]
        public DateTime? DateOfIdCardExpiry { get; set; }
        [Display(Name = "Remark")]
        public string Remarks { get; set; }
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }
        [Required, Display(Name = "Employment Category")]
        public int EmploymentCategory { get; set; }
        [Required, Display(Name = "Per Category Price")]
        public decimal PerCategoryPrice { get; set; }
        [Required, Display(Name = "Monthly Salary")]
        public decimal? MonthlySalaryPrice { get; set; }
        [Required, Display(Name = "Extra Per Hour Price")]
        public decimal? ExtraPerHourPrice { get; set; }
        [Display(Name = "Is Leave Forward")]
        public bool IsLeaveForward { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        [Display(Name = "Finger Print Enabled")]
        public bool IsFingerprintEnabled { get; set; }
        [Display(Name = "Profile Image")]
        public HttpPostedFileBase ProfileImageFile { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public string OTP { get; set; }
    }

    public class EmployeeFilterVM
    {
        public int? UserRole { get; set; }
        public int? UserStatus { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public List<EmployeeVM> EmployeeList { get; set; }
    }
}