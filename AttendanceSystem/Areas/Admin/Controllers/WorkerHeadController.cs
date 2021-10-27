using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class WorkerHeadController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public string SiteDirectoryPath = "";
        public WorkerHeadController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index()
        {
            List<WorkerHeadVM> lstWorkerHead = new List<WorkerHeadVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                lstWorkerHead = (from wh in _db.tbl_WorkerHead
                                 where !wh.IsDeleted && wh.CompanyId == companyId
                                 select new WorkerHeadVM
                                 {
                                     WorkerHeadId = wh.WorkerHeadId,
                                     HeadName = wh.HeadName,
                                     HeadCity = wh.HeadCity,
                                     HeadContactNo = wh.HeadContactNo,
                                     IsActive = wh.IsActive
                                 }).OrderByDescending(x => x.WorkerHeadId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(lstWorkerHead);
        }

        public ActionResult Add(long id)
        {
            WorkerHeadVM WorkerHeadVM = new WorkerHeadVM();
            if (id > 0)
            {
                WorkerHeadVM = (from wh in _db.tbl_WorkerHead
                                where wh.WorkerHeadId == id
                                select new WorkerHeadVM
                                {
                                    WorkerHeadId = wh.WorkerHeadId,
                                    HeadName = wh.HeadName,
                                    HeadCity = wh.HeadCity,
                                    HeadContactNo = wh.HeadContactNo,
                                    IsActive = wh.IsActive
                                }).FirstOrDefault();
            }

            return View(WorkerHeadVM);
        }

        [HttpPost]
        public ActionResult Add(WorkerHeadVM WorkerHeadVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;

                    if (WorkerHeadVM.WorkerHeadId > 0)
                    {
                        tbl_WorkerHead objWorkerHead = _db.tbl_WorkerHead.Where(x => x.WorkerHeadId == WorkerHeadVM.WorkerHeadId).FirstOrDefault();
                        objWorkerHead.HeadName = WorkerHeadVM.HeadName;
                        objWorkerHead.HeadContactNo = WorkerHeadVM.HeadContactNo;
                        objWorkerHead.HeadCity = WorkerHeadVM.HeadCity;
                        objWorkerHead.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objWorkerHead.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_WorkerHead objWorkerHead = new tbl_WorkerHead();
                        objWorkerHead.CompanyId = companyId;
                        objWorkerHead.HeadName = WorkerHeadVM.HeadName;
                        objWorkerHead.HeadContactNo = WorkerHeadVM.HeadContactNo;
                        objWorkerHead.HeadCity = WorkerHeadVM.HeadCity;
                        objWorkerHead.IsActive = true;
                        objWorkerHead.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objWorkerHead.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerHead.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objWorkerHead.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_WorkerHead.Add(objWorkerHead);
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

            return View(WorkerHeadVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_WorkerHead objWorkerHead = _db.tbl_WorkerHead.Where(x => x.WorkerHeadId == Id).FirstOrDefault();

                if (objWorkerHead != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objWorkerHead.IsActive = true;
                    }
                    else
                    {
                        objWorkerHead.IsActive = false;
                    }

                    objWorkerHead.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objWorkerHead.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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
        public string DeleteWorkerHead(int Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_WorkerHead objWorkerHead = _db.tbl_WorkerHead.Where(x => x.WorkerHeadId == Id).FirstOrDefault();

                if (objWorkerHead == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objWorkerHead.IsDeleted = true;
                    objWorkerHead.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objWorkerHead.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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


    }
}