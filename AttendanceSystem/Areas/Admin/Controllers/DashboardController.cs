using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Linq;
using System.Web.Mvc;
using static AttendanceSystem.ViewModel.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class DashboardController : Controller
    {
        AttendanceSystemEntities _db;
        public DashboardController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            DashboardVM dashboardVM = new DashboardVM();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                int roleId = clsAdminSession.RoleID;
               

                dashboardVM.PendingLeaves = (from lv in _db.tbl_Leave
                                             join ur in _db.tbl_Employee on lv.UserId equals ur.EmployeeId
                                             where !lv.IsDeleted
                                             && ur.CompanyId == companyId
                                             && lv.LeaveStatus == (int)LeaveStatus.Pending
                                             select lv.LeaveId
                                            ).Count();

                dashboardVM.PendingAttendance = (from at in _db.tbl_Attendance
                                                 join ur in _db.tbl_Employee on at.UserId equals ur.EmployeeId
                                                 where !at.IsDeleted
                                                 && ur.CompanyId == companyId
                                                 && at.Status == (int)AttendanceStatus.Pending
                                                 select at.AttendanceId
                                           ).Count();

                dashboardVM.AccountExpiryDate = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId).Select(z => z.EndDate).FirstOrDefault();

                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                dashboardVM.ThisMonthHoliday = _db.tbl_Holiday.Where(x => x.CompanyId == companyId.ToString() && x.IsActive && !x.IsDeleted && x.StartDate >= startDate && x.StartDate <= endDate).Count();

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                if (objCompany != null)
                {
                    if (objCompany.CompanyTypeId == (int)CompanyType.Banking_OfficeCompany)
                        dashboardVM.IsOfficeCompany = true;
                    else
                        dashboardVM.IsOfficeCompany = false;
                }

                dashboardVM.Employee = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Employee).Count();
                if (!dashboardVM.IsOfficeCompany)
                {
                    dashboardVM.Supervisor = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Supervisor).Count();
                    dashboardVM.Checker = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Checker).Count();
                    dashboardVM.Payer = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Payer).Count();
                    dashboardVM.Worker = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Worker).Count();
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                throw ex;
            }
            return View(dashboardVM);
        }


    }
}