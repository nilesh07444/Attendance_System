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
        long employeeId;
        long companyId;
        public AttendanceController()
        {
            _db = new AttendanceSystemEntities();
            employeeId = base.UTI.EmployeeId;
            companyId = base.UTI.CompanyId;
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

                //if (string.IsNullOrEmpty(attendanceVM.TodayWorkDetail))
                //{
                //    response.IsError = true;
                //    response.AddError(ErrorMessage.TodayWorkDetailRequired);
                //}

                //if (string.IsNullOrEmpty(attendanceVM.TomorrowWorkDetail))
                //{
                //    response.IsError = true;
                //    response.AddError(ErrorMessage.TomorrowWorkDetailRequired);
                //}

                DateTime date;
                bool isValidTIme = DateTime.TryParse(attendanceVM.InDateTime.ToString(), out date);
                if (!isValidTIme || attendanceVM.InDateTime.ToString() == ErrorMessage.DefaultTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InTimeIsNotValid);
                }

                isValidTIme = DateTime.TryParse(attendanceVM.OutDateTime.ToString(), out date);
                if (!isValidTIme || attendanceVM.InDateTime.ToString() == ErrorMessage.DefaultTime)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.OutTimeIsNotValid);
                }

                if (attendanceVM.OutDateTime < attendanceVM.InDateTime)
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

                if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == attendanceVM.AttendanceDate.Month && (x.IsEmployeeDone || x.IsWorkerDone)))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyAttendance);
                }
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();

                #endregion Validation
                if (!response.IsError)
                {
                    tbl_Attendance attendanceObject = new tbl_Attendance();
                    attendanceObject.CompanyId = companyId;
                    attendanceObject.UserId = employeeId;
                    attendanceObject.AttendanceDate = attendanceVM.AttendanceDate;
                    if (objEmployee.EmploymentCategory == (int)EmploymentCategory.MonthlyBased || objEmployee.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                    {
                        attendanceObject.DayType = attendanceVM.DayType;
                    }
                    attendanceObject.ExtraHours = attendanceVM.ExtraHours;
                    attendanceObject.TodayWorkDetail = attendanceVM.TodayWorkDetail;
                    attendanceObject.TomorrowWorkDetail = attendanceVM.TomorrowWorkDetail;
                    attendanceObject.Remarks = attendanceVM.Remarks;
                    attendanceObject.LocationFrom = attendanceVM.LocationFrom;
                    attendanceObject.Status = (int)AttendanceStatus.Pending;
                    attendanceObject.IsActive = true;
                    attendanceObject.InDateTime = attendanceVM.InDateTime;
                    attendanceObject.OutDateTime = attendanceVM.OutDateTime;
                    attendanceObject.CreatedBy = employeeId;
                    attendanceObject.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    attendanceObject.ModifiedBy = employeeId;
                    attendanceObject.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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
                                                     && at.AttendanceDate.Month >= attendanceFilterVM.StartMonth && at.AttendanceDate.Month <= attendanceFilterVM.EndMonth
                                                     && at.AttendanceDate.Year == attendanceFilterVM.Year
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
                                                         InDateTime = at.InDateTime,
                                                         OutDateTime = at.OutDateTime,
                                                         ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                                         EmploymentCategory = emp.EmploymentCategory,
                                                         InLatitude = at.InLatitude,
                                                         InLongitude = at.InLongitude,
                                                         OutLatitude = at.OutLatitude,
                                                         OutLongitude = at.OutLongitude
                                                     }).OrderByDescending(x => x.AttendanceDate).ToList();

                attendanceList.ForEach(x =>
                {
                    x.StatusText = CommonMethod.GetEnumDescription((AttendanceStatus)x.Status);
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                    if (x.DayType == 1)
                    {
                        x.DayTypeText = CommonMethod.GetEnumDescription(DayType.FullDay);
                    }
                    else if (x.DayType == 0.5)
                    {
                        x.DayTypeText = CommonMethod.GetEnumDescription(DayType.HalfDay);
                    }
                });
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
                                                 OutLocationFrom = at.OutLocationFrom,
                                                 Status = at.Status,
                                                 RejectReason = at.RejectReason,
                                                 InDateTime = at.InDateTime,
                                                 OutDateTime = at.OutDateTime,
                                                 ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                                 EmploymentCategory = emp.EmploymentCategory,
                                                 InLatitude = at.InLatitude,
                                                 InLongitude = at.InLongitude,
                                                 OutLatitude = at.OutLatitude,
                                                 OutLongitude = at.OutLongitude,
                                                 NoOfHoursWorked = at.NoOfHoursWorked.HasValue ? at.NoOfHoursWorked.Value : 0,
                                                 NoOfUnitWorked = at.NoOfUnitWorked.HasValue ? at.NoOfUnitWorked.Value : 0,
                                                 PerCategoryPrice = emp.PerCategoryPrice,
                                                 EmployeeCode = emp.EmployeeCode
                                             }).FirstOrDefault();
                leaveDetails.StatusText = CommonMethod.GetEnumDescription((AttendanceStatus)leaveDetails.Status);
                leaveDetails.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)leaveDetails.EmploymentCategory);
                leaveDetails.InDateTime = Convert.ToDateTime(CommonMethod.ConvertFromUTCNew(leaveDetails.InDateTime));
                leaveDetails.OutDateTime = Convert.ToDateTime(CommonMethod.ConvertFromUTCNew(leaveDetails.OutDateTime));
                if (leaveDetails.DayType == 1)
                {
                    leaveDetails.DayTypeText = CommonMethod.GetEnumDescription(DayType.FullDay);
                }
                else if (leaveDetails.DayType == 0.5)
                {
                    leaveDetails.DayTypeText = CommonMethod.GetEnumDescription(DayType.HalfDay);
                }
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
                #region Validation
                if (attendanceVM.AttendanceId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AttendanceIdIsNotValid);
                }

                //if (string.IsNullOrEmpty(attendanceVM.TodayWorkDetail))
                //{
                //    response.IsError = true;
                //    response.AddError(ErrorMessage.TodayWorkDetailRequired);
                //}

                //if (string.IsNullOrEmpty(attendanceVM.TomorrowWorkDetail))
                //{
                //    response.IsError = true;
                //    response.AddError(ErrorMessage.TomorrowWorkDetailRequired);
                //}

                tbl_Employee employeeObj = _db.tbl_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                {
                    if (attendanceVM.DayType == 0)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.PleaseProvideDayType);
                    }
                }
                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased && attendanceVM.NoOfHoursWorked == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PleaseProvideNoOfHoursWorked);
                }

                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased && attendanceVM.NoOfUnitWorked == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PleaseProvideNoOfHoursWorked);
                }

                if (attendanceVM.AttendanceId > 0)
                {
                    tbl_Attendance attendanceObject = _db.tbl_Attendance.Where(x => x.AttendanceId == attendanceVM.AttendanceId).FirstOrDefault();
                    if (attendanceObject.Status != (int)AttendanceStatus.Pending)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.PendingAttendanceCanBeEditOnly);
                    }

                    if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == attendanceVM.AttendanceDate.Month && (x.IsEmployeeDone || x.IsWorkerDone)))
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyAttendance);
                    }

                    #endregion Validation
                    if (!response.IsError)
                    {

                        attendanceObject.ExtraHours = attendanceVM.ExtraHours;
                        attendanceObject.TodayWorkDetail = attendanceVM.TodayWorkDetail;
                        attendanceObject.TomorrowWorkDetail = attendanceVM.TomorrowWorkDetail;
                        attendanceObject.Remarks = attendanceVM.Remarks;
                        attendanceObject.ModifiedBy = base.UTI.EmployeeId;
                        attendanceObject.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                        if (employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                        {
                            attendanceObject.DayType = attendanceVM.DayType;
                            attendanceObject.ExtraHours = attendanceVM.ExtraHours;
                        }

                        if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                        {
                            attendanceObject.NoOfHoursWorked = attendanceVM.NoOfHoursWorked;
                        }

                        if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                        {
                            attendanceObject.NoOfUnitWorked = attendanceVM.NoOfUnitWorked;
                        }

                        _db.SaveChanges();

                        if (employeeObj.EmploymentCategory != (int)EmploymentCategory.MonthlyBased)
                        {
                            tbl_EmployeePayment objEmployeePayment = _db.tbl_EmployeePayment.Where(x => x.AttendanceId == attendanceObject.AttendanceId).FirstOrDefault();
                            if (objEmployeePayment != null)
                            {
                                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                                {
                                    objEmployeePayment.CreditAmount = (employeeObj.PerCategoryPrice) + (employeeObj.ExtraPerHourPrice * attendanceVM.ExtraHours);
                                }
                                else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                                {
                                    objEmployeePayment.CreditAmount = employeeObj.PerCategoryPrice * attendanceVM.NoOfHoursWorked;
                                }
                                else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                                {
                                    objEmployeePayment.CreditAmount = employeeObj.PerCategoryPrice * attendanceVM.NoOfUnitWorked;
                                }

                                _db.SaveChanges();
                            }

                        }

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

                        if (attendanceObject.Status == (int)LeaveStatus.Accept)
                        {
                            response.IsError = true;
                            response.AddError(ErrorMessage.AttendanceIsAcceptedCanNotDelete);
                        }

                        if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == attendanceObject.AttendanceDate.Month && (x.IsEmployeeDone || x.IsWorkerDone)))
                        {
                            response.IsError = true;
                            response.AddError(ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyAttendance);
                        }

                        #endregion Validation
                        if (!response.IsError)
                        {
                            if (attendanceObject != null)
                            {
                                _db.tbl_Attendance.Remove(attendanceObject);
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

        [HttpPost]
        [Route("InTime")]
        public ResponseDataModel<bool> InTime(InTimeRequestVM inTimeRequestVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                #region Validation

                if (_db.tbl_Leave.Any(x => x.UserId == employeeId && x.StartDate <= today && x.EndDate >= today && x.LeaveStatus != (int)LeaveStatus.Reject))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YouAreOnLeaveForTheday);
                }

                if (_db.tbl_Attendance.Any(x => x.UserId == employeeId && x.AttendanceDate == today && x.InDateTime != null && x.OutDateTime == null))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AlreadyInForTheDay);
                }
                #endregion Validation
                if (!response.IsError)
                {
                    tbl_Employee employee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                    tbl_Attendance attendanceObject = new tbl_Attendance();
                    attendanceObject.CompanyId = companyId;
                    attendanceObject.UserId = employeeId;
                    attendanceObject.AttendanceDate = CommonMethod.CurrentIndianDateTime().Date;
                   
                    attendanceObject.LocationFrom = inTimeRequestVM.InLocationFrom;
                    attendanceObject.Status = (int)AttendanceStatus.Login;
                    attendanceObject.IsActive = true;
                    attendanceObject.InDateTime = CommonMethod.CurrentIndianDateTime();
                    attendanceObject.InLatitude = inTimeRequestVM.InLatitude;
                    attendanceObject.InLongitude = inTimeRequestVM.InLongitude;
                    attendanceObject.CreatedBy = employeeId;
                    attendanceObject.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    attendanceObject.ModifiedBy = employeeId;
                    attendanceObject.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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
        [Route("OutTime")]
        public ResponseDataModel<bool> OutTime(OutTimeRequestVM outTimeRequestVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                tbl_Employee employeeObj = _db.tbl_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);

                #region Validation
                if (!_db.tbl_Attendance.Any(x => x.UserId == employeeId && x.AttendanceDate == today && x.InDateTime != null && x.OutDateTime == null))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AlreadyOutForTheDay);
                }
                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                {
                    if (outTimeRequestVM.DayType == 0)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.PleaseProvideDayType);
                    }
                }

                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased && outTimeRequestVM.NoOfHoursWorked == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PleaseProvideNoOfHoursWorked);
                }

                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased && outTimeRequestVM.NoOfUnitWorked == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PleaseProvideNoOfHoursWorked);
                }

                #endregion Validation
                if (!response.IsError)
                {
                    tbl_Attendance attendanceObject = _db.tbl_Attendance.FirstOrDefault(x => x.UserId == employeeId && x.AttendanceDate == today && x.InDateTime != null && x.OutDateTime == null);
                    attendanceObject.OutLocationFrom = outTimeRequestVM.OutLocationFrom;
                    attendanceObject.Status = (int)AttendanceStatus.Pending;
                    attendanceObject.OutDateTime = CommonMethod.CurrentIndianDateTime();
                    attendanceObject.OutLatitude = outTimeRequestVM.OutLatitude;
                    attendanceObject.OutLongitude = outTimeRequestVM.OutLongitude;

                    if (employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                    {
                        attendanceObject.DayType = outTimeRequestVM.DayType;
                        attendanceObject.ExtraHours = outTimeRequestVM.ExtraHours;
                    }

                    if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                    {
                        attendanceObject.NoOfHoursWorked = outTimeRequestVM.NoOfHoursWorked;
                    }

                    if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                    {
                        attendanceObject.NoOfUnitWorked = outTimeRequestVM.NoOfUnitWorked;
                    }

                    attendanceObject.TodayWorkDetail = outTimeRequestVM.TodayWorkDetail;
                    attendanceObject.TomorrowWorkDetail = outTimeRequestVM.TomorrowWorkDetail;
                    attendanceObject.Remarks = outTimeRequestVM.Remarks;
                    attendanceObject.ModifiedBy = employeeId;
                    attendanceObject.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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

        [HttpGet]
        [Route("Status")]
        public ResponseDataModel<bool> Status()
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                bool isPresent = _db.tbl_Attendance.Any(x => x.AttendanceDate == today && x.InDateTime != null && x.OutDateTime == null && x.UserId == employeeId);
                response.Data = isPresent;
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
