using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using BarcodeSystem.Model;
using MyMobileApp.Helper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static AttendanceSystem.Models.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        AttendanceSystemEntities _db;
        string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        public LoginController()
        {
            _db = new AttendanceSystemEntities();
        }

        // GET: Admin/Login
        public ActionResult Index()
        {
            LoginModel login = new LoginModel()
            {
                UserName = "9999999999",
                Password = "12345"
            };
            return View(login);
        }

        public JsonResult Login(string userName, string password)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string encryptedUserDetails = string.Empty;
            string otp = string.Empty;
            try
            {
                string encryptedPassword = CommonMethod.Encrypt(password, psSult);
                var data = _db.tbl_AdminUser.Where(x => x.UserName == userName && x.Password == encryptedPassword).FirstOrDefault();

                if (data != null)
                {
                    if (data.AdminUserRoleId == (int)AdminRoles.SuperAdmin)
                    {

                        using (WebClient webClient = new WebClient())
                        {
                            Random random = new Random();
                            int num = random.Next(555555, 999999);
                            string msg = "Your Otp code for Login is " + num;
                            msg = HttpUtility.UrlEncode(msg);
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", data.MobileNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                status = 0;
                                errorMessage = ErrorMessage.InvalidMobileNo;
                            }
                            else
                            {
                                status = 1;
                                string encKey = userName + "," + data.AdminUserRoleId + "," + data.MobileNo;
                                encryptedUserDetails = CommonMethod.Encrypt(encKey, psSult);
                                otp = num.ToString();
                            }

                        }
                    }
                    else
                    {
                        status = 0;
                        errorMessage = ErrorMessage.YouAreNotAuthorized;
                    }
                }
                else
                {
                    status = 0;
                    errorMessage = ErrorMessage.InvalidCredentials;
                }
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, EncryptedUserDetails = encryptedUserDetails, Otp = otp, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult LoginSubmit(LoginModel login)
        {
            try
            {
                var data = _db.tbl_AdminUser.Where(x => x.UserName == login.UserName).FirstOrDefault();
                var roleData = _db.mst_AdminRole.Where(x => x.AdminRoleId == data.AdminUserRoleId).FirstOrDefault();
                clsAdminSession.SessionID = Session.SessionID;
                clsAdminSession.UserID = data.AdminUserId;
                clsAdminSession.RoleID = data.AdminUserRoleId;
                clsAdminSession.RoleName = roleData.AdminRoleName;
                clsAdminSession.UserName = data.UserName;
                clsAdminSession.ImagePath = ""; //data.ProfilePicture;
                clsAdminSession.MobileNumber = data.MobileNo;

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
            }

            return RedirectToAction("Index", "Dashboard");
        }

        public ActionResult ForgotPassword()
        {
            LoginModel loginModel = new LoginModel();
            return View(loginModel);
        }

        [HttpPost]
        public ActionResult ResetPassword(LoginModel login)
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
                //throw ex
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
                    using (WebClient webClient = new WebClient())
                    {
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        string msg = "Your Otp code for Login is " + num;
                        msg = HttpUtility.UrlEncode(msg);
                        string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", data.MobileNo).Replace("--MSG--", msg);
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

            return Json(new { Status = status, UserName= userName, Otp = otp, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

    }
}