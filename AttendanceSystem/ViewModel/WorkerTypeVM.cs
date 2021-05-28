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
        [Required, Display(Name = "Worker Type Name")]
        public string WorkerTypeName { get; set; }

        [Required, Display(Name = "Worker Type Description")]
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}