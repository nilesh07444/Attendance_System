using System;
using System.Collections.Generic; 

namespace AttendanceSystem
{
    public class ReminderVM
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public long CompanyId { get; set; }
        public string CompanyName { get; set; }
        public DateTime? ReminderDate { get; set; }
        public string EmployeeRole { get; set; }
        public string EmployeeCode { get; set; }
        public int EmploymentCategoryId { get; set; }
        public string EmploymentCategoryName { get; set; }
    }

    public class ReminderFilterVM
    {
        public ReminderFilterVM()
        {
            StartDate = CommonMethod.CurrentIndianDateTime();
            EndDate = StartDate;
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<ReminderVM> ReminderList { get; set; }        

    }

}