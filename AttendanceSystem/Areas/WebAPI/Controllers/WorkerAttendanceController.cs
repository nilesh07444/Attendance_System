using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
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
                if (workerAttendanceRequestVM.SiteId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteRequired);
                }
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
                            attendanceObject.EveningSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.EveningLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.EveningLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.EveningLocationFrom = workerAttendanceRequestVM.LocationFrom;

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
                            attendanceObject.AfternoonSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.AfternoonLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.AfternoonLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.AfternoonLocationFrom = workerAttendanceRequestVM.LocationFrom;
                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                        {
                            attendanceObject.IsMorning = true;
                            attendanceObject.MorningAttendanceBy = employeeId;
                            attendanceObject.MorningAttendanceDate = DateTime.UtcNow;
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
                            attendanceObject.AfternoonAttendanceDate = DateTime.UtcNow;
                            attendanceObject.AfternoonSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.AfternoonLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.AfternoonLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.AfternoonLocationFrom = workerAttendanceRequestVM.LocationFrom;
                        }
                        else if (workerAttendanceRequestVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                        {
                            attendanceObject.IsMorning = true;
                            attendanceObject.MorningAttendanceBy = employeeId;
                            attendanceObject.MorningAttendanceDate = DateTime.UtcNow;
                            attendanceObject.MorningSiteId = workerAttendanceRequestVM.SiteId;
                            attendanceObject.MorningLatitude = workerAttendanceRequestVM.Latitude;
                            attendanceObject.MorningLongitude = workerAttendanceRequestVM.Longitude;
                            attendanceObject.MorningLocationFrom = workerAttendanceRequestVM.LocationFrom;
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
                        objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                        objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                        //objEmployeePayment.Status=
                        //objEmployeePayment.ProcessStatusText
                        objWorkerPayment.CreatedDate = DateTime.UtcNow;
                        objWorkerPayment.CreatedBy = employeeId;
                        objWorkerPayment.ModifiedDate = DateTime.UtcNow;
                        objWorkerPayment.ModifiedBy = employeeId;

                        if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                        {
                            objWorkerPayment.CreditAmount = (attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (employeeObj.PerCategoryPrice) : (employeeObj.PerCategoryPrice / 2)) + (employeeObj.ExtraPerHourPrice * workerAttendanceRequestVM.ExtraHours);
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


                        if (workerAttendanceRequestVM.TodaySalary.HasValue && workerAttendanceRequestVM.TodaySalary.Value > 0 && employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                        {
                            tbl_WorkerPayment objWorkerPaymentDebit = new tbl_WorkerPayment();
                            objWorkerPaymentDebit.CompanyId = companyId;
                            objWorkerPaymentDebit.UserId = attendanceObject.EmployeeId;
                            objWorkerPaymentDebit.AttendanceId = attendanceObject.WorkerAttendanceId;
                            objWorkerPaymentDebit.PaymentDate = attendanceObject.AttendanceDate;
                            objWorkerPaymentDebit.PaymentType = (int)EmployeePaymentType.Salary;
                            objWorkerPaymentDebit.CreditOrDebitText = ErrorMessage.Debit;
                            objWorkerPaymentDebit.DebitAmount = workerAttendanceRequestVM.TodaySalary.Value;
                            objWorkerPaymentDebit.Remarks = ErrorMessage.AutoCreditOnEveningAttendance;
                            objWorkerPaymentDebit.Month = attendanceObject.AttendanceDate.Month;
                            objWorkerPaymentDebit.Year = attendanceObject.AttendanceDate.Year;
                            objWorkerPaymentDebit.CreatedDate = DateTime.UtcNow;
                            objWorkerPaymentDebit.CreatedBy = employeeId;
                            objWorkerPaymentDebit.ModifiedDate = DateTime.UtcNow;
                            objWorkerPaymentDebit.ModifiedBy = employeeId;
                            objWorkerPaymentDebit.CreditAmount = 0;

                            _db.tbl_WorkerPayment.Add(objWorkerPayment);
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
                                                               Name = emp.FirstName + " " + emp.LastName,
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = (at.IsMorning && at.MorningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsAfternoon = (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsEvening = (at.IsEvening && at.EveningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
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
                                                           && (at.MorningSiteId == workerAttendanceReportFilterVM.SiteId
                                                           || at.AfternoonSiteId == workerAttendanceReportFilterVM.SiteId
                                                           || at.EveningSiteId == workerAttendanceReportFilterVM.SiteId)
                                                           && (workerAttendanceReportFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceReportFilterVM.EmployeeId.Value : true)
                                                           select new WorkerAttendanceVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               Name = emp.FirstName + " " + emp.LastName,
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
                                                               Name = emp.FirstName + " " + emp.LastName,
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
    }
}
