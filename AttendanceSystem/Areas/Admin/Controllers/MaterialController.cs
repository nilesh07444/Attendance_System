using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class MaterialController : Controller
    {

        AttendanceSystemEntities _db;
        public string MaterialDirectoryPath = "";
        long loggedInUserId;

        public MaterialController()
        {
            _db = new AttendanceSystemEntities();
            loggedInUserId = clsAdminSession.UserID;
        }

        public ActionResult Index(long? SiteId, long? MaterialCategoryId, int? Status, DateTime? StartDate, DateTime? EndDate)
        {

            MaterialFilterVM materialFilterVM = new MaterialFilterVM();

            try
            {

                if (SiteId.HasValue)
                {
                    materialFilterVM.SiteId = SiteId.Value;
                }

                if (MaterialCategoryId.HasValue)
                {
                    materialFilterVM.MaterialCategoryId = MaterialCategoryId.Value;
                }

                if (Status.HasValue)
                {
                    materialFilterVM.Status = Status.Value;
                }

                if (StartDate.HasValue && EndDate.HasValue)
                {
                    materialFilterVM.StartDate = StartDate.Value;
                    materialFilterVM.EndDate = EndDate.Value;
                }

                long companyId = clsAdminSession.CompanyId;

                List<SelectListItem> materialStatusList = GetMaterialStatusList();

                materialFilterVM.MaterialList = (from mt in _db.tbl_Material
                                                 join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                                 join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                                 where !mt.IsDeleted && mt.CompanyId == companyId

                                                 && (materialFilterVM.StartDate.HasValue ? (DbFunctions.TruncateTime(mt.MaterialDate) >= DbFunctions.TruncateTime(materialFilterVM.StartDate)) : true)
                                                 && (materialFilterVM.EndDate.HasValue ? (DbFunctions.TruncateTime(mt.MaterialDate) <= DbFunctions.TruncateTime(materialFilterVM.EndDate)) : true)

                                                 && (materialFilterVM.SiteId.HasValue ? mt.SiteId == materialFilterVM.SiteId.Value : true)
                                                 && (materialFilterVM.MaterialCategoryId.HasValue ? mt.MaterialCategoryId == materialFilterVM.MaterialCategoryId.Value : true)
                                                 && (materialFilterVM.Status.HasValue ? mt.InOut == materialFilterVM.Status.Value : true)

                                                 select new MaterialVM
                                                 {
                                                     MaterialId = mt.MaterialId,
                                                     MaterialCategoryId = mt.MaterialCategoryId.Value,
                                                     MaterialCategoryText = mc.MaterialCategoryName,
                                                     MaterialDate = mt.MaterialDate,
                                                     SiteId = mt.SiteId,
                                                     SiteName = st.SiteName,
                                                     Qty = mt.Qty,
                                                     InOut = mt.InOut,
                                                     Remarks = mt.Remarks,
                                                     IsActive = mt.IsActive,
                                                     InwardOutwardBy = PaymentGivenBy.CompanyAdmin.ToString()
                                                 }).OrderByDescending(x => x.MaterialId).ToList();

                materialFilterVM.MaterialList.ForEach(x =>
                {
                    x.InOutText = materialStatusList.Where(z => z.Value == x.InOut.ToString()).Select(c => c.Text).FirstOrDefault();
                });

                materialFilterVM.MaterialCategoryList = GetMaterialCategoryList();
                materialFilterVM.MaterialStatusList = GetMaterialStatusList();
                materialFilterVM.SiteList = GetSiteList();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(materialFilterVM);
        }

        public ActionResult Add(long id)
        {
            MaterialVM materialVM = new MaterialVM();
            if (id > 0)
            {
                materialVM = (from mt in _db.tbl_Material
                              where mt.MaterialId == id && !mt.IsDeleted
                              select new MaterialVM
                              {
                                  MaterialId = mt.MaterialId,
                                  MaterialCategoryId = mt.MaterialCategoryId.Value,
                                  MaterialDate = mt.MaterialDate,
                                  SiteId = mt.SiteId,
                                  Qty = mt.Qty,
                                  InOut = mt.InOut,
                                  Remarks = mt.Remarks,
                              }).FirstOrDefault();
            }

            materialVM.SiteList = GetSiteList();
            materialVM.MaterialCategoryList = GetMaterialCategoryList();
            materialVM.MaterialStatusList = GetMaterialStatusList();
            return View(materialVM);
        }

        [HttpPost]
        public ActionResult Add(MaterialVM materialVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;

                    if (materialVM.MaterialId > 0)
                    {
                        tbl_Material objMaterial = _db.tbl_Material.Where(x => x.MaterialId == materialVM.MaterialId).FirstOrDefault();
                        objMaterial.MaterialCategoryId = materialVM.MaterialCategoryId;
                        objMaterial.MaterialDate = materialVM.MaterialDate;
                        objMaterial.SiteId = materialVM.SiteId;
                        objMaterial.Qty = materialVM.Qty;
                        objMaterial.InOut = materialVM.InOut;
                        objMaterial.Remarks = materialVM.Remarks;
                        objMaterial.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objMaterial.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_Material objMaterial = new tbl_Material();
                        objMaterial.CompanyId = companyId;
                        objMaterial.MaterialCategoryId = materialVM.MaterialCategoryId;
                        objMaterial.MaterialDate = materialVM.MaterialDate;
                        objMaterial.SiteId = materialVM.SiteId;
                        objMaterial.Qty = materialVM.Qty;
                        objMaterial.InOut = materialVM.InOut;
                        objMaterial.Remarks = materialVM.Remarks;
                        objMaterial.IsActive = true;
                        objMaterial.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objMaterial.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objMaterial.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objMaterial.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        objMaterial.FinancialYearId = CommonMethod.GetFinancialYearId();
                        objMaterial.IsYearlyConversionEntry = false;
                        _db.tbl_Material.Add(objMaterial);
                    }
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            materialVM.SiteList = GetSiteList();
            materialVM.MaterialCategoryList = GetMaterialCategoryList();
            materialVM.MaterialStatusList = GetMaterialStatusList();
            return View(materialVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Material objMaterial = _db.tbl_Material.Where(x => x.MaterialId == Id).FirstOrDefault();

                if (objMaterial != null)
                {

                    if (Status == "Active")
                    {
                        objMaterial.IsActive = true;
                    }
                    else
                    {
                        objMaterial.IsActive = false;
                    }

                    objMaterial.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objMaterial.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                    _db.SaveChanges();
                    ReturnMessage = ErrorMessage.Success;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = ErrorMessage.Exception;
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeleteMaterial(int materialId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Material objMaterial = _db.tbl_Material.Where(x => x.MaterialId == materialId).FirstOrDefault();

                if (objMaterial == null)
                {
                    ReturnMessage = ErrorMessage.NotFound;
                }
                else
                {
                    objMaterial.IsDeleted = true;
                    objMaterial.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objMaterial.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.SaveChanges();

                    ReturnMessage = ErrorMessage.Success;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = ErrorMessage.Exception;
            }

            return ReturnMessage;
        }

        private List<SelectListItem> GetMaterialCategoryList()
        {
            List<SelectListItem> lst = (from mc in _db.tbl_MaterialCategory
                                        where mc.CompanyId == clsAdminSession.CompanyId
                                        && mc.IsActive && !mc.IsDeleted
                                        select new SelectListItem
                                        {
                                            Text = mc.MaterialCategoryName,
                                            Value = mc.MaterialCategoryId.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetSiteList()
        {
            List<SelectListItem> lst = (from st in _db.tbl_Site
                                        where st.CompanyId == clsAdminSession.CompanyId
                                        && st.IsActive && !st.IsDeleted
                                        select new SelectListItem
                                        {
                                            Text = st.SiteName,
                                            Value = st.SiteId.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetMaterialStatusList()
        {
            string[] materialStatusArr = Enum.GetNames(typeof(MaterialStatus));
            var listMaterialStatus = materialStatusArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listMaterialStatus
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;

        }

        public ActionResult DeletedMaterial()
        {
            List<MaterialVM> material = new List<MaterialVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;

                List<SelectListItem> materialStatusList = GetMaterialStatusList();

                material = (from mt in _db.tbl_Material
                            join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                            join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                            where mt.IsDeleted && mt.CompanyId == companyId
                            select new MaterialVM
                            {
                                MaterialId = mt.MaterialId,
                                MaterialCategoryId = mt.MaterialCategoryId.Value,
                                MaterialCategoryText = mc.MaterialCategoryName,
                                MaterialDate = mt.MaterialDate,
                                SiteId = mt.SiteId,
                                SiteName = st.SiteName,
                                Qty = mt.Qty,
                                InOut = mt.InOut,
                                Remarks = mt.Remarks,
                                IsActive = mt.IsActive,
                            }).OrderByDescending(x => x.MaterialId).ToList();

                material.ForEach(x =>
                {
                    x.InOutText = materialStatusList.Where(z => z.Value == x.InOut.ToString()).Select(c => c.Text).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
            }
            return View(material);
        }

        public ActionResult MaterialReport(DateTime? startDate = null, DateTime? endDate = null, long? materialCategoryId = null, long? siteId = null, long? financialYearId = null)
        {
            MaterialInWardOutWardReportFilterVM materialInWardOutWardReportFilterVM = new MaterialInWardOutWardReportFilterVM();
            long companyId = clsAdminSession.CompanyId;

            if (materialCategoryId.HasValue)
            {
                materialInWardOutWardReportFilterVM.MaterialCategoryId = materialCategoryId.Value;
            }

            if (financialYearId == null)
                financialYearId = CommonMethod.GetFinancialYearId();

            if (financialYearId.HasValue)
            {
                materialInWardOutWardReportFilterVM.FinancialYearId = financialYearId.Value;
            }

            if (siteId.HasValue)
            {
                materialInWardOutWardReportFilterVM.SiteId = siteId.Value;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                materialInWardOutWardReportFilterVM.StartDate = startDate.Value;
                materialInWardOutWardReportFilterVM.EndDate = endDate.Value;
            }

            var materialCategoryIdParam = new SqlParameter()
            {
                ParameterName = "MaterialCategory",
                Value = materialInWardOutWardReportFilterVM.MaterialCategoryId
            };

            var siteIdParam = new SqlParameter()
            {
                ParameterName = "SiteId",
                Value = materialInWardOutWardReportFilterVM.SiteId
            };

            var companyIdParam = new SqlParameter()
            {
                ParameterName = "CompanyId",
                Value = companyId
            };

            var startDateParam = new SqlParameter()
            {
                ParameterName = "StartDate",
                Value = materialInWardOutWardReportFilterVM.StartDate
            };


            var endDateParam = new SqlParameter()
            {
                ParameterName = "EndDate",
                Value = materialInWardOutWardReportFilterVM.EndDate
            };

            var financialYearIdParam = new SqlParameter()
            {
                ParameterName = "FinancialYearId",
                Value = materialInWardOutWardReportFilterVM.FinancialYearId
            };

            materialInWardOutWardReportFilterVM.MaterialList = _db.Database.SqlQuery<MaterialInWardOutWardReportVM>("exec Usp_GetDateWiseMaterialReport @StartDate,@EndDate,@MaterialCategory,@SiteId,@CompanyId,@FinancialYearId", startDateParam, endDateParam, materialCategoryIdParam, siteIdParam, companyIdParam, financialYearIdParam).ToList<MaterialInWardOutWardReportVM>();

            materialInWardOutWardReportFilterVM.MaterialCategoryList = GetMaterialCategoryList();
            materialInWardOutWardReportFilterVM.SiteList = GetSiteList();
            materialInWardOutWardReportFilterVM.FinancialYearList = CommonMethod.GetFinancialYearList();

            return View(materialInWardOutWardReportFilterVM);
        }
    }
}