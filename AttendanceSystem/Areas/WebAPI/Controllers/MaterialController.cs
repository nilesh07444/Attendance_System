using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.WebPages.Html;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/Material")]
    public class MaterialController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public MaterialController()
        {
            _db = new AttendanceSystemEntities();
        }


        [HttpGet]
        [Route("List")]
        public ResponseDataModel<List<MaterialVM>> List(MaterialFilterVM materialFilterVM)
        {
            ResponseDataModel<List<MaterialVM>> response = new ResponseDataModel<List<MaterialVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<MaterialVM> materialList = (from mt in _db.tbl_Material
                                                 join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                                 join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                                 where !mt.IsDeleted && mt.CompanyId == companyId
                                                 && (materialFilterVM.SiteId.HasValue ? mt.SiteId == materialFilterVM.SiteId : true)
                                                 && (materialFilterVM.MaterialCategoryId.HasValue ? mt.MaterialCategoryId == materialFilterVM.MaterialCategoryId : true)
                                                 && (materialFilterVM.Status.HasValue ? mt.InOut == materialFilterVM.Status : true)
                                                 && (materialFilterVM.StartDate.HasValue && materialFilterVM.EndDate.HasValue ? mt.MaterialDate >= materialFilterVM.StartDate && mt.MaterialDate <= materialFilterVM.EndDate : true)
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

                List<SelectListItem> materialInOutStatusList = GetMaterialStatusList();
                materialList.ForEach(x =>
                {
                    x.InOutText = materialInOutStatusList.Where(z => z.Value == x.InOut.ToString()).Select(c => c.Text).FirstOrDefault();
                });

                response.Data = materialList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("Add")]
        public ResponseDataModel<bool> Add(MaterialVM materialVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                #region Validation
                if (materialVM.MaterialCategoryId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.LeaveDateRequired);
                }

                if (materialVM.SiteId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteIsNotValid);
                }


                if (materialVM.Qty <= 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MaterialQtyNotValid);
                }

                if (materialVM.InOut <= 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MaterialQtyNotValid);
                }

                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;
                
                #endregion Validation
                if (!response.IsError)
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
                    objMaterial.CreatedBy = employeeId;
                    objMaterial.CreatedDate = DateTime.UtcNow;
                    objMaterial.ModifiedBy = employeeId;
                    objMaterial.ModifiedDate = DateTime.UtcNow;
                    _db.tbl_Material.Add(objMaterial);
                    _db.SaveChanges();
                    response.Data = true;
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("Detail/{id}")]
        public ResponseDataModel<MaterialVM> WorkerDetail(long id)
        {
            ResponseDataModel<MaterialVM> response = new ResponseDataModel<MaterialVM>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                MaterialVM materialVM = (from mt in _db.tbl_Material
                                         join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                         join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                         where !mt.IsDeleted && mt.CompanyId == companyId
                                         && mt.MaterialId==id
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
                                         }).FirstOrDefault();

                response.Data = materialVM;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }
        private List<SelectListItem> GetMaterialStatusList()
        {
            string[] paymentTypeArr = Enum.GetNames(typeof(MateriaStatus));
            var listpaymentType = paymentTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listpaymentType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }
    }
}
