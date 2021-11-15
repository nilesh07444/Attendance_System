using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class LunchBreakController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        public LunchBreakController()
        {
            _db = new AttendanceSystemEntities();
        }

        [HttpPost]
        [Route("Add")]
        public ResponseDataModel<bool> Add(LunchBreakLocationVM lunchbreakVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            DateTime today = CommonMethod.CurrentIndianDateTime();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;
                bool IsStartLunchMode = false;
                int NoOfLunchBreakAllowed = 1;
                int LunchBreakNumber = 1;

                #region Get Lunch Info

                tbl_Attendance runningAttendance = _db.tbl_Attendance.Where(x => x.UserId == employeeId && x.InDateTime != null && x.OutDateTime == null).FirstOrDefault();

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                if (objCompany != null)
                {
                    NoOfLunchBreakAllowed = objCompany.NoOfLunchBreakAllowed != null ? objCompany.NoOfLunchBreakAllowed.Value : 1;
                }

                tbl_EmployeeLunchBreak existLunch = _db.tbl_EmployeeLunchBreak.Where(x => x.EmployeeId == employeeId).OrderByDescending(x => x.EmployeeLunchBreakId).FirstOrDefault();
                if (existLunch == null || existLunch.EndDateTime != null)
                {
                    IsStartLunchMode = true;
                }

                #endregion

                #region Validation

                if (runningAttendance == null)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YourAttendanceNotTakenYetYouCanNotTakeLunch);
                }

                // Get today's taken lunch breaks of employee
                if (runningAttendance != null)
                {
                    List<tbl_EmployeeLunchBreak> lstExistLunchs = _db.tbl_EmployeeLunchBreak.Where(x => x.EmployeeId == employeeId && x.AttendanceId == runningAttendance.AttendanceId).ToList();
                    if (lstExistLunchs != null && lstExistLunchs.Count > 0)
                    {
                        LunchBreakNumber = lstExistLunchs.Count + 1;
                    }

                    if (IsStartLunchMode && lstExistLunchs != null && lstExistLunchs.Count >= NoOfLunchBreakAllowed)
                    {
                        response.IsError = true;
                        response.AddError("Your Break Limit Reached, so you can not take lunch break more for today.");
                    }
                }

                #endregion Validation

                if (!response.IsError)
                {

                    // Add or Update Lunch Break

                    if (IsStartLunchMode)
                    {
                        // Add new lunch

                        tbl_EmployeeLunchBreak lunchObject = new tbl_EmployeeLunchBreak();
                        lunchObject.EmployeeId = base.UTI.EmployeeId;
                        lunchObject.AttendanceId = runningAttendance.AttendanceId;
                        lunchObject.LunchBreakNo = LunchBreakNumber;
                        lunchObject.StartDateTime = CommonMethod.CurrentIndianDateTime();
                        lunchObject.StartLunchLatitude = lunchbreakVM.Latitude;
                        lunchObject.StartLunchLongitude = lunchbreakVM.Longitude;
                        lunchObject.StartLunchLocationFrom = lunchbreakVM.LocationFrom;

                        _db.tbl_EmployeeLunchBreak.Add(lunchObject);
                    }
                    else
                    {
                        // Update with End Lunch
                        existLunch.EndDateTime = CommonMethod.CurrentIndianDateTime();
                        existLunch.EndLunchLatitude = lunchbreakVM.Latitude;
                        existLunch.EndLunchLongitude = lunchbreakVM.Longitude;
                        existLunch.EndLunchLocationFrom = lunchbreakVM.LocationFrom;
                    }

                    _db.SaveChanges();

                    response.Data = true;
                }

            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [Route("List/{Id}"), HttpGet]
        public ResponseDataModel<List<EmployeeLunchBreakVM>> List(long Id) // Id = attendanceId
        {
            ResponseDataModel<List<EmployeeLunchBreakVM>> response = new ResponseDataModel<List<EmployeeLunchBreakVM>>();
            List<EmployeeLunchBreakVM> lunchBreakVM = new List<EmployeeLunchBreakVM>();

            try
            {
                long attendanceId = Id;
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                lunchBreakVM = (from lunch in _db.tbl_EmployeeLunchBreak
                                join emp in _db.tbl_Employee on lunch.EmployeeId equals emp.EmployeeId
                                join role in _db.mst_AdminRole on emp.AdminRoleId equals role.AdminRoleId
                                join att in _db.tbl_Attendance on lunch.AttendanceId equals att.AttendanceId
                                where !emp.IsDeleted && emp.CompanyId == companyId && emp.EmployeeId == employeeId && lunch.AttendanceId == attendanceId
                                select new EmployeeLunchBreakVM
                                {
                                    EmployeeLunchBreakId = lunch.EmployeeLunchBreakId,
                                    EmployeeId = lunch.EmployeeId,
                                    EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                    EmployeeCode = emp.EmployeeCode,
                                    StartDateTime = lunch.StartDateTime,
                                    StartLunchLocationFrom = lunch.StartLunchLocationFrom,
                                    StartLunchLatitude = lunch.StartLunchLatitude,
                                    StartLunchLongitude = lunch.StartLunchLongitude,

                                    EndDateTime = lunch.EndDateTime,
                                    EndLunchLocationFrom = lunch.EndLunchLocationFrom,
                                    EndLunchLatitude = lunch.EndLunchLatitude,
                                    EndLunchLongitude = lunch.EndLunchLongitude,

                                    EmployeeRole = role.AdminRoleName,
                                    AttendaceId = lunch.AttendanceId,
                                    AttendaceDate = att.AttendanceDate,
                                    LunchBreakNo = lunch.LunchBreakNo
                                }).OrderBy(x => x.LunchBreakNo).ThenBy(x => x.StartDateTime).ToList();

                response.Data = lunchBreakVM;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message.ToString());
            }

            return response;
        }

        [Route("GetLunchBreakDetail/{Id}"), HttpGet]
        public ResponseDataModel<EmployeeLunchBreakVM> GetLunchBreakDetail(long Id)
        {
            ResponseDataModel<EmployeeLunchBreakVM> response = new ResponseDataModel<EmployeeLunchBreakVM>();
            EmployeeLunchBreakVM lunchBreakVM = new EmployeeLunchBreakVM();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                lunchBreakVM = (from lunch in _db.tbl_EmployeeLunchBreak
                                join emp in _db.tbl_Employee on lunch.EmployeeId equals emp.EmployeeId
                                join role in _db.mst_AdminRole on emp.AdminRoleId equals role.AdminRoleId
                                join att in _db.tbl_Attendance on lunch.AttendanceId equals att.AttendanceId
                                where !emp.IsDeleted && emp.CompanyId == companyId && emp.EmployeeId == employeeId && lunch.EmployeeLunchBreakId == Id
                                select new EmployeeLunchBreakVM
                                {
                                    EmployeeLunchBreakId = lunch.EmployeeLunchBreakId,
                                    EmployeeId = lunch.EmployeeId,
                                    EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                    EmployeeCode = emp.EmployeeCode,

                                    StartDateTime = lunch.StartDateTime,
                                    StartLunchLocationFrom = lunch.StartLunchLocationFrom,
                                    StartLunchLatitude = lunch.StartLunchLatitude,
                                    StartLunchLongitude = lunch.StartLunchLongitude,

                                    EndDateTime = lunch.EndDateTime,
                                    EndLunchLocationFrom = lunch.EndLunchLocationFrom,
                                    EndLunchLatitude = lunch.EndLunchLatitude,
                                    EndLunchLongitude = lunch.EndLunchLongitude,

                                    EmployeeRole = role.AdminRoleName,
                                    AttendaceId = lunch.AttendanceId,
                                    AttendaceDate = att.AttendanceDate,
                                    LunchBreakNo = lunch.LunchBreakNo,
                                    AttendaceInDate = att.InDateTime,
                                    AttendaceOutDate = att.OutDateTime
                                }).FirstOrDefault();

                response.Data = lunchBreakVM;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message.ToString());
            }

            return response;
        }

        [Route("CheckEmployeeLunchStatus"), HttpGet]
        public ResponseDataModel<LunchBreakStatusVM> CheckEmployeeLunchStatus()
        {
            LunchBreakStatusVM lunchBreakStatusVM = new LunchBreakStatusVM();
            ResponseDataModel<LunchBreakStatusVM> response = new ResponseDataModel<LunchBreakStatusVM>();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                lunchBreakStatusVM.EmployeeId = employeeId;

                tbl_EmployeeLunchBreak objBreak = _db.tbl_EmployeeLunchBreak.Where(x => x.EmployeeId == employeeId).OrderByDescending(x => x.EmployeeLunchBreakId).FirstOrDefault();
                if (objBreak != null && objBreak.EndDateTime == null)
                {
                    lunchBreakStatusVM.IsLunchBreakRunning = true;
                    lunchBreakStatusVM.AttendanceId = objBreak.AttendanceId;
                    lunchBreakStatusVM.LunchBreakId = objBreak.EmployeeLunchBreakId;
                }

                response.Data = lunchBreakStatusVM;
            }
            catch (Exception ex)
            {
            }

            return response;
        }

    }
}