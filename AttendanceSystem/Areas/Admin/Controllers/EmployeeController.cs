using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class EmployeeController : Controller
    {
        AttendanceSystemEntities _db;
        public string employeeDirectoryPath = "";
        string psSult;
        string enviornment;
        long companyId;
        long loggedInUserId;
        bool isTrailMode;

        public EmployeeController()
        {
            _db = new AttendanceSystemEntities();
            employeeDirectoryPath = ErrorMessage.EmployeeDirectoryPath;
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
            companyId = clsAdminSession.CompanyId;
            loggedInUserId = clsAdminSession.UserID;
            isTrailMode = clsAdminSession.IsTrialMode;
        }

        public ActionResult Index(int? userRole = null, int? userStatus = null)
        {
            EmployeeFilterVM employeeFilterVM = new EmployeeFilterVM();
            DateTime currentDateTime = CommonMethod.CurrentIndianDateTime();
            if (userRole.HasValue)
            {
                employeeFilterVM.UserRole = userRole.Value;
            }

            if (userStatus.HasValue)
            {
                employeeFilterVM.UserStatus = userStatus.Value;
            }

            try
            {

                employeeFilterVM.EmployeeList = (from emp in _db.tbl_Employee
                                                 join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId

                                                 join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                                 from st in state.DefaultIfEmpty()

                                                 join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                                 from dt in district.DefaultIfEmpty()

                                                 where !emp.IsDeleted && emp.CompanyId == companyId && emp.AdminRoleId != (int)AdminRoles.Worker
                                                 && (employeeFilterVM.UserRole.HasValue ? emp.AdminRoleId == employeeFilterVM.UserRole.Value : true)
                                                 && (employeeFilterVM.UserStatus.HasValue ? (employeeFilterVM.UserStatus.Value == (int)UserStatus.Active ? emp.IsActive == true : emp.IsActive == false) : true)
                                                 select new EmployeeVM
                                                 {
                                                     EmployeeId = emp.EmployeeId,
                                                     CompanyId = emp.CompanyId,
                                                     AdminRoleId = emp.AdminRoleId,
                                                     AdminRoleText = rl.AdminRoleName,
                                                     Prefix = emp.Prefix,
                                                     FirstName = emp.FirstName,
                                                     LastName = emp.LastName,
                                                     Email = emp.Email,
                                                     EmployeeCode = emp.EmployeeCode,
                                                     Password = emp.Password,
                                                     MobileNo = emp.MobileNo,
                                                     AlternateMobile = emp.AlternateMobile,
                                                     Address = emp.Address,
                                                     City = emp.City,
                                                     Pincode = emp.Pincode,
                                                     StateName = st != null ? st.StateName : "",
                                                     DistrictName = dt != null ? dt.DistrictName : "",
                                                     Designation = emp.Designation,
                                                     Dob = emp.Dob,
                                                     DateOfJoin = emp.DateOfJoin,
                                                     BloodGroup = emp.BloodGroup,
                                                     WorkingTime = emp.WorkingTime,
                                                     AdharCardNo = emp.AdharCardNo,
                                                     DateOfIdCardExpiry = emp.DateOfIdCardExpiry,
                                                     Remarks = emp.Remarks,
                                                     ProfilePicture = emp.ProfilePicture,
                                                     EmploymentCategory = emp.EmploymentCategory,
                                                     PerCategoryPrice = emp.PerCategoryPrice,
                                                     MonthlySalaryPrice = emp.MonthlySalaryPrice,
                                                     ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                                     IsLeaveForward = emp.IsLeaveForward,
                                                     IsActive = emp.IsActive,
                                                     IsDeleted = emp.IsDeleted,
                                                     IsFingerprintEnabled = emp.IsFingerprintEnabled,
                                                     NoOfFreeLeavePerMonth = emp.NoOfFreeLeavePerMonth,
                                                     EmployeeOfficeLocationType = emp.EmployeeOfficeLocationType,
                                                     TotalSavedFingerprint = _db.tbl_EmployeeFingerprint.Where(x => x.EmployeeId == emp.EmployeeId).ToList().Count

                                                 }).OrderByDescending(x => x.EmployeeId).ToList();

                List<tbl_Employee> totalEmployeeList = _db.tbl_Employee.Where(x => x.CompanyId == companyId && !x.IsDeleted).ToList();
                employeeFilterVM.NoOfEmployee = totalEmployeeList.Where(x => x.AdminRoleId != (int)AdminRoles.Worker).Count();
                employeeFilterVM.NoOfWorker = totalEmployeeList.Where(x => x.AdminRoleId == (int)AdminRoles.Worker).Count();
                employeeFilterVM.ActiveEmployee = totalEmployeeList.Where(x => x.IsActive && x.AdminRoleId != (int)AdminRoles.Worker).Count();
                employeeFilterVM.ActiveWorker = totalEmployeeList.Where(x => x.IsActive && x.AdminRoleId == (int)AdminRoles.Worker).Count();
                if (clsAdminSession.IsTrialMode)
                {
                    employeeFilterVM.IsNoOfEmployeeExceed = false;
                }
                else
                {
                    long? companyAccountPackageId = _db.tbl_Company.Where(x => x.CompanyId == companyId).Select(x => x.CurrentPackageId).FirstOrDefault();
                    clsAdminSession.CurrentAccountPackageId = companyAccountPackageId.HasValue ? companyAccountPackageId.Value : 0;

                    tbl_CompanyRenewPayment companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == companyId
                    && x.StartDate <= currentDateTime && x.EndDate >= currentDateTime
                        && (clsAdminSession.CurrentAccountPackageId > 0 ? x.CompanyRegistrationPaymentId == clsAdminSession.CurrentAccountPackageId : true)
                    && (clsAdminSession.CurrentAccountPackageId > 0 ? x.CompanyRegistrationPaymentId == clsAdminSession.CurrentAccountPackageId : true)).FirstOrDefault();
                    employeeFilterVM.NoOfEmployeeAllowed = companyPackage.NoOfEmployee + companyPackage.BuyNoOfEmployee;
                    if (employeeFilterVM.NoOfEmployeeAllowed > (employeeFilterVM.ActiveEmployee + employeeFilterVM.ActiveWorker))
                    {
                        employeeFilterVM.IsNoOfEmployeeExceed = false;
                    }
                    else
                    {
                        employeeFilterVM.IsNoOfEmployeeExceed = true;
                    }
                }
                employeeFilterVM.EmployeeList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                });
            }
            catch (Exception ex)
            {
            }

            employeeFilterVM.UserRoleList = GetUserRoleList();
            employeeFilterVM.ActiveEmployee = employeeFilterVM.EmployeeList.Where(x => x.IsActive).Count();
            return View(employeeFilterVM);
        }

        public ActionResult Add(long id)
        {
            EmployeeVM employeeVM = new EmployeeVM();
            if (id > 0)
            {
                employeeVM = (from emp in _db.tbl_Employee
                              join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId

                              join st in _db.tbl_State on emp.StateId equals st.StateId into state
                              from st in state.DefaultIfEmpty()

                              join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                              from dt in district.DefaultIfEmpty()

                              where !emp.IsDeleted && emp.EmployeeId == id
                              select new EmployeeVM
                              {
                                  EmployeeId = emp.EmployeeId,
                                  CompanyId = emp.CompanyId,
                                  AdminRoleId = emp.AdminRoleId,
                                  AdminRoleText = rl.AdminRoleName,
                                  Prefix = emp.Prefix,
                                  FirstName = emp.FirstName,
                                  LastName = emp.LastName,
                                  Email = emp.Email,
                                  EmployeeCode = emp.EmployeeCode,
                                  Password = emp.Password,
                                  MobileNo = emp.MobileNo,
                                  AlternateMobile = emp.AlternateMobile,
                                  Address = emp.Address,
                                  City = emp.City,
                                  Pincode = emp.Pincode,
                                  StateName = st != null ? st.StateName : "",
                                  DistrictName = dt != null ? dt.DistrictName : "",
                                  StateId = emp.StateId,
                                  DistrictId = emp.DistrictId,
                                  Designation = emp.Designation,
                                  Dob = emp.Dob,
                                  DateOfJoin = emp.DateOfJoin,
                                  BloodGroup = emp.BloodGroup,
                                  WorkingTime = emp.WorkingTime,
                                  AdharCardNo = emp.AdharCardNo,
                                  DateOfIdCardExpiry = emp.DateOfIdCardExpiry,
                                  Remarks = emp.Remarks,
                                  ProfilePicture = emp.ProfilePicture,
                                  EmploymentCategory = emp.EmploymentCategory,
                                  PerCategoryPrice = emp.PerCategoryPrice,
                                  MonthlySalaryPrice = emp.MonthlySalaryPrice,
                                  ExtraPerHourPrice = emp.ExtraPerHourPrice,
                                  IsLeaveForward = emp.IsLeaveForward,
                                  IsActive = emp.IsActive,
                                  IsDeleted = emp.IsDeleted,
                                  IsFingerprintEnabled = emp.IsFingerprintEnabled,
                                  NoOfFreeLeavePerMonth = emp.NoOfFreeLeavePerMonth,
                                  CarryForwardLeave = emp.CarryForwardLeave,
                                  EmployeeOfficeLocationType = emp.EmployeeOfficeLocationType
                              }).FirstOrDefault();
                employeeVM.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)employeeVM.EmploymentCategory);
                employeeVM.AdminRoleText = CommonMethod.GetEnumDescription((AdminRoles)employeeVM.AdminRoleId);

                if (employeeVM.StateId != null)
                {
                    employeeVM.DistrictList = GetDistrictListByStateId(employeeVM.StateId.Value);
                }

            }
            else
            {
                employeeVM.EmployeeOfficeLocationType = 1; // Anywhere
            }

            if (employeeVM.DistrictList == null)
            {
                employeeVM.DistrictList = new List<SelectListItem>();
            }

            employeeVM.UserRoleList = GetUserRoleList();
            employeeVM.StateList = CommonMethod.GetStateListOfIndia();

            List<EmployeeOfficeLocationVM> AssignedEmployeeLocationList = GetAssignedEmployeeLocationList(employeeVM.EmployeeId);
            ViewData["AssignedEmployeeLocationList"] = AssignedEmployeeLocationList;

            return View(employeeVM);
        }

        [HttpPost]
        public ActionResult Add(EmployeeVM employeeVM, HttpPostedFileBase profileImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    string fileName = string.Empty;
                    string path = Server.MapPath(employeeDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (profileImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(profileImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("ProfileImageFile", ErrorMessage.SelectOnlyImage);
                            return View(employeeVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(profileImageFile.FileName);
                        profileImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        if (employeeVM.EmployeeId == 0)
                        {
                            employeeVM.UserRoleList = GetUserRoleList();
                            ModelState.AddModelError("ProfileImageFile", ErrorMessage.ImageRequired);
                            return View(employeeVM);
                        }
                    }

                    if (employeeVM.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                    {
                        int MaximumFreeLeavePerMonth = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumFreeLeavePerMonth"]);
                        if (employeeVM.NoOfFreeLeavePerMonth > MaximumFreeLeavePerMonth)
                        {
                            employeeVM.UserRoleList = GetUserRoleList();
                            ModelState.AddModelError("NoOfFreeLeavePerMonth", ErrorMessage.Maximum10FreeLeavePerMonthAllowed);
                            return View(employeeVM);
                        }
                    }

                    if (employeeVM.EmployeeId > 0)
                    {
                        tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeVM.EmployeeId).FirstOrDefault();
                        objEmployee.ProfilePicture = profileImageFile != null ? fileName : objEmployee.ProfilePicture;
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = CommonMethod.SentenceCase(employeeVM.FirstName);
                        objEmployee.LastName = CommonMethod.SentenceCase(employeeVM.LastName);
                        objEmployee.Email = employeeVM.Email;
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.AlternateMobile = employeeVM.AlternateMobile;
                        objEmployee.Address = CommonMethod.SentenceCase(employeeVM.Address);
                        objEmployee.City = CommonMethod.SentenceCase(employeeVM.City);
                        objEmployee.Pincode = employeeVM.Pincode;
                        objEmployee.StateId = employeeVM.StateId;
                        objEmployee.DistrictId = employeeVM.DistrictId;
                        objEmployee.Designation = CommonMethod.SentenceCase(employeeVM.Designation);
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.WorkingTime = employeeVM.WorkingTime;
                        objEmployee.AdharCardNo = CommonMethod.UpperCase(employeeVM.AdharCardNo);
                        objEmployee.DateOfIdCardExpiry = employeeVM.DateOfIdCardExpiry;
                        objEmployee.Remarks = CommonMethod.SentenceCase(employeeVM.Remarks);
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.IsFingerprintEnabled = employeeVM.IsFingerprintEnabled;
                        objEmployee.NoOfFreeLeavePerMonth = employeeVM.NoOfFreeLeavePerMonth;
                        objEmployee.EmployeeOfficeLocationType = employeeVM.EmployeeOfficeLocationType;
                        objEmployee.UpdatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();

                        _db.SaveChanges();

                        #region Assign selected office locations

                        if (employeeVM.EmployeeOfficeLocationType == (int)EmployeeOfficeLocationType.SelectedOffice && !string.IsNullOrEmpty(employeeVM.strSelectedOfficeLocations))
                        {
                            List<string> lstSelectedOfficeLocations = employeeVM.strSelectedOfficeLocations.Split(',').ToList<string>();

                            long x = 0;
                            var longSelectedOfficeLocationsList = lstSelectedOfficeLocations.Where(str => long.TryParse(str, out x)).Select(str => x).ToList();

                            // delete unselected locations
                            List<tbl_EmployeeOfficeLocation> lstUnSelectedEmployeeOfficeLocations = _db.tbl_EmployeeOfficeLocation.Where(p => !longSelectedOfficeLocationsList.Contains(p.OfficeLocationId)).ToList();
                            if (lstUnSelectedEmployeeOfficeLocations != null && lstUnSelectedEmployeeOfficeLocations.Count > 0)
                            {
                                _db.tbl_EmployeeOfficeLocation.RemoveRange(lstUnSelectedEmployeeOfficeLocations);
                                _db.SaveChanges();
                            }

                            // Save missing locations
                            lstSelectedOfficeLocations.ForEach(strOfficeLocationId =>
                            {
                                long officeLocationId = Convert.ToInt64(strOfficeLocationId);

                                tbl_EmployeeOfficeLocation objDuplicateLocation = _db.tbl_EmployeeOfficeLocation.Where(l => l.EmployeeId == objEmployee.EmployeeId && l.OfficeLocationId == officeLocationId).FirstOrDefault();

                                if (objDuplicateLocation == null)
                                {
                                    tbl_EmployeeOfficeLocation employeeOfficeLocationVM = new tbl_EmployeeOfficeLocation();
                                    employeeOfficeLocationVM.EmployeeId = objEmployee.EmployeeId;
                                    employeeOfficeLocationVM.OfficeLocationId = officeLocationId;
                                    employeeOfficeLocationVM.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                                    employeeOfficeLocationVM.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    _db.tbl_EmployeeOfficeLocation.Add(employeeOfficeLocationVM);
                                    _db.SaveChanges();
                                }
                            });

                        }
                        else
                        {
                            // Remove old locations, if exists
                            List<tbl_EmployeeOfficeLocation> lstEmployeeOfficeLocation = _db.tbl_EmployeeOfficeLocation.Where(x => x.EmployeeId == objEmployee.EmployeeId).ToList();
                            if (lstEmployeeOfficeLocation != null && lstEmployeeOfficeLocation.Count > 0)
                            {
                                _db.tbl_EmployeeOfficeLocation.RemoveRange(lstEmployeeOfficeLocation);
                                _db.SaveChanges();
                            }
                        }

                        #endregion

                    }
                    else
                    {
                        int noOfEmployee = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyRegistrationPaymentId == clsAdminSession.CurrentAccountPackageId).Select(x => x.NoOfEmployee + x.BuyNoOfEmployee).FirstOrDefault();
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
                        objEmployee.ProfilePicture = fileName;
                        objEmployee.CompanyId = companyId;
                        objEmployee.Password = CommonMethod.Encrypt(randomPassword, psSult);
                        objEmployee.AdminRoleId = employeeVM.AdminRoleId;
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = CommonMethod.SentenceCase(employeeVM.FirstName);
                        objEmployee.LastName = CommonMethod.SentenceCase(employeeVM.LastName);
                        objEmployee.Email = employeeVM.Email;
                        objEmployee.EmployeeCode = CommonMethod.getEmployeeCodeFormat(companyId, objCompany.CompanyName, empCount.Count());
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.AlternateMobile = employeeVM.AlternateMobile;
                        objEmployee.Address = CommonMethod.SentenceCase(employeeVM.Address);
                        objEmployee.City = CommonMethod.SentenceCase(employeeVM.City);
                        objEmployee.Pincode = employeeVM.Pincode;
                        objEmployee.StateId = employeeVM.StateId;
                        objEmployee.DistrictId = employeeVM.DistrictId;
                        objEmployee.Designation = CommonMethod.SentenceCase(employeeVM.Designation);
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.WorkingTime = employeeVM.WorkingTime;
                        objEmployee.AdharCardNo = CommonMethod.UpperCase(employeeVM.AdharCardNo);
                        objEmployee.DateOfIdCardExpiry = employeeVM.DateOfIdCardExpiry;
                        objEmployee.Remarks = CommonMethod.SentenceCase(employeeVM.Remarks);
                        objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.NoOfFreeLeavePerMonth = employeeVM.NoOfFreeLeavePerMonth;
                        objEmployee.IsActive = isTrailMode ? true : (activeEmployee >= noOfEmployee ? false : true);
                        objEmployee.IsFingerprintEnabled = employeeVM.IsFingerprintEnabled;
                        objEmployee.EmployeeOfficeLocationType = employeeVM.EmployeeOfficeLocationType;
                        objEmployee.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objEmployee.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objEmployee.UpdatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_Employee.Add(objEmployee);
                        _db.SaveChanges();

                        #region Assign selected office locations WHILE CREATE NEW EMPLOYEE

                        if (employeeVM.EmployeeOfficeLocationType == (int)EmployeeOfficeLocationType.SelectedOffice && !string.IsNullOrEmpty(employeeVM.strSelectedOfficeLocations))
                        {
                            List<string> lstSelectedOfficeLocations = employeeVM.strSelectedOfficeLocations.Split(',').ToList<string>();

                            lstSelectedOfficeLocations.ForEach(strOfficeLocationId =>
                            {
                                long officeLocationId = Convert.ToInt64(strOfficeLocationId);

                                tbl_EmployeeOfficeLocation objDuplicateLocation = _db.tbl_EmployeeOfficeLocation.Where(x => x.EmployeeId == objEmployee.EmployeeId && x.OfficeLocationId == officeLocationId).FirstOrDefault();

                                if (objDuplicateLocation == null)
                                {
                                    tbl_EmployeeOfficeLocation employeeOfficeLocationVM = new tbl_EmployeeOfficeLocation();
                                    employeeOfficeLocationVM.EmployeeId = objEmployee.EmployeeId;
                                    employeeOfficeLocationVM.OfficeLocationId = officeLocationId;
                                    employeeOfficeLocationVM.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                                    employeeOfficeLocationVM.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    _db.tbl_EmployeeOfficeLocation.Add(employeeOfficeLocationVM);
                                    _db.SaveChanges();
                                }
                            });
                        }

                        #endregion

                        #region Send SMS of Create Employee

                        var json = string.Empty;
                        int SmsId = (int)SMSType.EmployeeCreate;
                        string msg = CommonMethod.GetSmsContent(SmsId);

                        string employmentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)objEmployee.EmploymentCategory);
                        string salaryText = Convert.ToString(objEmployee.PerCategoryPrice);

                        if (objEmployee.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                        {
                            salaryText = salaryText + ErrorMessage.PerMonth;
                        }
                        else if (objEmployee.EmploymentCategory == (int)EmploymentCategory.DailyBased)
                        {
                            salaryText = salaryText + ErrorMessage.PerDay;
                        }
                        else if (objEmployee.EmploymentCategory == (int)EmploymentCategory.HourlyBased)
                        {
                            salaryText = salaryText + ErrorMessage.PerHour;
                        }
                        else if (objEmployee.EmploymentCategory == (int)EmploymentCategory.UnitBased)
                        {
                            salaryText = salaryText + ErrorMessage.PerUnit;
                        }

                        Regex regReplace = new Regex("{#var#}");
                        msg = regReplace.Replace(msg, objEmployee.FirstName + " " + objEmployee.LastName, 1);
                        msg = regReplace.Replace(msg, objCompany.CompanyName, 1);
                        msg = regReplace.Replace(msg, objEmployee.EmployeeCode, 1);
                        msg = regReplace.Replace(msg, randomPassword, 1);
                        msg = regReplace.Replace(msg, salaryText, 1);
                        msg = regReplace.Replace(msg, employmentCategoryText, 1);
                        msg = msg.Replace("\r\n", "\n");

                        ResponseDataModel<string> smsResponse = CommonMethod.SendSMS(msg, objEmployee.MobileNo, objEmployee.CompanyId, objEmployee.EmployeeId, objEmployee.EmployeeCode, (int)PaymentGivenBy.CompanyAdmin, isTrailMode);
                        //CommonMethod.SendSMS(msg, objEmployee.MobileNo);

                        #endregion

                    }

                }
                else
                {
                    employeeVM.UserRoleList = GetUserRoleList();
                    return View(employeeVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return RedirectToAction("Index");
        }

        public List<EmployeeOfficeLocationVM> GetAssignedEmployeeLocationList(long employeeId)
        {
            List<EmployeeOfficeLocationVM> list = null;
            try
            {
                if (employeeId == 0)
                {
                    list = (from o in _db.tbl_OfficeLocation
                            where o.IsActive && !o.IsDeleted
                            && o.CompanyId == companyId
                            select new EmployeeOfficeLocationVM
                            {
                                OfficeLocationId = o.OfficeLocationId,
                                OfficeLocationName = o.OfficeLocationName,
                                EmployeeId = o.OfficeLocationId
                            }).OrderBy(x => x.OfficeLocationName).ToList();
                }
                else
                {
                    list = (from o in _db.tbl_OfficeLocation
                            join l in _db.tbl_EmployeeOfficeLocation.Where(x => x.EmployeeId == employeeId) on o.OfficeLocationId equals l.OfficeLocationId into location
                            from l in location.DefaultIfEmpty()
                            where o.IsActive && !o.IsDeleted && o.CompanyId == companyId
                            select new EmployeeOfficeLocationVM
                            {
                                OfficeLocationId = o.OfficeLocationId,
                                OfficeLocationName = o.OfficeLocationName,
                                EmployeeId = o.OfficeLocationId,
                                IsAssigned = l != null ? true : false
                            }).OrderBy(x => x.OfficeLocationName).ToList();
                }

            }
            catch (Exception ex)
            {
            }
            return list;
        }

        public ActionResult View(int Id)
        {
            EmployeeVM employeeVM = new EmployeeVM();

            employeeVM = (from emp in _db.tbl_Employee
                          join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId

                          join st in _db.tbl_State on emp.StateId equals st.StateId into state
                          from st in state.DefaultIfEmpty()

                          join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                          from dt in district.DefaultIfEmpty()

                          where !emp.IsDeleted && emp.EmployeeId == Id
                          select new EmployeeVM
                          {
                              EmployeeId = emp.EmployeeId,
                              CompanyId = emp.CompanyId,
                              AdminRoleId = emp.AdminRoleId,
                              AdminRoleText = rl.AdminRoleName,
                              Prefix = emp.Prefix,
                              FirstName = emp.FirstName,
                              LastName = emp.LastName,
                              Email = emp.Email,
                              EmployeeCode = emp.EmployeeCode,
                              Password = emp.Password,
                              MobileNo = emp.MobileNo,
                              AlternateMobile = emp.AlternateMobile,
                              Address = emp.Address,
                              City = emp.City,
                              StateName = st != null ? st.StateName : "",
                              DistrictName = dt != null ? dt.DistrictName : "",
                              Designation = emp.Designation,
                              Dob = emp.Dob,
                              DateOfJoin = emp.DateOfJoin,
                              BloodGroup = emp.BloodGroup,
                              WorkingTime = emp.WorkingTime,
                              AdharCardNo = emp.AdharCardNo,
                              DateOfIdCardExpiry = emp.DateOfIdCardExpiry,
                              Remarks = emp.Remarks,
                              ProfilePicture = emp.ProfilePicture,
                              EmploymentCategory = emp.EmploymentCategory,
                              PerCategoryPrice = emp.PerCategoryPrice,
                              MonthlySalaryPrice = emp.MonthlySalaryPrice,
                              ExtraPerHourPrice = emp.ExtraPerHourPrice,
                              IsLeaveForward = emp.IsLeaveForward,
                              IsActive = emp.IsActive,
                              IsDeleted = emp.IsDeleted,
                              IsFingerprintEnabled = emp.IsFingerprintEnabled,
                              NoOfFreeLeavePerMonth = emp.NoOfFreeLeavePerMonth,
                              CarryForwardLeave = emp.CarryForwardLeave
                          }).FirstOrDefault();
            employeeVM.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)employeeVM.EmploymentCategory);

            // Get Fingerprint List
            List<EmployeeFingerprintVM> lstFingerprint = (from f in _db.tbl_EmployeeFingerprint
                                                          where f.EmployeeId == Id
                                                          select new EmployeeFingerprintVM
                                                          {
                                                              EmployeeFingerprintId = f.EmployeeFingerprintId,
                                                              EmployeeId = f.EmployeeId,
                                                              BitmapCode = f.BitmapCode,
                                                              ISOCode = f.ISOCode,
                                                              CreatedDate = f.CreatedDate
                                                          }).ToList();

            ViewData["lstFingerprint"] = lstFingerprint;

            return View(employeeVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == Id).FirstOrDefault();

                if (objEmployee != null)
                {
                    if (Status == "Active")
                    {
                        objEmployee.IsActive = true;
                    }
                    else
                    {
                        objEmployee.IsActive = false;
                    }

                    objEmployee.UpdatedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();

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

        [HttpPost]
        public string DeleteEmployee(int EmployeeId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();

                if (objEmployee == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    objEmployee.IsDeleted = true;
                    objEmployee.UpdatedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();
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

        private List<SelectListItem> GetUserRoleList()
        {
            long[] adminRole = new long[] { (long)AdminRoles.CompanyAdmin, (long)AdminRoles.SuperAdmin, (long)AdminRoles.Worker };

            if (clsAdminSession.CompanyTypeId == (int)CompanyType.Banking_OfficeCompany)
            {
                adminRole = new long[] { (long)AdminRoles.CompanyAdmin, (long)AdminRoles.SuperAdmin, (long)AdminRoles.Supervisor, (long)AdminRoles.Checker, (long)AdminRoles.Payer, (long)AdminRoles.Worker };
            }
            List<SelectListItem> lst = (from ms in _db.mst_AdminRole
                                        where !adminRole.Contains(ms.AdminRoleId)
                                        select new SelectListItem
                                        {
                                            Text = ms.AdminRoleName,
                                            Value = ms.AdminRoleId.ToString()
                                        }).ToList();
            return lst;
        }

        public JsonResult VerifyMobileNo(string mobileNo, string fullname)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string otp = string.Empty;
            try
            {
                #region Send SMS

                Random random = new Random();
                int num = random.Next(555555, 999999);

                int SmsId = (int)SMSType.EmployeeCreateOTP;
                string msg = CommonMethod.GetSmsContent(SmsId);

                Regex regReplace = new Regex("{#var#}");
                //msg = regReplace.Replace(msg, fullname, 1);
                msg = regReplace.Replace(msg, num.ToString(), 1);

                msg = msg.Replace("\r\n", "\n");

                //var json = CommonMethod.SendSMSWithoutLog(msg, mobileNo);

                ResponseDataModel<string> smsResponse = CommonMethod.SendSMS(msg, mobileNo, companyId, 0, "", (int)PaymentGivenBy.CompanyAdmin, isTrailMode);

                if (smsResponse.Data != null && smsResponse.Data.Contains("invalidnumber"))
                {
                    status = 0;
                    errorMessage = ErrorMessage.InvalidMobileNo;
                }
                else
                {
                    status = 1;
                    otp = num.ToString();
                }

                #endregion

            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, Otp = otp, ErrorMessage = errorMessage, SetOtp = clsAdminSession.SetOtp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginHistory(int? employeeId = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            LoginHistoryFilterVM loginHistoryFilterVM = new LoginHistoryFilterVM();

            try
            {
                if (employeeId.HasValue)
                {
                    loginHistoryFilterVM.EmployeeId = employeeId.Value;
                }


                if (startDate.HasValue && endDate.HasValue)
                {
                    loginHistoryFilterVM.StartDate = startDate.Value;
                    loginHistoryFilterVM.EndDate = endDate.Value;
                }


                long companyId = clsAdminSession.CompanyId;
                loginHistoryFilterVM.LoginHistoryList = (from lh in _db.tbl_LoginHistory
                                                         join emp in _db.tbl_Employee on lh.EmployeeId equals emp.EmployeeId
                                                         where !emp.IsDeleted && emp.CompanyId == companyId
                                                         && DbFunctions.TruncateTime(lh.LoginDate) >= DbFunctions.TruncateTime(loginHistoryFilterVM.StartDate)
                                                         && DbFunctions.TruncateTime(lh.LoginDate) <= DbFunctions.TruncateTime(loginHistoryFilterVM.EndDate)
                                                         && (loginHistoryFilterVM.EmployeeId.HasValue ? lh.EmployeeId == loginHistoryFilterVM.EmployeeId.Value : true)
                                                         select new LoginHistoryVM
                                                         {
                                                             LoginHistoryId = lh.LoginHistoryId,
                                                             EmployeeId = lh.EmployeeId,
                                                             EmployeeCode = emp.EmployeeCode,
                                                             Prefix = emp.Prefix,
                                                             FirstName = emp.FirstName,
                                                             LastName = emp.LastName,
                                                             LoginDate = lh.LoginDate,
                                                             LocationFrom = lh.LocationFrom,
                                                             SiteId = lh.SiteId

                                                         }).OrderByDescending(x => x.LoginDate).ToList();
            }
            catch (Exception ex)
            {
            }
            loginHistoryFilterVM.EmployeeList = GetEmployeeList();
            return View(loginHistoryFilterVM);
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

        [HttpPost]
        public string DeleteEmployeeFingerprint(int EmployeeFingerprintId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_EmployeeFingerprint objEmployeeFingerprint = _db.tbl_EmployeeFingerprint.Where(x => x.EmployeeFingerprintId == EmployeeFingerprintId).FirstOrDefault();

                if (objEmployeeFingerprint == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_EmployeeFingerprint.Remove(objEmployeeFingerprint);
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

        public JsonResult GetOtherEmployeesFingerprints(int employeeId)
        {
            List<EmployeeFingerprintVM> lstEmployeeFingerprint = new List<EmployeeFingerprintVM>();
            bool isSuccess = false;
            try
            {
                lstEmployeeFingerprint = (from f in _db.tbl_EmployeeFingerprint
                                          join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                          where f.EmployeeId != employeeId
                                          //&& e.AdminRoleId != (int)AdminRoles.Worker
                                          && e.CompanyId == companyId
                                          select new EmployeeFingerprintVM
                                          {
                                              EmployeeFingerprintId = f.EmployeeFingerprintId,
                                              EmployeeId = f.EmployeeId,
                                              ISOCode = f.ISOCode
                                          }).ToList();

                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, data = lstEmployeeFingerprint }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetTotalPendingFingerprint(int employeeId)
        {
            int totalPendingFingerprintCount = 0;
            try
            {
                int maximumEmployeeFingerprint = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumEmployeeFingerprint"].ToString());

                List<tbl_EmployeeFingerprint> lstExistFingerprint = _db.tbl_EmployeeFingerprint.Where(x => x.EmployeeId == employeeId).ToList();
                if (lstExistFingerprint != null)
                {
                    totalPendingFingerprintCount = maximumEmployeeFingerprint - lstExistFingerprint.Count;
                }
            }
            catch (Exception ex)
            {
            }
            return Json(new { TotalPendingFingerprintCount = totalPendingFingerprintCount }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveEmployeeFingerprints(string fingerprintData)
        {
            string errorMessage = string.Empty;
            bool isSuccess = false;
            int totalPendingFingerprintCount = 0;
            int maximumEmployeeFingerprint = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumEmployeeFingerprint"].ToString());

            try
            {
                FingerprintSaveVM fingerprintDataVM = JsonConvert.DeserializeObject<FingerprintSaveVM>(fingerprintData);

                if (fingerprintDataVM != null && fingerprintDataVM.EmployeeId > 0 && fingerprintDataVM.FingerprintTemplateList != null && fingerprintDataVM.FingerprintTemplateList.Count > 0)
                {
                    fingerprintDataVM.FingerprintTemplateList.ForEach(fingerprint =>
                    {

                        List<tbl_EmployeeFingerprint> lstExistFingerprint = _db.tbl_EmployeeFingerprint.Where(x => x.EmployeeId == fingerprintDataVM.EmployeeId).ToList();
                        if (lstExistFingerprint != null)
                        {
                            totalPendingFingerprintCount = maximumEmployeeFingerprint - lstExistFingerprint.Count;
                        }

                        if (totalPendingFingerprintCount > 0)
                        {
                            tbl_EmployeeFingerprint objFingerprint = new tbl_EmployeeFingerprint();
                            objFingerprint.BitmapCode = fingerprint.BitmapCode;
                            objFingerprint.EmployeeId = fingerprintDataVM.EmployeeId;
                            objFingerprint.ISOCode = fingerprint.ISOCode;
                            objFingerprint.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            _db.tbl_EmployeeFingerprint.Add(objFingerprint);
                            _db.SaveChanges();
                        }
                        isSuccess = true;
                    });
                }

            }
            catch (Exception ex)
            {
                errorMessage = "exception";
                isSuccess = false;
            }

            return Json(new { IsSuccess = isSuccess, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetAllEmployeesFingerprintList()
        {
            List<EmployeeFingerprintDetailVM> lstEmployeeFingerprint = new List<EmployeeFingerprintDetailVM>();
            bool isSuccess = false;
            try
            {
                lstEmployeeFingerprint = (from f in _db.tbl_EmployeeFingerprint
                                          join e in _db.tbl_Employee on f.EmployeeId equals e.EmployeeId
                                          join r in _db.mst_AdminRole on e.AdminRoleId equals r.AdminRoleId
                                          where e.CompanyId == companyId
                                          select new EmployeeFingerprintDetailVM
                                          {
                                              EmployeeFingerprintId = f.EmployeeFingerprintId,
                                              EmployeeId = f.EmployeeId,
                                              ISOCode = f.ISOCode,
                                              BitmapCode = f.BitmapCode,
                                              EmployeeCode = e.EmployeeCode,
                                              EmployeeName = e.Prefix + " " + e.FirstName + " " + e.LastName,
                                              EmployeeRole = r.AdminRoleName
                                          }).ToList();

                isSuccess = true;
            }
            catch (Exception ex)
            {
                isSuccess = false;
            }
            return Json(new { IsSuccess = isSuccess, data = lstEmployeeFingerprint }, JsonRequestBehavior.AllowGet);
        }

        public List<SelectListItem> GetDistrictListByStateId(long StateId)
        {
            List<SelectListItem> lstDistricts = new List<SelectListItem>();

            List<SelectListItem> lst = (from st in _db.tbl_District
                                        where !st.IsDeleted && st.IsActive
                                        && st.StateId == StateId
                                        select new SelectListItem
                                        {
                                            Text = st.DistrictName,
                                            Value = st.DistrictId.ToString()
                                        }).OrderBy(x => x.Text).ToList();

            lstDistricts = lst != null ? lst : lstDistricts;

            return lstDistricts;
        }

        public JsonResult GeAjaxtDistrictListByStateId(long Id)
        {
            var DistrictList = _db.tbl_District.Where(x => (x.StateId == Id) && x.IsActive && !x.IsDeleted)
                         .Select(o => new SelectListItem { Value = SqlFunctions.StringConvert((double)o.DistrictId).Trim(), Text = o.DistrictName })
                         .OrderBy(x => x.Text).ToList();

            return Json(DistrictList, JsonRequestBehavior.AllowGet);
        }

    }
}