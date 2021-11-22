using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class ReportController : Controller
    {
        AttendanceSystemEntities _db;
        public ReportController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult WorkerHeadReport(DateTime? startDate = null, DateTime? endDate = null, long? workerHeadId = null)
        {
            WorkerHeadFilterVM workerHeadFilterVM = new WorkerHeadFilterVM();
            long companyId = clsAdminSession.CompanyId;

            if (workerHeadId.HasValue)
            {
                workerHeadFilterVM.WorkerHeadId = workerHeadId.Value;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                workerHeadFilterVM.StartDate = startDate.Value;
                workerHeadFilterVM.EndDate = endDate.Value;
            }
             
            #region Get attendance data with filter

            List<WorkerAttendanceReportVM> attendanceList = (from at in _db.tbl_WorkerAttendance
                                                             join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId

                                                             join siteInfo in _db.tbl_Site on at.MorningSiteId equals siteInfo.SiteId into outerSiteInfo
                                                             from siteInfo in outerSiteInfo.DefaultIfEmpty()

                                                             join workerTypeInfo in _db.tbl_WorkerType on emp.WorkerTypeId equals workerTypeInfo.WorkerTypeId into outerworkerTypeInfo
                                                             from workerTypeInfo in outerworkerTypeInfo.DefaultIfEmpty()

                                                             join workerHeadInfo in _db.tbl_WorkerHead on emp.WorkerHeadId equals workerHeadInfo.WorkerHeadId into outerWorkerHeadInfo
                                                             from workerHeadInfo in outerWorkerHeadInfo.DefaultIfEmpty()

                                                             where emp.CompanyId == companyId && emp.WorkerHeadId.HasValue
                                                             && (workerHeadFilterVM.WorkerHeadId.HasValue ? emp.WorkerHeadId == workerHeadFilterVM.WorkerHeadId.Value : true)
                                                             && DbFunctions.TruncateTime(at.AttendanceDate) >= DbFunctions.TruncateTime(workerHeadFilterVM.StartDate)
                                                             && DbFunctions.TruncateTime(at.AttendanceDate) <= DbFunctions.TruncateTime(workerHeadFilterVM.EndDate)
                                                             select new WorkerAttendanceReportVM
                                                             {
                                                                 AttendanceId = at.WorkerAttendanceId,
                                                                 CompanyId = emp.CompanyId,
                                                                 EmployeeId = emp.EmployeeId,
                                                                 WorkerHeadId = emp.WorkerHeadId,
                                                                 WorkerHeadName = (workerHeadInfo != null ? workerHeadInfo.HeadName : ""),
                                                                 EmployeeCode = emp.EmployeeCode,
                                                                 Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                                 WorkerTypeName = (workerTypeInfo != null ? workerTypeInfo.WorkerTypeName : ""),
                                                                 AttendanceDate = at.AttendanceDate,
                                                                 EmploymentCategory = emp.EmploymentCategory,
                                                                 IsMorning = at.IsMorning,
                                                                 IsAfternoon = at.IsAfternoon,
                                                                 IsEvening = at.IsEvening,
                                                                 ProfilePicture = emp.ProfilePicture,
                                                                 SiteName = (siteInfo != null ? siteInfo.SiteName : ""),
                                                                 SalaryGiven = at.SalaryGiven,
                                                                 TotalTodaySalary = at.TodaySalary,
                                                                 IsClosed = at.IsClosed,
                                                                 MonthlySalary = emp.MonthlySalaryPrice.HasValue ? emp.MonthlySalaryPrice.Value : 0,
                                                                 PerCategoryPrice = emp.PerCategoryPrice,
                                                                 MorningAttendanceBy = PaymentGivenBy.CompanyAdmin.ToString(),
                                                                 MorningAttendanceDate = at.MorningAttendanceDate,
                                                                 MorningLatitude = at.MorningLatitude,
                                                                 MorningLongitude = at.MorningLongitude,
                                                                 MorningLocationFrom = at.MorningLocationFrom,
                                                                 AfternoonAttendanceBy = PaymentGivenBy.CompanyAdmin.ToString(),
                                                                 AfternoonAttendanceDate = at.AfternoonAttendanceDate,
                                                                 AfternoonLatitude = at.AfternoonLatitude,
                                                                 AfternoonLongitude = at.AfternoonLongitude,
                                                                 AfternoonLocationFrom = at.AfternoonLocationFrom,
                                                                 EveningAttendanceBy = PaymentGivenBy.CompanyAdmin.ToString(),
                                                                 EveningAttendanceDate = at.EveningAttendanceDate,
                                                                 EveningLatitude = at.EveningLatitude,
                                                                 EveningLongitude = at.EveningLongitude,
                                                                 EveningLocationFrom = at.EveningLocationFrom,
                                                                 ExtraHours = at.ExtraHours,
                                                                 NoOfHoursWorked = at.NoOfHoursWorked,
                                                                 NoOfUnitWorked = at.NoOfUnitWorked,
                                                             }).OrderByDescending(x => x.AttendanceDate).ToList();

            attendanceList.ForEach(x =>
            {
                x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                x.ProfilePicture = (!string.IsNullOrEmpty(x.ProfilePicture) ? CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty);
                x.IsMorningText = x.IsMorning ? ErrorMessage.YES : ErrorMessage.NO;
                x.IsAfternoonText = x.IsAfternoon ? ErrorMessage.YES : ErrorMessage.NO;
                x.IsEveningText = x.IsEvening ? ErrorMessage.YES : ErrorMessage.NO;
                x.BgColor = attendanceList.Any(z => z.EmployeeId == x.EmployeeId && z.AttendanceDate == x.AttendanceDate && z.AttendanceId != x.AttendanceId) ? ErrorMessage.Red : string.Empty;

                decimal totalDaysinMonth = DateTime.DaysInMonth(x.AttendanceDate.Year, x.AttendanceDate.Month);
                if (x.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                {
                    if (x.IsClosed)
                    {
                        x.ActTodaySalary = (x.IsMorning && x.IsAfternoon && x.IsEvening ? (x.PerCategoryPrice) : (x.PerCategoryPrice / 2));
                        x.CalcTodaySalary = 0;
                    }
                    else
                    {
                        x.CalcTodaySalary = (x.IsMorning && x.IsAfternoon && x.IsEvening ? (x.PerCategoryPrice) : (x.PerCategoryPrice / 2));
                        x.ActTodaySalary = 0;
                    }
                }
                else if (x.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                {
                    if (x.IsClosed)
                    {
                        x.ActTodaySalary = x.TotalTodaySalary;
                        x.CalcTodaySalary = 0;
                    }
                    else
                    {
                        x.CalcTodaySalary = 0;
                        x.ActTodaySalary = 0;
                    }
                }
                else if (x.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                {
                    if (x.IsClosed)
                    {
                        x.ActTodaySalary = x.TotalTodaySalary;
                        x.CalcTodaySalary = 0;
                    }
                    else
                    {
                        x.CalcTodaySalary = 0;
                        x.ActTodaySalary = 0;
                    }
                }
                else if (x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                {
                    decimal perDayAmount = Math.Round(x.MonthlySalary / totalDaysinMonth, 2);
                    if (x.IsClosed)
                    {
                        x.ActTodaySalary = (x.IsMorning && x.IsAfternoon && x.IsEvening ? (perDayAmount) : (perDayAmount / 2));
                        x.CalcTodaySalary = 0;
                    }
                    else
                    {
                        x.CalcTodaySalary = (x.IsMorning && x.IsAfternoon && x.IsEvening ? (perDayAmount) : (perDayAmount / 2));
                        x.ActTodaySalary = 0;
                    }
                }

            });

            #endregion

            workerHeadFilterVM.WorkerHeadReportList = attendanceList
                        .GroupBy(x => new { x.WorkerHeadId, x.AttendanceDate })
                        .Select((cl) => new WorkerHeadReportVM
                        {
                            WorkerHeadId = cl.FirstOrDefault().WorkerHeadId.Value,
                            HeadName = cl.FirstOrDefault().WorkerHeadName,
                            PaymentDate = cl.FirstOrDefault().AttendanceDate,
                            TodaySalary = cl.Sum(c => c.TotalTodaySalary),
                            AmountGiven = cl.Sum(c => c.SalaryGiven),
                            ActSalary = cl.Sum(c => c.ActTodaySalary),
                        }).OrderBy(x => x.HeadName).ThenBy(x => x.PaymentDate).ToList();

            int counter = 1;
            long? tempWorkerHeadId = null;
            workerHeadFilterVM.WorkerHeadReportList.ForEach(x =>
            {
                if (tempWorkerHeadId != null && tempWorkerHeadId.Value != x.WorkerHeadId)
                {
                    counter = 1;
                }

                tempWorkerHeadId = x.WorkerHeadId;

                x.RowNumber = counter;
                counter++;
            });

            workerHeadFilterVM.WorkerHeadList = GetWorkerHeadList();

            return View(workerHeadFilterVM);
        }

        public ActionResult SalesReport(DateTime? startDate = null, DateTime? endDate = null, long? companyId = null, int? reportType = null)
        {
            SalesReportFilterVM salesReportFilterVM = new SalesReportFilterVM();
            if (companyId.HasValue)
            {
                salesReportFilterVM.CompanyId = companyId.Value;
            }

            if (reportType.HasValue)
            {
                salesReportFilterVM.ReportType = reportType.Value;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                salesReportFilterVM.StartDate = startDate.Value;
                salesReportFilterVM.EndDate = endDate.Value;
            }


            List<SalesReportVM> AccountSalesReportList = new List<SalesReportVM>();
            List<SalesReportVM> SMSSalesReportList = new List<SalesReportVM>();
            List<SalesReportVM> EmployeeSalesReportList = new List<SalesReportVM>();

            if (salesReportFilterVM.ReportType == 0 || salesReportFilterVM.ReportType == (int)SalesReportType.Account)
            {
                AccountSalesReportList = (from apkg in _db.tbl_CompanyRenewPayment
                                          join cmp in _db.tbl_Company on apkg.CompanyId equals cmp.CompanyId
                                          where DbFunctions.TruncateTime(apkg.CreatedDate) >= DbFunctions.TruncateTime(salesReportFilterVM.StartDate)
                                          && DbFunctions.TruncateTime(apkg.CreatedDate) <= DbFunctions.TruncateTime(salesReportFilterVM.EndDate)
                                          && (salesReportFilterVM.CompanyId.HasValue ? apkg.CompanyId == salesReportFilterVM.CompanyId.Value : true)
                                          select new SalesReportVM
                                          {
                                              BuyDate = apkg.CreatedDate,
                                              CompanyId = apkg.CompanyId,
                                              CompanyName = cmp.CompanyName,
                                              PackageType = SalesReportType.Account.ToString(),
                                              PackageName = apkg.PackageName,
                                              PackageAmount = 0,
                                              GST = apkg.GSTPer,
                                              GSTAmount = 0,
                                              FinalAmount = apkg.Amount,
                                              TotalFreeEmployee = apkg.NoOfEmployee,
                                              FreeAccessDays = apkg.AccessDays,
                                              TotalFreeSMS = apkg.NoOfSMS
                                          }).ToList();

                AccountSalesReportList.ForEach(x =>
                {
                    x.PackageAmount = Math.Round((x.FinalAmount / (100 + x.GST)) * 100, 2);
                    x.GSTAmount = Math.Round(x.PackageAmount * x.GST / 100, 2);
                });
            }

            if (salesReportFilterVM.ReportType == 0 || salesReportFilterVM.ReportType == (int)SalesReportType.SMS)
            {
                SMSSalesReportList = (from apkg in _db.tbl_CompanySMSPackRenew
                                      join cmp in _db.tbl_Company on apkg.CompanyId equals cmp.CompanyId
                                      where DbFunctions.TruncateTime(apkg.CreatedDate) >= DbFunctions.TruncateTime(salesReportFilterVM.StartDate)
                                      && DbFunctions.TruncateTime(apkg.CreatedDate) <= DbFunctions.TruncateTime(salesReportFilterVM.EndDate)
                                      && (salesReportFilterVM.CompanyId.HasValue ? apkg.CompanyId == salesReportFilterVM.CompanyId.Value : true)
                                      select new SalesReportVM
                                      {
                                          BuyDate = apkg.CreatedDate,
                                          CompanyId = apkg.CompanyId,
                                          CompanyName = cmp.CompanyName,
                                          PackageType = SalesReportType.SMS.ToString(),
                                          PackageName = apkg.SMSPackageName,
                                          PackageAmount = apkg.PackageAmount,
                                          GST = apkg.GSTPer.HasValue ? apkg.GSTPer.Value : 0,
                                          GSTAmount = 0,
                                          FinalAmount = apkg.PackageAmount,
                                          TotalFreeEmployee = 0,
                                          FreeAccessDays = apkg.AccessDays,
                                          TotalFreeSMS = (long)apkg.NoOfSMS
                                      }).ToList();

                SMSSalesReportList.ForEach(x =>
                {
                    x.PackageAmount = Math.Round((x.FinalAmount / (100 + x.GST)) * 100, 2);
                    x.GSTAmount = Math.Round(x.PackageAmount * x.GST / 100, 2);
                });
            }

            if (salesReportFilterVM.ReportType == 0 || salesReportFilterVM.ReportType == (int)SalesReportType.Employee)
            {
                EmployeeSalesReportList = (from apkg in _db.tbl_EmployeeBuyTransaction
                                           join cmp in _db.tbl_Company on apkg.CompanyId equals cmp.CompanyId
                                           where DbFunctions.TruncateTime(apkg.CreatedDate) >= DbFunctions.TruncateTime(salesReportFilterVM.StartDate)
                                           && DbFunctions.TruncateTime(apkg.CreatedDate) <= DbFunctions.TruncateTime(salesReportFilterVM.EndDate)
                                           && (salesReportFilterVM.CompanyId.HasValue ? apkg.CompanyId == salesReportFilterVM.CompanyId.Value : true)
                                           select new SalesReportVM
                                           {
                                               BuyDate = apkg.CreatedDate,
                                               CompanyId = apkg.CompanyId,
                                               CompanyName = cmp.CompanyName,
                                               PackageType = SalesReportType.Employee.ToString(),
                                               PackageName = string.Empty,
                                               PackageAmount = apkg.TotalPaidAmount,
                                               GST = apkg.GSTPer.HasValue ? apkg.GSTPer.Value : 0,
                                               GSTAmount = 0,
                                               FinalAmount = apkg.TotalPaidAmount,
                                               TotalFreeEmployee = apkg.NoOfEmpToBuy,
                                               FreeAccessDays = 0,
                                               TotalFreeSMS = 0
                                           }).ToList();

                EmployeeSalesReportList.ForEach(x =>
                {
                    x.PackageAmount = Math.Round((x.FinalAmount / (100 + x.GST)) * 100, 2);
                    x.GSTAmount = Math.Round(x.PackageAmount * x.GST / 100, 2);
                });

            }
            salesReportFilterVM.SalesReportList = AccountSalesReportList.Union(SMSSalesReportList).Union(EmployeeSalesReportList).OrderBy(x => x.BuyDate).ToList();
            salesReportFilterVM.CompanyList = GetCompanyList();
            salesReportFilterVM.ReportTypeList = GetReportTypeList();
            return View(salesReportFilterVM);
        }

        private List<SelectListItem> GetReportTypeList()
        {
            string[] reportTypeArr = Enum.GetNames(typeof(SalesReportType));
            var listreportType = reportTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listreportType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetCompanyList()
        {
            List<SelectListItem> lst = (from cmp in _db.tbl_Company
                                        orderby cmp.CompanyId
                                        select new SelectListItem
                                        {
                                            Text = cmp.CompanyName + " (" + cmp.CompanyCode + ")",
                                            Value = cmp.CompanyId.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetWorkerHeadList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from w in _db.tbl_WorkerHead
                                        where w.CompanyId == companyId
                                        orderby w.HeadName
                                        select new SelectListItem
                                        {
                                            Text = w.HeadName,
                                            Value = w.WorkerHeadId.ToString()
                                        }).ToList();
            return lst;
        }

    }
}