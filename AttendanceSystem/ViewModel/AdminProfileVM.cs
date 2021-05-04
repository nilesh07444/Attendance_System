using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class AdminProfileVM
    {
        public long AdminUserId { get; set; }
        public int AdminUserRoleId { get; set; }
        public long? CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailId { get; set; }
        public string MobileNo { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public long? ModifiedBy { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Prefix { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string AlternateMobileNo { get; set; }
        public string AadharCardNo { get; set; }
        public string PanCardNo { get; set; }
        public string PanCardPhoto { get; set; }
        public string UserPhoto { get; set; }
        public string AadharCardPhoto { get; set; }
    }
}