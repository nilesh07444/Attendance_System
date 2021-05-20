using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AttendanceSystem
{
    public class OurClientVM
    {
        public long SponsorId { get; set; }
        [Display(Name = "Client Name")]
        public string SponsorName { get; set; }
        [Display(Name = "Client Logo Image *")]
        public HttpPostedFileBase SponsorImageFile { get; set; }
        [Display(Name = "Client Website Link")]
        public string SponsorLink { get; set; }
        public bool IsActive { get; set; }

        //
        public string SponsorImage { get; set; }
    }
}