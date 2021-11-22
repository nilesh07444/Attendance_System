using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem
{
    public class WorkerHeadVM
    {
        public long WorkerHeadId { get; set; }
        public long CompanyId { get; set; }
        [Display(Name = "Head Name *")]
        [Required(ErrorMessage = "This field is required")]
        public string HeadName { get; set; }
        [Display(Name = "Head Contact No")]
        public string HeadContactNo { get; set; }
        [Display(Name = "Head City")]
        public string HeadCity { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class WorkerHeadReportVM
    {
        public long WorkerHeadId { get; set; }
        public string HeadName { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal ActSalary { get; set; }
        public decimal TodaySalary { get; set; }
        public decimal AmountGiven { get; set; }

        //
        public int RowNumber { get; set; }
    }

    public class WorkerHeadFilterVM
    {
        public WorkerHeadFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long? WorkerHeadId { get; set; }
        public List<SelectListItem> WorkerHeadList { get; set; }

        public List<WorkerHeadReportVM> WorkerHeadReportList { get; set; }
    }


}