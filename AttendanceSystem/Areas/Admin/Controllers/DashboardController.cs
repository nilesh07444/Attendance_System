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
    [NoDirectAccess]
    public class DashboardController : Controller
    {
        AttendanceSystemEntities _db;
        public DashboardController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            DashboardVM dashboardVM = new DashboardVM();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                int roleId = clsAdminSession.RoleID;
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                DateTime currentDateTime = CommonMethod.CurrentIndianDateTime();
                if (roleId == (int)AdminRoles.CompanyAdmin)
                {
                    tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    clsAdminSession.CurrentAccountPackageId = objCompany.CurrentPackageId.HasValue ? objCompany.CurrentPackageId.Value : 0;
                    clsAdminSession.CurrentSMSPackageId = objCompany.CurrentSMSPackageId.HasValue ? objCompany.CurrentSMSPackageId.Value : 0;

                    tbl_CompanySMSPackRenew objCompanySMSPackRenew =
                        _db.tbl_CompanySMSPackRenew
                        .Where(x => clsAdminSession.CurrentSMSPackageId > 0 ? (x.CompanySMSPackRenewId == clsAdminSession.CurrentSMSPackageId) :
                        (x.CompanyId == companyId && x.RenewDate <= currentDateTime && x.PackageExpiryDate >= today)).FirstOrDefault();

                    dashboardVM.SMSLeft = objCompanySMSPackRenew != null ? objCompanySMSPackRenew.RemainingSMS : 0;
                    dashboardVM.CurrentSMSPackageId = objCompanySMSPackRenew != null ? objCompanySMSPackRenew.CompanySMSPackRenewId : 0;

                    dashboardVM.PendingLeaves = (from lv in _db.tbl_Leave
                                                 join ur in _db.tbl_Employee on lv.UserId equals ur.EmployeeId
                                                 where !lv.IsDeleted
                                                 && ur.CompanyId == companyId
                                                 && lv.LeaveStatus == (int)LeaveStatus.Pending
                                                 select lv.LeaveId
                                            ).Count();

                    dashboardVM.PendingAttendance = (from at in _db.tbl_Attendance
                                                     join ur in _db.tbl_Employee on at.UserId equals ur.EmployeeId
                                                     where !at.IsDeleted
                                                     && ur.CompanyId == companyId
                                                     && at.Status == (int)AttendanceStatus.Pending
                                                     select at.AttendanceId
                                               ).Count();



                    if (clsAdminSession.IsTrialMode)
                    {
                        dashboardVM.AccountExpiryDate = objCompany.TrialExpiryDate.Value;
                    }
                    else
                    {
                        tbl_CompanyRenewPayment objCompanyRenewPayment = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId
                        && x.StartDate <= currentDateTime && x.EndDate >= currentDateTime
                        && (clsAdminSession.CurrentAccountPackageId > 0 ? x.CompanyRegistrationPaymentId == clsAdminSession.CurrentAccountPackageId : true)).FirstOrDefault();
                        if (objCompanyRenewPayment != null)
                        {
                            dashboardVM.AccountExpiryDate = objCompanyRenewPayment.EndDate;
                            dashboardVM.CurrentPackageId = objCompanyRenewPayment.CompanyRegistrationPaymentId;
                            dashboardVM.NoOfEmployeeAllowed = objCompanyRenewPayment.NoOfEmployee + objCompanyRenewPayment.BuyNoOfEmployee;
                        }
                        else
                        {
                            dashboardVM.AccountExpiryDate = objCompany.AccountExpiryDate.Value;
                            dashboardVM.NoOfEmployeeAllowed = 0;
                        }
                    }

                    var startDate = new DateTime(CommonMethod.CurrentIndianDateTime().Year, CommonMethod.CurrentIndianDateTime().Month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    var holidayList = (from hl in _db.tbl_Holiday
                                       where hl.CompanyId == companyId.ToString()
                                       && hl.IsActive && !hl.IsDeleted && hl.StartDate >= startDate && hl.StartDate <= endDate
                                       select new
                                       {
                                           EndDate = hl.EndDate,
                                           StartDate = hl.StartDate
                                       }).ToList();


                    dashboardVM.ThisMonthHoliday = Convert.ToInt64(holidayList.Select(x => (x.EndDate - x.StartDate).TotalDays + 1).Sum());

                    if (objCompany != null)
                    {
                        if (objCompany.CompanyTypeId == (int)CompanyType.Banking_OfficeCompany)
                            dashboardVM.IsOfficeCompany = true;
                        else
                            dashboardVM.IsOfficeCompany = false;
                    }

                    dashboardVM.Employee = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Employee).Count();
                    if (!dashboardVM.IsOfficeCompany)
                    {
                        dashboardVM.Supervisor = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Supervisor).Count();
                        dashboardVM.Checker = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Checker).Count();
                        dashboardVM.Payer = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Payer).Count();
                        dashboardVM.Worker = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Worker).Count();
                    }


                    int currentMonth = CommonMethod.CurrentIndianDateTime().Month;
                    int currentYear = CommonMethod.CurrentIndianDateTime().Year;
                    int applyYear = currentMonth == 1 ? currentYear - 1 : currentYear;
                    int applyMonth = currentMonth - 1;
                    List<long> workerIds = new List<long>();

                    bool IsConstructionCompany = false;

                    if (clsAdminSession.CompanyTypeId == (int)CompanyType.ConstructionCompany)
                    {
                        workerIds = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.AdminRoleId == (int)AdminRoles.Worker).Select(x => x.EmployeeId).ToList();
                        IsConstructionCompany = true;
                    }
                    else
                    {
                        dashboardVM.AllowForWorker = false;
                    }

                    List<SelectListItem> lstCalenderMonths = GetCalenderMonthList();
                    tbl_Conversion lastConversion = _db.tbl_Conversion.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefault();
                    if (lastConversion == null)
                    {
                        dashboardVM.Month = Convert.ToInt16(applyMonth);
                        dashboardVM.MonthName = CommonMethod.GetEnumDescription((CalenderMonths)applyMonth);
                        dashboardVM.Year = applyYear;
                        dashboardVM.AllowForEmployee = true;
                        dashboardVM.AllowForWorker = clsAdminSession.CompanyTypeId == (int)CompanyType.ConstructionCompany ? true : false;

                        if (dashboardVM.Employee == 0)
                        {
                            dashboardVM.AllowForEmployee = false;
                        }

                        if (dashboardVM.Worker == 0)
                        {
                            dashboardVM.AllowForWorker = false;
                        }
                    }
                    else
                    {
                        bool isLastConvertionPending = false;
                        if (!lastConversion.IsEmployeeDone)
                        {
                            isLastConvertionPending = true;
                            applyMonth = lastConversion.Month;
                            applyYear = lastConversion.Year;

                            dashboardVM.Month = applyMonth;
                            dashboardVM.MonthName = CommonMethod.GetEnumDescription((CalenderMonths)dashboardVM.Month);
                            dashboardVM.Year = applyYear;
                            dashboardVM.AllowForEmployee = true;
                        }

                        if (!lastConversion.IsWorkerDone && workerIds.Count > 0)
                        {
                            isLastConvertionPending = true;
                            applyMonth = lastConversion.Month;
                            applyYear = lastConversion.Year;
                            dashboardVM.Month = applyMonth;
                            dashboardVM.MonthName = CommonMethod.GetEnumDescription((CalenderMonths)dashboardVM.Month);
                            dashboardVM.Year = applyYear;
                            dashboardVM.AllowForWorker = true;

                        }

                        if (!isLastConvertionPending)
                        {
                            applyMonth = lastConversion.Month == 12 ? 1 : lastConversion.Month + 1;
                            applyYear = lastConversion.Month == 12 ? lastConversion.Year + 1 : lastConversion.Year;

                            dashboardVM.Month = applyMonth;
                            dashboardVM.MonthName = CommonMethod.GetEnumDescription((CalenderMonths)dashboardVM.Month);
                            dashboardVM.Year = applyYear;

                            if (applyYear < currentYear)
                            {
                                dashboardVM.AllowForEmployee = true;
                                dashboardVM.AllowForWorker = IsConstructionCompany ? true : false;
                            }
                            else
                            {
                                if (applyMonth < currentMonth)
                                {
                                    dashboardVM.AllowForEmployee = true;
                                    dashboardVM.AllowForWorker = IsConstructionCompany ? true : false;
                                }
                            }



                        }
                    }

                    //dashboardVM.AllowForEmployee = _db.tbl_Attendance.Any(x => x.CompanyId == companyId && x.AttendanceDate.Month == applyMonth && x.AttendanceDate.Year == applyYear);
                    //dashboardVM.AllowForWorker = workerIds.Count > 0 ? _db.tbl_WorkerAttendance.Any(x => workerIds.Contains(x.EmployeeId) && x.AttendanceDate.Month == applyMonth && x.AttendanceDate.Year == applyYear) : false;

                }

                if (roleId == (int)AdminRoles.SuperAdmin)
                {
                    List<tbl_CompanyRequest> companyRequestList = _db.tbl_CompanyRequest.ToList();
                    dashboardVM.TotalCustomer = companyRequestList.Count();
                    dashboardVM.PendingCompanyRequest = companyRequestList.Where(x => x.RequestStatus == (int)CompanyRequestStatus.Pending).Count();
                    dashboardVM.AccountPackage = _db.tbl_Package.Where(x => x.IsActive && !x.IsDeleted).Count();
                    dashboardVM.SMSPackage = _db.tbl_SMSPackage.Where(x => x.IsActive && !x.IsDeleted).Count();
                    dashboardVM.FeedBack_QueryPending = _db.tbl_Feedback.Where(x => x.IsActive && !x.IsDeleted && x.FeedbackStatus == (int)FeedbackStatus.Pending).Count();
                    dashboardVM.TotalClientRegistration = companyRequestList.Where(x => x.RequestStatus == (int)CompanyRequestStatus.Accept).Count();
                    dashboardVM.TotalClientForConstruction = companyRequestList.Where(x => x.RequestStatus == (int)CompanyRequestStatus.Accept && x.CompanyTypeId == (int)CompanyType.ConstructionCompany).Count();
                    dashboardVM.TotalClientForOffice = companyRequestList.Where(x => x.RequestStatus == (int)CompanyRequestStatus.Accept && x.CompanyTypeId == (int)CompanyType.Banking_OfficeCompany).Count();

                    // Get count of birthday and anniversary for SA
                    dashboardVM.NoOfTodayBirthday = _db.tbl_AdminUser.Where(x => x.DOB == today).ToList().Count;
                    dashboardVM.NoOfTodayAnniversary = _db.tbl_AdminUser.Where(x => x.DateOfMarriageAnniversary.HasValue && x.DateOfMarriageAnniversary.Value == today).ToList().Count;
                }
                else
                {
                    // Get count of birthday for CA
                    dashboardVM.NoOfTodayBirthday = _db.tbl_Employee.Where(x => !x.IsDeleted && x.CompanyId == companyId && x.Dob == today).ToList().Count;
                }

            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                throw ex;
            }
            return View(dashboardVM);
        }

        /// <summary>
        /// Conversion of Employment Types: Employee, Supervisor, Checker, Payer
        /// </summary>
        /// <returns></returns>
        public JsonResult ConversionOfEmployeeUsers(int month, int year)
        {
            int status = 1;
            string errorMessage = string.Empty;
            long companyId = clsAdminSession.CompanyId;
            int nextMonth = month == 12 ? 1 : month + 1;
            int dateYear = month == 12 ? year + 1 : year;
            int employeeId = (int)clsAdminSession.UserID;
            DateTime openDate = new DateTime(dateYear, nextMonth, 1);

            try
            {
                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();

                if (month == CommonMethod.CurrentIndianDateTime().Month && year == CommonMethod.CurrentIndianDateTime().Year)
                {
                    status = 0;
                    errorMessage = ErrorMessage.CanNotStartCurrentMonthConversion;
                }
                if (_db.tbl_Attendance.Any(x => x.CompanyId == companyId && x.AttendanceDate.Month == month && x.AttendanceDate.Year == dateYear && x.Status == (int)AttendanceStatus.Pending))
                {
                    status = 0;
                    errorMessage = ErrorMessage.AttendancePendingForAcceptCanNotCompleteConversion;
                }

                if (_db.tbl_Attendance.Any(x => x.CompanyId == companyId && x.AttendanceDate.Month == month && x.AttendanceDate.Year == dateYear && x.InDateTime != null && x.OutDateTime == null))
                {
                    status = 0;
                    errorMessage = ErrorMessage.OutAttendancePendingCanNotCompleteConversion;
                }

                List<long> employeeIds = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.AdminRoleId != (int)AdminRoles.Worker).Select(x => x.EmployeeId).ToList();

                if (_db.tbl_Leave.Any(x => employeeIds.Contains(x.UserId) && x.StartDate.Month == month && x.StartDate.Year == year && x.LeaveStatus == (int)LeaveStatus.Pending))
                {
                    status = 0;
                    errorMessage = !string.IsNullOrEmpty(errorMessage) ? errorMessage + ", " + ErrorMessage.LeavePendingForAcceptCanNotCompleteConversion : ErrorMessage.LeavePendingForAcceptCanNotCompleteConversion;
                }

                if (status == 1)
                {
                    int[] monthstoCheck = new int[] { month, nextMonth };
                    List<tbl_EmployeePayment> inProcessEmployeePaymentList = (from p in _db.tbl_EmployeePayment
                                                                              where !p.IsDeleted && p.CompanyId == companyId
                                                                              && p.ProcessStatusText == ErrorMessage.InProgress
                                                                              && monthstoCheck.Contains(p.Month)
                                                                              && p.Year == dateYear
                                                                              && p.PaymentType != (int)EmployeePaymentType.Extra
                                                                              select p
                        ).ToList();

                    if (inProcessEmployeePaymentList != null && inProcessEmployeePaymentList.Count > 0)
                    {
                        inProcessEmployeePaymentList.ForEach(x =>
                        {
                            _db.tbl_EmployeePayment.Remove(x);
                            _db.SaveChanges();
                        });
                    }

                    long loggedinUser = (int)PaymentGivenBy.CompanyAdmin;
                    DateTime firstDayOfMonth = new DateTime(year, month, 1);
                    DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    double totalDaysinMonth = DateTime.DaysInMonth(year, month);

                    var currentMonthHolidays = _db.tbl_Holiday.Where(x => !x.IsDeleted && x.CompanyId == companyId.ToString() && x.StartDate >= firstDayOfMonth && x.EndDate <= lastDayOfMonth).ToList();
                    //double holidays = currentMonthHolidays.Select(x => (x.EndDate - x.StartDate).TotalDays + 1).Sum();
                    // For Only Employees

                    // Get All Employees
                    List<tbl_Employee> lstEmployees = _db.tbl_Employee.Where(x => x.CompanyId == companyId && !x.IsDeleted && x.AdminRoleId != (int)AdminRoles.Worker).ToList();
                    List<long> employeeIdsExceptMonthly = lstEmployees.Where(x => x.EmploymentCategory != (int)EmploymentCategory.MonthlyBased).Select(x => x.EmployeeId).ToList();
                    var paymentList = (from emp in lstEmployees
                                       join epp in _db.tbl_EmployeePayment on
                                       new { Id = emp.EmployeeId, month = month, year = year }
                                       equals new { Id = epp.UserId, month = epp.Month, year = epp.Year }
                                       into jointData
                                       from jointRecord in jointData.DefaultIfEmpty()

                                       where
                                       employeeIdsExceptMonthly.Contains(emp.EmployeeId)
                                       // && (jointData != null ? jointRecord.PaymentType != (int)EmployeePaymentType.Extra : true)
                                       select new
                                       {
                                           EmployeeId = emp.EmployeeId,
                                           DebitAmount = jointRecord != null && jointRecord.DebitAmount != null ? jointRecord.DebitAmount : 0,
                                           CreditAmount = jointRecord != null && jointRecord.CreditAmount != null ? jointRecord.CreditAmount : 0,
                                           PaymentType = jointRecord != null && jointRecord.PaymentType != null ? jointRecord.PaymentType : 0
                                       }).ToList();

                    var paymentGroup = paymentList.Where(x => x.PaymentType != (int)EmployeePaymentType.Extra).GroupBy(l => l.EmployeeId)
                        .Select(cl => new
                        {
                            EmployeeId = cl.First().EmployeeId,
                            SumOfDebit = cl.Sum(c => c.DebitAmount != null ? c.DebitAmount : 0),
                            SumOfCredit = cl.Sum(c => c.CreditAmount != null ? c.CreditAmount : 0)
                        }).ToList();



                    paymentGroup.ForEach(x =>
                    {
                        tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                        objEmployeePayment.CompanyId = companyId;
                        objEmployeePayment.UserId = x.EmployeeId;
                        objEmployeePayment.PaymentDate = openDate;
                        //objEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                        objEmployeePayment.CreditOrDebitText = ErrorMessage.Credit;
                        objEmployeePayment.DebitAmount = 0;
                        objEmployeePayment.CreditAmount = x.SumOfCredit - x.SumOfDebit;
                        objEmployeePayment.Remarks = ErrorMessage.MonthlyConversion;
                        objEmployeePayment.Month = nextMonth;
                        objEmployeePayment.Year = year;
                        objEmployeePayment.Status = ErrorMessage.Open;
                        objEmployeePayment.ProcessStatusText = ErrorMessage.InProgress;
                        objEmployeePayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objEmployeePayment.CreatedBy = loggedinUser;
                        objEmployeePayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        objEmployeePayment.ModifiedBy = loggedinUser;
                        objEmployeePayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                        _db.tbl_EmployeePayment.Add(objEmployeePayment);
                        _db.SaveChanges();
                    });

                    lstEmployees.Where(x => x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased).ToList().ForEach(x =>
                    {
                        //List<tbl_Attendance> attendanceList = _db.tbl_Attendance.Where(y => y.UserId == x.EmployeeId && y.AttendanceDate >= firstDayOfMonth && y.AttendanceDate <= lastDayOfMonth).ToList();
                        //double presentDays = attendanceList.Count > 0 ? attendanceList.Select(z => z.DayType).Sum() : 0;
                        //decimal extraHours = attendanceList.Count > 0 ? attendanceList.Select(z => z.ExtraHours).Sum() : 0;
                        //double absentDays = totalDaysinMonth - presentDays;

                        //double deductedLeave = 0;
                        //if (absentDays > (holidays + (double)totalFreeLeave))
                        //{
                        //    deductedLeave = absentDays - (holidays + (double)totalFreeLeave);
                        //}

                        decimal carryForwardLeave = x.CarryForwardLeave;
                        List<tbl_Leave> leaveList = _db.tbl_Leave.Where(z => z.UserId == x.EmployeeId && !z.IsDeleted && z.StartDate.Month == month && z.StartDate.Year == year && z.LeaveStatus == (int)LeaveStatus.Accept).ToList();
                        decimal leaveDays = leaveList.Count > 0 ? leaveList.Sum(z => z.NoOfDays) : 0;

                        decimal perDayAmount = Math.Round((x.MonthlySalaryPrice.HasValue ? x.MonthlySalaryPrice.Value : 0) / (decimal)totalDaysinMonth, 2);
                        decimal totalFreeLeave = x.NoOfFreeLeavePerMonth + x.CarryForwardLeave;

                        if (leaveDays > 0)
                        {
                            decimal leaveIndex = totalFreeLeave;
                            if (leaveDays <= totalFreeLeave)
                            {
                                leaveIndex = leaveDays;
                                carryForwardLeave = carryForwardLeave - leaveDays;
                            }
                            else
                            {
                                carryForwardLeave = 0;
                            }

                            leaveList.ForEach(leave =>
                            {
                                if (leaveIndex > 0)
                                {
                                    tbl_EmployeePayment objLeaveEmployeePayment = new tbl_EmployeePayment();
                                    objLeaveEmployeePayment.CompanyId = companyId;
                                    objLeaveEmployeePayment.UserId = x.EmployeeId;
                                    objLeaveEmployeePayment.PaymentDate = leave.StartDate;
                                    objLeaveEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                                    objLeaveEmployeePayment.CreditOrDebitText = ErrorMessage.Credit;
                                    objLeaveEmployeePayment.DebitAmount = 0;
                                    objLeaveEmployeePayment.CreditAmount = perDayAmount;
                                    objLeaveEmployeePayment.Remarks = ErrorMessage.SalaryCreditForLeave;
                                    objLeaveEmployeePayment.Month = leave.StartDate.Month;
                                    objLeaveEmployeePayment.Year = leave.StartDate.Year;
                                    objLeaveEmployeePayment.Status = ErrorMessage.Open;
                                    objLeaveEmployeePayment.ProcessStatusText = ErrorMessage.InProgress;
                                    objLeaveEmployeePayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    objLeaveEmployeePayment.CreatedBy = loggedinUser;
                                    objLeaveEmployeePayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                    objLeaveEmployeePayment.ModifiedBy = loggedinUser;
                                    objLeaveEmployeePayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                                    _db.tbl_EmployeePayment.Add(objLeaveEmployeePayment);
                                    leaveIndex--;
                                }
                            });
                            _db.SaveChanges();
                        }

                        if (currentMonthHolidays.Count > 0)
                        {
                            currentMonthHolidays.ForEach(holiday =>
                            {
                                DateTime holidayDate = holiday.StartDate;
                                while (holidayDate <= holiday.EndDate)
                                {
                                    tbl_EmployeePayment objHolidayEmployeePayment = new tbl_EmployeePayment();
                                    objHolidayEmployeePayment.CompanyId = companyId;
                                    objHolidayEmployeePayment.UserId = x.EmployeeId;
                                    objHolidayEmployeePayment.PaymentDate = holidayDate;
                                    objHolidayEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                                    objHolidayEmployeePayment.CreditOrDebitText = ErrorMessage.Credit;
                                    objHolidayEmployeePayment.DebitAmount = 0;
                                    objHolidayEmployeePayment.CreditAmount = perDayAmount;
                                    objHolidayEmployeePayment.Remarks = ErrorMessage.SalaryCreditForHoliday;
                                    objHolidayEmployeePayment.Month = holidayDate.Month;
                                    objHolidayEmployeePayment.Year = holidayDate.Year;
                                    objHolidayEmployeePayment.Status = ErrorMessage.Open;
                                    objHolidayEmployeePayment.ProcessStatusText = ErrorMessage.InProgress;
                                    objHolidayEmployeePayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    objHolidayEmployeePayment.CreatedBy = loggedinUser;
                                    objHolidayEmployeePayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                    objHolidayEmployeePayment.ModifiedBy = loggedinUser;
                                    objHolidayEmployeePayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                                    _db.tbl_EmployeePayment.Add(objHolidayEmployeePayment);
                                    holidayDate = holidayDate.AddDays(1);
                                }
                            });
                            _db.SaveChanges();
                        }

                        //if (absentDays < holidays)
                        //{
                        //    carryForwardLeave = totalFreeLeave;
                        //}
                        //else if (absentDays < (holidays + (double)totalFreeLeave))
                        //{
                        //    carryForwardLeave = totalFreeLeave - (decimal)(absentDays - holidays);
                        //}
                        //double monthlySalary = (double)x.MonthlySalaryPrice;
                        //List<decimal> debitAmountList = _db.tbl_EmployeePayment.Where(e => e.UserId == x.EmployeeId && !x.IsDeleted
                        //                        && e.Month == month
                        //                        && e.Year == year
                        //                        && e.DebitAmount > 0 && e.PaymentType != (int)EmployeePaymentType.Extra).Select(e => e.DebitAmount.HasValue ? e.DebitAmount.Value : 0).ToList();
                        //double advancePaid = Convert.ToDouble(debitAmountList.Count > 0 ? debitAmountList.Sum() : 0);
                        //double extraHourSalary = Convert.ToDouble(extraHours * x.ExtraPerHourPrice);

                        //if (deductedLeave > 0)
                        //{
                        //    double perDaySalary = monthlySalary / totalDaysinMonth;
                        //    double deductedSalary = deductedLeave * perDaySalary;
                        //    monthlySalary = (monthlySalary - deductedSalary);
                        //}
                        //monthlySalary = monthlySalary + extraHourSalary - advancePaid;


                        decimal monthlySalary = _db.tbl_EmployeePayment.Any(z => z.UserId == x.EmployeeId && !z.IsDeleted && z.Month == month && z.Year == year && z.PaymentType != (int)EmployeePaymentType.Extra) ?
                    _db.tbl_EmployeePayment.Where(z => z.UserId == x.EmployeeId && !z.IsDeleted && z.Month == month && z.Year == year && z.PaymentType != (int)EmployeePaymentType.Extra).
                    Select(z => (z.CreditAmount.HasValue ? z.CreditAmount.Value : 0) - (z.DebitAmount.HasValue ? z.DebitAmount.Value : 0)).Sum() : 0;

                        tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                        objEmployeePayment.CompanyId = companyId;
                        objEmployeePayment.UserId = x.EmployeeId;
                        objEmployeePayment.PaymentDate = openDate;
                        //objEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                        objEmployeePayment.CreditOrDebitText = ErrorMessage.Credit;
                        objEmployeePayment.DebitAmount = 0;
                        objEmployeePayment.CreditAmount = monthlySalary;
                        objEmployeePayment.Remarks = ErrorMessage.MonthlyConversion;
                        objEmployeePayment.Month = nextMonth;
                        objEmployeePayment.Year = year;
                        objEmployeePayment.Status = ErrorMessage.Open;
                        objEmployeePayment.ProcessStatusText = ErrorMessage.InProgress;
                        objEmployeePayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objEmployeePayment.CreatedBy = loggedinUser;
                        objEmployeePayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        objEmployeePayment.ModifiedBy = loggedinUser;
                        objEmployeePayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                        _db.tbl_EmployeePayment.Add(objEmployeePayment);
                        _db.SaveChanges();

                        if (nextMonth == (int)CalenderMonths.January)
                        {
                            x.CarryForwardLeave = 0;
                        }
                        else
                        {
                            x.CarryForwardLeave = carryForwardLeave > 0 ? carryForwardLeave : 0;
                        }
                    });


                    List<tbl_EmployeePayment> employeePaymentList = _db.tbl_EmployeePayment.Where(x => !x.IsDeleted && x.ProcessStatusText == ErrorMessage.InProgress
                    && monthstoCheck.Contains(x.Month) && x.Year == dateYear && x.PaymentType != (int)EmployeePaymentType.Extra).ToList();
                    employeePaymentList.ForEach(x =>
                    {
                        x.ProcessStatusText = ErrorMessage.Complete;
                        if (x.Remarks == ErrorMessage.SalaryCreditForHoliday || x.Remarks == ErrorMessage.SalaryCreditForLeave)
                        {
                            x.Status = string.Empty;
                            x.ProcessStatusText = string.Empty;
                        }
                        _db.SaveChanges();
                    });

                    lstEmployees.Where(x => x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased).ToList().ForEach(x =>
                   {
                       tbl_Employee empObject = _db.tbl_Employee.Where(z => z.EmployeeId == x.EmployeeId).FirstOrDefault();
                       empObject.CarryForwardLeave = x.CarryForwardLeave;
                       _db.SaveChanges();
                   });

                    tbl_Conversion convertion = _db.tbl_Conversion.Where(x => x.CompanyId == companyId && x.Month == month && x.Year == year).FirstOrDefault();

                    if (convertion != null)
                    {
                        if (!convertion.IsEmployeeDone)
                        {
                            convertion.IsEmployeeDone = true;
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        convertion = new tbl_Conversion();
                        convertion.DateOfConversion = CommonMethod.CurrentIndianDateTime();
                        convertion.Month = month;
                        convertion.Year = year;
                        convertion.CompanyId = companyId;
                        convertion.IsEmployeeDone = true;
                        convertion.CreatedBy = employeeId;
                        _db.tbl_Conversion.Add(convertion);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new
            {
                Status = status,
                ErrorMessage = errorMessage
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ConversionOfWorkerUsers(int month, int year)
        {
            int status = 1;
            string errorMessage = string.Empty;
            int companyId = (int)clsAdminSession.CompanyId;
            int employeeId = (int)clsAdminSession.UserID;
            int nextMonth = month == 12 ? 1 : month + 1;
            int dateyear = month == 12 ? year + 1 : year;
            DateTime openDate = new DateTime(dateyear, nextMonth, 1);

            try
            {
                if (month == CommonMethod.CurrentIndianDateTime().Month && year == CommonMethod.CurrentIndianDateTime().Year)
                {
                    status = 0;
                    errorMessage = ErrorMessage.CanNotStartCurrentMonthConversion;
                }



                List<long> workerIds = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.AdminRoleId == (int)AdminRoles.Worker).Select(x => x.EmployeeId).ToList();
                if (_db.tbl_Leave.Any(x => workerIds.Contains(x.UserId) && x.StartDate.Month == month && x.StartDate.Year == year && x.LeaveStatus == (int)LeaveStatus.Pending))
                {
                    status = 0;
                    errorMessage = ErrorMessage.LeavePendingForAcceptCanNotCompleteConversion;
                }

                //if (_db.tbl_WorkerAttendance.Any(x => workerIds.Contains(x.EmployeeId) && x.AttendanceDate.Month == month && x.AttendanceDate.Year == dateyear && !x.IsClosed))
                //{
                //    status = 0;
                //    errorMessage = ErrorMessage.WorkerAttendanceAreNotClosedCanNotCompleteConversion;
                //}

                if (status == 1)
                {
                    int[] monthstoCheck = new int[] { month, nextMonth };
                    List<tbl_WorkerPayment> inProcessWorkerPaymentList = _db.tbl_WorkerPayment.Where(x => !x.IsDeleted && x.CompanyId == companyId && x.ProcessStatusText == ErrorMessage.InProgress
                    && monthstoCheck.Contains(x.Month) && x.Year == dateyear && x.PaymentType != (int)EmployeePaymentType.Extra).ToList();

                    inProcessWorkerPaymentList.ForEach(x =>
                    {
                        _db.tbl_WorkerPayment.Remove(x);
                        _db.SaveChanges();
                    });

                    long loggedinUser = (int)PaymentGivenBy.CompanyAdmin;

                    int companyTypeId = (int)clsAdminSession.CompanyTypeId;
                    DateTime firstDayOfMonth = new DateTime(year, month, 1);
                    DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                    double totalDaysinMonth = DateTime.DaysInMonth(year, month);


                    var currentMonthHolidays = _db.tbl_Holiday.Where(x => !x.IsDeleted && x.CompanyId == companyId.ToString() && x.StartDate >= firstDayOfMonth && x.EndDate <= lastDayOfMonth).ToList();
                    //double holidays = currentMonthHolidays.Select(x => (x.EndDate - x.StartDate).TotalDays + 1).Sum();
                    // For Only Employees

                    // Get All Employees
                    List<tbl_Employee> lstWorkers = _db.tbl_Employee.Where(x => x.CompanyId == companyId && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Worker).ToList();

                    List<long> employeeIdsExceptMonthly = lstWorkers.Where(x => x.EmploymentCategory != (int)EmploymentCategory.MonthlyBased).Select(x => x.EmployeeId).ToList();
                    var paymentList = (from emp in lstWorkers
                                       join epp in _db.tbl_WorkerPayment on
                                       new { Id = emp.EmployeeId, month = month, year = year }
                                       equals new { Id = epp.UserId, month = epp.Month, year = epp.Year }
                                       into jointData
                                       from jointRecord in jointData.DefaultIfEmpty()

                                       where
                                       employeeIdsExceptMonthly.Contains(emp.EmployeeId)
                                       //&& (jointData != null ? jointRecord.PaymentType != (int)EmployeePaymentType.Extra : true)
                                       select new
                                       {
                                           EmployeeId = emp.EmployeeId,
                                           DebitAmount = jointRecord != null && jointRecord.DebitAmount != null ? jointRecord.DebitAmount : 0,
                                           CreditAmount = jointRecord != null && jointRecord.CreditAmount != null ? jointRecord.CreditAmount : 0,
                                           PaymentType = jointRecord != null && jointRecord.PaymentType != null ? jointRecord.PaymentType : 0
                                       }).ToList();

                    var paymentGroup = paymentList.Where(x => x.PaymentType != (int)EmployeePaymentType.Extra).GroupBy(l => l.EmployeeId)
                        .Select(cl => new
                        {
                            EmployeeId = cl.First().EmployeeId,
                            SumOfDebit = cl.Sum(c => c.DebitAmount != null ? c.DebitAmount : 0),
                            SumOfCredit = cl.Sum(c => c.CreditAmount != null ? c.CreditAmount : 0)
                        }).ToList();

                    paymentGroup.ForEach(x =>
                    {
                        tbl_WorkerPayment objWorkerPayment = new tbl_WorkerPayment();
                        objWorkerPayment.CompanyId = companyId;
                        objWorkerPayment.UserId = x.EmployeeId;
                        objWorkerPayment.PaymentDate = openDate;
                        //objEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                        objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                        objWorkerPayment.DebitAmount = 0;
                        objWorkerPayment.CreditAmount = x.SumOfCredit - x.SumOfDebit;
                        objWorkerPayment.Remarks = ErrorMessage.MonthlyConversion;
                        objWorkerPayment.Month = nextMonth;
                        objWorkerPayment.Year = year;
                        objWorkerPayment.Status = ErrorMessage.Open;
                        objWorkerPayment.ProcessStatusText = ErrorMessage.InProgress;
                        objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerPayment.CreatedBy = loggedinUser;
                        objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerPayment.ModifiedBy = loggedinUser;
                        objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                        _db.tbl_WorkerPayment.Add(objWorkerPayment);
                        _db.SaveChanges();
                    });

                    lstWorkers.Where(x => x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased).ToList().ForEach(x =>
                    {
                        //var dayTypeAttendanceList = (from at in _db.tbl_WorkerAttendance
                        //                             where at.EmployeeId == x.EmployeeId
                        //                             && at.AttendanceDate >= firstDayOfMonth
                        //                             && at.AttendanceDate <= lastDayOfMonth
                        //                             select new
                        //                             {
                        //                                 dayType = at.IsMorning && at.IsAfternoon && at.IsEvening ? 1 : ((at.IsMorning || at.IsAfternoon) && at.IsEvening ? 0.5 : 0),
                        //                                 extraHours = at.ExtraHours.HasValue ? at.ExtraHours.Value : 0
                        //                             }).ToList();

                        //double presentDays = dayTypeAttendanceList.Select(z => z.dayType).Sum();
                        //decimal extraHours = dayTypeAttendanceList.Count > 0 ? dayTypeAttendanceList.Select(z => z.extraHours).Sum() : 0;
                        //double absentDays = totalDaysinMonth - presentDays;

                        decimal carryForwardLeave = x.CarryForwardLeave;
                        List<tbl_Leave> leaveList = _db.tbl_Leave.Where(z => z.UserId == x.EmployeeId && !z.IsDeleted && z.StartDate.Month == month && z.StartDate.Year == year && z.LeaveStatus == (int)LeaveStatus.Accept).ToList();
                        decimal leaveDays = leaveList.Count > 0 ? leaveList.Sum(z => z.NoOfDays) : 0;

                        decimal perDayAmount = Math.Round((x.MonthlySalaryPrice.HasValue ? x.MonthlySalaryPrice.Value : 0) / (decimal)totalDaysinMonth, 2);
                        decimal totalFreeLeave = x.NoOfFreeLeavePerMonth + x.CarryForwardLeave;

                        if (leaveDays > 0)
                        {
                            decimal leaveIndex = totalFreeLeave;
                            if (leaveDays <= totalFreeLeave)
                            {
                                leaveIndex = leaveDays;
                                carryForwardLeave = carryForwardLeave - leaveDays;
                            }
                            else
                            {
                                carryForwardLeave = 0;
                            }

                            leaveList.ForEach(leave =>
                            {
                                if (leaveIndex > 0)
                                {
                                    tbl_WorkerPayment objLeaveWorkerPayment = new tbl_WorkerPayment();
                                    objLeaveWorkerPayment.CompanyId = companyId;
                                    objLeaveWorkerPayment.UserId = x.EmployeeId;
                                    objLeaveWorkerPayment.PaymentDate = leave.StartDate;
                                    objLeaveWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                                    objLeaveWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                                    objLeaveWorkerPayment.DebitAmount = 0;
                                    objLeaveWorkerPayment.CreditAmount = perDayAmount;
                                    objLeaveWorkerPayment.Remarks = ErrorMessage.SalaryCreditForLeave;
                                    objLeaveWorkerPayment.Month = leave.StartDate.Month;
                                    objLeaveWorkerPayment.Year = leave.StartDate.Year;
                                    objLeaveWorkerPayment.Status = ErrorMessage.Open;
                                    objLeaveWorkerPayment.ProcessStatusText = ErrorMessage.InProgress;
                                    objLeaveWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    objLeaveWorkerPayment.CreatedBy = loggedinUser;
                                    objLeaveWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                    objLeaveWorkerPayment.ModifiedBy = loggedinUser;
                                    objLeaveWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                                    _db.tbl_WorkerPayment.Add(objLeaveWorkerPayment);
                                    leaveIndex--;
                                }
                            });
                            _db.SaveChanges();
                        }

                        if (currentMonthHolidays.Count > 0)
                        {
                            currentMonthHolidays.ForEach(holiday =>
                            {
                                DateTime holidayDate = holiday.StartDate;
                                while (holidayDate <= holiday.EndDate)
                                {
                                    tbl_WorkerPayment objHolidayWorkerPayment = new tbl_WorkerPayment();
                                    objHolidayWorkerPayment.CompanyId = companyId;
                                    objHolidayWorkerPayment.UserId = x.EmployeeId;
                                    objHolidayWorkerPayment.PaymentDate = holidayDate;
                                    objHolidayWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                                    objHolidayWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                                    objHolidayWorkerPayment.DebitAmount = 0;
                                    objHolidayWorkerPayment.CreditAmount = perDayAmount;
                                    objHolidayWorkerPayment.Remarks = ErrorMessage.SalaryCreditForHoliday;
                                    objHolidayWorkerPayment.Month = holidayDate.Month;
                                    objHolidayWorkerPayment.Year = holidayDate.Year;
                                    objHolidayWorkerPayment.Status = ErrorMessage.Open;
                                    objHolidayWorkerPayment.ProcessStatusText = ErrorMessage.InProgress;
                                    objHolidayWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    objHolidayWorkerPayment.CreatedBy = loggedinUser;
                                    objHolidayWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                    objHolidayWorkerPayment.ModifiedBy = loggedinUser;
                                    objHolidayWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                                    _db.tbl_WorkerPayment.Add(objHolidayWorkerPayment);
                                    holidayDate = holidayDate.AddDays(1);
                                }
                            });
                            _db.SaveChanges();
                        }

                        //double deductedLeave = 0;
                        //if (absentDays > (holidays + (double)totalFreeLeave))
                        //{
                        //    deductedLeave = absentDays - (holidays + (double)totalFreeLeave);
                        //}

                        //decimal carryForwardLeave = 0;
                        //if (absentDays < holidays)
                        //{
                        //    carryForwardLeave = totalFreeLeave;
                        //}
                        //else if (absentDays < (holidays + (double)totalFreeLeave))
                        //{
                        //    carryForwardLeave = totalFreeLeave - (decimal)(absentDays - holidays);
                        //}
                        //double monthlySalary = (double)x.MonthlySalaryPrice;
                        ////double advancePaid = (double)_db.tbl_WorkerPayment.Where(e => e.UserId == x.EmployeeId && !x.IsDeleted
                        ////                        && e.Month == month
                        ////                        && e.Year == year
                        ////                        && e.DebitAmount > 0 && e.PaymentType != (int)EmployeePaymentType.Extra).Select(e => e.DebitAmount).Sum();

                        //List<decimal> debitAmountList = _db.tbl_WorkerPayment.Where(e => e.UserId == x.EmployeeId && !x.IsDeleted
                        //                       && e.Month == month
                        //                       && e.Year == year
                        //                       && e.DebitAmount > 0 && e.PaymentType != (int)EmployeePaymentType.Extra).Select(e => e.DebitAmount.HasValue ? e.DebitAmount.Value : 0).ToList();
                        //double advancePaid = Convert.ToDouble(debitAmountList.Count > 0 ? debitAmountList.Sum() : 0);

                        //double extraHourSalary = Convert.ToDouble(extraHours * x.ExtraPerHourPrice);


                        //if (deductedLeave > 0)
                        //{
                        //    double perDaySalary = monthlySalary / totalDaysinMonth;
                        //    double deductedSalary = deductedLeave * perDaySalary;
                        //    monthlySalary = (monthlySalary - deductedSalary);
                        //}

                        //monthlySalary = monthlySalary + extraHourSalary - advancePaid;

                        decimal monthlySalary = _db.tbl_WorkerPayment.Any(z => z.UserId == x.EmployeeId && !z.IsDeleted && z.Month == month && z.Year == year && z.PaymentType != (int)EmployeePaymentType.Extra) ?
                  _db.tbl_WorkerPayment.Where(z => z.UserId == x.EmployeeId && !z.IsDeleted && z.Month == month && z.Year == year && z.PaymentType != (int)EmployeePaymentType.Extra).
                  Select(z => (z.CreditAmount.HasValue ? z.CreditAmount.Value : 0) - (z.DebitAmount.HasValue ? z.DebitAmount.Value : 0)).Sum() : 0;


                        tbl_WorkerPayment objWorkerPayment = new tbl_WorkerPayment();
                        objWorkerPayment.CompanyId = companyId;
                        objWorkerPayment.UserId = x.EmployeeId;
                        objWorkerPayment.PaymentDate = openDate;
                        //objEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                        objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                        objWorkerPayment.DebitAmount = 0;
                        objWorkerPayment.CreditAmount = (decimal)monthlySalary;
                        objWorkerPayment.Remarks = ErrorMessage.MonthlyConversion;
                        objWorkerPayment.Month = nextMonth;
                        objWorkerPayment.Year = year;
                        objWorkerPayment.Status = ErrorMessage.Open;
                        objWorkerPayment.ProcessStatusText = ErrorMessage.InProgress;
                        objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerPayment.CreatedBy = loggedinUser;
                        objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerPayment.ModifiedBy = loggedinUser;
                        objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                        _db.tbl_WorkerPayment.Add(objWorkerPayment);
                        _db.SaveChanges();

                        if (nextMonth == (int)CalenderMonths.January)
                        {
                            x.CarryForwardLeave = 0;
                        }
                        else
                        {
                            x.CarryForwardLeave = carryForwardLeave > 0 ? carryForwardLeave : 0;
                        }
                    });


                    List<tbl_WorkerPayment> workerPaymentList = _db.tbl_WorkerPayment.Where(x => !x.IsDeleted && x.ProcessStatusText == ErrorMessage.InProgress
                    && monthstoCheck.Contains(x.Month) && x.Year == dateyear
                    && x.PaymentType != (int)EmployeePaymentType.Extra).ToList();
                    workerPaymentList.ForEach(x =>
                    {
                        x.ProcessStatusText = ErrorMessage.Complete;
                        if (x.Remarks == ErrorMessage.SalaryCreditForHoliday || x.Remarks == ErrorMessage.SalaryCreditForLeave)
                        {
                            x.Status = string.Empty;
                            x.ProcessStatusText = string.Empty;
                        }
                        //x.Status= 
                        _db.SaveChanges();
                    });

                    lstWorkers.Where(x => x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased).ToList().ForEach(x =>
                    {
                        tbl_Employee empObject = _db.tbl_Employee.Where(z => z.EmployeeId == x.EmployeeId).FirstOrDefault();
                        empObject.CarryForwardLeave = x.CarryForwardLeave;
                        _db.SaveChanges();
                    });

                    tbl_Conversion convertion = _db.tbl_Conversion.Where(x => x.CompanyId == companyId && x.Month == month && x.Year == year).FirstOrDefault();

                    if (convertion != null)
                    {
                        if (!convertion.IsWorkerDone)
                        {
                            convertion.IsWorkerDone = true;
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        convertion = new tbl_Conversion();
                        convertion.DateOfConversion = CommonMethod.CurrentIndianDateTime();
                        convertion.Month = month;
                        convertion.Year = year;
                        convertion.CompanyId = companyId;
                        convertion.IsWorkerDone = true;
                        convertion.CreatedBy = employeeId;
                        _db.tbl_Conversion.Add(convertion);
                        _db.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetCalenderMonthList()
        {
            string[] calenderMonthArr = Enum.GetNames(typeof(CalenderMonths));
            var listcalenderMonth = calenderMonthArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listcalenderMonth
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

    }
}