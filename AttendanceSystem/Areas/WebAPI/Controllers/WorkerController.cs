using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/worker")]
    public class WorkerController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        long employeeId;
        long companyId;
        int roleId;
        string domainUrl = string.Empty;
        public WorkerController()
        {
            _db = new AttendanceSystemEntities();
            domainUrl = ConfigurationManager.AppSettings["DomainUrl"].ToString();
        }

        [HttpPost]
        [Route("AddFingerprint")]
        public ResponseDataModel<bool> AddFingerprint(EmployeeFingerprintVM fingerprintVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {

                if (fingerprintVM.EmployeeId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmployeeIdIsNotValid);
                }

                if (fingerprintVM.EmployeeId > 0)
                {
                    int maximumEmployeeFingerprint = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumEmployeeFingerprint"].ToString());

                    int workerFingerprintCount = _db.tbl_EmployeeFingerprint.Where(x => x.EmployeeId == fingerprintVM.EmployeeId).Count();
                    if (workerFingerprintCount >= maximumEmployeeFingerprint)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.WorkerFingerprintLimitReached);
                    }

                    if (!response.IsError)
                    {
                        tbl_EmployeeFingerprint objEmployeeFingerprint = new tbl_EmployeeFingerprint();
                        objEmployeeFingerprint.EmployeeId = fingerprintVM.EmployeeId;
                        objEmployeeFingerprint.ISOCode = fingerprintVM.ISOCode;
                        //objEmployeeFingerprint.BitmapCode = fingerprintVM.BitmapCode;
                        objEmployeeFingerprint.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_EmployeeFingerprint.Add(objEmployeeFingerprint);
                        _db.SaveChanges();
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

        [HttpGet]
        [Route("Search")]
        public ResponseDataModel<List<EmployeeVM>> Search(string searchText)
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;
            try
            {
                companyId = base.UTI.CompanyId;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                               from st in state.DefaultIfEmpty()

                                               join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                               from dt in district.DefaultIfEmpty()

                                               where !emp.IsDeleted && emp.IsActive
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               && emp.CompanyId == companyId
                                               && (!string.IsNullOrEmpty(searchText) ? (emp.EmployeeCode.Contains(searchText)
                                               || emp.FirstName.Contains(searchText)
                                               || emp.LastName.Contains(searchText)) : true)
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
        [Route("GetAllEmployeeFingerPrintList")]
        public ResponseDataModel<List<EmployeeFingerprintVM>> GetAllEmployeeFingerPrintList()
        {
            ResponseDataModel<List<EmployeeFingerprintVM>> response = new ResponseDataModel<List<EmployeeFingerprintVM>>();
            try
            {
                companyId = base.UTI.CompanyId;
                List<EmployeeFingerprintVM> lstEmployeeFingerPrints = (from f in _db.tbl_EmployeeFingerprint
                                                                       join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                                                       where e.AdminRoleId == (int)AdminRoles.Worker
                                                                       && e.CompanyId == companyId
                                                                       select new EmployeeFingerprintVM
                                                                       {
                                                                           EmployeeId = f.EmployeeId,
                                                                           ISOCode = f.ISOCode
                                                                       }).ToList();
                response.Data = lstEmployeeFingerPrints;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("GetAssignedEmployeeFingerPrintList/{id}")]
        public ResponseDataModel<List<EmployeeFingerprintVM>> GetAssignedEmployeeFingerPrintList(long id)
        {
            ResponseDataModel<List<EmployeeFingerprintVM>> response = new ResponseDataModel<List<EmployeeFingerprintVM>>();
            try
            {
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                companyId = base.UTI.CompanyId;
                List<EmployeeFingerprintVM> lstEmployeeFingerPrints = (from f in _db.tbl_EmployeeFingerprint
                                                                       join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                                                       join aw in _db.tbl_AssignWorker on e.EmployeeId equals aw.EmployeeId
                                                                       where e.AdminRoleId == (int)AdminRoles.Worker
                                                                       && e.CompanyId == companyId
                                                                       && aw.Date == today
                                                                       && aw.SiteId == id
                                                                       select new EmployeeFingerprintVM
                                                                       {
                                                                           EmployeeId = f.EmployeeId,
                                                                           ISOCode = f.ISOCode
                                                                       }).ToList();
                response.Data = lstEmployeeFingerPrints;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("GetWorkerPendingSalary/{id}")]
        public ResponseDataModel<EmployeePendingSalaryVM> GetWorkerPendingSalary(long id)
        {
            ResponseDataModel<EmployeePendingSalaryVM> response = new ResponseDataModel<EmployeePendingSalaryVM>();

            try
            {
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                companyId = base.UTI.CompanyId;
                int currMonth = today.Month;
                int currYear = today.Year;
                double totalDaysinMonth = DateTime.DaysInMonth(currYear, currMonth);

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();

                EmployeePendingSalaryVM employeeDetails = (from e in _db.tbl_Employee
                                                           where e.EmployeeId == id
                                                           select new EmployeePendingSalaryVM
                                                           {
                                                               EmployeeId = e.EmployeeId,
                                                               EmploymentCategory = e.EmploymentCategory,
                                                               PerCategoryPrice = e.PerCategoryPrice,
                                                               ExtraPerHourPrice = e.ExtraPerHourPrice,
                                                               MonthlySalary = e.MonthlySalaryPrice,
                                                               NoOfFreeLeavePerMonth = e.NoOfFreeLeavePerMonth
                                                           }).FirstOrDefault();

                if (employeeDetails.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                {

                    //decimal perDayAmount = Math.Round((employeeDetails.MonthlySalary.HasValue ? employeeDetails.MonthlySalary.Value : 0) / (decimal)totalDaysinMonth, 2);

                    decimal perDayAmount = 0;
                    if (objCompany.CompanyConversionType == (int)CompanyConversionType.MonthBased)
                    {
                        perDayAmount = Math.Round((employeeDetails.MonthlySalary.HasValue ? employeeDetails.MonthlySalary.Value : 0) / (decimal)totalDaysinMonth, 2);
                    }
                    else
                    {
                        decimal totalDaysToApply = (decimal)totalDaysinMonth - employeeDetails.NoOfFreeLeavePerMonth;
                        perDayAmount = Math.Round((employeeDetails.MonthlySalary.HasValue ? employeeDetails.MonthlySalary.Value : 0) / totalDaysToApply, 2);
                    }

                    employeeDetails.PerCategoryPrice = perDayAmount;
                    tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.Where(x => x.EmployeeId == id && x.AttendanceDate == today && !x.IsClosed).FirstOrDefault();
                    if (attendanceObject != null)
                    {
                        if (attendanceObject.IsMorning && attendanceObject.IsAfternoon)
                        {
                            employeeDetails.TodaySalary = perDayAmount;
                        }
                        else
                        {
                            employeeDetails.TodaySalary = Math.Round(perDayAmount / 2, 2);
                        }
                    }
                    else
                    {
                        employeeDetails.TodaySalary = 0;
                    }
                }

                employeeDetails.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)employeeDetails.EmploymentCategory);

                //employeeDetails.PendingSalary = _db.tbl_WorkerPayment.Any(x => x.UserId == id && !x.IsDeleted && x.Month == currMonth && x.Year == currYear && x.PaymentType != (int)EmployeePaymentType.Extra) ?
                //    _db.tbl_WorkerPayment.Where(x => x.UserId == id
                //    && !x.IsDeleted
                //    && x.Month == currMonth
                //    && x.Year == currYear
                //    && x.PaymentType != (int)EmployeePaymentType.Extra)
                //    .Select(x => (x.CreditAmount.HasValue ? x.CreditAmount.Value : 0) - (x.DebitAmount.HasValue ? x.DebitAmount.Value : 0)).Sum() : 0;

                decimal? totalCreditAmount1 = (from e in _db.tbl_WorkerPayment
                                              join att in _db.tbl_WorkerAttendance on e.AttendanceId equals att.WorkerAttendanceId
                                              where e.UserId == id
                                                 && !e.IsDeleted
                                                 && e.Month == currMonth
                                                 && e.Year == currYear
                                                 && e.PaymentType != (int)EmployeePaymentType.Extra 
                                                 && att.AttendanceDate != today
                                              select e).ToList().Sum(x => x.CreditAmount);

                decimal? totalCreditAmount2 = (from e in _db.tbl_WorkerPayment
                                               join att in _db.tbl_WorkerAttendance on e.AttendanceId equals att.WorkerAttendanceId
                                               where e.UserId == id
                                                  && !e.IsDeleted
                                                  && e.Month == currMonth
                                                  && e.Year == currYear
                                                  && e.PaymentType != (int)EmployeePaymentType.Extra
                                                  && att.IsClosed && att.AttendanceDate == today
                                               select e).ToList().Sum(x => x.CreditAmount);

                decimal? totalCreditAmount = totalCreditAmount1 + totalCreditAmount2;

                decimal? totalDebitAmount = _db.tbl_WorkerPayment.Where(x => x.UserId == id
                    && !x.IsDeleted
                    && x.Month == currMonth
                    && x.Year == currYear
                    && x.CreditOrDebitText == "Debit"
                    && DbFunctions.TruncateTime(x.PaymentDate) <= DbFunctions.TruncateTime(today)
                    && x.PaymentType != (int)EmployeePaymentType.Extra).ToList().Sum(x => x.DebitAmount);

                employeeDetails.PendingSalary = totalCreditAmount - totalDebitAmount;

                response.Data = employeeDetails;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("GetFingerPrintListOfSelectedWorker/{id}")]
        public ResponseDataModel<List<EmployeeFingerprintVM>> GetFingerPrintListOfSelectedWorker(long id)
        {
            ResponseDataModel<List<EmployeeFingerprintVM>> response = new ResponseDataModel<List<EmployeeFingerprintVM>>();
            try
            {
                companyId = base.UTI.CompanyId;
                List<EmployeeFingerprintVM> lstEmployeeFingerPrints = (from f in _db.tbl_EmployeeFingerprint
                                                                       join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                                                       where e.AdminRoleId == (int)AdminRoles.Worker
                                                                       && e.CompanyId == companyId && f.EmployeeId == id
                                                                       select new EmployeeFingerprintVM
                                                                       {
                                                                           EmployeeFingerprintId = f.EmployeeFingerprintId,
                                                                           EmployeeId = f.EmployeeId,
                                                                           ISOCode = f.ISOCode
                                                                       }).ToList();
                response.Data = lstEmployeeFingerPrints;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("WorkerHeadList")]
        public ResponseDataModel<List<WorkerHeadVM>> WorkerHeadList()
        {
            ResponseDataModel<List<WorkerHeadVM>> response = new ResponseDataModel<List<WorkerHeadVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<WorkerHeadVM> WorkerHeadList = (from wt in _db.tbl_WorkerHead
                                                     where !wt.IsDeleted && wt.IsActive && wt.CompanyId == companyId
                                                     select new WorkerHeadVM
                                                     {

                                                         WorkerHeadId = wt.WorkerHeadId,
                                                         HeadName = wt.HeadName,
                                                         HeadContactNo = wt.HeadContactNo,
                                                         HeadCity = wt.HeadCity,
                                                         IsActive = wt.IsActive
                                                     }).OrderBy(x => x.HeadName).ToList();

                response.Data = WorkerHeadList;
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