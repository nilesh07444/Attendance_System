using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class WorkerAttendanceController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        long employeeId;
        long companyId;
        int roleId;
        string domainUrl = string.Empty;
        public WorkerAttendanceController()
        {
            _db = new AttendanceSystemEntities();
            domainUrl = ConfigurationManager.AppSettings["DomainUrl"].ToString();
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
                employeeId = base.UTI.EmployeeId;
                companyId = base.UTI.CompanyId;
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                #region Validation

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();

                if (!_db.tbl_Attendance.Any(x => x.UserId == employeeId && x.InDateTime != null && x.OutDateTime == null))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YourAttendanceNotTakenYetYouCanNotAssignWorker);
                }

                if (workerAttendanceRequestVM.SiteId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteRequired);
                }
                if (!_db.tbl_AssignWorker.Any(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.SiteId == workerAttendanceRequestVM.SiteId && x.Date == today && !x.IsClosed))
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

                tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.Where(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId && x.AttendanceDate == today && !x.IsClosed).FirstOrDefault();
                if (attendanceObject == null && workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnMorningOrAfterNoon);
                }

                if (attendanceObject != null && !attendanceObject.IsMorning && attendanceObject.IsAfternoon && workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CanNotTakeMorningAttendanceAfterAfterNoonAttendance);
                }

                if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning
                    && attendanceObject != null && attendanceObject.IsMorning)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerMorningAttendanceAlreadyDone);
                }
                else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Afternoon
                    && attendanceObject != null && attendanceObject.IsAfternoon)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAfternoonAttendanceAlreadyDone);
                }
                else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening
                    && attendanceObject != null && attendanceObject.IsEvening)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerEveningAttendanceAlreadyDone);
                }

                if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening
                    && attendanceObject != null && !attendanceObject.IsAfternoon)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnAfterNoon);
                }

                tbl_Employee employeeObj = _db.tbl_Employee.Where(x => x.EmployeeId == workerAttendanceRequestVM.EmployeeId).FirstOrDefault();
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
                    response.AddError(ErrorMessage.PleaseProvideNoOfUnitWorked);
                }

                #endregion Validation

                if (!response.IsError)
                {
                    if (attendanceObject != null)
                    {
                        if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                        {
                            attendanceObject.IsEvening = true;
                            attendanceObject.EveningAttendanceBy = employeeId;
                            attendanceObject.EveningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                            attendanceObject.EveningSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.EveningLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.EveningLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.EveningLocationFrom = workerAttendanceRequestVM.LocationFrom;
                            attendanceObject.IsClosed = true;
                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                                attendanceObject.ExtraHours = workerAttendanceRequestVM.ExtraHours;

                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                                attendanceObject.NoOfHoursWorked = workerAttendanceRequestVM.NoOfHoursWorked;

                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                                attendanceObject.NoOfHoursWorked = workerAttendanceRequestVM.NoOfUnitWorked;

                            attendanceObject.SalaryGiven = workerAttendanceRequestVM.TodaySalary.HasValue ? workerAttendanceRequestVM.TodaySalary.Value : 0;

                            tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == workerAttendanceRequestVM.SiteId && x.Date == today && x.EmployeeId == workerAttendanceRequestVM.EmployeeId && !x.IsClosed).FirstOrDefault();
                            if (assignedWorker != null)
                            {
                                assignedWorker.IsClosed = true;
                            }

                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                        {
                            attendanceObject.IsAfternoon = true;
                            attendanceObject.AfternoonAttendanceBy = employeeId;
                            attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                            attendanceObject.AfternoonSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.AfternoonLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.AfternoonLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.AfternoonLocationFrom = workerAttendanceRequestVM.LocationFrom;
                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                        {
                            attendanceObject.IsMorning = true;
                            attendanceObject.MorningAttendanceBy = employeeId;
                            attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                            attendanceObject.MorningSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.MorningLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.MorningLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.MorningLocationFrom = workerAttendanceRequestVM.LocationFrom;
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
                            attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                            attendanceObject.AfternoonSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.AfternoonLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.AfternoonLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.AfternoonLocationFrom = workerAttendanceRequestVM.LocationFrom;
                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                        {
                            attendanceObject.IsMorning = true;
                            attendanceObject.MorningAttendanceBy = employeeId;
                            attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                            attendanceObject.MorningSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.MorningLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.MorningLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.MorningLocationFrom = workerAttendanceRequestVM.LocationFrom;
                        }
                        _db.tbl_WorkerAttendance.Add(attendanceObject);
                        _db.SaveChanges();
                    }

                    // Save worker payment entry

                    if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                    {
                        tbl_WorkerPayment objWorkerPayment = _db.tbl_WorkerPayment.Where(x => x.UserId == attendanceObject.EmployeeId
                                                                    && !x.IsDeleted && x.AttendanceId == attendanceObject.WorkerAttendanceId
                                                                    && x.PaymentType != (int)EmployeePaymentType.Extra).FirstOrDefault();

                        if (objWorkerPayment == null)
                        {
                            objWorkerPayment = new tbl_WorkerPayment();
                            _db.tbl_WorkerPayment.Add(objWorkerPayment);
                        }

                        objWorkerPayment.CompanyId = companyId;
                        objWorkerPayment.UserId = attendanceObject.EmployeeId;
                        objWorkerPayment.AttendanceId = attendanceObject.WorkerAttendanceId;
                        objWorkerPayment.PaymentDate = attendanceObject.AttendanceDate.Date;
                        objWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                        objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                        objWorkerPayment.DebitAmount = 0; // workerAttendanceRequestVM.TodaySalary.HasValue && employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased ? workerAttendanceRequestVM.TodaySalary.Value : 0;
                        objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnEveningAttendance;
                        objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                        objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                        objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerPayment.CreatedBy = employeeId;
                        objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerPayment.ModifiedBy = employeeId;

                        #region Get CreditAmount

                        if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                        {
                            decimal extraHoursAmount = CommonMethod.getPriceBasedOnHours((double)employeeObj.ExtraPerHourPrice.Value, (double)workerAttendanceRequestVM.ExtraHours);
                            objWorkerPayment.CreditAmount = (attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (employeeObj.PerCategoryPrice) : (employeeObj.PerCategoryPrice / 2)) + extraHoursAmount;
                        }
                        else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                        {
                            decimal totalAmount = CommonMethod.getPriceBasedOnHours((double)employeeObj.PerCategoryPrice, (double)workerAttendanceRequestVM.NoOfHoursWorked);
                            objWorkerPayment.CreditAmount = totalAmount;
                        }
                        else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                        {
                            objWorkerPayment.CreditAmount = employeeObj.PerCategoryPrice * workerAttendanceRequestVM.NoOfUnitWorked;
                        }
                        else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                        {
                            decimal totalDaysinMonth = DateTime.DaysInMonth(today.Year, today.Month);
                            //decimal perDayAmount = Math.Round((employeeObj.MonthlySalaryPrice.HasValue ? employeeObj.MonthlySalaryPrice.Value : 0) / totalDaysinMonth, 2);

                            decimal perDayAmount = 0;
                            if (objCompany.CompanyConversionType == (int)CompanyConversionType.MonthBased)
                            {
                                perDayAmount = Math.Round((employeeObj.MonthlySalaryPrice.HasValue ? employeeObj.MonthlySalaryPrice.Value : 0) / (decimal)totalDaysinMonth, 2);
                            }
                            else
                            {
                                decimal totalDaysToApply = (decimal)totalDaysinMonth - employeeObj.NoOfFreeLeavePerMonth;
                                perDayAmount = Math.Round((employeeObj.MonthlySalaryPrice.HasValue ? employeeObj.MonthlySalaryPrice.Value : 0) / totalDaysToApply, 2);
                            }
                            decimal extraHoursAmount = CommonMethod.getPriceBasedOnHours((double)employeeObj.ExtraPerHourPrice.Value, (double)workerAttendanceRequestVM.ExtraHours);
                            objWorkerPayment.CreditAmount = (attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? perDayAmount : Math.Round((perDayAmount / 2), 2)) + extraHoursAmount;
                        }

                        #endregion

                        attendanceObject.TodaySalary = objWorkerPayment.CreditAmount.HasValue ? objWorkerPayment.CreditAmount.Value : 0;
                        objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearIdFromDate(attendanceObject.AttendanceDate);
                        _db.SaveChanges();

                        #region Debit entry if entered Salary GIven

                        if (attendanceObject.SalaryGiven > 0)
                        {
                            tbl_WorkerPayment objWorkerDebitPayment = new tbl_WorkerPayment();
                            objWorkerDebitPayment.CompanyId = companyId;
                            objWorkerDebitPayment.UserId = attendanceObject.EmployeeId;
                            objWorkerDebitPayment.PaymentDate = CommonMethod.CurrentIndianDateTime().Date; //paymentVM.PaymentDate;
                            objWorkerDebitPayment.CreditOrDebitText = ErrorMessage.Debit;
                            objWorkerDebitPayment.DebitAmount = attendanceObject.SalaryGiven;
                            objWorkerDebitPayment.PaymentType = (int)EmployeePaymentType.Salary;
                            objWorkerDebitPayment.Month = attendanceObject.AttendanceDate.Month;
                            objWorkerDebitPayment.Year = attendanceObject.AttendanceDate.Year;
                            objWorkerDebitPayment.CreatedBy = employeeId;
                            objWorkerDebitPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerDebitPayment.ModifiedBy = employeeId;
                            objWorkerDebitPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerDebitPayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                            _db.tbl_WorkerPayment.Add(objWorkerDebitPayment);
                            _db.SaveChanges();
                        }

                        #endregion

                    }
                    else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                    {
                        if (attendanceObject.IsMorning && attendanceObject.IsAfternoon && (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased))
                        {

                            tbl_WorkerPayment objWorkerPayment = _db.tbl_WorkerPayment.Where(x => x.UserId == attendanceObject.EmployeeId
                                                                    && !x.IsDeleted && x.AttendanceId == attendanceObject.WorkerAttendanceId
                                                                    && x.PaymentType != (int)EmployeePaymentType.Extra).FirstOrDefault();

                            if (objWorkerPayment == null)
                            {
                                objWorkerPayment = new tbl_WorkerPayment();
                                _db.tbl_WorkerPayment.Add(objWorkerPayment);
                            }

                            objWorkerPayment.CompanyId = companyId;
                            objWorkerPayment.UserId = attendanceObject.EmployeeId;
                            objWorkerPayment.AttendanceId = attendanceObject.WorkerAttendanceId;
                            objWorkerPayment.PaymentDate = attendanceObject.AttendanceDate.Date;
                            objWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                            objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                            objWorkerPayment.DebitAmount = workerAttendanceRequestVM.TodaySalary.HasValue && employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased ? workerAttendanceRequestVM.TodaySalary.Value : 0;
                            objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnAfternoonAttendance;
                            objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                            objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                            objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerPayment.CreatedBy = employeeId;
                            objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerPayment.ModifiedBy = employeeId;
                            objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearIdFromDate(attendanceObject.AttendanceDate);

                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                            {
                                objWorkerPayment.CreditAmount = attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (employeeObj.PerCategoryPrice) : (employeeObj.PerCategoryPrice / 2);
                            }
                            else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                            {
                                decimal totalDaysinMonth = DateTime.DaysInMonth(today.Year, today.Month);
                                decimal perDayAmount = 0;
                                if (objCompany.CompanyConversionType == (int)CompanyConversionType.MonthBased)
                                {
                                    perDayAmount = Math.Round((employeeObj.MonthlySalaryPrice.HasValue ? employeeObj.MonthlySalaryPrice.Value : 0) / (decimal)totalDaysinMonth, 2);
                                }
                                else
                                {
                                    decimal totalDaysToApply = (decimal)totalDaysinMonth - employeeObj.NoOfFreeLeavePerMonth;
                                    perDayAmount = Math.Round((employeeObj.MonthlySalaryPrice.HasValue ? employeeObj.MonthlySalaryPrice.Value : 0) / totalDaysToApply, 2);
                                }

                                objWorkerPayment.CreditAmount = attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? perDayAmount : Math.Round((perDayAmount / 2), 2);
                            }

                            attendanceObject.TodaySalary = objWorkerPayment.CreditAmount.HasValue ? objWorkerPayment.CreditAmount.Value : 0;

                            _db.SaveChanges();
                        }
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

                /*                 
                List<WorkerAttendanceVM> attendanceList = (from at in _db.tbl_WorkerAttendance
                                                           join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                                           where emp.CompanyId == companyId
                                                           && at.AttendanceDate == workerAttendanceFilterVM.AttendanceDate
                                                           && (at.MorningSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.AfternoonSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.EveningSiteId == workerAttendanceFilterVM.SiteId)
                                                           && (workerAttendanceFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceFilterVM.EmployeeId.Value : true)
                                                           select new WorkerAttendanceVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               EmployeeCode = emp.EmployeeCode,
                                                               Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = (at.IsMorning && at.MorningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsAfternoon = (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsEvening = (at.IsEvening && at.EveningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               ProfilePicture = emp.ProfilePicture
                                                           }).OrderByDescending(x => x.AttendanceDate).ToList();

                */

                List<WorkerAttendanceVM> attendanceNewList = (from assi in _db.tbl_AssignWorker
                                                              join emp in _db.tbl_Employee on assi.EmployeeId equals emp.EmployeeId
                                                              join at in _db.tbl_WorkerAttendance
                                                              on new { EmployeeId = assi.EmployeeId, Date = assi.Date, assi.IsClosed } equals new { EmployeeId = at.EmployeeId, Date = at.AttendanceDate, IsClosed = at.IsClosed } into wtc
                                                              from at in wtc.DefaultIfEmpty()

                                                              where emp.CompanyId == companyId
                                                              && (assi.Date == workerAttendanceFilterVM.AttendanceDate || at.AttendanceDate == workerAttendanceFilterVM.AttendanceDate)
                                                              && assi.SiteId == workerAttendanceFilterVM.SiteId
                                                              && (workerAttendanceFilterVM.EmployeeId.HasValue ? emp.EmployeeId == workerAttendanceFilterVM.EmployeeId.Value : true)
                                                              select new WorkerAttendanceVM
                                                              {
                                                                  AttendanceId = (at != null ? at.WorkerAttendanceId : 0),
                                                                  CompanyId = emp.CompanyId,
                                                                  EmployeeId = emp.EmployeeId,
                                                                  EmployeeCode = emp.EmployeeCode,
                                                                  Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                                  AttendanceDate = assi.Date,
                                                                  EmploymentCategory = emp.EmploymentCategory,
                                                                  IsMorning = (at != null ? (at.IsMorning && at.MorningSiteId == workerAttendanceFilterVM.SiteId ? true : false) : false),
                                                                  IsAfternoon = (at != null ? (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceFilterVM.SiteId ? true : false) : false),
                                                                  IsEvening = (at != null ? (at.IsEvening && at.EveningSiteId == workerAttendanceFilterVM.SiteId ? true : false) : false),
                                                                  ProfilePicture = emp.ProfilePicture
                                                              }).OrderByDescending(x => x.AttendanceDate).ToList();

                attendanceNewList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                    x.ProfilePicture = (!string.IsNullOrEmpty(x.ProfilePicture) ? CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty);
                });
                response.Data = attendanceNewList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("Report")]
        public ResponseDataModel<List<WorkerAttendanceVM>> Report(WorkerAttendanceReportFilterVM workerAttendanceReportFilterVM)
        {
            ResponseDataModel<List<WorkerAttendanceVM>> response = new ResponseDataModel<List<WorkerAttendanceVM>>();
            response.IsError = false;
            try
            {
                companyId = base.UTI.CompanyId;

                List<WorkerAttendanceVM> attendanceList = (from at in _db.tbl_WorkerAttendance
                                                           join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                                           where emp.CompanyId == companyId
                                                           && at.AttendanceDate >= workerAttendanceReportFilterVM.StartDate && at.AttendanceDate <= workerAttendanceReportFilterVM.EndDate
                                                           && (workerAttendanceReportFilterVM.SiteId.HasValue ? (at.MorningSiteId == workerAttendanceReportFilterVM.SiteId
                                                           || at.AfternoonSiteId == workerAttendanceReportFilterVM.SiteId
                                                           || at.EveningSiteId == workerAttendanceReportFilterVM.SiteId) : true)
                                                           && (workerAttendanceReportFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceReportFilterVM.EmployeeId.Value : true)
                                                           select new WorkerAttendanceVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = (at.IsMorning && at.MorningSiteId == workerAttendanceReportFilterVM.SiteId ? true : false),
                                                               IsAfternoon = (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceReportFilterVM.SiteId ? true : false),
                                                               IsEvening = (at.IsEvening && at.EveningSiteId == workerAttendanceReportFilterVM.SiteId ? true : false),
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

        [HttpPost]
        [Route("ListDownload")]
        public ResponseDataModel<string> ListDownload(WorkerAttendanceReportFilterVM workerAttendanceReportFilterVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {

                ExcelPackage excel = new ExcelPackage();
                long companyId = base.UTI.CompanyId;
                bool hasrecord = false;
                List<WorkerAttendanceVM> attendanceList = (from at in _db.tbl_WorkerAttendance
                                                           join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                                           where emp.CompanyId == companyId
                                                           && at.AttendanceDate >= workerAttendanceReportFilterVM.StartDate && at.AttendanceDate <= workerAttendanceReportFilterVM.EndDate
                                                           && (at.MorningSiteId == workerAttendanceReportFilterVM.SiteId
                                                           || at.AfternoonSiteId == workerAttendanceReportFilterVM.SiteId
                                                           || at.EveningSiteId == workerAttendanceReportFilterVM.SiteId)
                                                           && (workerAttendanceReportFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceReportFilterVM.EmployeeId.Value : true)
                                                           select new WorkerAttendanceVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = (at.IsMorning && at.MorningSiteId == workerAttendanceReportFilterVM.SiteId ? true : false),
                                                               IsAfternoon = (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceReportFilterVM.SiteId ? true : false),
                                                               IsEvening = (at.IsEvening && at.EveningSiteId == workerAttendanceReportFilterVM.SiteId ? true : false),
                                                               ProfilePicture = emp.ProfilePicture
                                                           }).OrderByDescending(x => x.AttendanceDate).ToList();
                attendanceList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                    x.ProfilePicture = (!string.IsNullOrEmpty(x.ProfilePicture) ? CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty);
                });

                string[] arrycolmns = new string[] { "Name", "Date", "Morning", "Afternoon", "Evening" };

                var workSheet = excel.Workbook.Worksheets.Add("Report");
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 20;
                workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Cells[1, 1].Value = "Worker Attendance Report: " + workerAttendanceReportFilterVM.StartDate.ToString("dd-MM-yyyy") + " to " + workerAttendanceReportFilterVM.EndDate.ToString("dd-MM-yyyy");
                for (var col = 1; col < arrycolmns.Length + 1; col++)
                {
                    workSheet.Cells[2, col].Style.Font.Bold = true;
                    workSheet.Cells[2, col].Style.Font.Size = 12;
                    workSheet.Cells[2, col].Value = arrycolmns[col - 1];
                    workSheet.Cells[2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[2, col].AutoFitColumns(30, 70);
                    workSheet.Cells[2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.WrapText = true;
                }

                int row1 = 1;

                if (attendanceList != null && attendanceList.Count() > 0)
                {
                    hasrecord = true;
                    foreach (var attendance in attendanceList)
                    {
                        workSheet.Cells[row1 + 2, 1].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 1].Value = attendance.Name;
                        workSheet.Cells[row1 + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 1].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 2].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 2].Value = attendance.AttendanceDate.ToString("dd-MM-yyyy");
                        workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 3].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 3].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 3].Value = attendance.IsMorning;
                        workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);



                        workSheet.Cells[row1 + 2, 4].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 4].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 4].Value = attendance.IsAfternoon;
                        workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);


                        workSheet.Cells[row1 + 2, 5].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 5].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 5].Value = attendance.IsEvening;
                        workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);
                        row1 = row1 + 1;
                    }
                }

                string guidstr = Guid.NewGuid().ToString();
                guidstr = guidstr.Substring(0, 5);

                string documentPath = HttpContext.Current.Server.MapPath(ErrorMessage.DocumentDirectoryPath);

                bool folderExists = Directory.Exists(documentPath);
                if (!folderExists)
                    Directory.CreateDirectory(documentPath);

                string flname = "WorkerAttendance_" + workerAttendanceReportFilterVM.StartDate.ToString("dd-MM-yyyy") + "_" + workerAttendanceReportFilterVM.EndDate.ToString("dd-MM-yyyy") + guidstr + ".xlsx";
                excel.SaveAs(new FileInfo(documentPath + flname));
                if (hasrecord == true)
                {
                    response.Data = CommonMethod.GetCurrentDomain() + ErrorMessage.DocumentDirectoryPath + flname;
                }
                else
                {
                    response.Data = "";
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }

        [HttpGet]
        [Route("GetWorkerListOfPendingMorningAttendance")]
        public ResponseDataModel<List<EmployeeVM>> GetWorkerListOfPendingMorningAttendance(long siteId, string searchText)
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;

            try
            {
                companyId = base.UTI.CompanyId;
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join assi in _db.tbl_AssignWorker on emp.EmployeeId equals assi.EmployeeId

                                               join at in _db.tbl_WorkerAttendance
                                               on new { EmployeeId = assi.EmployeeId, Date = assi.Date, SiteId = assi.SiteId, assi.IsClosed } equals new { EmployeeId = at.EmployeeId, Date = at.AttendanceDate, SiteId = at.MorningSiteId.Value, IsClosed = at.IsClosed } into wtc
                                               from att in wtc.DefaultIfEmpty()

                                               join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                               from st in state.DefaultIfEmpty()

                                               join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                               from dt in district.DefaultIfEmpty()

                                               let isMorning = att == null ? false : att.IsMorning

                                               where !emp.IsDeleted && emp.IsActive
                                               && assi.Date == today
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               && emp.CompanyId == companyId

                                               && (!string.IsNullOrEmpty(searchText) ? (emp.EmployeeCode.Contains(searchText)
                                               || emp.FirstName.Contains(searchText)
                                               || emp.LastName.Contains(searchText)) : true)

                                               && !isMorning
                                               && !assi.IsClosed
                                               && assi.SiteId == siteId
                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
                                                   EmployeeCode = emp.EmployeeCode,
                                                   CompanyId = emp.CompanyId,
                                                   Prefix = emp.Prefix,
                                                   FirstName = emp.FirstName,
                                                   LastName = emp.LastName,
                                                   MobileNo = emp.MobileNo,
                                                   Dob = emp.Dob,
                                                   DateOfJoin = emp.DateOfJoin,
                                                   BloodGroup = emp.BloodGroup,
                                                   EmploymentCategory = emp.EmploymentCategory,
                                                   MonthlySalaryPrice = emp.MonthlySalaryPrice,
                                                   AdharCardNo = emp.AdharCardNo,
                                                   Address = emp.Address,
                                                   City = emp.City,
                                                   Pincode = emp.Pincode,
                                                   StateName = st != null ? st.StateName : "",
                                                   DistrictName = dt != null ? dt.DistrictName : "",
                                                   IsActive = emp.IsActive,
                                                   ProfilePicture = !string.IsNullOrEmpty(emp.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + emp.ProfilePicture : string.Empty
                                               }).ToList();

                response.Data = workerList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("GetWorkerListOfPendingAfternoonAttendance")]
        public ResponseDataModel<List<EmployeeVM>> GetWorkerListOfPendingAfternoonAttendance(long siteId, string searchText)
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;

            try
            {
                companyId = base.UTI.CompanyId;
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join assi in _db.tbl_AssignWorker on emp.EmployeeId equals assi.EmployeeId

                                               join att in _db.tbl_WorkerAttendance
                                               on new { EmployeeId = assi.EmployeeId, Date = assi.Date, SiteId = assi.SiteId, assi.IsClosed } equals new { EmployeeId = att.EmployeeId, Date = att.AttendanceDate, SiteId = att.AfternoonSiteId.Value, IsClosed = att.IsClosed } into wtc
                                               from att in wtc.DefaultIfEmpty()

                                               join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                               from st in state.DefaultIfEmpty()

                                               join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                               from dt in district.DefaultIfEmpty()

                                               let isAfternoon = att == null ? false : att.IsAfternoon

                                               where !emp.IsDeleted && emp.IsActive
                                               && assi.Date == today
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               && emp.CompanyId == companyId

                                               && (!string.IsNullOrEmpty(searchText) ? (emp.EmployeeCode.Contains(searchText)
                                               || emp.FirstName.Contains(searchText)
                                               || emp.LastName.Contains(searchText)) : true)

                                               && !isAfternoon
                                               && !assi.IsClosed
                                               && assi.SiteId == siteId

                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
                                                   EmployeeCode = emp.EmployeeCode,
                                                   CompanyId = emp.CompanyId,
                                                   Prefix = emp.Prefix,
                                                   FirstName = emp.FirstName,
                                                   LastName = emp.LastName,
                                                   MobileNo = emp.MobileNo,
                                                   Dob = emp.Dob,
                                                   DateOfJoin = emp.DateOfJoin,
                                                   BloodGroup = emp.BloodGroup,
                                                   EmploymentCategory = emp.EmploymentCategory,
                                                   MonthlySalaryPrice = emp.MonthlySalaryPrice,
                                                   AdharCardNo = emp.AdharCardNo,
                                                   Address = emp.Address,
                                                   City = emp.City,
                                                   Pincode = emp.Pincode,
                                                   StateName = st != null ? st.StateName : "",
                                                   DistrictName = dt != null ? dt.DistrictName : "",
                                                   IsActive = emp.IsActive,
                                                   ProfilePicture = !string.IsNullOrEmpty(emp.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + emp.ProfilePicture : string.Empty
                                               }).ToList();
                response.Data = workerList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("GetWorkerListOfPendingEveningAttendance")]
        public ResponseDataModel<List<EmployeeVM>> GetWorkerListOfPendingEveningAttendance(long siteId, string searchText)
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;

            try
            {
                companyId = base.UTI.CompanyId;
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join assi in _db.tbl_AssignWorker on emp.EmployeeId equals assi.EmployeeId
                                               join att in _db.tbl_WorkerAttendance on assi.EmployeeId equals att.EmployeeId

                                               join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                               from st in state.DefaultIfEmpty()

                                               join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                               from dt in district.DefaultIfEmpty()

                                               where !emp.IsDeleted && emp.IsActive
                                               && assi.Date == today
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               && emp.CompanyId == companyId

                                               && (!string.IsNullOrEmpty(searchText) ? (emp.EmployeeCode.Contains(searchText)
                                               || emp.FirstName.Contains(searchText)
                                               || emp.LastName.Contains(searchText)) : true)

                                               && att.AttendanceDate == today
                                               && att.IsAfternoon
                                               && !att.IsEvening
                                               && !att.IsClosed
                                               && !assi.IsClosed
                                               && assi.SiteId == siteId

                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
                                                   EmployeeCode = emp.EmployeeCode,
                                                   CompanyId = emp.CompanyId,
                                                   Prefix = emp.Prefix,
                                                   FirstName = emp.FirstName,
                                                   LastName = emp.LastName,
                                                   MobileNo = emp.MobileNo,
                                                   Dob = emp.Dob,
                                                   DateOfJoin = emp.DateOfJoin,
                                                   BloodGroup = emp.BloodGroup,
                                                   EmploymentCategory = emp.EmploymentCategory,
                                                   MonthlySalaryPrice = emp.MonthlySalaryPrice,
                                                   AdharCardNo = emp.AdharCardNo,
                                                   Address = emp.Address,
                                                   City = emp.City,
                                                   Pincode = emp.Pincode,
                                                   StateName = st != null ? st.StateName : "",
                                                   DistrictName = dt != null ? dt.DistrictName : "",
                                                   IsActive = emp.IsActive,
                                                   ProfilePicture = !string.IsNullOrEmpty(emp.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + emp.ProfilePicture : string.Empty
                                               }).ToList();
                response.Data = workerList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("SaveMultipleAfternoonWorkerAttendance")]
        public ResponseDataModel<bool> SaveMultipleAfternoonWorkerAttendance(AfternoonAttendanceRequestVM afternoonAttendanceRequestVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;

            try
            {
                roleId = base.UTI.RoleId;
                employeeId = base.UTI.EmployeeId;
                companyId = base.UTI.CompanyId;
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                #region Validation

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();

                if (!_db.tbl_Attendance.Any(x => x.UserId == employeeId && x.InDateTime != null && x.OutDateTime == null))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YourAttendanceNotTakenYetYouCanNotAssignWorker);
                }

                if (roleId == (int)AdminRoles.Payer)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PayerCanNotTakeMorningOrAfterNoonAttendance);
                }

                if (afternoonAttendanceRequestVM.SiteId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteRequired);
                }

                #endregion Validation

                if (!response.IsError && afternoonAttendanceRequestVM != null && afternoonAttendanceRequestVM.EmployeeList != null && afternoonAttendanceRequestVM.EmployeeList.Count > 0)
                {
                    afternoonAttendanceRequestVM.EmployeeList.ForEach(objEmployee =>
                    {
                        tbl_Employee employeeObj = _db.tbl_Employee.Where(x => x.EmployeeId == objEmployee.EmployeeId).FirstOrDefault();
                        bool IsWorkerAssigned = _db.tbl_AssignWorker.Any(x => x.EmployeeId == objEmployee.EmployeeId && x.SiteId == afternoonAttendanceRequestVM.SiteId && x.Date == today && !x.IsClosed);
                        if (IsWorkerAssigned)
                        {
                            tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.Where(x => x.EmployeeId == objEmployee.EmployeeId && x.AttendanceDate == today && !x.IsClosed).FirstOrDefault();
                            if (attendanceObject != null && attendanceObject.IsMorning && !attendanceObject.IsAfternoon)
                            {
                                // update attendance with afternoon data
                                attendanceObject.IsAfternoon = true;
                                attendanceObject.AfternoonAttendanceBy = employeeId;
                                attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.AfternoonSiteId = afternoonAttendanceRequestVM.SiteId;
                                attendanceObject.AfternoonLatitude = afternoonAttendanceRequestVM.Latitude;
                                attendanceObject.AfternoonLongitude = afternoonAttendanceRequestVM.Longitude;
                                attendanceObject.AfternoonLocationFrom = afternoonAttendanceRequestVM.LocationFrom;
                                _db.SaveChanges();
                            }
                            else
                            {
                                tbl_WorkerAttendance attendanceObjectAfternoon = new tbl_WorkerAttendance();
                                attendanceObjectAfternoon.EmployeeId = objEmployee.EmployeeId;
                                attendanceObjectAfternoon.AttendanceDate = today;
                                attendanceObjectAfternoon.IsAfternoon = true;
                                attendanceObjectAfternoon.AfternoonAttendanceBy = employeeId;
                                attendanceObjectAfternoon.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObjectAfternoon.AfternoonSiteId = afternoonAttendanceRequestVM.SiteId;
                                attendanceObjectAfternoon.AfternoonLatitude = afternoonAttendanceRequestVM.Latitude;
                                attendanceObjectAfternoon.AfternoonLongitude = afternoonAttendanceRequestVM.Longitude;
                                attendanceObjectAfternoon.AfternoonLocationFrom = afternoonAttendanceRequestVM.LocationFrom;
                                _db.tbl_WorkerAttendance.Add(attendanceObjectAfternoon);
                                _db.SaveChanges();
                            }

                            // Save Worker payment entry on afternoon attendance
                            
                            if (attendanceObject != null && attendanceObject.IsMorning && attendanceObject.IsAfternoon && (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased))
                            {

                                tbl_WorkerPayment objWorkerPayment = _db.tbl_WorkerPayment.Where(x => x.UserId == attendanceObject.EmployeeId
                                                                        && !x.IsDeleted && x.AttendanceId == attendanceObject.WorkerAttendanceId
                                                                        && x.PaymentType != (int)EmployeePaymentType.Extra).FirstOrDefault();

                                if (objWorkerPayment == null)
                                {
                                    objWorkerPayment = new tbl_WorkerPayment();
                                    _db.tbl_WorkerPayment.Add(objWorkerPayment);
                                }

                                objWorkerPayment.CompanyId = companyId;
                                objWorkerPayment.UserId = attendanceObject.EmployeeId;
                                objWorkerPayment.AttendanceId = attendanceObject.WorkerAttendanceId;
                                objWorkerPayment.PaymentDate = attendanceObject.AttendanceDate.Date;
                                objWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                                objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                                //objWorkerPayment.DebitAmount = workerAttendanceRequestVM.TodaySalary.HasValue && employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased ? workerAttendanceRequestVM.TodaySalary.Value : 0;
                                objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnAfternoonAttendance;
                                objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                                objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                                objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                objWorkerPayment.CreatedBy = employeeId;
                                objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                objWorkerPayment.ModifiedBy = employeeId;
                                objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearIdFromDate(attendanceObject.AttendanceDate);

                                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                                {
                                    objWorkerPayment.CreditAmount = attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (employeeObj.PerCategoryPrice) : (employeeObj.PerCategoryPrice / 2);
                                }
                                else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                                {
                                    decimal totalDaysinMonth = DateTime.DaysInMonth(today.Year, today.Month);
                                    decimal perDayAmount = 0;
                                    if (objCompany.CompanyConversionType == (int)CompanyConversionType.MonthBased)
                                    {
                                        perDayAmount = Math.Round((employeeObj.MonthlySalaryPrice.HasValue ? employeeObj.MonthlySalaryPrice.Value : 0) / (decimal)totalDaysinMonth, 2);
                                    }
                                    else
                                    {
                                        decimal totalDaysToApply = (decimal)totalDaysinMonth - employeeObj.NoOfFreeLeavePerMonth;
                                        perDayAmount = Math.Round((employeeObj.MonthlySalaryPrice.HasValue ? employeeObj.MonthlySalaryPrice.Value : 0) / totalDaysToApply, 2);
                                    }

                                    objWorkerPayment.CreditAmount = attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? perDayAmount : Math.Round((perDayAmount / 2), 2);
                                }

                                attendanceObject.TodaySalary = objWorkerPayment.CreditAmount.HasValue ? objWorkerPayment.CreditAmount.Value : 0;

                                _db.SaveChanges();
                            }

                        }

                    });
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

    }
}