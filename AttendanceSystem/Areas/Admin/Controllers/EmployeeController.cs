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
    public class EmployeeController : Controller
    {
        AttendanceSystemEntities _db;
        public string employeeDirectoryPath = "";
        string psSult;
        string enviornment;
        // GET: Admin/Employee
        public EmployeeController()
        {
            _db = new AttendanceSystemEntities();
            employeeDirectoryPath = ErrorMessage.EmployeeDirectoryPath;
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
        }
        public ActionResult Index(int? userRole = null, int? userStatus = null)
        {
            EmployeeFilterVM employeeFilterVM = new EmployeeFilterVM();
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
                long companyId = clsAdminSession.CompanyId;
                employeeFilterVM.EmployeeList = (from emp in _db.tbl_Employee
                                                 join rl in _db.mst_AdminRole on emp.AdminRoleId equals rl.AdminRoleId
                                                 where !emp.IsDeleted && emp.CompanyId == companyId
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
                                                     IsFingerprintEnabled = emp.IsFingerprintEnabled

                                                 }).OrderByDescending(x => x.EmployeeId).ToList();
            }
            catch (Exception ex)
            {
            }
            employeeFilterVM.UserRoleList = GetUserRoleList();
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
                                  IsFingerprintEnabled = emp.IsFingerprintEnabled

                              }).FirstOrDefault();
            }

            employeeVM.UserRoleList = GetUserRoleList();
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
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = Int64.Parse(clsAdminSession.CompanyId.ToString());

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

                    if (employeeVM.EmployeeId > 0)
                    {
                        tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeVM.EmployeeId).FirstOrDefault();
                        objEmployee.ProfilePicture = profileImageFile != null ? fileName : objEmployee.ProfilePicture;
                        objEmployee.AdminRoleId = employeeVM.AdminRoleId;
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = employeeVM.FirstName;
                        objEmployee.LastName = employeeVM.LastName;
                        objEmployee.Email = employeeVM.Email;
                        objEmployee.EmployeeCode = employeeVM.EmployeeCode;
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.AlternateMobile = employeeVM.AlternateMobile;
                        objEmployee.Address = employeeVM.Address;
                        objEmployee.City = employeeVM.City;
                        objEmployee.Designation = employeeVM.Designation;
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.WorkingTime = employeeVM.WorkingTime;
                        objEmployee.AdharCardNo = employeeVM.AdharCardNo;
                        objEmployee.DateOfIdCardExpiry = employeeVM.DateOfIdCardExpiry;
                        objEmployee.Remarks = employeeVM.Remarks;
                        objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.IsFingerprintEnabled = employeeVM.IsFingerprintEnabled;
                        objEmployee.UpdatedBy = LoggedInUserId;
                        objEmployee.UpdatedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        int empCount = _db.tbl_Employee.Where(x => x.CompanyId == companyId).Count();
                        tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                        tbl_Employee objEmployee = new tbl_Employee();
                        objEmployee.ProfilePicture = fileName;
                        objEmployee.CompanyId = companyId;
                        objEmployee.Password = CommonMethod.Encrypt(CommonMethod.RandomString(6, true), psSult); ;
                        objEmployee.AdminRoleId = employeeVM.AdminRoleId;
                        objEmployee.Prefix = employeeVM.Prefix;
                        objEmployee.FirstName = employeeVM.FirstName;
                        objEmployee.LastName = employeeVM.LastName;
                        objEmployee.Email = employeeVM.Email;
                        objEmployee.EmployeeCode = CommonMethod.getEmployeeCodeFormat(companyId, objCompany.CompanyName, empCount);
                        objEmployee.MobileNo = employeeVM.MobileNo;
                        objEmployee.AlternateMobile = employeeVM.AlternateMobile;
                        objEmployee.Address = employeeVM.Address;
                        objEmployee.City = employeeVM.City;
                        objEmployee.Designation = employeeVM.Designation;
                        objEmployee.Dob = employeeVM.Dob;
                        objEmployee.DateOfJoin = employeeVM.DateOfJoin;
                        objEmployee.BloodGroup = employeeVM.BloodGroup;
                        objEmployee.WorkingTime = employeeVM.WorkingTime;
                        objEmployee.AdharCardNo = employeeVM.AdharCardNo;
                        objEmployee.DateOfIdCardExpiry = employeeVM.DateOfIdCardExpiry;
                        objEmployee.Remarks = employeeVM.Remarks;
                        objEmployee.EmploymentCategory = employeeVM.EmploymentCategory;
                        objEmployee.PerCategoryPrice = employeeVM.PerCategoryPrice;
                        objEmployee.MonthlySalaryPrice = employeeVM.MonthlySalaryPrice;
                        objEmployee.ExtraPerHourPrice = employeeVM.ExtraPerHourPrice;
                        objEmployee.IsLeaveForward = employeeVM.IsLeaveForward;
                        objEmployee.IsActive = true;
                        objEmployee.IsFingerprintEnabled = employeeVM.IsFingerprintEnabled;
                        objEmployee.CreatedBy = LoggedInUserId;
                        objEmployee.CreatedDate = DateTime.UtcNow;
                        objEmployee.UpdatedBy = LoggedInUserId;
                        objEmployee.UpdatedDate = DateTime.UtcNow;
                        _db.tbl_Employee.Add(objEmployee);
                    }
                    _db.SaveChanges();
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

        public ActionResult View(int id)
        {
            EmployeeVM employeeVM = new EmployeeVM();

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
                              IsFingerprintEnabled = emp.IsFingerprintEnabled

                          }).FirstOrDefault();
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
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objEmployee.IsActive = true;
                    }
                    else
                    {
                        objEmployee.IsActive = false;
                    }

                    objEmployee.UpdatedBy = LoggedInUserId;
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

            try
            {
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == EmployeeId).FirstOrDefault();

                if (objEmployee == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objEmployee.IsDeleted = true;
                    objEmployee.UpdatedBy = LoggedInUserId;
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
                        msg = HttpUtility.UrlEncode(msg);
                        string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", mobileNo).Replace("--MSG--", msg);
                        var json = webClient.DownloadString(url);
                        if (json.Contains("invalidnumber"))
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

            return Json(new { Status = status, Otp = otp, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult LoginHistory(int? employeeId = null)
        {
            LoginHistoryFilterVM loginHistoryFilterVM = new LoginHistoryFilterVM();

            try
            {
                if (employeeId.HasValue)
                {
                    loginHistoryFilterVM.EmployeeId = employeeId.Value;
                }

                long companyId = clsAdminSession.CompanyId;
                loginHistoryFilterVM.LoginHistoryList = (from lh in _db.tbl_LoginHistory
                                                         join emp in _db.tbl_Employee on lh.EmployeeId equals emp.EmployeeId
                                                         where !emp.IsDeleted && emp.CompanyId == companyId
                                                         && (loginHistoryFilterVM.EmployeeId.HasValue ? lh.EmployeeId == loginHistoryFilterVM.EmployeeId.Value : true)
                                                         select new LoginHistoryVM
                                                         {
                                                             LoginHistoryId = lh.LoginHistoryId,
                                                             EmployeeId = lh.EmployeeId,
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
                                            Text = emp.FirstName + " " + emp.LastName,
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

    }
}