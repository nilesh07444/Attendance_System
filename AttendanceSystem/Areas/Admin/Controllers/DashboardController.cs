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

                dashboardVM.AccountExpiryDate = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId).Select(z => z.EndDate).FirstOrDefault();

                var startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);
                dashboardVM.ThisMonthHoliday = _db.tbl_Holiday.Where(x => x.CompanyId == companyId.ToString() && x.IsActive && !x.IsDeleted && x.StartDate >= startDate && x.StartDate <= endDate).Count();

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
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

                if (roleId == (int)AdminRoles.CompanyAdmin)
                {
                    int currentMonth = DateTime.Now.Month;
                    int currentYear = DateTime.Now.Year;
                    int applyYear = currentMonth == 1 ? currentYear - 1 : currentYear;
                    int applyMonth = currentMonth - 1;

                    List<SelectListItem> lstCalenderMonths = GetCalenderMonthList();
                    tbl_Conversion lastConversion = _db.tbl_Conversion.Where(x => x.CompanyId == companyId).OrderByDescending(x => x.Year).ThenByDescending(x => x.Month).FirstOrDefault();
                    if (lastConversion == null)
                    {
                        dashboardVM.Month = Convert.ToInt16(applyMonth);
                        dashboardVM.MonthName = CommonMethod.GetEnumDescription((CalenderMonths)applyMonth);
                        dashboardVM.Year = applyYear;
                    }
                    else
                    {
                        dashboardVM.Month = lastConversion.Month == 12 ? 1 : lastConversion.Month + 1;
                        dashboardVM.MonthName = CommonMethod.GetEnumDescription((CalenderMonths)dashboardVM.Month);
                        dashboardVM.Year = lastConversion.Month == 12 ? lastConversion.Year + 1 : lastConversion.Year;
                    }

                    dashboardVM.AllowForEmployee = _db.tbl_Attendance.Any(x => x.CompanyId == companyId);
                    List<long> workerIds = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.AdminRoleId == (int)AdminRoles.Worker).Select(x => x.EmployeeId).ToList();
                    dashboardVM.AllowForWorker = workerIds.Count > 0 ? _db.tbl_WorkerAttendance.Any(x => workerIds.Contains(x.EmployeeId)) : false;

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
        public ActionResult ConversionOfEmployeeUsers(int month, int year)
        {
            // tbl_Conversion, tbl_EmployeePayment
            List<tbl_EmployeePayment> inProcessEmployeePaymentList = _db.tbl_EmployeePayment.Where(x => x.ProcessStatusText == ErrorMessage.InProgress && x.Month == month && x.Year == year).ToList();
            inProcessEmployeePaymentList.ForEach(x =>
            {
                _db.tbl_EmployeePayment.Remove(x);
            });

            long loggedinUser = clsAdminSession.UserID;
            int companyId = (int)clsAdminSession.CompanyId;
            int companyTypeId = (int)clsAdminSession.CompanyTypeId;
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            double totalDaysinMonth = DateTime.DaysInMonth(year, month);

            List<double> currentMonthHolidays = _db.tbl_Holiday.Where(x => x.CompanyId == companyId.ToString() && x.StartDate >= firstDayOfMonth && x.EndDate <= lastDayOfMonth).Select(x => (x.EndDate - x.StartDate).TotalDays).ToList();
            double holidays = currentMonthHolidays.Sum();
            // For Only Employees

            // Get All Employees
            List<tbl_Employee> lstEmployees = _db.tbl_Employee.Where(x => x.CompanyId == companyId && !x.IsDeleted && x.AdminRoleId != (int)AdminRoles.Worker).ToList();

            List<tbl_EmployeePayment> paymentList = (from epp in _db.tbl_EmployeePayment
                                                     join emp in lstEmployees.Where(x => x.EmploymentCategory != (int)EmploymentCategory.MonthlyBased) on epp.UserId equals emp.EmployeeId
                                                     where epp.Month == month
                                                     && epp.Year == year
                                                     select epp).ToList();
            var paymentGroup = paymentList.GroupBy(l => l.UserId)
                .Select(cl => new
                {
                    EmployeeId = cl.First().UserId,
                    SumOfDebit = cl.Sum(c => c.DebitAmount),
                    SumOfCredit = cl.Sum(c => c.CreditAmount)
                }).ToList();

            int nextMonth = month == 12 ? 1 : month + 1;
            int dateYear = month == 12 ? year + 1 : year;
            DateTime openDate = new DateTime(dateYear, nextMonth, 1);

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
                objEmployeePayment.CreatedDate = DateTime.UtcNow;
                objEmployeePayment.CreatedBy = loggedinUser;
                objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                objEmployeePayment.ModifiedBy = loggedinUser;

                _db.tbl_EmployeePayment.Add(objEmployeePayment);
                _db.SaveChanges();
            });

            lstEmployees.Where(x => x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased).ToList().ForEach(x =>
            {
                double presentDays = _db.tbl_Attendance.Where(y => y.UserId == x.EmployeeId && y.AttendanceDate >= firstDayOfMonth && y.AttendanceDate <= lastDayOfMonth).Select(z => z.DayType).Sum();
                decimal extraHours = _db.tbl_Attendance.Where(y => y.UserId == x.EmployeeId && y.AttendanceDate >= firstDayOfMonth && y.AttendanceDate <= lastDayOfMonth).Select(z => z.ExtraHours).Sum();
                double absentDays = totalDaysinMonth - presentDays;
                double deductedLeave = absentDays - holidays - (double)x.NoOfFreeLeavePerMonth;
                double monthlySalary = (double)x.MonthlySalaryPrice;
                double advancePaid = (double)_db.tbl_EmployeePayment.Where(e => e.UserId == x.EmployeeId
                                        && e.Month == month
                                        && e.Year == year
                                        && e.DebitAmount > 0).Select(e => e.DebitAmount).Sum();
                double extraHourSalary = Convert.ToDouble(extraHours * x.ExtraPerHourPrice);

                monthlySalary = monthlySalary + extraHourSalary - advancePaid;
                if (deductedLeave > 0)
                {
                    double perDaySalary = monthlySalary / totalDaysinMonth;
                    double deductedSalary = deductedLeave * perDaySalary;
                    monthlySalary = (monthlySalary - deductedSalary);
                }

                tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                objEmployeePayment.CompanyId = companyId;
                objEmployeePayment.UserId = x.EmployeeId;
                objEmployeePayment.PaymentDate = openDate;
                //objEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                objEmployeePayment.CreditOrDebitText = ErrorMessage.Credit;
                objEmployeePayment.DebitAmount = 0;
                objEmployeePayment.CreditAmount = (decimal)monthlySalary;
                objEmployeePayment.Remarks = ErrorMessage.MonthlyConversion;
                objEmployeePayment.Month = nextMonth;
                objEmployeePayment.Year = year;
                objEmployeePayment.Status = ErrorMessage.Open;
                objEmployeePayment.ProcessStatusText = ErrorMessage.InProgress;
                objEmployeePayment.CreatedDate = DateTime.UtcNow;
                objEmployeePayment.CreatedBy = loggedinUser;
                objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                objEmployeePayment.ModifiedBy = loggedinUser;

                _db.tbl_EmployeePayment.Add(objEmployeePayment);
                _db.SaveChanges();

            });


            List<tbl_EmployeePayment> employeePaymentList = _db.tbl_EmployeePayment.Where(x => x.ProcessStatusText == ErrorMessage.InProgress && x.Month == month && x.Year == year).ToList();
            employeePaymentList.ForEach(x =>
            {
                x.ProcessStatusText = ErrorMessage.Complete;
                _db.SaveChanges();
            });

            return View();
        }

        public ActionResult ConversionOfWorkerUsers(int month, int year)
        {

            List<tbl_WorkerPayment> inProcessWorkerPaymentList = _db.tbl_WorkerPayment.Where(x => x.ProcessStatusText == ErrorMessage.InProgress && x.Month == month && x.Year == year).ToList();
            inProcessWorkerPaymentList.ForEach(x =>
            {
                _db.tbl_WorkerPayment.Remove(x);
            });

            long loggedinUser = clsAdminSession.UserID;
            int companyId = (int)clsAdminSession.CompanyId;
            int companyTypeId = (int)clsAdminSession.CompanyTypeId;
            DateTime firstDayOfMonth = new DateTime(year, month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            double totalDaysinMonth = DateTime.DaysInMonth(year, month);
            int nextMonth = month == 12 ? 1 : month + 1;
            int dateyear = month == 12 ? year + 1 : year;
            DateTime openDate = new DateTime(dateyear, nextMonth, 1);

            List<double> currentMonthHolidays = _db.tbl_Holiday.Where(x => x.CompanyId == companyId.ToString() && x.StartDate >= firstDayOfMonth && x.EndDate <= lastDayOfMonth).Select(x => (x.EndDate - x.StartDate).TotalDays).ToList();
            double holidays = currentMonthHolidays.Sum();
            // For Only Employees

            // Get All Employees
            List<tbl_Employee> lstWorkers = _db.tbl_Employee.Where(x => x.CompanyId == companyId && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Worker).ToList();

            List<tbl_WorkerPayment> paymentList = (from epp in _db.tbl_WorkerPayment
                                                   join emp in lstWorkers.Where(x => x.EmploymentCategory != (int)EmploymentCategory.MonthlyBased) on epp.UserId equals emp.EmployeeId
                                                   where epp.Month == month
                                                   && epp.Year == year
                                                   select epp).ToList();
            var paymentGroup = paymentList.GroupBy(l => l.UserId)
                .Select(cl => new
                {
                    EmployeeId = cl.First().UserId,
                    SumOfDebit = cl.Sum(c => c.DebitAmount),
                    SumOfCredit = cl.Sum(c => c.CreditAmount)
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
                objWorkerPayment.CreatedDate = DateTime.UtcNow;
                objWorkerPayment.CreatedBy = loggedinUser;
                objWorkerPayment.ModifiedDate = DateTime.UtcNow;
                objWorkerPayment.ModifiedBy = loggedinUser;

                _db.tbl_WorkerPayment.Add(objWorkerPayment);
                _db.SaveChanges();
            });

            lstWorkers.Where(x => x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased).ToList().ForEach(x =>
            {
                double presentDays = _db.tbl_WorkerAttendance.Where(y => y.EmployeeId == x.EmployeeId && y.AttendanceDate >= firstDayOfMonth && y.AttendanceDate <= lastDayOfMonth).Count();
                decimal extraHours = _db.tbl_WorkerAttendance.Where(y => y.EmployeeId == x.EmployeeId && y.AttendanceDate >= firstDayOfMonth && y.AttendanceDate <= lastDayOfMonth).Select(z => z.ExtraHours.HasValue ? z.ExtraHours.Value : 0).Sum();
                double absentDays = totalDaysinMonth - presentDays;
                double deductedLeave = absentDays - holidays - (double)x.NoOfFreeLeavePerMonth;
                double monthlySalary = (double)x.MonthlySalaryPrice;
                double advancePaid = (double)_db.tbl_EmployeePayment.Where(e => e.UserId == x.EmployeeId
                                        && e.Month == month
                                        && e.Year == year
                                        && e.DebitAmount > 0).Select(e => e.DebitAmount).Sum();

                double extraHourSalary = Convert.ToDouble(extraHours * x.ExtraPerHourPrice);

                monthlySalary = monthlySalary + extraHourSalary - advancePaid;
                if (deductedLeave > 0)
                {
                    double perDaySalary = monthlySalary / totalDaysinMonth;
                    double deductedSalary = deductedLeave * perDaySalary;
                    monthlySalary = (monthlySalary - deductedSalary);
                }

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
                objWorkerPayment.CreatedDate = DateTime.UtcNow;
                objWorkerPayment.CreatedBy = loggedinUser;
                objWorkerPayment.ModifiedDate = DateTime.UtcNow;
                objWorkerPayment.ModifiedBy = loggedinUser;

                _db.tbl_WorkerPayment.Add(objWorkerPayment);
                _db.SaveChanges();

            });


            List<tbl_WorkerPayment> workerPaymentList = _db.tbl_WorkerPayment.Where(x => x.ProcessStatusText == ErrorMessage.InProgress && x.Month == month && x.Year == year).ToList();
            workerPaymentList.ForEach(x =>
            {
                x.ProcessStatusText = ErrorMessage.Complete;
                _db.SaveChanges();
            });
            return View();
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