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

    public class EmployeeController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        string psSult;
        long employeeId;
        string defaultPassword;
        public EmployeeController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            employeeId = base.UTI.EmployeeId;
            defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"].ToString(); ;
        }

        [HttpPost]
        [Route("AddWorker")]
        public ResponseDataModel<bool> AddWorker(EmployeeVM employeeVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                if (!response.IsError)
                {
                    int noOfEmployee = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId && DateTime.Today >= x.StartDate && DateTime.Today < x.EndDate).Select(x => x.NoOfEmployee).FirstOrDefault();
                    var empCount = (from emp in _db.tbl_Employee
                                    where emp.CompanyId == companyId
                                    select new
                                    {
                                        employeeId = emp.EmployeeId,
                                        isActive = emp.IsActive
                                    }).ToList();

                    int activeEmployee = empCount.Where(x => x.isActive).Count();

                    tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    tbl_Employee objEmployee = new tbl_Employee();
                    objEmployee.ProfilePicture = employeeVM.ProfilePicture;
                    objEmployee.CompanyId = companyId;
                    objEmployee.Password = CommonMethod.Encrypt(defaultPassword, psSult); ;
                    objEmployee.AdminRoleId = (int)AdminRoles.Worker;
                    objEmployee.Prefix = employeeVM.Prefix;
                    objEmployee.FirstName = employeeVM.FirstName;
                    objEmployee.LastName = employeeVM.LastName;
                    objEmployee.MobileNo = employeeVM.MobileNo;
                    objEmployee.Dob = employeeVM.Dob;
                    objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                    objEmployee.BloodGroup = employeeVM.BloodGroup;
                    objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                    objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                    objEmployee.AdharCardNo = employeeVM.AdharCardNo;
                    objEmployee.EmployeeCode = CommonMethod.getEmployeeCodeFormat(companyId, objCompany.CompanyName, empCount.Count());
                    objEmployee.Address = employeeVM.Address;
                    objEmployee.City = employeeVM.City;
                    objEmployee.Pincode = employeeVM.Pincode;
                    objEmployee.State = employeeVM.State;
                    objEmployee.IsActive = activeEmployee >= noOfEmployee ? false : true; 
                    objEmployee.CreatedBy = employeeId;
                    objEmployee.CreatedDate = DateTime.UtcNow;
                    objEmployee.UpdatedBy = employeeId;
                    objEmployee.UpdatedDate = DateTime.UtcNow;
                    _db.tbl_Employee.Add(objEmployee);
                    _db.SaveChanges();
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

        [HttpGet]
        [Route("ListWorkers")]
        public ResponseDataModel<List<EmployeeVM>> ListWorkers()
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               where !emp.IsDeleted && emp.CompanyId == companyId
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
                                                   EmployeeCode = emp.EmployeeCode,
                                                   ProfilePicture = emp.ProfilePicture,
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
                                                   IsActive = true,
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
        [Route("WorkerDetail/{id}")]
        public ResponseDataModel<EmployeeVM> WorkerDetail(long id)
        {
            ResponseDataModel<EmployeeVM> response = new ResponseDataModel<EmployeeVM>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                EmployeeVM workerDetails = (from emp in _db.tbl_Employee
                                            where !emp.IsDeleted && emp.CompanyId == companyId
                                            && emp.AdminRoleId == (int)AdminRoles.Worker
                                            && emp.EmployeeId == id
                                            select new EmployeeVM
                                            {
                                                EmployeeId = emp.EmployeeId,
                                                EmployeeCode = emp.EmployeeCode,
                                                ProfilePicture = emp.ProfilePicture,
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
                                                IsActive = true,
                                            }).FirstOrDefault();

                response.Data = workerDetails;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("EditWorker")]
        public ResponseDataModel<bool> EditWorker(EmployeeVM employeeVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                if (employeeVM.EmployeeId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmployeeIdIsNotValid);
                }

                if (employeeVM.EmployeeId > 0)
                {
                    bool isWorkerExist = _db.tbl_Employee.Any(x => x.EmployeeId == employeeVM.EmployeeId && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Worker && x.CompanyId == companyId);
                    if (!isWorkerExist)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.EmployeeDoesNotExist);
                    }

                    tbl_Employee employeeObject = _db.tbl_Employee.Where(x => x.EmployeeId != employeeVM.EmployeeId).FirstOrDefault();

                    if (!response.IsError)
                    {

                        employeeObject.ProfilePicture = employeeVM.ProfilePicture;
                        employeeObject.Prefix = employeeVM.Prefix;
                        employeeObject.FirstName = employeeVM.FirstName;
                        employeeObject.LastName = employeeVM.LastName;
                        employeeObject.MobileNo = employeeVM.MobileNo;
                        employeeObject.Dob = employeeVM.Dob;
                        employeeObject.DateOfJoin = employeeVM.DateOfJoin;
                        employeeObject.BloodGroup = employeeVM.BloodGroup;
                        employeeObject.EmploymentCategory = employeeVM.EmploymentCategory;
                        employeeObject.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        employeeObject.AdharCardNo = employeeVM.AdharCardNo;
                        employeeObject.Address = employeeVM.Address;
                        employeeObject.City = employeeVM.City;
                        employeeObject.Pincode = employeeVM.Pincode;
                        employeeObject.State = employeeVM.State;
                        employeeObject.UpdatedBy = employeeId;
                        employeeObject.UpdatedDate = DateTime.UtcNow;
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
                long companyId = base.UTI.CompanyId;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               where !emp.IsDeleted && emp.IsActive && emp.CompanyId == companyId
                                               && (!string.IsNullOrEmpty(searchText) ? (emp.EmployeeCode.Contains(searchText)
                                               || emp.FirstName.Contains(searchText)
                                               || emp.LastName.Contains(searchText)) : true)
                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
                                                   EmployeeCode = emp.EmployeeCode,
                                                   ProfilePicture = emp.ProfilePicture,
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
                                                   IsActive = true,
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
        [Route("ListWorkersToAssign")]
        public ResponseDataModel<List<EmployeeVM>> ListWorkersToAssign(ViewModel.WebAPI.WorkerListRequestVM requestVM)
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join wk in _db.tbl_AssignWorker.Where(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date)
                                               on emp.EmployeeId equals wk.EmployeeId into lfwk
                                               from wkr in lfwk.DefaultIfEmpty()
                                               where !emp.IsDeleted && emp.CompanyId == companyId
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               && wkr == null
                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
                                                   ProfilePicture = emp.ProfilePicture,
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
                                                   EmployeeCode = emp.EmployeeCode,
                                                   Address = emp.Address,
                                                   City = emp.City,
                                                   Pincode = emp.Pincode,
                                                   State = emp.State,
                                                   IsActive = true,
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
        [Route("AssignedWorkerList")]
        public ResponseDataModel<List<EmployeeVM>> AssignedWorkerList(ViewModel.WebAPI.WorkerListRequestVM requestVM)
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join wk in _db.tbl_AssignWorker
                                               on emp.EmployeeId equals wk.EmployeeId
                                               where !emp.IsDeleted && emp.CompanyId == companyId
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               && wk.SiteId == requestVM.SiteId
                                               && wk.Date == requestVM.Date
                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
                                                   ProfilePicture = emp.ProfilePicture,
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
                                                   EmployeeCode = emp.EmployeeCode,
                                                   Address = emp.Address,
                                                   City = emp.City,
                                                   Pincode = emp.Pincode,
                                                   State = emp.State,
                                                   IsActive = true,
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
        [Route("AssignWorker")]
        public ResponseDataModel<bool> AssignWorker(ViewModel.WebAPI.AssignWorkerRequestVM requestVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                #region Validation
                if (requestVM.Date != DateTime.Today)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerCanAssignForTodayOnly);
                }

                if (!_db.tbl_Site.Any(x => x.CompanyId == companyId && x.SiteId == requestVM.SiteId && x.IsActive && !x.IsDeleted))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteDoesNotExistForCurrentCompany);
                }
                #endregion Validation

                if (!response.IsError)
                {
                    List<tbl_AssignWorker> listAssignedWorker = new List<tbl_AssignWorker>();
                    requestVM.WorkerList.ForEach(x =>
                    {
                        tbl_AssignWorker assignedWorker = new tbl_AssignWorker();
                        assignedWorker.SiteId = requestVM.SiteId;
                        assignedWorker.Date = requestVM.Date;
                        assignedWorker.EmployeeId = x;
                        assignedWorker.IsClosed = false;
                        assignedWorker.CreatedBy = employeeId;
                        assignedWorker.CreatedDate = DateTime.UtcNow;
                        assignedWorker.ModifiedBy = employeeId;
                        assignedWorker.ModifiedDate = DateTime.UtcNow;
                        listAssignedWorker.Add(assignedWorker);
                    });

                    _db.tbl_AssignWorker.AddRange(listAssignedWorker);
                    _db.SaveChanges();
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

        [HttpPost]
        [Route("CloseWorker")]
        public ResponseDataModel<bool> CloseWorker(ViewModel.WebAPI.CloseWorkerRequestVM requestVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                #region Validation
                if (requestVM.Date != DateTime.Today)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerCanCloseForTodayOnly);
                }

                if (!_db.tbl_Site.Any(x => x.CompanyId == companyId && x.SiteId == requestVM.SiteId && x.IsActive && !x.IsDeleted))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteDoesNotExistForCurrentCompany);
                }

                if (!_db.tbl_AssignWorker.Any(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerDidNotAssignedToThisSite);
                }

                if (_db.tbl_AssignWorker.Any(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId && x.IsClosed))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAlreadyClosed);
                }

                #endregion Validation

                if (!response.IsError)
                {
                    tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId).FirstOrDefault();
                    if (assignedWorker != null)
                    {
                        assignedWorker.IsClosed = true;
                        assignedWorker.ModifiedBy = employeeId;
                        assignedWorker.ModifiedDate = DateTime.UtcNow;
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

        [HttpPost]
        [Route("RemoveWorker")]
        public ResponseDataModel<bool> RemoveWorker(ViewModel.WebAPI.CloseWorkerRequestVM requestVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                #region Validation
                if (requestVM.Date != DateTime.Today)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerCanCloseForTodayOnly);
                }

                if (!_db.tbl_Site.Any(x => x.CompanyId == companyId && x.SiteId == requestVM.SiteId && x.IsActive && !x.IsDeleted))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteDoesNotExistForCurrentCompany);
                }

                if (!_db.tbl_AssignWorker.Any(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerDidNotAssignedToThisSite);
                }

                if (_db.tbl_AssignWorker.Any(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId && x.IsClosed))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAlreadyClosed);
                }

                #endregion Validation

                if (!response.IsError)
                {
                    tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId).FirstOrDefault();
                    if (assignedWorker != null)
                    {
                        _db.tbl_AssignWorker.Remove(assignedWorker);
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
    }
}
