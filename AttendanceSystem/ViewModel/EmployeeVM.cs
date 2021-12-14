using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class EmployeeVM
    {
        public long EmployeeId { get; set; }

        public long CompanyId { get; set; }

        [Display(Name = "Employee Role *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int AdminRoleId { get; set; }

        [Display(Name = "Prefix *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string Prefix { get; set; }

        [Display(Name = "First Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string FirstName { get; set; }

        [Display(Name = "Last Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string LastName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Employee Code *")]
        public string EmployeeCode { get; set; }

        public string Password { get; set; }

        [Display(Name = "Mobile No *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string MobileNo { get; set; }

        [Display(Name = "Alternate Mobile no")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Alternate Mobile Number.")]
        public string AlternateMobile { get; set; }

        [Display(Name = "Address *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string Address { get; set; }

        [Display(Name = "City *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string City { get; set; }

        [Display(Name = "Pincode *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        [RegularExpression(@"^([0-9]{6})$", ErrorMessage = "Invalid Pincode.")]
        public string Pincode { get; set; }

        //[Display(Name = "State *")]
        //[Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        //public string State { get; set; }

        [Display(Name = "State *")]
        //[Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public long? StateId { get; set; }

        [Display(Name = "District *")]
        //[Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public long? DistrictId { get; set; }

        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Date of Birth *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public DateTime? Dob { get; set; }

        [Display(Name = "Date of Join *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public DateTime? DateOfJoin { get; set; }

        [Display(Name = "Blood Group")]
        public string BloodGroup { get; set; }

        [Display(Name = "Working TIme")]
        public string WorkingTime { get; set; }

        [Display(Name = "Aadhar Card No *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        [RegularExpression(@"^([0-9]{12})$", ErrorMessage = "Invalid Adhar Card Number.")]
        public string AdharCardNo { get; set; }

        [Display(Name = "Date of Id card Expiry")]
        public DateTime? DateOfIdCardExpiry { get; set; }

        [Display(Name = "Remark")]
        public string Remarks { get; set; }

        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        [Display(Name = "Employment Category *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int EmploymentCategory { get; set; }

        [Display(Name = "Per Category Price *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public decimal PerCategoryPrice { get; set; }

        [Display(Name = "Monthly Salary *")]
        public decimal? MonthlySalaryPrice { get; set; }

        [Display(Name = "Extra Per Hour Price *")]
        public decimal? ExtraPerHourPrice { get; set; }

        [Display(Name = "Is Leave Forward *")]
        public bool IsLeaveForward { get; set; } = false;

        [Display(Name = "Finger Print Enabled * ")]
        public bool IsFingerprintEnabled { get; set; } = false;

        [Display(Name = "Profile Image *")]
        public HttpPostedFileBase ProfileImageFile { get; set; }

        [Display(Name = "Free Leave Per Month *")]
        //[MaxLength(10)]
        public decimal NoOfFreeLeavePerMonth { get; set; }
        [Display(Name = "Carry Forward Leave")]
        public decimal CarryForwardLeave { get; set; }

        [Display(Name = "Worker Type *")]
        public long? WorkerTypeId { get; set; }

        [Display(Name = "Worker Head")]
        public long? WorkerHeadId { get; set; }

        [Display(Name = "Office Location Access Type *")]
        public int? EmployeeOfficeLocationType { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        // Additional 
        public string AdminRoleText { get; set; }
        public bool IsFingerprintLimitExceed { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public string OTP { get; set; }
        public string WorkerTypeText { get; set; }
        public List<SelectListItem> WorkerTypeList { get; set; }
        public string EmploymentCategoryText { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int TotalSavedFingerprint { get; set; }
        public List<SelectListItem> WorkerHeadList { get; set; }
        public List<SelectListItem> StateList { get; set; }
        public List<SelectListItem> DistrictList { get; set; }
        public string StateName { get; set; }
        public string DistrictName { get; set; }

        public string strSelectedOfficeLocations { get; set; }
        public string WorkerHeadName { get; set; } 
    }

    public class EmployeeFilterVM
    {
        public int? UserRole { get; set; }
        public int? UserStatus { get; set; }        
        public int NoOfEmployee { get; set; }
        public int NoOfWorker { get; set; }
        public int ActiveEmployee { get; set; }
        public int ActiveWorker { get; set; }
        public int NoOfEmployeeAllowed { get; set; }
        public bool IsNoOfEmployeeExceed { get; set; }
        public long? WorkerHeadId { get; set; }
        public List<SelectListItem> UserRoleList { get; set; }
        public List<EmployeeVM> EmployeeList { get; set; }
        public List<SelectListItem> WorkerHeadList { get; set; }
    }

    public class EmployeePendingSalaryVM
    {
        public long EmployeeId { get; set; }
        public int EmploymentCategory { get; set; }
        public string EmploymentCategoryText { get; set; }
        public decimal PerCategoryPrice { get; set; }
        public decimal? ExtraPerHourPrice { get; set; }
        public decimal? PendingSalary { get; set; }
        public decimal? MonthlySalary { get; set; }
        public decimal? TodaySalary { get; set; }
        public decimal NoOfFreeLeavePerMonth { get; set; }
    }

    public class SiteAssignedWorkerVM
    {
        public long EmployeeId { get; set; }
        public long CompanyId { get; set; }
        public int AdminRoleId { get; set; }
        public string Prefix { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNo { get; set; }
        public DateTime? Dob { get; set; }
        public DateTime? DateOfJoin { get; set; }
        public string BloodGroup { get; set; }
        public int EmploymentCategory { get; set; }
        public decimal? MonthlySalaryPrice { get; set; }
        public string AdharCardNo { get; set; }
        public string EmployeeCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Pincode { get; set; }
        public string State { get; set; }
        public bool IsActive { get; set; }
        public string ProfilePicture { get; set; }
        public bool IsClosed { get; set; }
        public long? AttendanceId { get; set; }
    }
    public class EmployeeOfficeLocationTypeVM
    {
        public long EmployeeId { get; set; }
        public int? EmployeeOfficeLocationType { get; set; }
    }
}