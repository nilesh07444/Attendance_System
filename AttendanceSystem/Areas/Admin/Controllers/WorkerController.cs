using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class WorkerController : Controller
    {
        AttendanceSystemEntities _db;
        public string employeeDirectoryPath = "";
        string psSult;
        string enviornment;
        long companyId;
        long loggedInUserId;
        bool isTrailMode;
        string defaultPassword;
        // GET: Admin/Employee
        public WorkerController()
        {
            _db = new AttendanceSystemEntities();
            employeeDirectoryPath = ErrorMessage.EmployeeDirectoryPath;
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
            companyId = clsAdminSession.CompanyId;
            loggedInUserId = clsAdminSession.UserID;
            isTrailMode = clsAdminSession.IsTrialMode;
            defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"].ToString();
        }
        public ActionResult Index(int? userStatus = null)
        {
            EmployeeFilterVM employeeFilterVM = new EmployeeFilterVM();

            if (userStatus.HasValue)
            {
                employeeFilterVM.UserStatus = userStatus.Value;
            }

            try
            {

                employeeFilterVM.EmployeeList = (from emp in _db.tbl_Employee
                                                 join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId
                                                 join wt in _db.tbl_WorkerType on emp.WorkerTypeId equals wt.WorkerTypeId
                                                 into wtc
                                                 from w in wtc.DefaultIfEmpty()
                                                 where !emp.IsDeleted && emp.CompanyId == companyId && emp.AdminRoleId == (int)AdminRoles.Worker
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
                                                     State = emp.State,
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

                                                 }).OrderByDescending(x => x.EmployeeId).ToList();
                employeeFilterVM.NoOfEmployee = _db.tbl_Employee.Where(x => x.CompanyId == companyId && x.IsActive).Count();
                employeeFilterVM.ActiveEmployee = employeeFilterVM.EmployeeList.Where(x => x.IsActive).Count();
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
                                  State = emp.State,
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
                                  WorkerTypeId = emp.WorkerTypeId

                              }).FirstOrDefault();
            }

            employeeVM.WorkerTypeList = GetWorkerTypeList();
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
                            ModelState.AddModelError("ProfileImageFile", ErrorMessage.ImageRequired);
                            return View(employeeVM);
                        }
                    }

                    if (employeeVM.EmployeeId > 0)
                    {
                        tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeVM.EmployeeId).FirstOrDefault();
                        objEmployee.ProfilePicture = profileImageFile != null ? fileName : objEmployee.ProfilePicture;
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = employeeVM.FirstName;
                        objEmployee.LastName = employeeVM.LastName;
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.Address = employeeVM.Address;
                        objEmployee.City = employeeVM.City;
                        objEmployee.Pincode = employeeVM.Pincode;
                        objEmployee.State = employeeVM.State;
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.AdharCardNo = employeeVM.AdharCardNo;
                        objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                        objEmployee.WorkerTypeId = employeeVM.WorkerTypeId;
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.NoOfFreeLeavePerMonth = employeeVM.NoOfFreeLeavePerMonth;
                        objEmployee.UpdatedBy = loggedInUserId;
                        objEmployee.UpdatedDate = DateTime.UtcNow;
                    }
                    else
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
                        objEmployee.ProfilePicture = fileName;
                        objEmployee.CompanyId = companyId;
                        objEmployee.AdminRoleId = (int)AdminRoles.Worker;
                        objEmployee.Password = CommonMethod.Encrypt(defaultPassword, psSult); ;
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = employeeVM.FirstName;
                        objEmployee.LastName = employeeVM.LastName;
                        objEmployee.EmployeeCode = CommonMethod.getEmployeeCodeFormat(companyId, objCompany.CompanyName, empCount.Count());
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.Address = employeeVM.Address;
                        objEmployee.City = employeeVM.City;
                        objEmployee.Pincode = employeeVM.Pincode;
                        objEmployee.State = employeeVM.State;
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.AdharCardNo = employeeVM.AdharCardNo;
                        objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.WorkerTypeId = employeeVM.WorkerTypeId;
                        objEmployee.NoOfFreeLeavePerMonth = employeeVM.NoOfFreeLeavePerMonth;
                        objEmployee.IsActive = isTrailMode ? true : (activeEmployee >= noOfEmployee ? false : true);
                        objEmployee.CreatedBy = loggedInUserId;
                        objEmployee.CreatedDate = DateTime.UtcNow;
                        objEmployee.UpdatedBy = loggedInUserId;
                        objEmployee.UpdatedDate = DateTime.UtcNow;
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

        public ActionResult View(int id)
        {
            EmployeeVM employeeVM = new EmployeeVM();

            employeeVM = (from emp in _db.tbl_Employee
                          join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId
                          join wt in _db.tbl_WorkerType on emp.WorkerTypeId equals wt.WorkerTypeId into wtc
                          from w in wtc.DefaultIfEmpty()
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
            return View(employeeVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                DateTime today = DateTime.UtcNow.Date;
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

                    objEmployee.UpdatedBy = loggedInUserId;
                    objEmployee.UpdatedDate = DateTime.UtcNow;

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
            DateTime today = DateTime.UtcNow.Date;
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
                    objEmployee.UpdatedBy = loggedInUserId;
                    objEmployee.UpdatedDate = DateTime.UtcNow;
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

        public JsonResult VerifyMobileNo(string mobileNo)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string otp = string.Empty;
            try
            {

                using (WebClient webClient = new WebClient())
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    if (enviornment != "Development")
                    {
                        string msg = "Your Otp code for Login is " + num;
                        ResponseDataModel<string> response = CommonMethod.SendSMS(msg, mobileNo, companyId, loggedInUserId, isTrailMode);
                        var json = response.Data;
                        if (response.IsError)
                        {
                            status = 0;
                            errorMessage = string.Join(", ", response.ErrorData);
                        }
                        else if (json.Contains("invalidnumber"))
                        {
                            status = 0;
                            errorMessage = ErrorMessage.InvalidMobileNo;
                        }
                        else
                        {
                            status = 1;
                            otp = num.ToString();
                        }
                    }
                    else
                    {
                        status = 1;
                        otp = num.ToString();
                    }
                }

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
    }
}