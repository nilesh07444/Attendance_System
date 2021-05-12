using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class AttendanceController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public AttendanceController()
        {
            _db = new AttendanceSystemEntities();
        }

        [HttpPost]
        [Route("Add")]
        public ResponseDataModel<bool> Add(AttendanceVM attendanceVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;
                #region Validation
                if (attendanceVM.AttendanceDate == null)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceDateRequired);
                }

                if (attendanceVM.DayType != ErrorMessage.FullDay && attendanceVM.DayType != ErrorMessage.HalfDay)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceDayTypeNotValid);
                }

                if (string.IsNullOrEmpty(attendanceVM.TodayWorkDetail))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.TodayWorkDetailRequired);
                }

                if (string.IsNullOrEmpty(attendanceVM.TomorrowWorkDetail))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.TomorrowWorkDetailRequired);
                }

                TimeSpan time;
                bool isValidTIme = TimeSpan.TryParse(attendanceVM.InTime.ToString(), out time);
                if (!isValidTIme || attendanceVM.InTime.ToString() == ErrorMessage.DefaultTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InTimeIsNotValid);
                }

                isValidTIme = TimeSpan.TryParse(attendanceVM.OutTime.ToString(), out time);
                if (!isValidTIme || attendanceVM.InTime.ToString() == ErrorMessage.DefaultTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.OutTimeIsNotValid);
                }

                if (attendanceVM.OutTime < attendanceVM.InTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.OutTimeIsNotValid);
                }

                bool isAttendanceExist = _db.tbl_Attendance.Any(x => !x.IsDeleted && x.Status != (int)AttendanceStatus.Reject && x.AttendanceDate == attendanceVM.AttendanceDate && x.UserId == employeeId);
                if (isAttendanceExist)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceOnSameDateAlreadyExist);
                }

                #endregion Validation
                if (!response.IsError)
                {
                    tbl_Attendance attendanceObject = new tbl_Attendance();
                    attendanceObject.CompanyId = companyId;
                    attendanceObject.UserId = employeeId;
                    attendanceObject.AttendanceDate = attendanceVM.AttendanceDate;
                    attendanceObject.DayType = attendanceVM.DayType;
                    attendanceObject.ExtraHours = attendanceVM.ExtraHours;
                    attendanceObject.TodayWorkDetail = attendanceVM.TodayWorkDetail;
                    attendanceObject.TomorrowWorkDetail = attendanceVM.TomorrowWorkDetail;
                    attendanceObject.Remarks = attendanceVM.Remarks;
                    attendanceObject.LocationFrom = attendanceVM.LocationFrom;
                    attendanceObject.Status = (int)AttendanceStatus.Pending;
                    attendanceObject.IsActive = true;
                    attendanceObject.InTime = attendanceVM.InTime;
                    attendanceObject.OutTime = attendanceVM.OutTime;
                    attendanceObject.CreatedBy = employeeId;
                    attendanceObject.CreatedDate = DateTime.UtcNow;
                    attendanceObject.ModifiedBy = employeeId;
                    attendanceObject.ModifiedDate = DateTime.UtcNow;
                    _db.tbl_Attendance.Add(attendanceObject);
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

        [HttpPost]
        [Route("List")]
        public ResponseDataModel<List<AttendanceVM>> List(AttendanceFilterVM attendanceFilterVM)
        {
            ResponseDataModel<List<AttendanceVM>> response = new ResponseDataModel<List<AttendanceVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                List<AttendanceVM> attendanceList = (from at in _db.tbl_Attendance
                                                     join emp in _db.tbl_Employee on at.UserId equals emp.EmployeeId
                                                     where !at.IsDeleted
                                                     && emp.EmployeeId == employeeId
                                                     && at.AttendanceDate >= attendanceFilterVM.StartDate && at.AttendanceDate <= attendanceFilterVM.EndDate
                                                     && (attendanceFilterVM.AttendanceStatus.HasValue ? at.Status == attendanceFilterVM.AttendanceStatus.Value : true)
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
                                                         RejectReason = at.RejectReason,
                                                         InTime = at.InTime,
                                                         OutTime = at.OutTime
                                                     }).OrderByDescending(x => x.AttendanceDate).ToList();
                response.Data = attendanceList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("Detail/{id}")]
        public ResponseDataModel<AttendanceVM> Detail(long id)
        {
            ResponseDataModel<AttendanceVM> response = new ResponseDataModel<AttendanceVM>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                AttendanceVM leaveDetails = (from at in _db.tbl_Attendance
                                             join emp in _db.tbl_Employee on at.UserId equals emp.EmployeeId
                                             where !at.IsDeleted
                                             && emp.EmployeeId == employeeId
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
                                                 RejectReason = at.RejectReason,
                                                 InTime = at.InTime,
                                                 OutTime = at.OutTime
                                             }).FirstOrDefault();

                response.Data = leaveDetails;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("Edit")]
        public ResponseDataModel<bool> Edit(AttendanceVM attendanceVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                #region Validation
                if (attendanceVM.AttendanceId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceIdIsNotValid);
                }

                if (attendanceVM.AttendanceDate == null)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceDateRequired);
                }

                if (attendanceVM.DayType != ErrorMessage.FullDay && attendanceVM.DayType != ErrorMessage.HalfDay)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceDayTypeNotValid);
                }

                if (string.IsNullOrEmpty(attendanceVM.TodayWorkDetail))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.TodayWorkDetailRequired);
                }

                if (string.IsNullOrEmpty(attendanceVM.TomorrowWorkDetail))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.TomorrowWorkDetailRequired);
                }

                TimeSpan time;
                bool isValidTIme = TimeSpan.TryParse(attendanceVM.InTime.ToString(), out time);
                if (!isValidTIme || attendanceVM.InTime.ToString() == ErrorMessage.DefaultTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InTimeIsNotValid);
                }

                isValidTIme = TimeSpan.TryParse(attendanceVM.OutTime.ToString(), out time);
                if (!isValidTIme || attendanceVM.InTime.ToString() == ErrorMessage.DefaultTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.OutTimeIsNotValid);
                }

                if (attendanceVM.OutTime < attendanceVM.InTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.OutTimeIsNotValid);
                }


                if (attendanceVM.AttendanceId > 0)
                {
                    bool isAttendanceExist = _db.tbl_Attendance.Any(x => x.AttendanceId != attendanceVM.AttendanceId && !x.IsDeleted && x.Status != (int)AttendanceStatus.Reject && x.AttendanceDate == attendanceVM.AttendanceDate && x.UserId == employeeId);
                    if (isAttendanceExist)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.AttendanceOnSameDateAlreadyExist);
                    }

                    tbl_Attendance attendanceObject = _db.tbl_Attendance.Where(x => x.AttendanceId == attendanceVM.AttendanceId).FirstOrDefault();
                    if (attendanceObject.Status != (int)AttendanceStatus.Pending)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.PendingLeaveCanBeEditOnly);
                    }
                    #endregion Validation
                    if (!response.IsError)
                    {

                        attendanceObject.AttendanceDate = attendanceVM.AttendanceDate;
                        attendanceObject.DayType = attendanceVM.DayType;
                        attendanceObject.ExtraHours = attendanceVM.ExtraHours;
                        attendanceObject.TodayWorkDetail = attendanceVM.TodayWorkDetail;
                        attendanceObject.TomorrowWorkDetail = attendanceVM.TomorrowWorkDetail;
                        attendanceObject.Remarks = attendanceVM.Remarks;
                        attendanceObject.LocationFrom = attendanceVM.LocationFrom;
                        attendanceObject.Status = attendanceVM.Status;
                        attendanceObject.InTime = attendanceVM.InTime;
                        attendanceObject.OutTime = attendanceVM.OutTime;
                        attendanceObject.ModifiedBy = base.UTI.EmployeeId;
                        attendanceObject.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                        response.Data = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("Delete/{id}")]
        public ResponseDataModel<bool> Delete(long id)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {


                #region Validation
                if (id == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceIdIsNotValid);
                }

                if (id > 0)
                {
                    long employeeId = base.UTI.EmployeeId;

                    bool isValidAttendance = _db.tbl_Attendance.Any(x => x.AttendanceId == id && !x.IsDeleted && x.UserId == employeeId);
                    if (!isValidAttendance)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.AttendanceIdIsNotValid);
                    }

                    if (isValidAttendance)

                    {
                        tbl_Attendance attendanceObject = _db.tbl_Attendance.Where(x => x.AttendanceId == id && x.UserId == employeeId).FirstOrDefault();

                        if (attendanceObject.Status != (int)LeaveStatus.Pending)
                        {
                            response.IsError = true;
                            response.AddError(ErrorMessage.PendingAttendanceCanBeDeleteOnly);
                        }
                        #endregion Validation
                        if (!response.IsError)
                        {
                            if (attendanceObject != null)
                            {
                                attendanceObject.IsDeleted = true;
                                attendanceObject.ModifiedBy = employeeId;
                                attendanceObject.ModifiedDate = DateTime.UtcNow;
                                _db.SaveChanges();
                                response.Data = true;
                            }
                            else
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.PendingLeaveCanBeDeleteOnly);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }
    }
}
