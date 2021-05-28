using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class WorkerAttendanceController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        long employeeId;
        long companyId;
        int roleId;
        public WorkerAttendanceController()
        {
            _db = new AttendanceSystemEntities();
            employeeId = base.UTI.EmployeeId;
            companyId = base.UTI.CompanyId;
            roleId = base.UTI.RoleId;
        }

        [HttpPost]
        [Route("Add")]
        public ResponseDataModel<bool> Add(WorkerAttendanceRequestVM workerAttendanceRequestVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                DateTime today = DateTime.UtcNow.Date;
                #region Validation
                if (!_db.tbl_AssignWorker.Any(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.Date == today))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorketNotAssignedToday);
                }

                if (roleId == (int)AdminRoles.Checker && workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CheckerCanNotTakeEveningAttendance);
                }

                if (roleId == (int)AdminRoles.Payer && (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning || workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Afternoon))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PayerCanNotTakeMorningOrAfterNoonAttendance);
                }

                if (roleId != (int)AdminRoles.Payer && roleId != (int)AdminRoles.Checker)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PayerCanNotTakeMorningOrAfterNoonAttendance);
                }

                if (workerAttendanceRequestVM.AttendanceType != (int)WorkerAttendanceType.Morning
                    && workerAttendanceRequestVM.AttendanceType != (int)WorkerAttendanceType.Afternoon
                    && workerAttendanceRequestVM.AttendanceType != (int)WorkerAttendanceType.Evening)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InValidWorkerAttendanceType);
                }

                if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening
                    && !(_db.tbl_WorkerAttendance.Any(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.IsAfternoon)))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnAfterNoon);
                }



                #endregion Validation
                if (!response.IsError)
                {
                    tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.FirstOrDefault(x => x.AttendanceDate == today && x.EmployeeId == workerAttendanceRequestVM.EmployeeId);

                    if (attendanceObject != null)
                    {
                        if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                        {
                            attendanceObject.IsEvening = true;
                            attendanceObject.EveningAttendanceBy = employeeId;
                            attendanceObject.EveningAttendanceDate = DateTime.UtcNow;
                            attendanceObject.ExtraHours = workerAttendanceRequestVM.ExtraHours;
                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                        {
                            attendanceObject.IsAfternoon = true;
                            attendanceObject.AfternoonAttendanceBy = employeeId;
                            attendanceObject.AfternoonAttendanceDate = DateTime.UtcNow;
                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                        {
                            attendanceObject.IsMorning = true;
                            attendanceObject.MorningAttendanceBy = employeeId;
                            attendanceObject.MorningAttendanceDate = DateTime.UtcNow;
                        }
                        _db.SaveChanges();
                    }
                    else
                    {
                        attendanceObject = new tbl_WorkerAttendance();
                        attendanceObject.EmployeeId = workerAttendanceRequestVM.EmployeeId;
                        attendanceObject.AttendanceDate = today;
                        if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                        {
                            attendanceObject.IsAfternoon = true;
                            attendanceObject.AfternoonAttendanceBy = employeeId;
                            attendanceObject.AfternoonAttendanceDate = DateTime.UtcNow;
                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                        {
                            attendanceObject.IsMorning = true;
                            attendanceObject.MorningAttendanceBy = employeeId;
                            attendanceObject.MorningAttendanceDate = DateTime.UtcNow;
                        }
                        _db.tbl_WorkerAttendance.Add(attendanceObject);
                        _db.SaveChanges();
                    }
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

      
        
    }
}
