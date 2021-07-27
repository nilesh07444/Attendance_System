using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem
{
    public class FollowupVM
    {
        public long FollowupId { get; set; }
        public long CompanyId { get; set; }
        public int? FollowupStatus { get; set; }
        public DateTime? NextFollowupDate { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public DateTime CreatedDate { get; set; }

        //
        public string CompanyCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CompanyAccountExpiryDate { get; set; }
        public string CompanyName { get; set; }
        public string FollowupStatusText { get; set; }
        public bool IsCompanyInTrialMode { get; set; }
    }

    public class FollowupFilterVM
    {
        public int? PrimaryFollowupStatus { get; set; }
        public int? SecondaryFollowupStatus { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public List<FollowupVM> FollowupList { get; set; }
        public List<SelectListItem> PrimaryStatusList { get; set; }
        public List<SelectListItem> SecondaryStatusList { get; set; }
    }

    public class CreateFollowupVM
    {

    }

}