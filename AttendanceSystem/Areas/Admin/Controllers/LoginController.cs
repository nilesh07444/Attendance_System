using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using static AttendanceSystem.ViewModel.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        AttendanceSystemEntities _db;
        string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        string enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
        bool? setOtp = Convert.ToBoolean(ConfigurationManager.AppSettings["SetOtp"].ToString());
        public LoginController()
        {
            _db = new AttendanceSystemEntities();
        }

        // GET: Admin/Login
        public ActionResult Index()
        {

            /*
            int SmsId = (int)SMSType.EmployeeCreateOTP;
            string msg = CommonMethod.GetSmsContent(SmsId);
            msg = msg.Replace("{#var#}", "12345"); 
            msg = msg.Replace("\r\n", "\n");

            var json = CommonMethod.SendSMSWithoutLog(msg, "9824936252");
            if (json.Contains("invalidnumber"))
            {
                int status = 0; 
            }
            else
            {
                int status = 1; 
            }
            */

            LoginVM login = new LoginVM();
            return View(login);
        }

        public JsonResult Login(string userName, string password)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string otp = string.Empty;
            try
            {
                string encryptedPassword = CommonMethod.Encrypt(password, psSult);
                var data = _db.tbl_AdminUser.Where(x => x.UserName == userName && x.Password == encryptedPassword).FirstOrDefault();

                if (data != null)
                {
                    if (data.AdminUserRoleId != (int)AdminRoles.SuperAdmin && data.CompanyId > 0)
                    {
                        tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == data.CompanyId).FirstOrDefault();
                        if (objCompany != null)
                        {
                            if (!objCompany.IsActive)
                            {
                                status = 0;
                                errorMessage = ErrorMessage.CompanyIsNotActive;
                            }
                        }
                        else
                        {
                            status = 0;
                            errorMessage = ErrorMessage.CompanyDoesNotExist;
                        }
                    }

                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        if (data != null)
                        {
                            if (data.AdminUserRoleId == (int)AdminRoles.SuperAdmin || data.AdminUserRoleId == (int)AdminRoles.CompanyAdmin)
                            {
                                Random random = new Random();
                                int num = random.Next(555555, 999999);
                                if (enviornment != "Development")
                                {
                                    string msg = string.Empty;

                                    if (data.AdminUserRoleId == (int)AdminRoles.SuperAdmin)
                                    {
                                        int SmsId = (int)SMSType.SuperAdminLoginOTP;
                                        msg = CommonMethod.GetSmsContent(SmsId);

                                        Regex regReplace = new Regex("{#var#}");
                                        msg = regReplace.Replace(msg, data.FirstName + " " + data.LastName, 1);
                                        msg = regReplace.Replace(msg, num.ToString(), 1);
                                    }
                                    else
                                    {
                                        int SmsId = (int)SMSType.CompanyAdminLoginOTP;
                                        msg = CommonMethod.GetSmsContent(SmsId);

                                        msg = msg.Replace("{#var#}", num.ToString());
                                    }
                                    msg = msg.Replace("\r\n", "\n");
                                    var json = CommonMethod.SendSMSWithoutLog(msg, data.MobileNo);
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
                            else
                            {
                                status = 0;
                                errorMessage = ErrorMessage.UserNameOrPasswordInvalid;
                            }
                        }
                        else
                        {
                            status = 0;
                            errorMessage = ErrorMessage.InvalidCredentials;
                        }
                    }
                }
                else
                {
                    status = 0;
                    errorMessage = ErrorMessage.UserNameOrPasswordInvalid;
                }
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, Otp = otp, ErrorMessage = errorMessage, SetOtp = setOtp }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoginSubmit(LoginVM login)
        {
            try
            {
                var data = _db.tbl_AdminUser.Where(x => x.UserName == login.UserName).FirstOrDefault();
                var roleData = _db.mst_AdminRole.Where(x => x.AdminRoleId == data.AdminUserRoleId).FirstOrDefault();
                DateTime today = CommonMethod.CurrentIndianDateTime();
                tbl_Company companyObj = null;
                tbl_CompanyRenewPayment companyPackage = null;
                if (data.CompanyId.HasValue)
                {
                    companyObj = _db.tbl_Company.Where(x => x.CompanyId == data.CompanyId).FirstOrDefault();
                    if (!companyObj.IsTrialMode)
                    {
                        if (companyObj.AccountExpiryDate < today)
                        {
                            companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == data.CompanyId && today >= x.StartDate && today < x.EndDate).FirstOrDefault();
                            if (companyPackage != null)
                            {
                                companyObj.CurrentPackageId = companyPackage.PackageId;
                                companyObj.AccountStartDate = companyPackage.StartDate;
                                companyObj.AccountExpiryDate = companyPackage.EndDate;
                                companyObj.CurrentEmployeeAccess = companyPackage.NoOfEmployee;
                                _db.SaveChanges();

                                List<tbl_Employee> listEmployee = _db.tbl_Employee.Where(x => x.CompanyId == companyObj.CompanyId && x.IsActive).ToList();
                                if (listEmployee.Count > companyPackage.NoOfEmployee)
                                {
                                    List<tbl_Employee> listEmployeeToDeactive = listEmployee.OrderBy(x => x.EmployeeId).Skip(companyPackage.NoOfEmployee).ToList();
                                    listEmployeeToDeactive.ForEach(emp =>
                                    {
                                        emp.IsActive = false;
                                        emp.UpdatedBy = -1;
                                        emp.UpdatedDate = CommonMethod.CurrentIndianDateTime();
                                        _db.SaveChanges();
                                    });
                                }
                            }
                        }
                        else
                        {
                            companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyRegistrationPaymentId == companyObj.CurrentPackageId).FirstOrDefault();
                        }
                    }
                    else
                    {
                        if (companyObj.TrialExpiryDate < today)
                        {
                            companyObj.IsTrialMode = false;
                            _db.SaveChanges();
                        }
                    }
                }

                clsAdminSession.SessionID = Session.SessionID;
                clsAdminSession.UserID = data.AdminUserId;
                clsAdminSession.RoleID = data.AdminUserRoleId;
                clsAdminSession.RoleName = roleData.AdminRoleName;
                clsAdminSession.UserName = data.UserName;
                clsAdminSession.FullName = data.FirstName + " " + data.LastName;
                clsAdminSession.ImagePath = ""; //data.ProfilePicture;
                clsAdminSession.MobileNumber = data.MobileNo;
                clsAdminSession.CompanyId = data.CompanyId.HasValue ? data.CompanyId.Value : 0;
                clsAdminSession.CompanyTypeId = companyObj != null ? companyObj.CompanyTypeId : 0;
                clsAdminSession.IsTrialMode = companyObj != null ? companyObj.IsTrialMode : false;
                clsAdminSession.CompanyLogo = companyObj != null ? companyObj.CompanyLogoImage : string.Empty;
                clsAdminSession.SetOtp = setOtp != null ? setOtp.Value : false;
                clsAdminSession.IsPackageExpired = companyObj != null && companyObj.IsTrialMode ? companyObj.TrialExpiryDate < today : (companyPackage != null ? companyPackage.EndDate < today : true);
                clsAdminSession.CurrentAccountPackageId = companyObj != null && companyObj.CurrentPackageId.HasValue ? companyObj.CurrentPackageId.Value : 0;
                clsAdminSession.CurrentSMSPackageId = companyObj != null && companyObj.CurrentSMSPackageId.HasValue ? companyObj.CurrentSMSPackageId.Value : 0;
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult ForgotPassword()
        {
            LoginVM loginModel = new LoginVM();
            return View(loginModel);
        }

        [HttpPost]
        public ActionResult ResetPassword(LoginVM login)
        {
            try
            {
                string encryptedPassword = CommonMethod.Encrypt(login.Password, psSult);
                tbl_AdminUser data = _db.tbl_AdminUser.Where(x => x.UserName == login.UserName).FirstOrDefault();

                if (data != null)
                {
                    data.Password = encryptedPassword;
                    _db.SaveChanges();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("Index", "Login");
        }

        public JsonResult SendOTPToReset(string userName)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string otp = string.Empty;
            try
            {
                var data = _db.tbl_AdminUser.Where(x => x.UserName == userName && x.IsActive).FirstOrDefault();

                if (data != null)
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);

                    var json = string.Empty;
                    int SmsId = (int)SMSType.ForgetPasswordOTP;
                    string msg = CommonMethod.GetSmsContent(SmsId);

                    Regex regReplace = new Regex("{#var#}");
                    msg = regReplace.Replace(msg, data.FirstName + " " + data.LastName, 1);
                    msg = regReplace.Replace(msg, num.ToString(), 1);

                    msg = msg.Replace("\r\n", "\n");

                    if (data.AdminUserRoleId == (int)AdminRoles.SuperAdmin)
                    {
                        json = CommonMethod.SendSMSWithoutLog(msg, data.MobileNo);
                    }
                    else
                    {
                        json = CommonMethod.SendSMSWithoutLog(msg, data.MobileNo);
                    }

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
                    status = 0;
                    errorMessage = ErrorMessage.InvalidUserName;
                }
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, UserName = userName, Otp = otp, ErrorMessage = errorMessage, SetOtp = setOtp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Signout()
        {
            clsAdminSession.SessionID = "";
            clsAdminSession.UserID = 0;
            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();

            return RedirectToAction("Index");
        }

    }
}