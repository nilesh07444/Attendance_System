using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{

    public class EmployeeController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        string psSult;
        long employeeId;
        long companyId;
        string domainUrl = string.Empty;
        bool isTrailMode;
        public EmployeeController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            domainUrl = ConfigurationManager.AppSettings["DomainUrl"].ToString();
        }

        [HttpPost]
        [Route("AddWorker")]
        public ResponseDataModel<long> AddWorker(EmployeeVM employeeVM)
        {
            employeeId = base.UTI.EmployeeId;
            companyId = base.UTI.CompanyId;
            isTrailMode = base.UTI.IsTrailMode;

            ResponseDataModel<long> response = new ResponseDataModel<long>();
            response.IsError = false;
            response.Data = 0;
            try
            {

                #region validation
                if (employeeVM.EmploymentCategory == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmploymentCategoryRequired);
                }

                if (!employeeVM.WorkerTypeId.HasValue || employeeVM.WorkerTypeId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorketTypeRequired);
                }

                if (employeeVM.EmploymentCategory == (int)EmploymentCategory.DailyBased && employeeVM.PerCategoryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PerDayPriceRequiredForDailyBasedWorker);
                }
                //else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.DailyBased && employeeVM.ExtraPerHourPrice == 0)
                //{
                //    response.IsError = true;
                //    response.AddError(ErrorMessage.ExtraPerHourPriceRequired);
                //}
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.HourlyBased && employeeVM.PerCategoryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PerhourPriceRequiredForHourlyBasedWorker);
                }
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.MonthlyBased && employeeVM.MonthlySalaryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MonthlySalaryRequiredForMonthlyBasedWorker);
                }
                //else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.MonthlyBased && employeeVM.ExtraPerHourPrice == 0)
                //{
                //    response.IsError = true;
                //    response.AddError(ErrorMessage.ExtraPerHourPriceRequired);
                //}
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.UnitBased && employeeVM.PerCategoryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PerUnitPriceRequiredForUnitBasedWorker);
                }
                #endregion validation

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

                    string randomPassword = CommonMethod.GetRandomPassword(8);

                    tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                    tbl_Employee objEmployee = new tbl_Employee();
                    objEmployee.ProfilePicture = employeeVM.ProfilePicture;
                    objEmployee.CompanyId = companyId;
                    objEmployee.Password = CommonMethod.Encrypt(randomPassword, psSult);
                    objEmployee.AdminRoleId = (int)AdminRoles.Worker;
                    objEmployee.Prefix = employeeVM.Prefix;
                    objEmployee.FirstName = CommonMethod.SentenceCase(employeeVM.FirstName);
                    objEmployee.LastName = CommonMethod.SentenceCase(employeeVM.LastName);
                    objEmployee.MobileNo = employeeVM.MobileNo;
                    objEmployee.Dob = employeeVM.Dob;
                    objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                    objEmployee.BloodGroup = employeeVM.BloodGroup;
                    objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                    objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                    objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                    objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice.HasValue ? employeeVM.ExtraPerHourPrice.Value : 0;
                    objEmployee.AdharCardNo = CommonMethod.UpperCase(employeeVM.AdharCardNo);
                    objEmployee.EmployeeCode = CommonMethod.getEmployeeCodeFormat(companyId, objCompany.CompanyName, empCount.Count());
                    objEmployee.Address = CommonMethod.SentenceCase(employeeVM.Address);
                    objEmployee.City = employeeVM.City;
                    objEmployee.Pincode = employeeVM.Pincode;
                    objEmployee.StateId = employeeVM.StateId;
                    objEmployee.DistrictId = employeeVM.DistrictId;
                    objEmployee.IsActive = isTrailMode ? true : (activeEmployee >= noOfEmployee ? false : true);
                    objEmployee.WorkerTypeId = employeeVM.WorkerTypeId;
                    objEmployee.WorkerHeadId = employeeVM.WorkerHeadId;
                    objEmployee.CreatedBy = employeeId;
                    objEmployee.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    objEmployee.UpdatedBy = employeeId;
                    objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();
                    _db.tbl_Employee.Add(objEmployee);
                    _db.SaveChanges();

                    response.Data = objEmployee.EmployeeId;
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
        public ResponseDataModel<List<EmployeeVM>> ListWorkers(string searchText = "")
        {
            companyId = base.UTI.CompanyId;
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;
            try
            {
                int maximumEmployeeFingerprint = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumEmployeeFingerprint"].ToString());

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                               from st in state.DefaultIfEmpty()

                                               join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                               from dt in district.DefaultIfEmpty()
                                               where !emp.IsDeleted && emp.CompanyId == companyId
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
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
                                                   PerCategoryPrice = emp.PerCategoryPrice,
                                                   ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                                   MonthlySalaryPrice = emp.MonthlySalaryPrice,
                                                   AdharCardNo = emp.AdharCardNo,
                                                   Address = emp.Address,
                                                   City = emp.City,
                                                   Pincode = emp.Pincode,
                                                   StateName = st != null ? st.StateName : "",
                                                   DistrictName = dt != null ? dt.DistrictName : "",
                                                   IsActive = emp.IsActive,
                                                   ProfilePicture = !string.IsNullOrEmpty(emp.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + emp.ProfilePicture : string.Empty,
                                                   IsFingerprintLimitExceed = (_db.tbl_EmployeeFingerprint.Where(x => x.EmployeeId == emp.EmployeeId).Count() >= maximumEmployeeFingerprint)
                                               }).ToList();
                workerList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                });

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
                companyId = base.UTI.CompanyId;

                EmployeeVM workerDetails = (from emp in _db.tbl_Employee

                                            join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                            from st in state.DefaultIfEmpty()

                                            join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                            from dt in district.DefaultIfEmpty()

                                            where !emp.IsDeleted && emp.CompanyId == companyId
                                            && emp.AdminRoleId == (int)AdminRoles.Worker
                                            && emp.EmployeeId == id
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
                                                PerCategoryPrice = emp.PerCategoryPrice,
                                                ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                                MonthlySalaryPrice = emp.MonthlySalaryPrice,
                                                AdharCardNo = emp.AdharCardNo,
                                                Address = emp.Address,
                                                City = emp.City,
                                                Pincode = emp.Pincode,
                                                StateName = st != null ? st.StateName : "",
                                                DistrictName = dt != null ? dt.DistrictName : "",
                                                WorkerHeadId = emp.WorkerHeadId,
                                                IsActive = emp.IsActive,
                                                ProfilePicture = !string.IsNullOrEmpty(emp.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + emp.ProfilePicture : string.Empty
                                            }).FirstOrDefault();

                workerDetails.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)workerDetails.EmploymentCategory);
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
            employeeId = base.UTI.EmployeeId;
            companyId = base.UTI.CompanyId;
            try
            {
                #region validation
                if (employeeVM.EmployeeId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmployeeIdIsNotValid);
                }
                if (employeeVM.EmploymentCategory == (int)EmploymentCategory.DailyBased && employeeVM.PerCategoryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PerDayPriceRequiredForDailyBasedWorker);
                }
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.DailyBased && employeeVM.ExtraPerHourPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.ExtraPerHourPriceRequired);
                }
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.HourlyBased && employeeVM.PerCategoryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PerhourPriceRequiredForHourlyBasedWorker);
                }
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.MonthlyBased && employeeVM.MonthlySalaryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MonthlySalaryRequiredForMonthlyBasedWorker);
                }
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.MonthlyBased && employeeVM.ExtraPerHourPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.ExtraPerHourPriceRequired);
                }
                else if (employeeVM.EmploymentCategory == (int)EmploymentCategory.UnitBased && employeeVM.PerCategoryPrice == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PerUnitPriceRequiredForUnitBasedWorker);
                }
                #endregion validation
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
                        employeeObject.FirstName = CommonMethod.SentenceCase(employeeVM.FirstName);
                        employeeObject.LastName = CommonMethod.SentenceCase(employeeVM.LastName);
                        employeeObject.MobileNo = employeeVM.MobileNo;
                        employeeObject.Dob = employeeVM.Dob;
                        employeeObject.DateOfJoin = employeeVM.DateOfJoin;
                        employeeObject.BloodGroup = employeeVM.BloodGroup;
                        employeeObject.EmploymentCategory = employeeVM.EmploymentCategory;
                        employeeObject.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        employeeObject.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        employeeObject.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        employeeObject.AdharCardNo = CommonMethod.UpperCase(employeeVM.AdharCardNo);
                        employeeObject.Address = CommonMethod.SentenceCase(employeeVM.Address);
                        employeeObject.City = CommonMethod.SentenceCase(employeeVM.City);
                        employeeObject.Pincode = employeeVM.Pincode;
                        employeeObject.StateId = employeeVM.StateId;
                        employeeObject.DistrictId = employeeVM.DistrictId;
                        employeeObject.UpdatedBy = employeeId;
                        employeeObject.UpdatedDate = CommonMethod.CurrentIndianDateTime();
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

                                               where !emp.IsDeleted && emp.IsActive && emp.CompanyId == companyId
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

        [HttpPost]
        [Route("ListWorkersToAssign")]
        public ResponseDataModel<List<EmployeeVM>> ListWorkersToAssign(ViewModel.WebAPI.WorkerListRequestVM requestVM)
        {
            ResponseDataModel<List<EmployeeVM>> response = new ResponseDataModel<List<EmployeeVM>>();
            response.IsError = false;
            try
            {
                companyId = base.UTI.CompanyId;

                List<tbl_Employee> fingerprintEmployees = (from emp in _db.tbl_Employee
                                                           join wt in _db.tbl_EmployeeFingerprint on emp.EmployeeId equals wt.EmployeeId
                                                           where emp.AdminRoleId == (int)AdminRoles.Worker && emp.CompanyId == companyId
                                                           select emp).ToList();

                fingerprintEmployees = fingerprintEmployees.GroupBy(x => x.EmployeeId).Select(g => g.First()).ToList();
                List<long> fingerprintEmployeesIds = fingerprintEmployees.Select(x => x.EmployeeId).ToList();

                //List<long> assignedEmployeeIds = _db.tbl_AssignWorker.Where(x => x.Date == requestVM.Date && !x.IsClosed).Select(x => x.EmployeeId).ToList();

                List<long> assignedEmployeeIds = (from emp in _db.tbl_Employee
                                                  join wt in _db.tbl_AssignWorker on emp.EmployeeId equals wt.EmployeeId
                                                  where emp.AdminRoleId == (int)AdminRoles.Worker && emp.CompanyId == companyId
                                                  && wt.Date == requestVM.Date && !wt.IsClosed
                                                  select emp.EmployeeId).ToList();

                List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                                               join wt in _db.tbl_WorkerType on emp.WorkerTypeId equals wt.WorkerTypeId into wtc
                                               from w in wtc.DefaultIfEmpty()

                                               join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                               from st in state.DefaultIfEmpty()

                                               join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                               from dt in district.DefaultIfEmpty()

                                               where emp.IsActive && !emp.IsDeleted && emp.CompanyId == companyId
                                               && emp.AdminRoleId == (int)AdminRoles.Worker
                                               && !assignedEmployeeIds.Contains(emp.EmployeeId)
                                               && fingerprintEmployeesIds.Contains(emp.EmployeeId)
                                               select new EmployeeVM
                                               {
                                                   EmployeeId = emp.EmployeeId,
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
                                                   StateName = st != null ? st.StateName : "",
                                                   DistrictName = dt != null ? dt.DistrictName : "",
                                                   IsActive = emp.IsActive,
                                                   ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                                   WorkerTypeId = emp.WorkerTypeId,
                                                   WorkerTypeText = w.WorkerTypeName,
                                                   ProfilePicture = !string.IsNullOrEmpty(emp.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + emp.ProfilePicture : string.Empty
                                               }).ToList();

                workerList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                });
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
        public ResponseDataModel<List<SiteAssignedWorkerVM>> AssignedWorkerList(ViewModel.WebAPI.WorkerListRequestVM requestVM)
        {
            ResponseDataModel<List<SiteAssignedWorkerVM>> response = new ResponseDataModel<List<SiteAssignedWorkerVM>>();
            response.IsError = false;
            try
            {
                companyId = base.UTI.CompanyId;

                //List<EmployeeVM> workerList = (from emp in _db.tbl_Employee
                //                               join wk in _db.tbl_AssignWorker
                //                               on emp.EmployeeId equals wk.EmployeeId
                //                               where !emp.IsDeleted && emp.CompanyId == companyId
                //                               && emp.AdminRoleId == (int)AdminRoles.Worker
                //                               && wk.SiteId == requestVM.SiteId
                //                               && wk.Date == requestVM.Date
                //                               select new EmployeeVM
                //                               {
                //                                   EmployeeId = emp.EmployeeId,
                //                                   CompanyId = emp.CompanyId,
                //                                   Prefix = emp.Prefix,
                //                                   FirstName = emp.FirstName,
                //                                   LastName = emp.LastName,
                //                                   MobileNo = emp.MobileNo,
                //                                   Dob = emp.Dob,
                //                                   DateOfJoin = emp.DateOfJoin,
                //                                   BloodGroup = emp.BloodGroup,
                //                                   EmploymentCategory = emp.EmploymentCategory,
                //                                   MonthlySalaryPrice = emp.MonthlySalaryPrice,
                //                                   AdharCardNo = emp.AdharCardNo,
                //                                   EmployeeCode = emp.EmployeeCode,
                //                                   Address = emp.Address,
                //                                   City = emp.City,
                //                                   Pincode = emp.Pincode,
                //                                   State = emp.State,
                //                                   IsActive = emp.IsActive,
                //                                   ProfilePicture = !string.IsNullOrEmpty(emp.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + emp.ProfilePicture : string.Empty
                //                               }).ToList();

                var companyParam = new SqlParameter
                {
                    ParameterName = "CompanyId",
                    Value = companyId
                };

                var siteParam = new SqlParameter
                {
                    ParameterName = "RoleId",
                    Value = (int)AdminRoles.Worker
                };

                var roleParam = new SqlParameter
                {
                    ParameterName = "SiteId",
                    Value = requestVM.SiteId
                };

                var dateParam = new SqlParameter
                {
                    ParameterName = "Date",
                    Value = requestVM.Date
                };
                //Get student name of string type
                List<SiteAssignedWorkerVM> workerList = _db.Database.SqlQuery<SiteAssignedWorkerVM>("exec Usp_GetSiteAssignedWorkerList @CompanyId,@RoleId,@SiteId,@Date ", companyParam, siteParam, roleParam, dateParam).ToList<SiteAssignedWorkerVM>();

                workerList.ForEach(x =>
                {
                    x.ProfilePicture = !string.IsNullOrEmpty(x.ProfilePicture) ? domainUrl + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty;
                });
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
                employeeId = base.UTI.EmployeeId;
                companyId = base.UTI.CompanyId;

                #region Validation
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                if (!_db.tbl_Attendance.Any(x => x.UserId == employeeId && x.InDateTime != null && x.OutDateTime == null))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YourAttendanceNotTakenYetYouCanNotAssignWorker);
                }

                if (requestVM.Date.Date != today)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerCanAssignForTodayOnly);
                }

                if (!_db.tbl_Site.Any(x => x.CompanyId == companyId && x.SiteId == requestVM.SiteId && x.IsActive && !x.IsDeleted))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteDoesNotExistForCurrentCompany);
                }

                if (_db.tbl_Employee.Any(x => x.CompanyId != companyId && requestVM.WorkerList.Contains(x.EmployeeId)))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InvalidWorkersSelectedToAssign);
                }

                if (_db.tbl_Employee.Any(x => !x.IsActive && requestVM.WorkerList.Contains(x.EmployeeId)))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InactiveWorkersCanNotAssign);
                }

                if (_db.tbl_AssignWorker.Any(x => x.Date == requestVM.Date && !x.IsClosed && requestVM.WorkerList.Contains(x.EmployeeId)))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAlreadyAssigned);
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
                        assignedWorker.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        assignedWorker.ModifiedBy = employeeId;
                        assignedWorker.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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
                employeeId = base.UTI.EmployeeId;
                companyId = base.UTI.CompanyId;

                DateTime today = CommonMethod.CurrentIndianDateTime().Date;

                #region Validation
                if (requestVM.Date != today)
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

                if (_db.tbl_AssignWorker.Any(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId && x.IsClosed)
                    && !_db.tbl_AssignWorker.Any(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId && !x.IsClosed
                    ))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAlreadyClosed);
                }

                tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.Where(x => x.EmployeeId == requestVM.EmployeeId && x.AttendanceDate == requestVM.Date && !x.IsClosed).FirstOrDefault();
                if (attendanceObject == null)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAttendancePendingCanNotClose);
                }


                if (attendanceObject != null && ((attendanceObject.IsMorning && !attendanceObject.IsAfternoon) || (!attendanceObject.IsMorning && attendanceObject.IsAfternoon && !attendanceObject.IsEvening)))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAttendancePendingCanNotClose);
                }

                tbl_Employee employeeObj = _db.tbl_Employee.Where(x => x.EmployeeId == requestVM.EmployeeId).FirstOrDefault();
                if (employeeObj.EmploymentCategory == (int)EmploymentCategory.HourlyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                {
                    if (attendanceObject != null && (!attendanceObject.IsMorning || !attendanceObject.IsAfternoon || !attendanceObject.IsEvening))
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.WorkerAttendancePendingCanNotClose);
                    }
                }
                #endregion Validation

                if (!response.IsError)
                {
                    tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId && !x.IsClosed).FirstOrDefault();
                    if (assignedWorker != null)
                    {
                        assignedWorker.IsClosed = true;
                        assignedWorker.ModifiedBy = employeeId;
                        assignedWorker.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                        #region Removed this code due to this logic moved on afternoon attendance, save worker payment entry

                        /*
                        if (attendanceObject != null && (employeeObj.EmploymentCategory == (int)EmploymentCategory.DailyBased || employeeObj.EmploymentCategory == (int)EmploymentCategory.MonthlyBased))
                        {
                            tbl_WorkerPayment objWorkerPayment = new tbl_WorkerPayment();
                            objWorkerPayment.CompanyId = companyId;
                            objWorkerPayment.UserId = requestVM.EmployeeId;
                            objWorkerPayment.AttendanceId = attendanceObject.WorkerAttendanceId;
                            objWorkerPayment.PaymentDate = attendanceObject.AttendanceDate;
                            objWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                            objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                            objWorkerPayment.DebitAmount = 0;
                            objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnEveningAttendance;
                            objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                            objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                            objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerPayment.CreatedBy = employeeId;
                            objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                            objWorkerPayment.ModifiedBy = employeeId;
                            objWorkerPayment.CreditAmount = (attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (employeeObj.PerCategoryPrice) : (employeeObj.PerCategoryPrice / 2));
                            objWorkerPayment.FinancialYearId = CommonMethod.GetFinancialYearId();
                            _db.tbl_WorkerPayment.Add(objWorkerPayment);
                        }
                        */
                        #endregion

                        attendanceObject.IsClosed = true;
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
                employeeId = base.UTI.EmployeeId;
                companyId = base.UTI.CompanyId;

                #region Validation
                DateTime todayDate = CommonMethod.CurrentIndianDateTime();
                if (requestVM.Date.Date != todayDate.Date)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AssignWorkerCanRemoveForTodayOnly);
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

                if (!_db.tbl_AssignWorker.Any(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId && !x.IsClosed))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAlreadyClosed);
                }

                if (_db.tbl_WorkerAttendance.Any(x => x.EmployeeId == requestVM.EmployeeId && x.AttendanceDate == requestVM.Date && !x.IsClosed))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.WorkerAttendanceAlreadyTakenCanNotremove);
                }

                #endregion Validation

                if (!response.IsError)
                {
                    tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == requestVM.SiteId && x.Date == requestVM.Date && x.EmployeeId == requestVM.EmployeeId && !x.IsClosed).FirstOrDefault();
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

        [Route("SaveProfileImage/{id}")]
        [HttpPost]
        public ResponseDataModel<bool> SaveProfileImage(int id)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                employeeId = base.UTI.EmployeeId;
                //Create the Directory.
                string path = HttpContext.Current.Server.MapPath("~" + ErrorMessage.EmployeeDirectoryPath);
                //ErrorMessage.EmployeeDirectoryPath;
                bool folderExists = Directory.Exists(path);
                if (!folderExists)
                    Directory.CreateDirectory(path);

                //Fetch the File.
                HttpPostedFile profileImageFile = HttpContext.Current.Request.Files[0];

                if (profileImageFile != null)
                {
                    // Image file validation
                    string ext = Path.GetExtension(profileImageFile.FileName);
                    if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.SelectOnlyImage);
                    }

                    int fileNameLength = profileImageFile.FileName.Length;
                    // Save file in folder
                    string fileNameforConcate = fileNameLength > 50 ? profileImageFile.FileName.Substring(fileNameLength - 50, 50) : profileImageFile.FileName;
                    string fileName = Guid.NewGuid() + "-" + fileNameforConcate;
                    profileImageFile.SaveAs(path + fileName);

                    tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == id).FirstOrDefault();
                    if (objEmployee != null)
                    {
                        objEmployee.ProfilePicture = profileImageFile != null ? fileName : objEmployee.ProfilePicture;
                        objEmployee.UpdatedBy = employeeId;
                        objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();
                        _db.SaveChanges();
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
        [Route("GetOtherEmployeesFingerprints/{id}")]
        public ResponseDataModel<List<EmployeeFingerprintVM>> GetOtherEmployeesFingerprints(int id)
        {
            ResponseDataModel<List<EmployeeFingerprintVM>> response = new ResponseDataModel<List<EmployeeFingerprintVM>>();
            response.IsError = false;
            try
            {
                long employeeId = id;
                companyId = base.UTI.CompanyId;

                List<EmployeeFingerprintVM> lstEmployeeFingerprint = (from f in _db.tbl_EmployeeFingerprint
                                                                      join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                                                      where f.EmployeeId != employeeId
                                                                      && e.CompanyId == companyId
                                                                      select new EmployeeFingerprintVM
                                                                      {
                                                                          EmployeeFingerprintId = f.EmployeeFingerprintId,
                                                                          EmployeeId = f.EmployeeId,
                                                                          ISOCode = f.ISOCode
                                                                      }).ToList();

                response.Data = lstEmployeeFingerprint;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }
            return response;
        }

        [HttpPost]
        [Route("ValidateSiteLocationPassword")]
        public ResponseDataModel<bool> ValidateSiteLocationPassword(LocationPasswordVM passwordVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                companyId = base.UTI.CompanyId;
                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();

                #region Validation

                int loggedInUserRoleId = base.UTI.RoleId;

                if (loggedInUserRoleId != (int)AdminRoles.Supervisor)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AccessDeniedOfSiteLocation);
                }

                if (!response.IsError && string.IsNullOrEmpty(passwordVM.SiteLocationAccessPassword))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EnterSiteLocationAccessPassword);
                }

                if (!response.IsError && string.IsNullOrEmpty(objCompany.SiteLocationAccessPassword))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteLocationAccessPasswordStillNotSaved);
                }

                if (!response.IsError && objCompany != null && objCompany.SiteLocationAccessPassword != passwordVM.SiteLocationAccessPassword)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InvalidSiteLocationAccessPassword);
                }

                #endregion Validation
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }
            return response;
        }


        [HttpGet]
        [Route("GetEmployeeOfficeLocationType")]
        public ResponseDataModel<EmployeeOfficeLocationTypeVM> GetEmployeeOfficeLocationType()
        {
            ResponseDataModel<EmployeeOfficeLocationTypeVM> response = new ResponseDataModel<EmployeeOfficeLocationTypeVM>();
            response.IsError = false;

            try
            {
                employeeId = base.UTI.EmployeeId;

                EmployeeOfficeLocationTypeVM employeeDetails = (from emp in _db.tbl_Employee
                                                                where emp.EmployeeId == employeeId
                                                                select new EmployeeOfficeLocationTypeVM
                                                                {
                                                                    EmployeeId = emp.EmployeeId,
                                                                    EmployeeOfficeLocationType = emp.EmployeeOfficeLocationType
                                                                }).FirstOrDefault();
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
