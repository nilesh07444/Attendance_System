using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class WorkerAttendanceController : Controller
    {

        long companyId;
        AttendanceSystemEntities _db;
        public WorkerAttendanceController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
        }
        public ActionResult Index(DateTime? startDate, DateTime? endDate, int? siteId, long? employeeId, long? employmentCategory, long? workerHeadId = null)
        {
            WorkerAttendanceReportListFilterVM workerAttendanceFilterVM = new WorkerAttendanceReportListFilterVM();
            try
            {
                companyId = clsAdminSession.CompanyId;
                if (startDate.HasValue && endDate.HasValue)
                {
                    workerAttendanceFilterVM.StartDate = startDate.Value;
                    workerAttendanceFilterVM.EndDate = endDate.Value;
                }
                if (siteId.HasValue)
                {
                    workerAttendanceFilterVM.SiteId = siteId.Value;
                }

                if (employeeId.HasValue)
                {
                    workerAttendanceFilterVM.EmployeeId = employeeId.Value;
                }

                if (employmentCategory.HasValue)
                {
                    workerAttendanceFilterVM.EmploymentCategory = employmentCategory.Value;
                }

                if (workerHeadId.HasValue)
                {
                    workerAttendanceFilterVM.WorkerHeadId = workerHeadId.Value;
                }

                workerAttendanceFilterVM.AttendanceList = (from at in _db.tbl_WorkerAttendance
                                                           join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId

                                                           join siteInfo in _db.tbl_Site on at.MorningSiteId equals siteInfo.SiteId into outerSiteInfo
                                                           from siteInfo in outerSiteInfo.DefaultIfEmpty()

                                                           join workerTypeInfo in _db.tbl_WorkerType on emp.WorkerTypeId equals workerTypeInfo.WorkerTypeId into outerworkerTypeInfo
                                                           from workerTypeInfo in outerworkerTypeInfo.DefaultIfEmpty()

                                                           join whead in _db.tbl_WorkerHead on emp.WorkerHeadId equals whead.WorkerHeadId into outerWorkerHead
                                                           from whead in outerWorkerHead.DefaultIfEmpty()

                                                           where emp.CompanyId == companyId
                                                           && DbFunctions.TruncateTime(at.AttendanceDate) >= DbFunctions.TruncateTime(workerAttendanceFilterVM.StartDate)
                                                           && DbFunctions.TruncateTime(at.AttendanceDate) <= DbFunctions.TruncateTime(workerAttendanceFilterVM.EndDate)
                                                           && (workerAttendanceFilterVM.SiteId > 0 ? (at.MorningSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.AfternoonSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.EveningSiteId == workerAttendanceFilterVM.SiteId) : true)
                                                           && (workerAttendanceFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceFilterVM.EmployeeId.Value : true)
                                                           && (workerAttendanceFilterVM.EmploymentCategory.HasValue ? emp.EmploymentCategory == workerAttendanceFilterVM.EmploymentCategory.Value : true)
                                                           && (workerAttendanceFilterVM.WorkerHeadId.HasValue ? emp.WorkerHeadId == workerAttendanceFilterVM.WorkerHeadId.Value : true)
                                                           select new WorkerAttendanceReportVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               EmployeeCode = emp.EmployeeCode,
                                                               Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                               WorkerTypeName = (workerTypeInfo != null ? workerTypeInfo.WorkerTypeName : ""),
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = (workerAttendanceFilterVM.SiteId == 0 && at.IsMorning ? true : (at.IsMorning && at.MorningSiteId == workerAttendanceFilterVM.SiteId ? true : false)),
                                                               IsAfternoon = (workerAttendanceFilterVM.SiteId == 0 && at.IsAfternoon ? true : (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceFilterVM.SiteId ? true : false)),
                                                               IsEvening = (workerAttendanceFilterVM.SiteId == 0 && at.IsEvening ? true : (at.IsEvening && at.EveningSiteId == workerAttendanceFilterVM.SiteId ? true : false)),
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
                                                               WorkerHeadId = emp.WorkerHeadId,
                                                               WorkerHeadName = whead != null ? whead.HeadName : string.Empty,
                                                           }).OrderByDescending(x => x.AttendanceDate).ToList();

                workerAttendanceFilterVM.AttendanceList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                    x.ProfilePicture = (!string.IsNullOrEmpty(x.ProfilePicture) ? CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty);
                    x.IsMorningText = x.IsMorning ? ErrorMessage.YES : ErrorMessage.NO;
                    x.IsAfternoonText = x.IsAfternoon ? ErrorMessage.YES : ErrorMessage.NO;
                    x.IsEveningText = x.IsEvening ? ErrorMessage.YES : ErrorMessage.NO;
                    x.BgColor = workerAttendanceFilterVM.AttendanceList.Any(z => z.EmployeeId == x.EmployeeId && z.AttendanceDate == x.AttendanceDate && z.AttendanceId != x.AttendanceId) ? ErrorMessage.Red : string.Empty;

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
                workerAttendanceFilterVM.EmployeeList = GetWorkerList();
                workerAttendanceFilterVM.SiteList = GetSiteList();
                workerAttendanceFilterVM.EmploymentCategoryList = GetEmploymentCategoryList();
                workerAttendanceFilterVM.WorkerHeadList = GetWorkerHeadList();
            }
            catch (Exception ex)
            {
            }
            return View(workerAttendanceFilterVM);
        }

        private List<SelectListItem> GetSiteList()
        {
            companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from st in _db.tbl_Site
                                        where !st.IsDeleted && st.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = st.SiteName,
                                            Value = st.SiteId.ToString()
                                        }).ToList();
            return lst;
        }

        [HttpPost]
        public JsonResult SearchWorker(string searchText)
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId

                                        select new SelectListItem
                                        {
                                            Text = emp.Prefix + " " + emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();

            return Json(lst.Select(c => new { Name = c.Text, ID = c.Value }).ToList(), JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetWorkerList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        && emp.AdminRoleId == (int)AdminRoles.Worker
                                        select new SelectListItem
                                        {
                                            Text = emp.Prefix + " " + emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

        public ActionResult AssignedWorkerList(int? siteId, DateTime? date, long? employeeId)
        {
            AssignedWorkerFilterVM assignedWorkerFilterVM = new AssignedWorkerFilterVM();
            try
            {
                companyId = clsAdminSession.CompanyId;

                if (siteId.HasValue && siteId.Value > 0)
                {
                    assignedWorkerFilterVM.SiteId = siteId.Value;
                }

                if (date.HasValue)
                {
                    assignedWorkerFilterVM.Date = date.Value;
                }

                if (employeeId.HasValue)
                {
                    assignedWorkerFilterVM.EmployeeId = employeeId.Value;
                }

                var companyIdParam = new SqlParameter
                {
                    ParameterName = "CompanyId",
                    Value = companyId
                };

                var roleIdParam = new SqlParameter
                {
                    ParameterName = "RoleId",
                    Value = (int)AdminRoles.Worker
                };

                var dateParam = new SqlParameter
                {
                    ParameterName = "Date",
                    Value = assignedWorkerFilterVM.Date
                };

                var siteIdParam = new SqlParameter()
                {
                    ParameterName = "SiteId"
                };
                if (assignedWorkerFilterVM.SiteId.HasValue)
                {
                    siteIdParam.Value = assignedWorkerFilterVM.SiteId.Value;
                }
                else
                {
                    siteIdParam.Value = DBNull.Value;
                }

                var employeeIdParam = new SqlParameter
                {
                    ParameterName = "EmployeeId"
                };

                if (assignedWorkerFilterVM.EmployeeId.HasValue)
                {
                    employeeIdParam.Value = assignedWorkerFilterVM.EmployeeId.Value;
                }
                else
                {
                    employeeIdParam.Value = DBNull.Value;
                }

                assignedWorkerFilterVM.AssignedWorkerList = _db.Database.SqlQuery<AssignedWorkerVM>("exec Usp_GetAssignedWorkerListWithAttendance @CompanyId,@RoleId,@Date,@SiteId,@EmployeeId", companyIdParam, roleIdParam, dateParam, siteIdParam, employeeIdParam).ToList<AssignedWorkerVM>();

                assignedWorkerFilterVM.AssignedWorkerList.ForEach(x =>
                {
                    x.IsMorningText = x.IsMorning ? ErrorMessage.YES : ErrorMessage.NO;
                    x.IsAfternoonText = x.IsAfternoon ? ErrorMessage.YES : ErrorMessage.NO;
                    x.IsEveningText = x.IsEvening ? ErrorMessage.YES : ErrorMessage.NO;
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);

                });
                assignedWorkerFilterVM.EmployeeList = GetWorkerList();
                assignedWorkerFilterVM.SiteList = GetSiteList();
                assignedWorkerFilterVM.AttendanceTypeList = GetAttendanceTypeList();

            }
            catch (Exception ex)
            {

            }

            return View(assignedWorkerFilterVM);
        }

        public ActionResult Add(int siteId, long employeeId, string dateStr, long? workerAttendanceId)
        {
            string[] formats = { "MM/dd/yyyy", "dd/MM/yyyy", "MM-dd-yyyy", "dd-MM-yyyy" };
            DateTime date = DateTime.ParseExact(dateStr, formats, new CultureInfo("en-US"), DateTimeStyles.None);

            //DateTime date = Convert.ToDateTime(dateStr, CultureInfo.InvariantCulture);// CommonMethod.CurrentIndianDateTime().Date;
            //DateTime date = DateTime.Parse(dateStr, CultureInfo.CreateSpecificCulture("en-US"));

            DateTime today = CommonMethod.CurrentIndianDateTime().Date;
            int currMonth = today.Month;
            int currYear = today.Year;
            AddWorkerAttendanceVM addWorkerAttendanceVM = null;
            try
            {
                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == clsAdminSession.CompanyId).FirstOrDefault();

                addWorkerAttendanceVM = (from at in _db.tbl_WorkerAttendance
                                         join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                         where at.EmployeeId == employeeId
                                         && !at.IsClosed
                                         && (at.MorningSiteId == siteId || at.AfternoonSiteId == siteId || at.EveningSiteId == siteId)
                                         && (workerAttendanceId.HasValue && workerAttendanceId.Value > 0 ? at.WorkerAttendanceId == workerAttendanceId.Value : at.AttendanceDate == date)
                                         select new AddWorkerAttendanceVM
                                         {
                                             AttendanceId = at.WorkerAttendanceId,
                                             AttendanceDate = at.AttendanceDate,
                                             EmployeeId = at.EmployeeId,
                                             SiteId = siteId,
                                             EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                             EmployeeCode = emp.EmployeeCode,
                                             EmploymentCategoryId = emp.EmploymentCategory,
                                             MonthlySalary = emp.MonthlySalaryPrice,
                                             PerCategoryPrice = emp.PerCategoryPrice,
                                             ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                             IsMorning = at.IsMorning,
                                             IsAfternoon = at.IsAfternoon,
                                             IsEvening = at.IsEvening,
                                             NoOfFreeLeavePerMonth = emp.NoOfFreeLeavePerMonth
                                         }).FirstOrDefault();



                if (addWorkerAttendanceVM == null)
                {
                    addWorkerAttendanceVM = new AddWorkerAttendanceVM();
                    addWorkerAttendanceVM.SiteId = siteId;
                    addWorkerAttendanceVM.AttendanceDate = date;
                    addWorkerAttendanceVM.EmployeeId = employeeId;
                    addWorkerAttendanceVM.IsMorning = false;
                    addWorkerAttendanceVM.IsAfternoon = false;
                    addWorkerAttendanceVM.IsEvening = false;
                    tbl_Employee objEmloyee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                    addWorkerAttendanceVM.EmployeeCode = objEmloyee.EmployeeCode;
                    addWorkerAttendanceVM.EmployeeName = objEmloyee.FirstName + " " + objEmloyee.LastName;
                    addWorkerAttendanceVM.EmploymentCategoryId = objEmloyee.EmploymentCategory;
                    addWorkerAttendanceVM.MonthlySalary = objEmloyee.MonthlySalaryPrice;
                    addWorkerAttendanceVM.PerCategoryPrice = objEmloyee.PerCategoryPrice;
                    addWorkerAttendanceVM.ExtraPerHourPrice = objEmloyee.ExtraPerHourPrice;
                }
                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.MonthlyBased)
                {
                    decimal totalDaysinMonth = DateTime.DaysInMonth(addWorkerAttendanceVM.AttendanceDate.Year, addWorkerAttendanceVM.AttendanceDate.Month);
                    //decimal perDaySalary = Math.Round(addWorkerAttendanceVM.MonthlySalary.Value / totalDaysinMonth, 2);

                    decimal perDaySalary = 0;
                    if (objCompany.CompanyConversionType == (int)CompanyConversionType.MonthBased)
                    {
                        perDaySalary = Math.Round((addWorkerAttendanceVM.MonthlySalary.HasValue ? addWorkerAttendanceVM.MonthlySalary.Value : 0) / (decimal)totalDaysinMonth, 2);
                    }
                    else
                    {
                        decimal totalDaysToApply = (decimal)totalDaysinMonth - addWorkerAttendanceVM.NoOfFreeLeavePerMonth;
                        perDaySalary = Math.Round((addWorkerAttendanceVM.MonthlySalary.HasValue ? addWorkerAttendanceVM.MonthlySalary.Value : 0) / totalDaysToApply, 2);
                    }

                    addWorkerAttendanceVM.PerCategoryPrice = perDaySalary;
                }
                addWorkerAttendanceVM.SiteName = _db.tbl_Site.Where(x => x.SiteId == siteId).Select(x => x.SiteName).FirstOrDefault();

                addWorkerAttendanceVM.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)addWorkerAttendanceVM.EmploymentCategoryId);

                //addWorkerAttendanceVM.PendingSalary = _db.tbl_WorkerPayment.Any(x => x.UserId == employeeId && !x.IsDeleted && x.Month == currMonth && x.Year == currYear && x.PaymentType != (int)EmployeePaymentType.Extra) ? _db.tbl_WorkerPayment.Where(x => x.UserId == employeeId && !x.IsDeleted && x.Month == currMonth && x.Year == currYear && x.PaymentType != (int)EmployeePaymentType.Extra).Select(x => (x.CreditAmount.HasValue ? x.CreditAmount.Value : 0) - (x.DebitAmount.HasValue ? x.DebitAmount.Value : 0)).Sum() : 0;

                decimal? totalCreditAmount1 = (from e in _db.tbl_WorkerPayment
                                               join att in _db.tbl_WorkerAttendance on e.AttendanceId equals att.WorkerAttendanceId
                                               where e.UserId == employeeId
                                                  && !e.IsDeleted
                                                  && e.Month == currMonth
                                                  && e.Year == currYear
                                                  && e.PaymentType != (int)EmployeePaymentType.Extra
                                                  && att.AttendanceDate != today
                                               select e).ToList().Sum(x => x.CreditAmount);

                decimal? totalCreditAmount2 = (from e in _db.tbl_WorkerPayment
                                               join att in _db.tbl_WorkerAttendance on e.AttendanceId equals att.WorkerAttendanceId
                                               where e.UserId == employeeId
                                                  && !e.IsDeleted
                                                  && e.Month == currMonth
                                                  && e.Year == currYear
                                                  && e.PaymentType != (int)EmployeePaymentType.Extra
                                                  && att.IsClosed && att.AttendanceDate == today
                                               select e).ToList().Sum(x => x.CreditAmount);

                decimal? totalCreditAmount = totalCreditAmount1 + totalCreditAmount2;

                decimal? totalDebitAmount = _db.tbl_WorkerPayment.Where(x => x.UserId == employeeId
                    && !x.IsDeleted
                    && x.Month == currMonth
                    && x.Year == currYear
                    && x.CreditOrDebitText == "Debit"
                    && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(today)
                    && x.PaymentType != (int)EmployeePaymentType.Extra).ToList().Sum(x => x.DebitAmount);

                addWorkerAttendanceVM.PendingSalary = Convert.ToDecimal(totalCreditAmount) - Convert.ToDecimal(totalDebitAmount);

                addWorkerAttendanceVM.IsMorningText = addWorkerAttendanceVM.IsMorning ? ErrorMessage.YES : ErrorMessage.NO;
                addWorkerAttendanceVM.IsAfternoonText = addWorkerAttendanceVM.IsAfternoon ? ErrorMessage.YES : ErrorMessage.NO;
                addWorkerAttendanceVM.IsEveningText = addWorkerAttendanceVM.IsEvening ? ErrorMessage.YES : ErrorMessage.NO;

                addWorkerAttendanceVM.WorkerAttendanceTypeList = GetAttendanceTypeListFromAttendanceFilled(addWorkerAttendanceVM.IsMorning, addWorkerAttendanceVM.IsAfternoon, addWorkerAttendanceVM.IsEvening);
            }
            catch (Exception ex)
            {
            }
            return View(addWorkerAttendanceVM);
        }

        private List<SelectListItem> GetAttendanceTypeList()
        {
            string[] workerAttendanceTypeArr = Enum.GetNames(typeof(WorkerAttendanceType));
            var listWorkerAttendanceType = workerAttendanceTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listWorkerAttendanceType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetAttendanceTypeListFromAttendanceFilled(bool isMorning, bool isAfternoon, bool isEvening)
        {
            string[] workerAttendanceTypeArr = Enum.GetNames(typeof(WorkerAttendanceType));
            var listWorkerAttendanceType = workerAttendanceTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listWorkerAttendanceType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();


            if (!isMorning && !isAfternoon)
            {
                lst.Remove(lst.First(item => item.Value.Equals(WorkerAttendanceType.Evening.GetHashCode().ToString())));
            }
            else if (isMorning && !isAfternoon)
            {
                lst.Remove(lst.First(item => item.Value.Equals(WorkerAttendanceType.Morning.GetHashCode().ToString())));
                lst.Remove(lst.First(item => item.Value.Equals(WorkerAttendanceType.Evening.GetHashCode().ToString())));
            }
            else if (!isMorning && isAfternoon)
            {
                lst.Remove(lst.First(item => item.Value.Equals(WorkerAttendanceType.Morning.GetHashCode().ToString())));
                lst.Remove(lst.First(item => item.Value.Equals(WorkerAttendanceType.Afternoon.GetHashCode().ToString())));
            }
            else if (isMorning && isAfternoon)
            {
                lst.Remove(lst.First(item => item.Value.Equals(WorkerAttendanceType.Morning.GetHashCode().ToString())));
                lst.Remove(lst.First(item => item.Value.Equals(WorkerAttendanceType.Afternoon.GetHashCode().ToString())));
            }

            return lst;
        }

        [HttpPost]
        public ActionResult Add(AddWorkerAttendanceVM addWorkerAttendanceVM)
        {
            DateTime today = CommonMethod.CurrentIndianDateTime().Date;
            try
            {
                long employeeId = clsAdminSession.UserID;
                tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.Where(x => x.WorkerAttendanceId == addWorkerAttendanceVM.AttendanceId && x.EmployeeId == addWorkerAttendanceVM.EmployeeId && x.AttendanceDate == addWorkerAttendanceVM.AttendanceDate && !x.IsClosed).FirstOrDefault();

                if (ModelState.IsValid)
                {
                    #region Validation

                    if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == addWorkerAttendanceVM.AttendanceDate.Month && (x.IsEmployeeDone || x.IsWorkerDone)))
                    {
                        ModelState.AddModelError("", ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyAttendance);
                    }

                    if (addWorkerAttendanceVM.SiteId == 0)
                    {
                        ModelState.AddModelError("", ErrorMessage.SiteRequired);
                    }
                    if (!_db.tbl_AssignWorker.Any(x => x.EmployeeId == addWorkerAttendanceVM.EmployeeId && x.SiteId == addWorkerAttendanceVM.SiteId && x.Date == addWorkerAttendanceVM.AttendanceDate && !x.IsClosed))
                    {
                        ModelState.AddModelError("", ErrorMessage.WorketNotAssignedToday);
                    }


                    if (addWorkerAttendanceVM.AttendanceType != (int)WorkerAttendanceType.Morning
                        && addWorkerAttendanceVM.AttendanceType != (int)WorkerAttendanceType.Afternoon
                        && addWorkerAttendanceVM.AttendanceType != (int)WorkerAttendanceType.Evening)
                    {
                        ModelState.AddModelError("", ErrorMessage.InValidWorkerAttendanceType);
                    }


                    if (attendanceObject == null && addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                    {
                        ModelState.AddModelError("", ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnMorningOrAfterNoon);
                    }

                    if (attendanceObject != null && !attendanceObject.IsMorning && attendanceObject.IsAfternoon && addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                    {
                        ModelState.AddModelError("", ErrorMessage.CanNotTakeMorningAttendanceAfterAfterNoonAttendance);
                    }

                    if (attendanceObject != null && !attendanceObject.IsMorning && attendanceObject.IsAfternoon && addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                    {
                        ModelState.AddModelError("", ErrorMessage.CanNotTakeMorningAttendanceAfterAfterNoonAttendance);
                    }


                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning
                        && attendanceObject != null && attendanceObject.IsMorning)
                    {
                        ModelState.AddModelError("", ErrorMessage.WorkerMorningAttendanceAlreadyDone);
                    }
                    else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Afternoon
                        && attendanceObject != null && attendanceObject.IsAfternoon)
                    {
                        ModelState.AddModelError("", ErrorMessage.WorkerAfternoonAttendanceAlreadyDone);
                    }
                    else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && attendanceObject != null && attendanceObject.IsEvening)
                    {
                        ModelState.AddModelError("", ErrorMessage.WorkerEveningAttendanceAlreadyDone);
                    }

                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && attendanceObject != null && !attendanceObject.IsAfternoon)
                    {
                        ModelState.AddModelError("", ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnAfterNoon);
                    }


                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.HourlyBased && addWorkerAttendanceVM.NoOfHoursWorked == 0)
                    {
                        ModelState.AddModelError("", ErrorMessage.PleaseProvideNoOfHoursWorked);
                    }

                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.UnitBased && addWorkerAttendanceVM.NoOfUnitWorked == 0)
                    {
                        ModelState.AddModelError("", ErrorMessage.PleaseProvideNoOfUnitWorked);
                    }

                    #endregion Validation

                    if (ModelState.IsValid)
                    {
                        #region Add attendance

                        if (attendanceObject != null)
                        {
                            if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                            {
                                attendanceObject.IsEvening = true;
                                attendanceObject.EveningAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                attendanceObject.EveningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.EveningSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.IsClosed = true;

                                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.DailyBased || addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.MonthlyBased)
                                    attendanceObject.ExtraHours = addWorkerAttendanceVM.ExtraHours;

                                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.HourlyBased)
                                    attendanceObject.NoOfHoursWorked = addWorkerAttendanceVM.NoOfHoursWorked;

                                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.UnitBased)
                                    attendanceObject.NoOfHoursWorked = addWorkerAttendanceVM.NoOfUnitWorked;

                                attendanceObject.SalaryGiven = addWorkerAttendanceVM.SalaryGiven > 0 ? addWorkerAttendanceVM.SalaryGiven : 0;
                                attendanceObject.TodaySalary = (addWorkerAttendanceVM.TodaySalary.HasValue ? addWorkerAttendanceVM.TodaySalary.Value : 0) + addWorkerAttendanceVM.ExtraHoursAmount;

                                tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == addWorkerAttendanceVM.SiteId && x.Date == addWorkerAttendanceVM.AttendanceDate && x.EmployeeId == addWorkerAttendanceVM.EmployeeId && !x.IsClosed).FirstOrDefault();
                                if (assignedWorker != null)
                                {
                                    assignedWorker.IsClosed = true;
                                }

                            }
                            else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                            {
                                attendanceObject.IsAfternoon = true;
                                attendanceObject.AfternoonAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.AfternoonSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.AfternoonLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.AfternoonLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.AfternoonLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }
                            else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                            {
                                attendanceObject.IsMorning = true;
                                attendanceObject.MorningAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.MorningSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.MorningLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.MorningLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.MorningLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }

                            _db.SaveChanges();
                        }
                        else
                        {
                            attendanceObject = new tbl_WorkerAttendance();
                            attendanceObject.EmployeeId = addWorkerAttendanceVM.EmployeeId;
                            attendanceObject.AttendanceDate = addWorkerAttendanceVM.AttendanceDate;
                            if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                            {
                                attendanceObject.IsAfternoon = true;
                                attendanceObject.AfternoonAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.AfternoonSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.AfternoonLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.AfternoonLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.AfternoonLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }
                            else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                            {
                                attendanceObject.IsMorning = true;
                                attendanceObject.MorningAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.MorningSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.MorningLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.MorningLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.MorningLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }
                            _db.tbl_WorkerAttendance.Add(attendanceObject);
                            _db.SaveChanges();
                        }

                        #endregion

                        #region Payment Entry

                        if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening)
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
                            objWorkerPayment.DebitAmount = 0; // addWorkerAttendanceVM.SalaryGiven > 0 ? addWorkerAttendanceVM.SalaryGiven : 0;
                            objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnEveningAttendance;
                            objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                            objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                            objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerPayment.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                            objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerPayment.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                            objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearId();

                            if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.DailyBased)
                            {
                                decimal extraHoursAmount = CommonMethod.getPriceBasedOnHours((double)addWorkerAttendanceVM.ExtraPerHourPrice.Value, (double)addWorkerAttendanceVM.ExtraHours);
                                objWorkerPayment.CreditAmount = (attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (addWorkerAttendanceVM.PerCategoryPrice) : (addWorkerAttendanceVM.PerCategoryPrice / 2)) + extraHoursAmount;
                            }
                            else if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.HourlyBased)
                            {
                                decimal totalAmount = CommonMethod.getPriceBasedOnHours((double)addWorkerAttendanceVM.PerCategoryPrice, (double)addWorkerAttendanceVM.NoOfHoursWorked);
                                objWorkerPayment.CreditAmount = totalAmount;
                            }
                            else if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.UnitBased)
                            {
                                objWorkerPayment.CreditAmount = addWorkerAttendanceVM.PerCategoryPrice * addWorkerAttendanceVM.NoOfUnitWorked;
                            }
                            else if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.MonthlyBased)
                            {
                                decimal extraHoursAmount = CommonMethod.getPriceBasedOnHours((double)addWorkerAttendanceVM.ExtraPerHourPrice.Value, (double)addWorkerAttendanceVM.ExtraHours);
                                //objWorkerPayment.CreditAmount = addWorkerAttendanceVM.TodaySalary + addWorkerAttendanceVM.ExtraHoursAmount;
                                objWorkerPayment.CreditAmount = addWorkerAttendanceVM.TodaySalary + extraHoursAmount;
                            }
                            else
                            {
                                objWorkerPayment.CreditAmount = 0;
                            }

                            _db.SaveChanges();


                            #region Debit entry if entered Salary GIven

                            if (addWorkerAttendanceVM.SalaryGiven > 0)
                            {
                                tbl_WorkerPayment objWorkerDebitPayment = new tbl_WorkerPayment();
                                objWorkerDebitPayment.CompanyId = companyId;
                                objWorkerDebitPayment.UserId = attendanceObject.EmployeeId;
                                objWorkerDebitPayment.PaymentDate = CommonMethod.CurrentIndianDateTime().Date; //paymentVM.PaymentDate;
                                objWorkerDebitPayment.CreditOrDebitText = ErrorMessage.Debit;
                                objWorkerDebitPayment.DebitAmount = addWorkerAttendanceVM.SalaryGiven;
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

                        #endregion

                    }
                    else
                    {
                        addWorkerAttendanceVM.WorkerAttendanceTypeList = GetAttendanceTypeListFromAttendanceFilled(attendanceObject.IsMorning, attendanceObject.IsAfternoon, attendanceObject.IsEvening);
                        return View(addWorkerAttendanceVM);
                    }
                }
                else
                {
                    addWorkerAttendanceVM.WorkerAttendanceTypeList = GetAttendanceTypeListFromAttendanceFilled(attendanceObject.IsMorning, attendanceObject.IsAfternoon, attendanceObject.IsEvening);
                    if (addWorkerAttendanceVM.IsMorning)
                    {
                        addWorkerAttendanceVM.WorkerAttendanceTypeList.Remove(addWorkerAttendanceVM.WorkerAttendanceTypeList.First(item => item.Value.Equals(WorkerAttendanceType.Morning.GetHashCode().ToString())));
                    }

                    if (addWorkerAttendanceVM.IsAfternoon)
                    {
                        addWorkerAttendanceVM.WorkerAttendanceTypeList.Remove(addWorkerAttendanceVM.WorkerAttendanceTypeList.First(item => item.Value.Equals(WorkerAttendanceType.Afternoon.GetHashCode().ToString())));
                    }

                    if (addWorkerAttendanceVM.IsEvening)
                    {
                        addWorkerAttendanceVM.WorkerAttendanceTypeList.Remove(addWorkerAttendanceVM.WorkerAttendanceTypeList.First(item => item.Value.Equals(WorkerAttendanceType.Evening.GetHashCode().ToString())));
                    }
                    return View(addWorkerAttendanceVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return RedirectToAction("AssignedWorkerList", new { siteId = addWorkerAttendanceVM.SiteId, date = today });
        }

        [HttpPost]
        public string AddAttendance(int siteId, int attendanceType, string ids, DateTime date)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == clsAdminSession.CompanyId).FirstOrDefault();

                //long loggedinUser = clsAdminSession.UserID;
                // DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                string[] ids_array = ids.Split(',');
                List<tbl_Employee> employeeList = _db.tbl_Employee.Where(x => ids_array.Contains(x.EmployeeId.ToString())).ToList();
                bool isValid = true;

                if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == date.Month && (x.IsEmployeeDone || x.IsWorkerDone)))
                {
                    ReturnMessage = ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyAttendance;
                    isValid = false;
                }
                else
                {
                    if (employeeList != null)
                    {
                        employeeList.ForEach(emp =>
                        {

                            #region Validation

                            if (siteId == 0)
                            {
                                isValid = false;
                            }

                            if (!_db.tbl_AssignWorker.Any(x => x.EmployeeId == emp.EmployeeId && x.SiteId == siteId && x.Date == date && !x.IsClosed))
                            {
                                isValid = false;
                            }

                            if (attendanceType != (int)WorkerAttendanceType.Morning
                                && attendanceType != (int)WorkerAttendanceType.Afternoon
                                && attendanceType != (int)WorkerAttendanceType.Evening)
                            {
                                isValid = false;
                            }

                            tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.Where(x => x.EmployeeId == emp.EmployeeId && x.AttendanceDate == date && !x.IsClosed).FirstOrDefault();
                            if (attendanceObject == null && attendanceType == (int)WorkerAttendanceType.Evening)
                            {
                                isValid = false;
                            }

                            if (attendanceObject != null && !attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceType == (int)WorkerAttendanceType.Morning)
                            {
                                isValid = false;
                            }

                            if (attendanceType == (int)WorkerAttendanceType.Morning && attendanceObject != null && attendanceObject.IsMorning)
                            {
                                isValid = false;
                            }
                            else if (attendanceType == (int)WorkerAttendanceType.Afternoon && attendanceObject != null && attendanceObject.IsAfternoon)
                            {
                                isValid = false;
                            }
                            else if (attendanceType == (int)WorkerAttendanceType.Evening && attendanceObject != null && attendanceObject.IsEvening)
                            {
                                isValid = false;
                            }

                            if (attendanceType == (int)WorkerAttendanceType.Evening && attendanceObject != null && !attendanceObject.IsAfternoon)
                            {
                                isValid = false;
                            }

                            if (attendanceType == (int)WorkerAttendanceType.Evening && emp.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                            {
                                isValid = false;
                                ReturnMessage = ErrorMessage.HourlyBasedWorkerNotAllowedWithoutTotalHours;
                            }

                            if (attendanceType == (int)WorkerAttendanceType.Evening && emp.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                            {
                                isValid = false;
                                ReturnMessage = ErrorMessage.UnitBasedWorkerNotAllowedWithoutTotalUnits;
                            }

                            #endregion Validation

                            if (isValid)
                            {
                                if (attendanceObject != null)
                                {
                                    if (attendanceType == (int)WorkerAttendanceType.Evening)
                                    {
                                        attendanceObject.IsEvening = true;
                                        attendanceObject.EveningAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                        attendanceObject.EveningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                        attendanceObject.EveningSiteId = siteId;
                                        attendanceObject.IsClosed = true;
                                        if (emp.EmploymentCategory == (int)EmploymentCategory.DailyBased || emp.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                                            attendanceObject.ExtraHours = 0;

                                        tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == siteId && x.Date == date && x.EmployeeId == emp.EmployeeId && !x.IsClosed).FirstOrDefault();
                                        if (assignedWorker != null)
                                        {
                                            assignedWorker.IsClosed = true;
                                        }

                                    }
                                    else if (attendanceType == (int)WorkerAttendanceType.Afternoon)
                                    {
                                        attendanceObject.IsAfternoon = true;
                                        attendanceObject.AfternoonAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                        attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                        attendanceObject.AfternoonSiteId = siteId;
                                    }
                                    else if (attendanceType == (int)WorkerAttendanceType.Morning)
                                    {
                                        attendanceObject.IsMorning = true;
                                        attendanceObject.MorningAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                        attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                        attendanceObject.MorningSiteId = siteId;
                                    }

                                    _db.SaveChanges();
                                }
                                else
                                {
                                    attendanceObject = new tbl_WorkerAttendance();
                                    attendanceObject.EmployeeId = emp.EmployeeId;
                                    attendanceObject.AttendanceDate = date;
                                    if (attendanceType == (int)WorkerAttendanceType.Afternoon)
                                    {
                                        attendanceObject.IsAfternoon = true;
                                        attendanceObject.AfternoonAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                        attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                        attendanceObject.AfternoonSiteId = siteId;
                                    }
                                    else if (attendanceType == (int)WorkerAttendanceType.Morning)
                                    {
                                        attendanceObject.IsMorning = true;
                                        attendanceObject.MorningAttendanceBy = (int)PaymentGivenBy.CompanyAdmin;
                                        attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                        attendanceObject.MorningSiteId = siteId;
                                    }
                                    _db.tbl_WorkerAttendance.Add(attendanceObject);
                                    _db.SaveChanges();
                                }

                                #region Payment Entry

                                if (attendanceType == (int)WorkerAttendanceType.Evening)
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
                                    objWorkerPayment.DebitAmount = 0;
                                    objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnEveningAttendance;
                                    objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                                    objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                                    objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    objWorkerPayment.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                                    objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                    objWorkerPayment.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;

                                    if (emp.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                                    {
                                        objWorkerPayment.CreditAmount = (attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (emp.PerCategoryPrice) : (emp.PerCategoryPrice / 2));
                                    }
                                    else if (emp.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                                    {
                                        decimal totalDaysinMonth = DateTime.DaysInMonth(attendanceObject.AttendanceDate.Year, attendanceObject.AttendanceDate.Month);

                                        decimal perDayAmount = 0;
                                        if (objCompany.CompanyConversionType == (int)CompanyConversionType.MonthBased)
                                        {
                                            perDayAmount = Math.Round((emp.MonthlySalaryPrice.HasValue ? emp.MonthlySalaryPrice.Value : 0) / (decimal)totalDaysinMonth, 2);
                                        }
                                        else
                                        {
                                            decimal totalDaysToApply = (decimal)totalDaysinMonth - emp.NoOfFreeLeavePerMonth;
                                            perDayAmount = Math.Round((emp.MonthlySalaryPrice.HasValue ? emp.MonthlySalaryPrice.Value : 0) / totalDaysToApply, 2);
                                        }

                                        objWorkerPayment.CreditAmount = Math.Round((attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (perDayAmount) : (perDayAmount / 2)), 2);
                                    }
                                    objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearId();

                                    attendanceObject.TodaySalary = objWorkerPayment.CreditAmount.HasValue ? objWorkerPayment.CreditAmount.Value : 0;

                                    _db.SaveChanges();

                                }
                                else if (attendanceType == (int)WorkerAttendanceType.Afternoon && attendanceObject.IsMorning && attendanceObject.IsAfternoon)
                                {
                                    if (emp.EmploymentCategory == (int)EmploymentCategory.DailyBased || emp.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
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
                                        objWorkerPayment.DebitAmount = 0; // workerAttendanceRequestVM.TodaySalary.HasValue && emp.EmploymentCategory == (int)EmploymentCategory.DailyBased ? workerAttendanceRequestVM.TodaySalary.Value : 0;
                                        objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnAfternoonAttendance;
                                        objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                                        objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                                        objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                        objWorkerPayment.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                                        objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                        objWorkerPayment.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                                        objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearIdFromDate(attendanceObject.AttendanceDate);

                                        if (emp.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                                        {
                                            objWorkerPayment.CreditAmount = attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (emp.PerCategoryPrice) : (emp.PerCategoryPrice / 2);
                                        }
                                        else if (emp.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                                        {
                                            decimal totalDaysinMonth = DateTime.DaysInMonth(attendanceObject.AttendanceDate.Year, attendanceObject.AttendanceDate.Month);
                                            decimal perDayAmount = 0;
                                            if (objCompany.CompanyConversionType == (int)CompanyConversionType.MonthBased)
                                            {
                                                perDayAmount = Math.Round((emp.MonthlySalaryPrice.HasValue ? emp.MonthlySalaryPrice.Value : 0) / (decimal)totalDaysinMonth, 2);
                                            }
                                            else
                                            {
                                                decimal totalDaysToApply = (decimal)totalDaysinMonth - emp.NoOfFreeLeavePerMonth;
                                                perDayAmount = Math.Round((emp.MonthlySalaryPrice.HasValue ? emp.MonthlySalaryPrice.Value : 0) / totalDaysToApply, 2);
                                            }

                                            objWorkerPayment.CreditAmount = attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? perDayAmount : Math.Round((perDayAmount / 2), 2);
                                        }

                                        attendanceObject.TodaySalary = objWorkerPayment.CreditAmount.HasValue ? objWorkerPayment.CreditAmount.Value : 0;

                                        _db.SaveChanges();

                                    }
                                }

                                #endregion 
                            }


                        });

                        _db.SaveChanges();
                        ReturnMessage = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult View(long id)
        {
            WorkerAttendanceViewVM workerAttendanceViewVM = new WorkerAttendanceViewVM();

            try
            {
                companyId = clsAdminSession.CompanyId;
                workerAttendanceViewVM = (from at in _db.tbl_WorkerAttendance
                                          join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId

                                          join mSiteInfo in _db.tbl_Site on at.MorningSiteId equals mSiteInfo.SiteId into outerMSiteInfo
                                          from mSiteInfo in outerMSiteInfo.DefaultIfEmpty()
                                          join aSiteInfo in _db.tbl_Site on at.AfternoonSiteId equals aSiteInfo.SiteId into outerASiteInfo
                                          from aSiteInfo in outerASiteInfo.DefaultIfEmpty()
                                          join eSiteInfo in _db.tbl_Site on at.EveningSiteId equals eSiteInfo.SiteId into outerESiteInfo
                                          from eSiteInfo in outerESiteInfo.DefaultIfEmpty()

                                          where emp.CompanyId == companyId
                                          && at.WorkerAttendanceId == id
                                          select new WorkerAttendanceViewVM
                                          {
                                              AttendanceId = at.WorkerAttendanceId,
                                              EmployeeId = emp.EmployeeId,
                                              EmployeeCode = emp.EmployeeCode,
                                              EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                              AttendanceDate = at.AttendanceDate,
                                              EmploymentCategory = emp.EmploymentCategory,
                                              IsMorning = at.IsMorning,
                                              IsAfternoon = at.IsAfternoon,
                                              IsEvening = at.IsEvening,
                                              MorningSite = (mSiteInfo != null ? mSiteInfo.SiteName : ""),
                                              MorningAttendanceBy = at.MorningAttendanceBy,
                                              MorningAttendanceDate = at.MorningAttendanceDate,
                                              MorningLatitude = at.MorningLatitude,
                                              MorningLongitude = at.MorningLongitude,
                                              MorningLocationFrom = at.MorningLocationFrom,
                                              AfternoonSite = (aSiteInfo != null ? aSiteInfo.SiteName : ""),
                                              AfternoonAttendanceBy = at.AfternoonAttendanceBy,
                                              AfternoonAttendanceDate = at.AfternoonAttendanceDate,
                                              AfternoonLatitude = at.AfternoonLatitude,
                                              AfternoonLongitude = at.AfternoonLongitude,
                                              AfternoonLocationFrom = at.AfternoonLocationFrom,
                                              EveningSite = (eSiteInfo != null ? eSiteInfo.SiteName : ""),
                                              EveningAttendanceBy = at.EveningAttendanceBy,
                                              EveningAttendanceDate = at.EveningAttendanceDate,
                                              EveningLatitude = at.EveningLatitude,
                                              EveningLongitude = at.EveningLongitude,
                                              EveningLocationFrom = at.EveningLocationFrom,
                                              ExtraHours = at.ExtraHours,
                                              NoOfHoursWorked = at.NoOfHoursWorked,
                                              NoOfUnitWorked = at.NoOfHoursWorked,
                                              ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                              PerCategoryPrice = emp.PerCategoryPrice,
                                              MonthlySalaryPrice = emp.MonthlySalaryPrice
                                          }).OrderByDescending(x => x.AttendanceDate).FirstOrDefault();

                if (workerAttendanceViewVM.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                {
                    decimal totalAmount = CommonMethod.getPriceBasedOnHours((double)workerAttendanceViewVM.PerCategoryPrice, (double)workerAttendanceViewVM.NoOfHoursWorked);                    
                    workerAttendanceViewVM.WorkedHoursAmount = totalAmount;
                }

                if (workerAttendanceViewVM.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                {
                    workerAttendanceViewVM.WorkedUnitAmount = (workerAttendanceViewVM.NoOfUnitWorked != null ? workerAttendanceViewVM.NoOfUnitWorked.Value : 0) * workerAttendanceViewVM.PerCategoryPrice.Value;
                }

                workerAttendanceViewVM.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)workerAttendanceViewVM.EmploymentCategory);
                workerAttendanceViewVM.IsMorningText = workerAttendanceViewVM.IsMorning ? ErrorMessage.YES : ErrorMessage.NO;
                workerAttendanceViewVM.MorningAttendanceByName = workerAttendanceViewVM.MorningAttendanceBy != null ?
                    (workerAttendanceViewVM.MorningAttendanceBy == (int)PaymentGivenBy.CompanyAdmin ? ErrorMessage.CompanyAdmin : _db.tbl_Employee.Where(x => x.EmployeeId == workerAttendanceViewVM.MorningAttendanceBy).Select(z => z.FirstName + " " + z.LastName).FirstOrDefault())
                    : string.Empty;
                workerAttendanceViewVM.IsAfternoonText = workerAttendanceViewVM.IsAfternoon ? ErrorMessage.YES : ErrorMessage.NO;
                workerAttendanceViewVM.AfternoonAttendanceByName = workerAttendanceViewVM.AfternoonAttendanceBy != null ?
                    (workerAttendanceViewVM.AfternoonAttendanceBy == (int)PaymentGivenBy.CompanyAdmin ? ErrorMessage.CompanyAdmin : _db.tbl_Employee.Where(x => x.EmployeeId == workerAttendanceViewVM.AfternoonAttendanceBy).Select(z => z.FirstName + " " + z.LastName).FirstOrDefault())
                    : string.Empty;
                workerAttendanceViewVM.IsEveningText = workerAttendanceViewVM.IsEvening ? ErrorMessage.YES : ErrorMessage.NO;
                workerAttendanceViewVM.EveningAttendanceByName = workerAttendanceViewVM.EveningAttendanceBy != null ?
                    (workerAttendanceViewVM.EveningAttendanceBy == (int)PaymentGivenBy.CompanyAdmin ? ErrorMessage.CompanyAdmin : _db.tbl_Employee.Where(x => x.EmployeeId == workerAttendanceViewVM.EveningAttendanceBy).Select(z => z.FirstName + " " + z.LastName).FirstOrDefault())
                    : string.Empty;

            }
            catch (Exception ex)
            {
            }
            return View(workerAttendanceViewVM);
        }

        public static List<SelectListItem> GetEmploymentCategoryList()
        {
            string[] employmentCategoryArr = Enum.GetNames(typeof(EmploymentCategory));
            var listEmploymentCategory = employmentCategoryArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listEmploymentCategory
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetWorkerHeadList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            var data = (from wt in _db.tbl_WorkerHead
                        where wt.IsActive && !wt.IsDeleted && wt.CompanyId == companyId
                        select new SelectListItem
                        {
                            Text = wt.HeadName,
                            Value = wt.WorkerHeadId.ToString()
                        }).ToList();

            if (data != null)
            {
                lst = data;
            }

            return lst;
        }

    }
}