using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace AttendanceSystem.ViewModel
{
    public class MaterialVM
    {
        public MaterialVM()
        {
            MaterialDate = CommonMethod.CurrentIndianDateTime().Date;
        }
        public long MaterialId { get; set; }
        public long CompanyId { get; set; }

        [Display(Name = "Material Category *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public long MaterialCategoryId { get; set; }

        public string MaterialCategoryText { get; set; }

        [Display(Name = "Material Date *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public DateTime MaterialDate { get; set; }

        [Display(Name = "Site Location *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public long SiteId { get; set; }
        public string SiteName { get; set; }

        [Display(Name = "Qty *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public decimal Qty { get; set; }

        [Display(Name = "Material Type *")]
        [Required(ErrorMessage = ErrorMessage.ThisFieldRequired)]
        public int InOut { get; set; }
        public string InOutText { get; set; }

        [Display(Name = "Remark")]
        public string Remarks { get; set; }

        public bool IsActive { get; set; }

        public List<SelectListItem> MaterialCategoryList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> MaterialStatusList { get; set; }
        public string InwardOutwardBy { get; set; }
    }

    public class MaterialFilterVM
    {
        public long? SiteId { get; set; }
        public long? MaterialCategoryId { get; set; }
        public int? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public List<MaterialVM> MaterialList { get; set; }
        public List<SelectListItem> MaterialCategoryList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
        public List<SelectListItem> MaterialStatusList { get; set; }

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

    public class MaterialInWardOutWardReportVM
    {
        public long MaterialId { get; set; }
        public long MaterialCategoryId { get; set; }
        public DateTime MaterialDate { get; set; }
        public long SiteId { get; set; }
        public string SiteName { get; set; }
        public decimal Opening { get; set; }
        public decimal InWard { get; set; }
        public decimal OutWard { get; set; }
        public decimal Closing { get; set; }
        public string EmployeeName { get; set; }
    }
    public class MaterialInWardOutWardReportFilterVM
    {
        public MaterialInWardOutWardReportFilterVM()
        {
            StartDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
            EndDate = StartDate.AddMonths(1).AddDays(-1);
        }
        public long SiteId { get; set; }
        public long MaterialCategoryId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<MaterialInWardOutWardReportVM> MaterialList { get; set; }
        public List<SelectListItem> MaterialCategoryList { get; set; }
        public List<SelectListItem> SiteList { get; set; }
    }
}