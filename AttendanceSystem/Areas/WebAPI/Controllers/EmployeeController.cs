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
        public EmployeeController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
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
                    int empCount = _db.tbl_Employee.Where(x => x.CompanyId == companyId).Count();
                    tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    tbl_Employee objEmployee = new tbl_Employee();
                    objEmployee.ProfilePicture = employeeVM.ProfilePicture;
                    objEmployee.CompanyId = companyId;
                    objEmployee.Password = CommonMethod.Encrypt(CommonMethod.RandomString(6, true), psSult); ;
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
                    objEmployee.EmployeeCode = CommonMethod.getEmployeeCodeFormat(companyId, objCompany.CompanyName, empCount);
                    objEmployee.Address = employeeVM.Address;
                    objEmployee.City = employeeVM.City;
                    objEmployee.IsActive = true;
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
                                                   IsActive = true,
                                               }).OrderByDescending(x => x.EmployeeId).ToList();
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
                    bool isWorkerExist = _db.tbl_Employee.Any(x => x.EmployeeId != employeeVM.EmployeeId && !x.IsDeleted && x.AdminRoleId != (int)AdminRoles.Worker && x.CompanyId == companyId );
                    if (isWorkerExist)
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

    }
}
