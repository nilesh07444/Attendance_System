using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel.WebAPI.ViewModel
{
    public class LoginResponseVM
    {
        //public int Status { get; set; }
        //public string ErrorMessage { get; set; }
        public bool IsFingerprintEnabled { get; set; }
        public long EmployeeId { get; set; }
        public string OTP { get; set; }
    }
    public class AuthenticateVM
    {
        public string Access_token { get; set; }
        public long EmployeeId { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public long CompanyTypeId { get; set; }
        public string CompanyTypeText { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string EmployeeCode { get; set; }
        public string Password { get; set; }
        public string MobileNo { get; set; }
        public string AlternateMobile { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Designation { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? DateOfJoin { get; set; }
        public string BloodGroup { get; set; }
        public string WorkingTime { get; set; }
        public string AdharCardNo { get; set; }
        public DateTime? DateOfIdCardExpiry { get; set; }
        public string Remarks { get; set; }
        public string ProfilePicture { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public bool IsFingerprintEnabled { get; set; }
        public bool IsLeaveForward { get; set; }
        public string State { get; set; }
        public string Pincode { get; set; }
        public bool IsTrialMode { get; set; }
        public decimal? MonthlySalaryPrice { get; set; }
        public decimal? ExtraPerHourPrice { get; set; }
        public decimal NoOfFreeLeavePerMonth { get; set; }
        public decimal? PerCategoryPrice { get; set; }
    }
}