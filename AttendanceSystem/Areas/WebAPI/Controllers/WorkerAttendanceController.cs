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


        }

        [HttpPost]
        [Route("Add")]
        public ResponseDataModel<string> Add(WorkerAttendanceRequestVM workerAttendanceRequestVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            response.IsError = false;
            response.Data = string.Empty;
            try
            {
                roleId = base.UTI.RoleId;
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

                if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning
                    && _db.tbl_WorkerAttendance.Any(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.AttendanceDate == today && x.IsMorning))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerMorningAttendanceAlreadyDone);
                }
                else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Afternoon
                    && _db.tbl_WorkerAttendance.Any(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.AttendanceDate == today && x.IsAfternoon))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAfternoonAttendanceAlreadyDone);
                }
                else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening
                    && _db.tbl_WorkerAttendance.Any(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.AttendanceDate == today && x.IsEvening))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerEveningAttendanceAlreadyDone);
                }

                if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening
                    && !(_db.tbl_WorkerAttendance.Any(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.AttendanceDate == today && x.IsAfternoon)))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnAfterNoon);
                }

                tbl_Employee employeeObj = _db.tbl_Employee.FirstOrDefault(x => x.EmployeeId == employeeId);
                if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening
                    && employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased && workerAttendanceRequestVM.NoOfHoursWorked == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PleaseProvideNoOfHoursWorked);
                }

                if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening
                    && employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased && workerAttendanceRequestVM.NoOfUnitWorked == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PleaseProvideNoOfHoursWorked);
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

                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                                attendanceObject.ExtraHours = workerAttendanceRequestVM.ExtraHours;

                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                                attendanceObject.NoOfHoursWorked = workerAttendanceRequestVM.NoOfHoursWorked;

                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                                attendanceObject.NoOfHoursWorked = workerAttendanceRequestVM.NoOfUnitWorked;

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

                    if (employeeObj.EmploymentCategory != (int)EmploymentCategory.MonthlyBased && workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                    {
                        tbl_WorkerPayment objWorkerPayment = new tbl_WorkerPayment();
                        objWorkerPayment.CompanyId = companyId;
                        objWorkerPayment.UserId = attendanceObject.EmployeeId;
                        objWorkerPayment.AttendanceId = attendanceObject.WorkerAttendanceId;
                        objWorkerPayment.PaymentDate = attendanceObject.AttendanceDate;
                        objWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                        objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                        objWorkerPayment.DebitAmount = 0;
                        objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnEveningAttendance;
                        //objEmployeePayment.Status=
                        //objEmployeePayment.ProcessStatusText
                        objWorkerPayment.CreatedDate = DateTime.UtcNow;
                        objWorkerPayment.CreatedBy = employeeId;
                        objWorkerPayment.ModifiedDate = DateTime.UtcNow;
                        objWorkerPayment.ModifiedBy = employeeId;

                        if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                        {
                            objWorkerPayment.CreditAmount = (employeeObj.PerCategoryPrice) + (employeeObj.ExtraPerHourPrice * workerAttendanceRequestVM.ExtraHours);
                        }
                        else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                        {
                            objWorkerPayment.CreditAmount = employeeObj.PerCategoryPrice * workerAttendanceRequestVM.NoOfHoursWorked;
                        }
                        else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                        {
                            objWorkerPayment.CreditAmount = employeeObj.PerCategoryPrice * workerAttendanceRequestVM.NoOfUnitWorked;
                        }
                        _db.tbl_WorkerPayment.Add(objWorkerPayment);
                        _db.SaveChanges();

                    }
                    response.Data = employeeObj.FirstName + " " + employeeObj.LastName;
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
        public ResponseDataModel<List<WorkerAttendanceVM>> List(WorkerAttendanceFilterVM workerAttendanceFilterVM)
        {
            ResponseDataModel<List<WorkerAttendanceVM>> response = new ResponseDataModel<List<WorkerAttendanceVM>>();
            response.IsError = false;
            try
            {
                companyId = base.UTI.CompanyId;

                List<WorkerAttendanceVM> attendanceList = (from at in _db.tbl_WorkerAttendance
                                                           join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                                           join st in _db.tbl_AssignWorker on at.EmployeeId equals st.EmployeeId
                                                           where emp.CompanyId == companyId
                                                           && st.Date == workerAttendanceFilterVM.AttendanceDate
                                                           && at.AttendanceDate == workerAttendanceFilterVM.AttendanceDate
                                                           && (workerAttendanceFilterVM.SiteId.HasValue ? st.SiteId == workerAttendanceFilterVM.SiteId.Value : true)
                                                           && (workerAttendanceFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceFilterVM.EmployeeId.Value : true)
                                                           select new WorkerAttendanceVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               Name = emp.FirstName + " " + emp.LastName,
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = at.IsMorning,
                                                               IsAfternoon = at.IsAfternoon,
                                                               IsEvening = at.IsEvening,
                                                               ProfilePicture = emp.ProfilePicture
                                                           }).OrderByDescending(x => x.AttendanceDate).ToList();

                attendanceList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                    x.ProfilePicture = (!string.IsNullOrEmpty(x.ProfilePicture) ? CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty);
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
