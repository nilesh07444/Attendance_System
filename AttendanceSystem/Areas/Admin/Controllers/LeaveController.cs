using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class LeaveController : Controller
    {
        // GET: Admin/Leave
        AttendanceSystemEntities _db;
        public LeaveController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index(int? userRole = null, int? leaveStatus = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            LeaveFilterVM leaveFIlterVM = new LeaveFilterVM();
            try
            {
                if (userRole.HasValue)
                    leaveFIlterVM.UserRole = userRole.Value;
                if (leaveStatus.HasValue)
                    leaveFIlterVM.LeaveStatus = leaveStatus.Value;

                if (startDate.HasValue && endDate.HasValue)
                {
                    leaveFIlterVM.StartDate = startDate.Value;
                    leaveFIlterVM.EndDate = endDate.Value;
                }

                long companyId = clsAdminSession.CompanyId;

                leaveFIlterVM.LeaveList = (from lv in _db.tbl_Leave
                                           join ur in _db.tbl_AdminUser on lv.UserId equals ur.AdminUserId
                                           join cm in _db.tbl_Company on ur.CompanyId equals cm.CompanyId
                                           where !lv.IsDeleted
                                           && cm.CompanyId == companyId
                                           && lv.StartDate >= leaveFIlterVM.StartDate && lv.StartDate <= leaveFIlterVM.EndDate
                                           && (leaveFIlterVM.LeaveStatus.HasValue ? lv.LeaveStatus == leaveFIlterVM.LeaveStatus.Value : true)
                                           && (leaveFIlterVM.UserRole.HasValue ? ur.AdminUserRoleId == leaveFIlterVM.LeaveStatus.Value : true)

                                           select new LeaveVM
                                           {
                                               LeaveId = lv.LeaveId,
                                               UserId = lv.UserId,
                                               UserName = ur.FirstName,
                                               StartDate = lv.StartDate,
                                               EndDate = lv.EndDate,
                                               NoOfDays = lv.NoOfDays,
                                               LeaveStatus = lv.LeaveStatus,
                                               RejectReason = lv.RejectReason,
                                               CancelledReason = lv.CancelledReason
                                           }).OrderByDescending(x => x.StartDate).ToList();
                leaveFIlterVM.UserRoleList = GetUserRoleList();
            }
            catch (Exception ex)
            {
            }
            return View(leaveFIlterVM);
        }

        private List<SelectListItem> GetUserRoleList()
        {
            long[] adminRole = new long[] { (long)AdminRoles.CompanyAdmin, (long)AdminRoles.SuperAdmin };
            List<SelectListItem> lst = (from ms in _db.mst_AdminRole
                                        where !adminRole.Contains(ms.AdminRoleId)
                                        select new SelectListItem
                                        {
                                            Text = ms.AdminRoleName,
                                            Value = ms.AdminRoleId.ToString()
                                        }).ToList();
            return lst;
        }
    }
}