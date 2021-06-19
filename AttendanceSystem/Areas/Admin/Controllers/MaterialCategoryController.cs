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
    public class MaterialCategoryController : Controller
    {
        // GET: Admin/MaterialCategory
        AttendanceSystemEntities _db;
        public string MaterialCategoryDirectoryPath = "";
        public MaterialCategoryController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<MaterialCategoryVM> MaterialCategory = new List<MaterialCategoryVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                MaterialCategory = (from st in _db.tbl_MaterialCategory
                        where !st.IsDeleted && st.CompanyId == companyId
                        select new MaterialCategoryVM
                        {
                            MaterialCategoryId = st.MaterialCategoryId,
                            MaterialCategoryName = st.MaterialCategoryName,
                            Description = st.Description,
                            IsActive = st.IsActive
                        }).OrderByDescending(x => x.MaterialCategoryId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(MaterialCategory);
        }

        public ActionResult Add(long id)
        {
            MaterialCategoryVM MaterialCategoryVM = new MaterialCategoryVM();
            if (id > 0)
            {
                MaterialCategoryVM = (from st in _db.tbl_MaterialCategory
                          where st.MaterialCategoryId == id && !st.IsDeleted
                          select new MaterialCategoryVM
                          {
                              MaterialCategoryId = st.MaterialCategoryId,
                              MaterialCategoryName = st.MaterialCategoryName,
                              Description = st.Description,
                              IsActive = st.IsActive
                          }).FirstOrDefault();
            }

            return View(MaterialCategoryVM);
        }

        [HttpPost]
        public ActionResult Add(MaterialCategoryVM MaterialCategoryVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;

                    if (MaterialCategoryVM.MaterialCategoryId > 0)
                    {
                        tbl_MaterialCategory objMaterialCategory = _db.tbl_MaterialCategory.Where(x => x.MaterialCategoryId == MaterialCategoryVM.MaterialCategoryId).FirstOrDefault();
                        objMaterialCategory.MaterialCategoryName = MaterialCategoryVM.MaterialCategoryName;
                        objMaterialCategory.Description = MaterialCategoryVM.Description;
                        objMaterialCategory.ModifiedBy = LoggedInUserId;
                        objMaterialCategory.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_MaterialCategory objMaterialCategory = new tbl_MaterialCategory();
                        objMaterialCategory.CompanyId = companyId;
                        objMaterialCategory.MaterialCategoryName = MaterialCategoryVM.MaterialCategoryName;
                        objMaterialCategory.Description = MaterialCategoryVM.Description;
                        objMaterialCategory.IsActive = true;
                        objMaterialCategory.CreatedBy = LoggedInUserId;
                        objMaterialCategory.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objMaterialCategory.ModifiedBy = LoggedInUserId;
                        objMaterialCategory.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_MaterialCategory.Add(objMaterialCategory);
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

            return View(MaterialCategoryVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_MaterialCategory objMaterialCategory = _db.tbl_MaterialCategory.Where(x => x.MaterialCategoryId == Id).FirstOrDefault();

                if (objMaterialCategory != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objMaterialCategory.IsActive = true;
                    }
                    else
                    {
                        objMaterialCategory.IsActive = false;
                    }

                    objMaterialCategory.ModifiedBy = LoggedInUserId;
                    objMaterialCategory.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeleteMaterialCategory(int MaterialCategoryId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_MaterialCategory objMaterialCategory = _db.tbl_MaterialCategory.Where(x => x.MaterialCategoryId == MaterialCategoryId).FirstOrDefault();

                if (objMaterialCategory == null)
                {
                    ReturnMessage = ErrorMessage.NotFound;
                }
                else
                {
                    if (_db.tbl_Material.Any(x => x.MaterialCategoryId == MaterialCategoryId))
                    {
                        ReturnMessage = ErrorMessage.MaterialExistForThisCategory;
                    }
                    else
                    {
                        _db.tbl_MaterialCategory.Remove(objMaterialCategory);
                        _db.SaveChanges();

                        ReturnMessage = ErrorMessage.Success;
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = ErrorMessage.Exception;
            }

            return ReturnMessage;
        }

        public JsonResult CheckMaterialCategoryName(string MaterialCategoryName)
        {
            bool isExist = false;
            try
            {

                isExist = _db.tbl_MaterialCategory.Any(x => !x.IsDeleted && x.CompanyId == clsAdminSession.CompanyId && x.MaterialCategoryName == MaterialCategoryName);
            }
            catch (Exception ex)
            {
                isExist = false;
            }

            return Json(new { Status = isExist }, JsonRequestBehavior.AllowGet);
        }
    }
}