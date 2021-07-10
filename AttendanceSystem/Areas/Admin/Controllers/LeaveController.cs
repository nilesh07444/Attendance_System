using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class LeaveController : Controller
    {
        // GET: Admin/Leave
        AttendanceSystemEntities _db;
        long companyId;
        long LoggedInUserId;
        public LeaveController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
            LoggedInUserId = clsAdminSession.UserID;
        }
        public ActionResult Index(int? userRole = null, int? leaveStatus = null, int? startMonth = null, int? endMonth = null, int? year = null)
        {
            LeaveFilterVM leaveFIlterVM = new LeaveFilterVM();
            try
            {
                if (userRole.HasValue)
                    leaveFIlterVM.UserRole = userRole.Value;
                if (leaveStatus.HasValue)
                    leaveFIlterVM.LeaveStatus = leaveStatus.Value;

                if (startMonth.HasValue && endMonth.HasValue)
                {
                    leaveFIlterVM.StartMonth = startMonth.Value;
                    leaveFIlterVM.EndMonth = endMonth.Value;
                }

                if (year.HasValue)
                {
                    leaveFIlterVM.Year = year.Value;
                }



                leaveFIlterVM.LeaveList = (from lv in _db.tbl_Leave
                                           join ur in _db.tbl_Employee on lv.UserId equals ur.EmployeeId
                                           join cm in _db.tbl_Company on ur.CompanyId equals cm.CompanyId
                                           where !lv.IsDeleted
                                           && cm.CompanyId == companyId
                                           && (leaveFIlterVM.StartMonth > 0 && leaveFIlterVM.EndMonth > 0 ? lv.StartDate.Month >= leaveFIlterVM.StartMonth && lv.StartDate.Month <= leaveFIlterVM.EndMonth : true)
                                           && lv.StartDate.Year == leaveFIlterVM.Year
                                           && (leaveFIlterVM.LeaveStatus.HasValue ? lv.LeaveStatus == leaveFIlterVM.LeaveStatus.Value : true)
                                           && (leaveFIlterVM.UserRole.HasValue ? ur.AdminRoleId == leaveFIlterVM.UserRole.Value : true)

                                           select new LeaveVM
                                           {
                                               LeaveId = lv.LeaveId,
                                               UserId = lv.UserId,
                                               UserName = ur.Prefix + " " + ur.FirstName + " " + ur.LastName,
                                               EmployeeCode=ur.EmployeeCode,
                                               StartDate = lv.StartDate,
                                               EndDate = lv.EndDate,
                                               NoOfDays = lv.NoOfDays,
                                               LeaveStatus = lv.LeaveStatus,
                                               RejectReason = lv.RejectReason,
                                               CreatedDate = lv.CreatedDate,
                                           }).OrderByDescending(x => x.StartDate).ToList();

                leaveFIlterVM.LeaveList.ForEach(x =>
                {
                    x.LeaveStatusText = CommonMethod.GetEnumDescription((LeaveStatus)x.LeaveStatus); 
                });
                leaveFIlterVM.UserRoleList = GetUserRoleList();
                leaveFIlterVM.CalenderMonth = CommonMethod.GetCalenderMonthList();
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

        [HttpPost]
        public string AcceptLeave(string ids)
        {
            string ReturnMessage = "";
            try
            {
                long loggedinUser = clsAdminSession.UserID;
                List<tbl_Leave> leaveList = _db.tbl_Leave.Where(x => ids.Contains(x.LeaveId.ToString())).ToList();

                if (leaveList != null)
                {
                    leaveList.ForEach(leave =>
                    {
                        leave.LeaveStatus = (int)LeaveStatus.Accept;
                        leave.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        leave.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                        #region Send SMS

                        int SmsId = (int)SMSType.LeaveApproved;
                        string msg = CommonMethod.GetSmsContent(SmsId);

                        tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == leave.UserId).FirstOrDefault();
                        Regex regReplace = new Regex("{#var#}");
                        msg = regReplace.Replace(msg, objEmployee.FirstName + " " + objEmployee.LastName, 1);
                        msg = regReplace.Replace(msg, leave.StartDate.ToString("dd/MM/yyyy") + " to " + leave.EndDate.ToString("dd/MM/yyyy"), 1);

                        msg = msg.Replace("\r\n", "\n");

                        var json = CommonMethod.SendSMSWithoutLog(msg, objEmployee.MobileNo);

                        #endregion

                    });

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

        public ActionResult Reject(long id)
        {
            LeaveVM leaveVM = new LeaveVM();
            try
            {
                leaveVM = (from lv in _db.tbl_Leave
                           join ur in _db.tbl_Employee on lv.UserId equals ur.EmployeeId
                           join cm in _db.tbl_Company on ur.CompanyId equals cm.CompanyId
                           where !lv.IsDeleted
                           && cm.CompanyId == companyId
                           && lv.LeaveId == id
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
                           }).FirstOrDefault();

                leaveVM.LeaveStatusText = CommonMethod.GetEnumDescription((LeaveStatus)leaveVM.LeaveStatus);
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(leaveVM);
        }

        [HttpPost]
        public ActionResult Reject(LeaveVM leaveVM)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    tbl_Leave objleave = _db.tbl_Leave.FirstOrDefault(x => x.LeaveId == leaveVM.LeaveId);
                    if (objleave != null)
                    {
                        objleave.LeaveStatus = leaveVM.LeaveStatus;
                        objleave.RejectReason = leaveVM.RejectReason;
                        objleave.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objleave.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.SaveChanges();

                        #region Send SMS of Accept/Reject

                        tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == objleave.UserId).FirstOrDefault();

                        if (leaveVM.LeaveStatus == (int)LeaveStatus.Accept)
                        {
                            int SmsId = (int)SMSType.LeaveApproved;
                            string msg = CommonMethod.GetSmsContent(SmsId);

                            Regex regReplace = new Regex("{#var#}");
                            msg = regReplace.Replace(msg, objEmployee.FirstName + " " + objEmployee.LastName, 1);
                            msg = regReplace.Replace(msg, objleave.StartDate.ToString("dd/MM/yyyy") + " to " + objleave.EndDate.ToString("dd/MM/yyyy"), 1);

                            msg = msg.Replace("\r\n", "\n");

                            var json = CommonMethod.SendSMSWithoutLog(msg, objEmployee.MobileNo);
                        }
                        if (leaveVM.LeaveStatus == (int)LeaveStatus.Reject)
                        {
                            int SmsId = (int)SMSType.LeaveApproved;
                            string msg = CommonMethod.GetSmsContent(SmsId);

                            Regex regReplace = new Regex("{#var#}");
                            msg = regReplace.Replace(msg, objEmployee.FirstName + " " + objEmployee.LastName, 1);
                            msg = regReplace.Replace(msg, leaveVM.RejectReason, 1);

                            msg = msg.Replace("\r\n", "\n");

                            var json = CommonMethod.SendSMSWithoutLog(msg, objEmployee.MobileNo);
                        }


                        #endregion

                    }

                }
                else
                {
                    return View(leaveVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return RedirectToAction("index");
        }
    }
}