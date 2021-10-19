using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class ReminderController : Controller
    {
        private readonly AttendanceSystemEntities _db;

        public ReminderController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Anniversary(DateTime? startDate = null, DateTime? endDate = null)
        {
            ReminderFilterVM reminderFilterVM = new ReminderFilterVM();

            if (startDate.HasValue && endDate.HasValue)
            {
                reminderFilterVM.StartDate = startDate.Value;
                reminderFilterVM.EndDate = endDate.Value;
            }

            try
            {

                long companyId = clsAdminSession.CompanyId;

                reminderFilterVM.ReminderList = (from user in _db.tbl_AdminUser
                                                 join company in _db.tbl_Company on user.CompanyId equals company.CompanyId
                                                 where user.DateOfMarriageAnniversary.HasValue && !user.IsDeleted
                                                 && user.DateOfMarriageAnniversary.Value.Day >= reminderFilterVM.StartDate.Day && user.DateOfMarriageAnniversary.Value.Month >= reminderFilterVM.StartDate.Month
                                                 && user.DateOfMarriageAnniversary.Value.Day <= reminderFilterVM.EndDate.Day && user.DateOfMarriageAnniversary.Value.Month <= reminderFilterVM.EndDate.Month
                                                 select new ReminderVM
                                                 {
                                                     UserId = user.AdminUserId,
                                                     UserName = user.Prefix + " " + user.FirstName + " " + user.LastName,
                                                     ReminderDate = user.DateOfMarriageAnniversary,
                                                     CompanyId = company.CompanyId,
                                                     CompanyName = company.CompanyName
                                                 }).OrderBy(x => x.ReminderDate).ToList();

            }
            catch (Exception ex)
            {
            }

            return View(reminderFilterVM);
        }

        public ActionResult Birthday(DateTime? startDate = null, DateTime? endDate = null)
        {
            ReminderFilterVM reminderFilterVM = new ReminderFilterVM();

            if (startDate.HasValue && endDate.HasValue)
            {
                reminderFilterVM.StartDate = startDate.Value;
                reminderFilterVM.EndDate = endDate.Value;
            }

            try
            {

                long companyId = clsAdminSession.CompanyId;

                reminderFilterVM.ReminderList = (from user in _db.tbl_AdminUser
                                                 join company in _db.tbl_Company on user.CompanyId equals company.CompanyId
                                                 where user.DOB != null && !user.IsDeleted
                                                 && user.DOB.Day >= reminderFilterVM.StartDate.Day && user.DOB.Month >= reminderFilterVM.StartDate.Month
                                                 && user.DOB.Day <= reminderFilterVM.EndDate.Day && user.DOB.Month <= reminderFilterVM.EndDate.Month
                                                 select new ReminderVM
                                                 {
                                                     UserId = user.AdminUserId,
                                                     UserName = user.Prefix + " " + user.FirstName + " " + user.LastName,
                                                     ReminderDate = user.DOB,
                                                     CompanyId = company.CompanyId,
                                                     CompanyName = company.CompanyName
                                                 }).OrderBy(x => x.ReminderDate).ToList();

            }
            catch (Exception ex)
            {
            }

            return View(reminderFilterVM);
        }

        public ActionResult EmployeeBirthday(DateTime? startDate = null, DateTime? endDate = null)
        {
            ReminderFilterVM reminderFilterVM = new ReminderFilterVM();

            if (startDate.HasValue && endDate.HasValue)
            {
                reminderFilterVM.StartDate = startDate.Value;
                reminderFilterVM.EndDate = endDate.Value;
            }

            try
            {

                long companyId = clsAdminSession.CompanyId;

                int startDateDay = reminderFilterVM.StartDate.Day;
                int startDateMonth = reminderFilterVM.StartDate.Month;

                int endDateDay = reminderFilterVM.EndDate.Day;
                int endDateMonth = reminderFilterVM.EndDate.Month;
                
                reminderFilterVM.ReminderList = (from user in _db.tbl_Employee
                                                 join company in _db.tbl_Company on user.CompanyId equals company.CompanyId
                                                 join role in _db.mst_AdminRole on user.AdminRoleId equals role.AdminRoleId
                                                 where user.Dob != null && !user.IsDeleted && user.CompanyId == companyId && user.AdminRoleId != (int)AdminRoles.Worker

                                                 //&& user.Dob.Value.Day >= reminderFilterVM.StartDate.Day && user.Dob.Value.Month >= reminderFilterVM.StartDate.Month
                                                 //&& user.Dob.Value.Day <= reminderFilterVM.EndDate.Day && user.Dob.Value.Month <= reminderFilterVM.EndDate.Month

                                                 && user.Dob.Value.Day >= startDateDay && user.Dob.Value.Month >= startDateMonth
                                                 && user.Dob.Value.Day <= endDateDay && user.Dob.Value.Month <= endDateMonth
                                                 
                                                 && user.Dob.Value.Month >= startDateMonth

                                                 select new ReminderVM
                                                 {
                                                     UserId = user.EmployeeId,
                                                     UserName = user.Prefix + " " + user.FirstName + " " + user.LastName,
                                                     ReminderDate = user.Dob,
                                                     CompanyId = company.CompanyId,
                                                     CompanyName = company.CompanyName,
                                                     EmployeeRole = role.AdminRoleName,
                                                     EmploymentCategoryId = user.EmploymentCategory,
                                                     EmployeeCode = user.EmployeeCode
                                                 }).OrderBy(x => x.ReminderDate).ToList();

                reminderFilterVM.ReminderList.ForEach(x =>
                {
                    x.EmploymentCategoryName = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategoryId);
                });

            }
            catch (Exception ex)
            {
            }

            return View(reminderFilterVM);
        }

        public dynamic WithinTimeRange(DateTime begin, DateTime end)
        {
            var items = from a in _db.tbl_Employee
                        where a.Dob.Value.Month.Equals(begin.Month) && a.Dob.Value.Day >= begin.Day
                        && a.Dob.Value.Month.Equals(end.Month) && a.Dob.Value.Day <= end.Day
                        select a;

            return items.ToList();
        }

    }
}