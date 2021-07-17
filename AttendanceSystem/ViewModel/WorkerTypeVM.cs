using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem.ViewModel
{
    public class WorkerTypeVM
    {
        public long WorkerTypeId { get; set; }

        [Display(Name = "Worker Type Name *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string WorkerTypeName { get; set; }

        [Display(Name = "Worker Type Description *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public string Description { get; set; }

        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}