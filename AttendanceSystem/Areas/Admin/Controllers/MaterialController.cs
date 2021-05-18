using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class MaterialController : Controller
    {
        // GET: Admin/Material
        AttendanceSystemEntities _db;
        public string MaterialDirectoryPath = "";
        public MaterialController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<MaterialVM> material = new List<MaterialVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;

                List<SelectListItem> materialStatusList = GetMaterialStatusList();

                material = (from mt in _db.tbl_Material
                            join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                            join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                            where !mt.IsDeleted && mt.CompanyId == companyId
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

                material.ForEach(x => {
                    x.InOutText = materialStatusList.Where(z => z.Value == x.InOut.ToString()).Select(c => c.Text).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
            }
            return View(material);
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
                        objMaterial.ModifiedBy = LoggedInUserId;
                        objMaterial.ModifiedDate = DateTime.UtcNow;
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
                        objMaterial.CreatedBy = LoggedInUserId;
                        objMaterial.CreatedDate = DateTime.UtcNow;
                        objMaterial.ModifiedBy = LoggedInUserId;
                        objMaterial.ModifiedDate = DateTime.UtcNow;
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
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objMaterial.IsActive = true;
                    }
                    else
                    {
                        objMaterial.IsActive = false;
                    }

                    objMaterial.ModifiedBy = LoggedInUserId;
                    objMaterial.ModifiedDate = DateTime.UtcNow;

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
                    _db.tbl_Material.Remove(objMaterial);
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
            string[] materialStatusArr = Enum.GetNames(typeof(MateriaStatus));
            var listMaterialStatus = materialStatusArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listMaterialStatus
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;

        }
    }
}