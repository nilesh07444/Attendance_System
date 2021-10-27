﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
}