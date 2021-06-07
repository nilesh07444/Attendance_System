using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class MaterialVM
    {
        public MaterialVM()
        {
            MaterialDate = DateTime.Now.Date;
        }
        public long MaterialId { get; set; }
        public long CompanyId { get; set; }
        [Required, Display(Name = "Material Category")]
        public long MaterialCategoryId { get; set; }
        public string MaterialCategoryText { get; set; }
        [Required, Display(Name = "Material Date")]
        public DateTime MaterialDate { get; set; }
        [Required, Display(Name = "Site")]
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        [Required, Display(Name = "Qty")]
        public decimal Qty { get; set; }
        [Required, Display(Name = "In/Out")]
        public int InOut { get; set; }
        public string InOutText { get; set; }
        [Required, Display(Name = "Remark")]
        public string Remarks { get; set; }
        public bool IsActive { get; set; }

        public List<SelectListItem> MaterialCategoryList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> MaterialStatusList { get; set; }
    }

    public class MaterialFilterVM
    {
        public long? SiteId { get; set; }
        public long? MaterialCategoryId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class MaterialReportVM
    {
        public long MaterialId { get; set; }
        public long CompanyId { get; set; }
        public long MaterialCategoryId { get; set; }
        public string MaterialCategoryText { get; set; }
        public DateTime MaterialDate { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public decimal InWard { get; set; }
        public decimal OutWard { get; set; }
        public decimal Qty { get; set; }
        public int InOut { get; set; }
        public string Remarks { get; set; }
    }
}