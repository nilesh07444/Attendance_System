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
    public class AttendanceController : Controller
    {
        // GET: Admin/Attendance
        AttendanceSystemEntities _db;
        long companyId;
        long LoggedInUserId;
        public AttendanceController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
            LoggedInUserId = clsAdminSession.UserID;
        }
        public ActionResult Index(int? userId = null, int? attendanceStatus = null, int? startMonth = null, int? endMonth = null, int? year = null)
        {
            AttendanceFilterVM attendanceFilterVM = new AttendanceFilterVM();
            try
            {
                if (userId.HasValue)
                    attendanceFilterVM.UserId = userId.Value;
                if (attendanceStatus.HasValue)
                    attendanceFilterVM.AttendanceStatus = attendanceStatus.Value;

                if (startMonth.HasValue && endMonth.HasValue)
                {
                    attendanceFilterVM.StartMonth = startMonth.Value;
                    attendanceFilterVM.EndMonth = endMonth.Value;
                }

                if (year.HasValue)
                {
                    attendanceFilterVM.Year = year.Value;
                }

                long companyId = clsAdminSession.CompanyId;

                List<SelectListItem> attendanceStatusList = GetAttendanceStatusList();



                attendanceFilterVM.AttendanceList = (from at in _db.tbl_Attendance
                                                     join emp in _db.tbl_Employee on at.UserId equals emp.EmployeeId
                                                     where !at.IsDeleted
                                                     && emp.CompanyId == companyId
                                                     && at.AttendanceDate.Month >= attendanceFilterVM.StartMonth && at.AttendanceDate.Month <= attendanceFilterVM.EndMonth
                                                     && at.AttendanceDate.Year== attendanceFilterVM.Year
                                                     && (attendanceFilterVM.AttendanceStatus.HasValue ? at.Status == attendanceFilterVM.AttendanceStatus.Value : true)
                                                     && (attendanceFilterVM.UserId.HasValue ? emp.EmployeeId == attendanceFilterVM.UserId.Value : true)
                                                     select new AttendanceVM
                                                     {
                                                         AttendanceId = at.AttendanceId,
                                                         CompanyId = at.CompanyId,
                                                         UserId = at.UserId,
                                                         Name = emp.FirstName + " " + emp.LastName,
                                                         AttendanceDate = at.AttendanceDate,
                                                         DayType = at.DayType,
                                                         ExtraHours = at.ExtraHours,
                                                         TodayWorkDetail = at.TodayWorkDetail,
                                                         TomorrowWorkDetail = at.TomorrowWorkDetail,
                                                         Remarks = at.Remarks,
                                                         LocationFrom = at.LocationFrom,
                                                         Status = at.Status,
                                                         RejectReason = at.RejectReason
                                                     }).OrderByDescending(x => x.AttendanceDate).ToList();

                attendanceFilterVM.EmployeeList = GetEmployeeList();
                attendanceFilterVM.CalenderMonth =CommonMethod.GetCalenderMonthList();
                attendanceFilterVM.AttendanceList.ForEach(x =>
                {
                    x.StatusText = attendanceStatusList.Where(z => z.Value == x.Status.ToString()).Select(c => c.Text).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
            }
            return View(attendanceFilterVM);
        }

        private List<SelectListItem> GetEmployeeList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName,
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetAttendanceStatusList()
        {
            string[] paymentTypeArr = Enum.GetNames(typeof(AttendanceStatus));
            var listpaymentType = paymentTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listpaymentType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        [HttpPost]
        public string Accept(string ids)
        {
            string ReturnMessage = "";
            try
            {
                long loggedinUser = clsAdminSession.UserID;
                List<tbl_Attendance> attendanceList = _db.tbl_Attendance.Where(x => ids.Contains(x.AttendanceId.ToString())).ToList();

                if (attendanceList != null)
                {
                    attendanceList.ForEach(attendance =>
                    {
                        attendance.Status = (int)LeaveStatus.Accept;
                        attendance.ModifiedBy = loggedinUser;
                        attendance.ModifiedDate = DateTime.UtcNow;
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
            AttendanceVM attendanceVM = new AttendanceVM();
            try
            {
                attendanceVM = (from at in _db.tbl_Attendance
                                join emp in _db.tbl_Employee on at.UserId equals emp.EmployeeId
                                where !at.IsDeleted
                                && emp.CompanyId == companyId
                                && at.AttendanceId == id
                                select new AttendanceVM
                                {
                                    AttendanceId = at.AttendanceId,
                                    CompanyId = at.CompanyId,
                                    UserId = at.UserId,
                                    Name = emp.FirstName + " " + emp.LastName,
                                    AttendanceDate = at.AttendanceDate,
                                    DayType = at.DayType,
                                    ExtraHours = at.ExtraHours,
                                    TodayWorkDetail = at.TodayWorkDetail,
                                    TomorrowWorkDetail = at.TomorrowWorkDetail,
                                    Remarks = at.Remarks,
                                    LocationFrom = at.LocationFrom,
                                    Status = at.Status,
                                    RejectReason = at.RejectReason
                                }).FirstOrDefault();

                attendanceVM.StatusText = CommonMethod.GetEnumDescription((AttendanceStatus)attendanceVM.Status);
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(attendanceVM);
        }

        [HttpPost]
        public ActionResult Reject(AttendanceVM attendanceVM)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    tbl_Attendance objAttendance = _db.tbl_Attendance.FirstOrDefault(x => x.AttendanceId == attendanceVM.AttendanceId);
                    if (objAttendance != null)
                    {
                        objAttendance.Status = attendanceVM.Status;
                        objAttendance.RejectReason = attendanceVM.RejectReason;
                        objAttendance.ModifiedBy = LoggedInUserId;
                        objAttendance.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                    }

                }
                else
                {
                    return View(attendanceVM);
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