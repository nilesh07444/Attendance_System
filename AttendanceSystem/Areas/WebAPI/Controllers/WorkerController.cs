using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        public ResponseDataModel<bool> AddFingerprint(EmployeeFirgerprintVM fingerprintVM)
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
                        objEmployeeFingerprint.BitmapCode = fingerprintVM.BitmapCode;
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
                                                   State = emp.State,
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
        public ResponseDataModel<List<EmployeeFirgerprintVM>> GetAllEmployeeFingerPrintList()
        {
            ResponseDataModel<List<EmployeeFirgerprintVM>> response = new ResponseDataModel<List<EmployeeFirgerprintVM>>();
            try
            {
                companyId = base.UTI.CompanyId;
                List<EmployeeFirgerprintVM> lstEmployeeFingerPrints = (from f in _db.tbl_EmployeeFingerprint
                                                                       join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                                                       where e.AdminRoleId == (int)AdminRoles.Worker
                                                                       && e.CompanyId == companyId
                                                                       select new EmployeeFirgerprintVM
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
        public ResponseDataModel<List<EmployeeFirgerprintVM>> GetAssignedEmployeeFingerPrintList(long id)
        {
            ResponseDataModel<List<EmployeeFirgerprintVM>> response = new ResponseDataModel<List<EmployeeFirgerprintVM>>();
            try
            {
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                companyId = base.UTI.CompanyId;
                List<EmployeeFirgerprintVM> lstEmployeeFingerPrints = (from f in _db.tbl_EmployeeFingerprint
                                                                       join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                                                       join aw in _db.tbl_AssignWorker on e.EmployeeId equals aw.EmployeeId
                                                                       where e.AdminRoleId == (int)AdminRoles.Worker
                                                                       && e.CompanyId == companyId
                                                                       && aw.Date == today
                                                                       && aw.SiteId == id
                                                                       select new EmployeeFirgerprintVM
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


                EmployeePendingSalaryVM employeeDetails = (from e in _db.tbl_Employee
                                                           where e.EmployeeId == id
                                                           select new EmployeePendingSalaryVM
                                                           {
                                                               EmployeeId = e.EmployeeId,
                                                               EmploymentCategory = e.EmploymentCategory,
                                                               PerCategoryPrice = e.PerCategoryPrice,
                                                               ExtraPerHourPrice = e.ExtraPerHourPrice
                                                           }).FirstOrDefault();

                employeeDetails.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)employeeDetails.EmploymentCategory);
                employeeDetails.PendingSalary = _db.tbl_WorkerPayment.Any(x => x.UserId == id && !x.IsDeleted) ? _db.tbl_WorkerPayment.Where(x => x.UserId == id && !x.IsDeleted).Select(x => x.CreditAmount - x.DebitAmount).Sum() : 0;
                response.Data = employeeDetails;
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