﻿using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class WorkerController : Controller
    {
        AttendanceSystemEntities _db;
        public string employeeDirectoryPath = "";
        string psSult;
        long companyId;
        long loggedInUserId;
        bool isTrailMode;

        public WorkerController()
        {
            _db = new AttendanceSystemEntities();
            employeeDirectoryPath = ErrorMessage.EmployeeDirectoryPath;
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            companyId = clsAdminSession.CompanyId;
            loggedInUserId = clsAdminSession.UserID;
            isTrailMode = clsAdminSession.IsTrialMode;
        }
        public ActionResult Index(int? userStatus = null, long? workerHeadId = null)
        {
            EmployeeFilterVM employeeFilterVM = new EmployeeFilterVM();
            DateTime currentDateTime = CommonMethod.CurrentIndianDateTime();
            if (userStatus.HasValue)
            {
                employeeFilterVM.UserStatus = userStatus.Value;
            }

            if (workerHeadId.HasValue)
            {
                employeeFilterVM.WorkerHeadId = workerHeadId.Value;
            }

            try
            {

                employeeFilterVM.EmployeeList = (from emp in _db.tbl_Employee
                                                 join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId
                                                 join wt in _db.tbl_WorkerType on emp.WorkerTypeId equals wt.WorkerTypeId
                                                 into wtc
                                                 from w in wtc.DefaultIfEmpty()

                                                 join st in _db.tbl_State on emp.StateId equals st.StateId into state
                                                 from st in state.DefaultIfEmpty()

                                                 join dt in _db.tbl_District on emp.DistrictId equals dt.DistrictId into district
                                                 from dt in district.DefaultIfEmpty()

                                                 join whead in _db.tbl_WorkerHead on emp.WorkerHeadId equals whead.WorkerHeadId into outerWorkerHead
                                                 from whead in outerWorkerHead.DefaultIfEmpty()

                                                 where !emp.IsDeleted && emp.CompanyId == companyId && emp.AdminRoleId == (int)AdminRoles.Worker
                                                 && (employeeFilterVM.UserStatus.HasValue ? (employeeFilterVM.UserStatus.Value == (int)UserStatus.Active ? emp.IsActive == true : emp.IsActive == false) : true)
                                                 && (employeeFilterVM.WorkerHeadId.HasValue ? emp.WorkerHeadId == employeeFilterVM.WorkerHeadId.Value : true)
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
                                                     WorkerTypeId = emp.WorkerTypeId,
                                                     WorkerTypeText = w.WorkerTypeName,
                                                     TotalSavedFingerprint = _db.tbl_EmployeeFingerprint.Where(x => x.EmployeeId == emp.EmployeeId).ToList().Count,
                                                     WorkerHeadName = whead != null ? whead.HeadName : string.Empty,
                                                     WorkerHeadId = emp.WorkerHeadId
                                                 }).OrderByDescending(x => x.EmployeeId).ToList();

                employeeFilterVM.EmployeeList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                });

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

                employeeFilterVM.WorkerHeadList = GetWorkerHeadList();
                 
            }
            catch (Exception ex)
            {
            }
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
                                  WorkerTypeId = emp.WorkerTypeId,
                                  WorkerHeadId = emp.WorkerHeadId
                              }).FirstOrDefault();

                if (employeeVM.StateId != null)
                {
                    employeeVM.DistrictList = CommonMethod.GetDistrictListByStateId(employeeVM.StateId.Value);
                }
            }

            if (employeeVM.DistrictList == null)
            {
                employeeVM.DistrictList = new List<SelectListItem>();
            }

            employeeVM.WorkerTypeList = GetWorkerTypeList();
            employeeVM.WorkerHeadList = GetWorkerHeadList();
            employeeVM.StateList = CommonMethod.GetStateListOfIndia();
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
                    #region Upload Profile Image

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
                            ModelState.AddModelError("ProfileImageFile", ErrorMessage.ImageRequired);
                            return View(employeeVM);
                        }
                    }

                    #endregion

                    #region Validation

                    if (employeeVM.EmploymentCategory == (int)EmploymentCategory.MonthlyBased)
                    {
                        int MaximumFreeLeavePerMonth = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumFreeLeavePerMonth"]);
                        if (employeeVM.NoOfFreeLeavePerMonth > MaximumFreeLeavePerMonth)
                        {
                            ModelState.AddModelError("NoOfFreeLeavePerMonth", ErrorMessage.Maximum10FreeLeavePerMonthAllowed);
                            return View(employeeVM);
                        }
                    }

                    #endregion

                    if (employeeVM.EmployeeId > 0)
                    {
                        tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeVM.EmployeeId).FirstOrDefault();
                        objEmployee.ProfilePicture = profileImageFile != null ? fileName : objEmployee.ProfilePicture;
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = CommonMethod.SentenceCase(employeeVM.FirstName);
                        objEmployee.LastName = CommonMethod.SentenceCase(employeeVM.LastName);
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.Address = CommonMethod.SentenceCase(employeeVM.Address);
                        objEmployee.City = CommonMethod.SentenceCase(employeeVM.City);
                        objEmployee.Pincode = employeeVM.Pincode;
                        objEmployee.StateId = employeeVM.StateId;
                        objEmployee.DistrictId = employeeVM.DistrictId;
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.AdharCardNo = CommonMethod.SentenceCase(employeeVM.AdharCardNo);
                        objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                        objEmployee.WorkerTypeId = employeeVM.WorkerTypeId;
                        objEmployee.WorkerHeadId = employeeVM.WorkerHeadId;
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.NoOfFreeLeavePerMonth = 0; // employeeVM.NoOfFreeLeavePerMonth;
                        objEmployee.UpdatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();
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
                        objEmployee.AdminRoleId = (int)AdminRoles.Worker;
                        objEmployee.Password = CommonMethod.Encrypt(randomPassword, psSult);
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = CommonMethod.SentenceCase(employeeVM.FirstName);
                        objEmployee.LastName = CommonMethod.SentenceCase(employeeVM.LastName);
                        objEmployee.EmployeeCode = CommonMethod.getEmployeeCodeFormat(companyId, objCompany.CompanyName, empCount.Count());
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.Address = CommonMethod.SentenceCase(employeeVM.Address);
                        objEmployee.City = CommonMethod.SentenceCase(employeeVM.City);
                        objEmployee.Pincode = employeeVM.Pincode;
                        objEmployee.StateId = employeeVM.StateId;
                        objEmployee.DistrictId = employeeVM.DistrictId;
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.AdharCardNo = CommonMethod.SentenceCase(employeeVM.AdharCardNo);
                        objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.WorkerTypeId = employeeVM.WorkerTypeId;
                        objEmployee.NoOfFreeLeavePerMonth = 0; // employeeVM.NoOfFreeLeavePerMonth;
                        objEmployee.IsActive = isTrailMode ? true : (activeEmployee >= noOfEmployee ? false : true);
                        objEmployee.WorkerHeadId = employeeVM.WorkerHeadId;
                        objEmployee.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objEmployee.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objEmployee.UpdatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objEmployee.UpdatedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_Employee.Add(objEmployee);
                    }
                    _db.SaveChanges();
                }
                else
                {
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

        public ActionResult View(int Id)
        {
            EmployeeVM employeeVM = new EmployeeVM();

            employeeVM = (from emp in _db.tbl_Employee
                          join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId
                          join wt in _db.tbl_WorkerType on emp.WorkerTypeId equals wt.WorkerTypeId into wtc
                          from w in wtc.DefaultIfEmpty()

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
                              Pincode = emp.Pincode,
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
                              WorkerTypeId = emp.WorkerTypeId,
                              WorkerTypeText = w.WorkerTypeName
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
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == Id).FirstOrDefault();
                bool isAssigned = _db.tbl_AssignWorker.Any(x => x.EmployeeId == Id && x.Date == today);

                if (isAssigned)
                {
                    ReturnMessage = "AlreadyAssigned";
                }
                else if (objEmployee != null)
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
            DateTime today = CommonMethod.CurrentIndianDateTime().Date;
            try
            {
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();
                bool isAssigned = _db.tbl_AssignWorker.Any(x => x.EmployeeId == EmployeeId && x.Date == today);

                if (isAssigned)
                {
                    ReturnMessage = "AlreadyAssigned";
                }
                else if (objEmployee == null)
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

        private List<SelectListItem> GetWorkerTypeList()
        {
            List<SelectListItem> lst = (from wt in _db.tbl_WorkerType
                                        where wt.IsActive && !wt.IsDeleted && wt.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = wt.WorkerTypeName,
                                            Value = wt.WorkerTypeId.ToString()
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

        [HttpPost]
        public JsonResult GetWorkerList(string prefix)
        {
            var customerList = (from c in _db.tbl_Employee
                                where c.CompanyId == companyId
                                && c.IsActive
                                && (c.EmployeeCode.ToLower().Contains(prefix.ToLower()) || c.FirstName.ToLower().Contains(prefix.ToLower()) || c.LastName.ToLower().Contains(prefix.ToLower()))
                                select new
                                {
                                    label = c.FirstName + " " + c.LastName + " (" + c.EmployeeCode + ")",
                                    val = c.EmployeeId
                                }).ToList();

            return Json(customerList);
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
          
    }
}