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
    public class WorkerTypeController : Controller
    {
        // GET: Admin/WorkerType
        AttendanceSystemEntities _db;
        public string WorkerTypeDirectoryPath = "";
        long companyId;
        public WorkerTypeController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
        }
        public ActionResult Index()
        {
            List<WorkerTypeVM> WorkerType = new List<WorkerTypeVM>();
            try
            {

                WorkerType = (from st in _db.tbl_WorkerType
                              where !st.IsDeleted && st.CompanyId == companyId
                              select new WorkerTypeVM
                              {
                                  WorkerTypeId = st.WorkerTypeId,
                                  WorkerTypeName = st.WorkerTypeName,
                                  Description = st.Description,
                                  IsActive = st.IsActive
                              }).OrderByDescending(x => x.WorkerTypeId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(WorkerType);
        }

        public ActionResult Add(long id)
        {
            WorkerTypeVM WorkerTypeVM = new WorkerTypeVM();
            if (id > 0)
            {
                WorkerTypeVM = (from wt in _db.tbl_WorkerType
                                where wt.WorkerTypeId == id && !wt.IsDeleted
                                select new WorkerTypeVM
                                {
                                    WorkerTypeId = wt.WorkerTypeId,
                                    WorkerTypeName = wt.WorkerTypeName,
                                    Description = wt.Description,
                                    IsActive = wt.IsActive
                                }).FirstOrDefault();
            }

            return View(WorkerTypeVM);
        }

        [HttpPost]
        public ActionResult Add(WorkerTypeVM workerTypeVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;

                    if (workerTypeVM.WorkerTypeId > 0)
                    {
                        tbl_WorkerType objWorkerType = _db.tbl_WorkerType.Where(x => x.WorkerTypeId == workerTypeVM.WorkerTypeId).FirstOrDefault();
                        objWorkerType.WorkerTypeName = workerTypeVM.WorkerTypeName;
                        objWorkerType.Description = workerTypeVM.Description;
                        objWorkerType.ModifiedBy = LoggedInUserId;
                        objWorkerType.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_WorkerType objWorkerType = new tbl_WorkerType();
                        objWorkerType.CompanyId = companyId;
                        objWorkerType.WorkerTypeName = workerTypeVM.WorkerTypeName;
                        objWorkerType.Description = workerTypeVM.Description;
                        objWorkerType.IsActive = true;
                        objWorkerType.CreatedBy = LoggedInUserId;
                        objWorkerType.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerType.ModifiedBy = LoggedInUserId;
                        objWorkerType.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_WorkerType.Add(objWorkerType);
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

            return View(workerTypeVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_WorkerType objWorkerType = _db.tbl_WorkerType.Where(x => x.WorkerTypeId == Id).FirstOrDefault();

                if (objWorkerType != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objWorkerType.IsActive = true;
                    }
                    else
                    {
                        objWorkerType.IsActive = false;
                    }

                    objWorkerType.ModifiedBy = LoggedInUserId;
                    objWorkerType.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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
        public string DeleteWorkerType(int workerTypeId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_WorkerType objWorkerType = _db.tbl_WorkerType.Where(x => x.WorkerTypeId == workerTypeId).FirstOrDefault();

                if (objWorkerType == null)
                {
                    ReturnMessage = ErrorMessage.NotFound;
                }
                else
                {
                    if (_db.tbl_Employee.Any(x => x.WorkerTypeId == workerTypeId))
                    {
                        ReturnMessage = ErrorMessage.WorkerExistForThisWorkerType;
                    }
                    else
                    {
                        _db.tbl_WorkerType.Remove(objWorkerType);
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

        public JsonResult CheckWorkerTypeName(string workerTypeName, long workerTypeId)
        {
            bool isExist = false;
            try
            {

                isExist = _db.tbl_WorkerType.Any(x => !x.IsDeleted && x.CompanyId == companyId && x.WorkerTypeId != workerTypeId && x.WorkerTypeName == workerTypeName);
            }
            catch (Exception ex)
            {
                isExist = false;
            }

            return Json(new { Status = isExist }, JsonRequestBehavior.AllowGet);
        }
    }
}