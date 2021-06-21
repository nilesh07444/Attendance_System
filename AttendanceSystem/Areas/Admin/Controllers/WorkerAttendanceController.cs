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
    public class WorkerAttendanceController : Controller
    {
        // GET: Admin/WorkerAttendance
        long companyId;
        AttendanceSystemEntities _db;
        public WorkerAttendanceController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
        }
        public ActionResult Index(DateTime? attendanceDate, int? siteId, long? employeeId)
        {
            WorkerAttendanceFilterVM workerAttendanceFilterVM = new WorkerAttendanceFilterVM();
            try
            {
                companyId = clsAdminSession.CompanyId;
                if (attendanceDate.HasValue)
                {
                    workerAttendanceFilterVM.AttendanceDate = attendanceDate.Value;
                }
                if (siteId.HasValue)
                {
                    workerAttendanceFilterVM.SiteId = siteId.Value;
                }

                if (employeeId.HasValue)
                {
                    workerAttendanceFilterVM.EmployeeId = employeeId.Value;
                }

                workerAttendanceFilterVM.AttendanceList = (from at in _db.tbl_WorkerAttendance
                                                           join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                                           where emp.CompanyId == companyId
                                                           && at.AttendanceDate == workerAttendanceFilterVM.AttendanceDate
                                                           && (at.MorningSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.AfternoonSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.EveningSiteId == workerAttendanceFilterVM.SiteId)
                                                           && (workerAttendanceFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceFilterVM.EmployeeId.Value : true)
                                                           select new WorkerAttendanceVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               EmployeeCode = emp.EmployeeCode,
                                                               Name = emp.FirstName + " " + emp.LastName,
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = (at.IsMorning && at.MorningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsAfternoon = (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsEvening = (at.IsEvening && at.EveningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               ProfilePicture = emp.ProfilePicture
                                                           }).OrderByDescending(x => x.AttendanceDate).ToList();

                workerAttendanceFilterVM.AttendanceList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                    x.ProfilePicture = (!string.IsNullOrEmpty(x.ProfilePicture) ? CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty);
                });
                workerAttendanceFilterVM.EmployeeList = GetWorkerList();
                workerAttendanceFilterVM.SiteList = GetSiteList();
            }
            catch (Exception ex)
            {
            }
            return View(workerAttendanceFilterVM);
        }

        private List<SelectListItem> GetSiteList()
        {
            companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from st in _db.tbl_Site
                                        where !st.IsDeleted && st.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = st.SiteName,
                                            Value = st.SiteId.ToString()
                                        }).ToList();
            return lst;
        }

        [HttpPost]
        public JsonResult SearchWorker(string searchText)
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId

                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();


            return Json(lst.Select(c => new { Name = c.Text, ID = c.Value }).ToList(), JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetWorkerList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        && emp.AdminRoleId == (int)AdminRoles.Worker
                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

    }
}