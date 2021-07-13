using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class AttendanceController : Controller
    {

        AttendanceSystemEntities _db;
        long companyId;
        long LoggedInUserId;
        public AttendanceController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
            LoggedInUserId = clsAdminSession.UserID;
        }
        public ActionResult Index(int? userId = null, int? attendanceStatus = null, int? startMonth = null, int? endMonth = null, int? year = null)
        {
            AttendanceFilterVM attendanceFilterVM = new AttendanceFilterVM();
            try
            {
                if (userId.HasValue)
                    attendanceFilterVM.UserId = userId.Value;
                if (attendanceStatus.HasValue)
                    attendanceFilterVM.AttendanceStatus = attendanceStatus.Value;

                if (startMonth.HasValue && endMonth.HasValue)
                {
                    attendanceFilterVM.StartMonth = startMonth.Value;
                    attendanceFilterVM.EndMonth = endMonth.Value;
                }

                if (year.HasValue)
                {
                    attendanceFilterVM.Year = year.Value;
                }

                long companyId = clsAdminSession.CompanyId;

                List<SelectListItem> attendanceStatusList = GetAttendanceStatusList();

                attendanceFilterVM.AttendanceList = (from at in _db.tbl_Attendance
                                                     join emp in _db.tbl_Employee on at.UserId equals emp.EmployeeId
                                                     where !at.IsDeleted
                                                     && emp.CompanyId == companyId

                                                     && ((attendanceFilterVM.StartMonth > 0 && attendanceFilterVM.EndMonth > 0) ? (at.AttendanceDate.Month >= attendanceFilterVM.StartMonth && at.AttendanceDate.Month <= attendanceFilterVM.EndMonth) : true)
                                                     && at.AttendanceDate.Year == attendanceFilterVM.Year
                                                     && (attendanceFilterVM.AttendanceStatus.HasValue && attendanceFilterVM.AttendanceStatus.Value > 0 ? at.Status == attendanceFilterVM.AttendanceStatus.Value : true)
                                                     && (attendanceFilterVM.UserId.HasValue ? emp.EmployeeId == attendanceFilterVM.UserId.Value : true)
                                                     select new AttendanceVM
                                                     {
                                                         AttendanceId = at.AttendanceId,
                                                         CompanyId = at.CompanyId,
                                                         UserId = at.UserId,
                                                         Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                         EmployeeCode = emp.EmployeeCode,
                                                         EmployeeDesignation = emp.Designation,
                                                         AttendanceDate = at.AttendanceDate,
                                                         DayType = at.DayType,
                                                         ExtraHours = at.ExtraHours,
                                                         TodayWorkDetail = at.TodayWorkDetail,
                                                         TomorrowWorkDetail = at.TomorrowWorkDetail,
                                                         Remarks = at.Remarks,
                                                         LocationFrom = at.LocationFrom,
                                                         Status = at.Status,
                                                         RejectReason = at.RejectReason,
                                                         EmploymentCategory = emp.EmploymentCategory
                                                     }).OrderByDescending(x => x.AttendanceDate).ToList();

                attendanceFilterVM.EmployeeList = GetEmployeeList();
                attendanceFilterVM.CalenderMonth = CommonMethod.GetCalenderMonthList();
                attendanceFilterVM.AttendanceList.ForEach(x =>
                {
                    x.StatusText = attendanceStatusList.Where(z => z.Value == x.Status.ToString()).Select(c => c.Text).FirstOrDefault();
                    x.DayTypeText = (x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased || x.EmploymentCategory == (int)EmploymentCategory.MonthlyBased) ? (x.DayType == 1 ? CommonMethod.GetEnumDescription(DayType.FullDay) : CommonMethod.GetEnumDescription(DayType.HalfDay)) : string.Empty;
                    x.BgColor = attendanceFilterVM.AttendanceList.Any(z => z.UserId == x.UserId && z.AttendanceDate == x.AttendanceDate && z.AttendanceId != x.AttendanceId) ? ErrorMessage.Red : string.Empty;
                });
            }
            catch (Exception ex)
            {
            }
            return View(attendanceFilterVM);
        }

        private List<SelectListItem> GetEmployeeList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = emp.Prefix + " " + emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetAttendanceStatusList()
        {
            string[] paymentTypeArr = Enum.GetNames(typeof(AttendanceStatus));
            var listpaymentType = paymentTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listpaymentType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        [HttpPost]
        public string Accept(string ids)
        {
            string ReturnMessage = "";
            try
            {
                long loggedinUser = clsAdminSession.UserID;
                string[] ids_array = ids.Split(',');
                List<tbl_Attendance> attendanceList = _db.tbl_Attendance.Where(x => ids_array.Contains(x.AttendanceId.ToString())).ToList();

                if (attendanceList != null)
                {
                    attendanceList.ForEach(attendance =>
                    {
                        attendance.Status = (int)LeaveStatus.Accept;
                        attendance.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        attendance.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                        tbl_Employee employeeObj = _db.tbl_Employee.Where(x => x.EmployeeId == attendance.UserId).FirstOrDefault();

                        if (employeeObj.EmploymentCategory != (int)EmploymentCategory.MonthlyBased)
                        {
                            tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                            objEmployeePayment.CompanyId = attendance.CompanyId;
                            objEmployeePayment.UserId = attendance.UserId;
                            objEmployeePayment.AttendanceId = attendance.AttendanceId;
                            objEmployeePayment.PaymentDate = attendance.AttendanceDate;
                            objEmployeePayment.PaymentType = (int)EmployeePaymentType.Salary;
                            objEmployeePayment.CreditOrDebitText = ErrorMessage.Credit;
                            objEmployeePayment.DebitAmount = 0;
                            objEmployeePayment.Remarks = ErrorMessage.AutoCreditOnAttendanceAccept;
                            objEmployeePayment.Month = attendance.AttendanceDate.Month;
                            objEmployeePayment.Year = attendance.AttendanceDate.Year;
                            objEmployeePayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            objEmployeePayment.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                            objEmployeePayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                            objEmployeePayment.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;

                            if (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                            {
                                objEmployeePayment.CreditAmount = (employeeObj.PerCategoryPrice) + (employeeObj.ExtraPerHourPrice * attendance.ExtraHours);
                            }
                            else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                            {
                                objEmployeePayment.CreditAmount = employeeObj.PerCategoryPrice * attendance.NoOfHoursWorked;
                            }
                            else if (employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                            {
                                objEmployeePayment.CreditAmount = employeeObj.PerCategoryPrice * attendance.NoOfUnitWorked;
                            }
                            _db.tbl_EmployeePayment.Add(objEmployeePayment);
                            _db.SaveChanges();

                        }

                    });

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public ActionResult Detail(long id)
        {
            AttendanceVM attendanceVM = new AttendanceVM();
            try
            {
                attendanceVM = (from at in _db.tbl_Attendance
                                join emp in _db.tbl_Employee on at.UserId equals emp.EmployeeId
                                where !at.IsDeleted
                                && emp.CompanyId == companyId
                                && at.AttendanceId == id
                                select new AttendanceVM
                                {
                                    AttendanceId = at.AttendanceId,
                                    CompanyId = at.CompanyId,
                                    UserId = at.UserId,
                                    Name = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
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
                                    NoOfHoursWorked = at.NoOfHoursWorked.HasValue ? at.NoOfHoursWorked.Value : 0,
                                    NoOfUnitWorked = at.NoOfUnitWorked.HasValue ? at.NoOfUnitWorked.Value : 0,
                                    PerCategoryPrice = emp.PerCategoryPrice,
                                    EmploymentCategory = emp.EmploymentCategory
                                }).FirstOrDefault();

                attendanceVM.StatusText = CommonMethod.GetEnumDescription((AttendanceStatus)attendanceVM.Status);
                attendanceVM.DayTypeText = attendanceVM.DayType == 1 ? CommonMethod.GetEnumDescription(DayType.FullDay) : CommonMethod.GetEnumDescription(DayType.HalfDay);
                attendanceVM.WorkedHoursAmount = attendanceVM.NoOfHoursWorked * attendanceVM.PerCategoryPrice;
                attendanceVM.WorkedUnitAmount = attendanceVM.NoOfUnitWorked * attendanceVM.PerCategoryPrice;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(attendanceVM);
        }

        [HttpPost]
        public ActionResult Detail(AttendanceVM attendanceVM)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    tbl_Attendance objAttendance = _db.tbl_Attendance.FirstOrDefault(x => x.AttendanceId == attendanceVM.AttendanceId);
                    if (objAttendance != null)
                    {
                        objAttendance.Status = attendanceVM.Status;
                        objAttendance.RejectReason = attendanceVM.RejectReason;
                        objAttendance.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objAttendance.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.SaveChanges();

                        if (attendanceVM.Status == (int)AttendanceStatus.Reject)
                        {
                            tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == objAttendance.UserId).FirstOrDefault();

                            int SmsId = (int)SMSType.AttendanceRejected;
                            string msg = CommonMethod.GetSmsContent(SmsId);

                            Regex regReplace = new Regex("{#var#}");
                            msg = regReplace.Replace(msg, objEmployee.FirstName + " " + objEmployee.LastName, 1);
                            msg = regReplace.Replace(msg, objAttendance.RejectReason, 1);

                            msg = msg.Replace("\r\n", "\n");

                            //var json = CommonMethod.SendSMSWithoutLog(msg, objEmployee.MobileNo);
                            ResponseDataModel<string> smsResponse = CommonMethod.SendSMS(msg, objEmployee.MobileNo, objEmployee.CompanyId, objEmployee.EmployeeId, objEmployee.EmployeeCode, (int)PaymentGivenBy.CompanyAdmin, clsAdminSession.IsTrialMode);
                        }

                    }

                }
                else
                {
                    return View(attendanceVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return RedirectToAction("index");
        }


    }
}