using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using AttendanceSystem.ViewModel.WebAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class AccountController : ApiController
    {

        private readonly AttendanceSystemEntities _db;
        private string psSult = string.Empty;
        private string enviornment = string.Empty;
        public AccountController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
        }

        [Route("TestMethod"), HttpGet]
        public string TestMethod()
        {
            string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            string password = "12345";
            string encryptedPwd = CommonMethod.Encrypt(password, psSult);

            return encryptedPwd;
        }

        [HttpPost]
        [Route("login")]
        public ResponseDataModel<LoginResponseVM> Login(LoginRequestVM loginRequestVM)
        {
            ResponseDataModel<LoginResponseVM> response = new ResponseDataModel<LoginResponseVM>();
            try
            {
                LoginResponseVM loginResponseVM = new LoginResponseVM();

                if (!string.IsNullOrEmpty(loginRequestVM.UserName) && !string.IsNullOrEmpty(loginRequestVM.PassWord))
                {
                    string encryptPassword = CommonMethod.Encrypt(loginRequestVM.PassWord, psSult);

                    var data = _db.tbl_Employee.Where(x => x.EmployeeCode == loginRequestVM.UserName && x.Password == encryptPassword && x.IsActive && !x.IsDeleted).FirstOrDefault();

                    if (data != null)
                    {
                        tbl_Company companyObj = _db.tbl_Company.Where(x => x.CompanyId == data.CompanyId).FirstOrDefault();

                        if (companyObj != null)
                        {
                            if (!companyObj.IsActive)
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.CompanyIsNotActive);
                            }

                            tbl_CompanyRenewPayment companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == data.CompanyId && DateTime.Today >= x.StartDate && DateTime.Today < x.EndDate).FirstOrDefault();
                            tbl_CompanySMSPackRenew companySMSPackage = _db.tbl_CompanySMSPackRenew.Where(x => x.CompanyId == data.CompanyId && DateTime.Today >= x.RenewDate && DateTime.Today < x.PackageExpiryDate).FirstOrDefault();

                            if (companyObj.IsTrialMode && companyObj.TrialExpiryDate < DateTime.Today && companyPackage == null)
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.CompanyTrailAccountIsExpired);
                            }
                            else if (companyObj.IsTrialMode && companyObj.TrialExpiryDate < DateTime.Today && companyPackage != null)
                            {
                                companyObj.IsTrialMode = false;
                                companyObj.TrialExpiryDate = null;
                                companyObj.CurrentPackageId = companyPackage.PackageId;
                                companyObj.AccountStartDate = companyPackage.StartDate;
                                companyObj.AccountExpiryDate = companyPackage.EndDate;
                                //companyObj.CurrentEmployeeAccess
                                //companyObj.RemainingEmployeeAccess
                                if (companySMSPackage != null)
                                {
                                    companyObj.CurrentSMSPackageId = (int)companySMSPackage.SMSPackageId;
                                    companyObj.SMSPackStartDate = companySMSPackage.RenewDate;
                                    companyObj.SMSPackExpiryDate = companySMSPackage.PackageExpiryDate;
                                }

                                List<tbl_Employee> listEmployee = _db.tbl_Employee.Where(x => x.CompanyId == companyObj.CompanyId && x.IsActive).ToList();
                                if (listEmployee.Count > companyPackage.NoOfEmployee)
                                {

                                }
                            }
                            else if (!companyObj.IsTrialMode && companyObj.AccountExpiryDate < DateTime.Today && companyPackage == null)
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.CompanyAccountIsExpired);
                            }
                            else if (!companyObj.IsTrialMode && companyObj.AccountExpiryDate < DateTime.Today && companyPackage != null)
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.CompanyAccountIsExpired);
                            }

                            response.IsError = false;
                            loginResponseVM.IsFingerprintEnabled = data.IsFingerprintEnabled;
                            loginResponseVM.EmployeeId = data.EmployeeId;

                            using (WebClient webClient = new WebClient())
                            {
                                Random random = new Random();
                                int num = random.Next(555555, 999999);
                                if (enviornment != "Development")
                                {
                                    string msg = "Your Otp code for Login is " + num;
                                    msg = HttpUtility.UrlEncode(msg);
                                    string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", data.MobileNo).Replace("--MSG--", msg);
                                    var json = webClient.DownloadString(url);
                                    if (json.Contains("invalidnumber"))
                                    {
                                        response.IsError = true;
                                        response.AddError(ErrorMessage.InvalidMobileNo);
                                    }
                                    else
                                    {
                                        loginResponseVM.OTP = num.ToString();
                                    }
                                }
                                else
                                {
                                    loginResponseVM.OTP = num.ToString();
                                }
                            }
                            response.Data = loginResponseVM;
                        }

                    }
                    else
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.UserNameOrPasswordInvalid);
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.UserNamePasswordRequired);
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }


            return response;
        }

        [Route("SampleAPI"), HttpGet]
        public ResponseDataModel<bool> SampleAPI()
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();

            try
            {
                response.Data = true;

                // for testing error
                if (response.Data == true)
                {
                    throw new Exception("custom error by nilesh");
                }

            }
            catch (Exception ex)
            {
                response.Data = false;

                response.AddError(ex.Message.ToString());
            }

            return response;
        }

        [Route("Authenticate"), HttpPost]
        public ResponseDataModel<AuthenticateVM> Authenticate(AuthenticateRequestVM authenticateRequestVM)
        {
            ResponseDataModel<AuthenticateVM> response = new ResponseDataModel<AuthenticateVM>();
            AuthenticateVM authenticateVM = new AuthenticateVM();

            try
            {
                var data = _db.tbl_Employee.Where(x => x.EmployeeId == authenticateRequestVM.EmployeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    tbl_Company company = _db.tbl_Company.Where(x => x.CompanyId == data.CompanyId).FirstOrDefault();
                    UserTokenVM userToken = new UserTokenVM()
                    {
                        EmployeeCode = data.EmployeeCode,
                        EmployeeId = data.EmployeeId,
                        RoleId = data.AdminRoleId,
                        UserName = data.FirstName + " " + data.LastName,
                        CompanyId = data.CompanyId,
                        CompanyTypeId = company.CompanyTypeId
                    };

                    JWTAccessTokenVM tokenVM = new JWTAccessTokenVM();
                    tokenVM = JWTAuthenticationHelper.GenerateToken(userToken);
                    authenticateVM.Access_token = tokenVM.Token;
                    authenticateVM.EmployeeId = data.EmployeeId;
                    authenticateVM.RoleId = data.AdminRoleId;
                    authenticateVM.CompanyId = data.CompanyId;
                    authenticateVM.CompanyTypeId = company.CompanyTypeId;
                    authenticateVM.Prefix = data.Prefix;
                    authenticateVM.FirstName = data.FirstName;
                    authenticateVM.LastName = data.LastName;
                    authenticateVM.Email = data.Email;
                    authenticateVM.EmployeeCode = data.EmployeeCode;
                    authenticateVM.Password = data.Password;
                    authenticateVM.MobileNo = data.MobileNo;
                    authenticateVM.AlternateMobile = data.AlternateMobile;
                    authenticateVM.Address = data.Address;
                    authenticateVM.City = data.City;
                    authenticateVM.Designation = data.Designation;
                    authenticateVM.Dob = data.Dob;
                    authenticateVM.DateOfJoin = data.DateOfJoin;
                    authenticateVM.BloodGroup = data.BloodGroup;
                    authenticateVM.WorkingTime = data.WorkingTime;
                    authenticateVM.AdharCardNo = data.AdharCardNo;
                    authenticateVM.DateOfIdCardExpiry = data.DateOfIdCardExpiry;
                    authenticateVM.Remarks = data.Remarks;
                    authenticateVM.ProfilePicture = ErrorMessage.EmployeeDirectoryPath + data.ProfilePicture;
                    authenticateVM.EmploymentCategory = data.EmploymentCategory;
                    response.Data = authenticateVM;

                    tbl_LoginHistory objLoginHistory = new tbl_LoginHistory();
                    objLoginHistory.EmployeeId = data.EmployeeId;
                    objLoginHistory.LoginDate = DateTime.UtcNow;
                    objLoginHistory.LocationFrom = authenticateRequestVM.LocationFrom;
                    objLoginHistory.SiteId = data.CompanyId;
                    objLoginHistory.Latitude = authenticateRequestVM.Latitude;
                    objLoginHistory.Longitude = authenticateRequestVM.Longitude ;
                    objLoginHistory.CreatedBy = data.EmployeeId;
                    objLoginHistory.CreatedDate = DateTime.UtcNow;
                    objLoginHistory.ModifiedBy = data.EmployeeId;
                    objLoginHistory.ModifiedDate = DateTime.UtcNow;
                    _db.tbl_LoginHistory.Add(objLoginHistory);
                    _db.SaveChanges();
                }
                else
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.UserNameOrPasswordInvalid);
                }
            }
            catch (Exception ex)
            {
                response.IsError = false;
                response.AddError(ex.Message.ToString());
            }

            return response;
        }

        [Route("ResendLoginOtp"), HttpGet]
        public ResponseDataModel<LoginResponseVM> ResendLoginOtp(long employeeId)
        {
            ResponseDataModel<LoginResponseVM> response = new ResponseDataModel<LoginResponseVM>();
            LoginResponseVM loginResponseVM = new LoginResponseVM();

            try
            {
                var data = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    response.IsError = false;
                    loginResponseVM.IsFingerprintEnabled = data.IsFingerprintEnabled;
                    loginResponseVM.EmployeeId = data.EmployeeId;

                    using (WebClient webClient = new WebClient())
                    {
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        if (enviornment != "Development")
                        {
                            string msg = "Your Otp code for Login is " + num;
                            msg = HttpUtility.UrlEncode(msg);
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", data.MobileNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.InvalidMobileNo);
                            }
                            else
                            {
                                loginResponseVM.OTP = num.ToString();
                            }
                        }
                        else
                        {
                            loginResponseVM.OTP = num.ToString();
                        }
                    }
                    response.Data = loginResponseVM;
                }
                else
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YouAreNotAuthorized);
                }
            }
            catch (Exception ex)
            {
                response.IsError = false;
                response.AddError(ex.Message.ToString());
            }

            return response;
        }

        [Route("ForgotPassword"), HttpGet]
        public ResponseDataModel<LoginResponseVM> ForgotPassword(string employeeCode)
        {
            ResponseDataModel<LoginResponseVM> response = new ResponseDataModel<LoginResponseVM>();
            LoginResponseVM loginResponseVM = new LoginResponseVM();

            try
            {
                var data = _db.tbl_Employee.Where(x => x.EmployeeCode == employeeCode && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    response.IsError = false;
                    loginResponseVM.IsFingerprintEnabled = data.IsFingerprintEnabled;
                    loginResponseVM.EmployeeId = data.EmployeeId;

                    using (WebClient webClient = new WebClient())
                    {
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        if (enviornment != "Development")
                        {
                            string msg = "Your Otp code for Login is " + num;
                            msg = HttpUtility.UrlEncode(msg);
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", data.MobileNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
                            if (json.Contains("invalidnumber"))
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.InvalidMobileNo);
                            }
                            else
                            {
                                loginResponseVM.OTP = num.ToString();
                            }
                        }
                        else
                        {
                            loginResponseVM.OTP = num.ToString();
                        }
                    }
                    response.Data = loginResponseVM;
                }
                else
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YouAreNotAuthorized);
                }
            }
            catch (Exception ex)
            {
                response.IsError = false;
                response.AddError(ex.Message.ToString());
            }

            return response;
        }

        [HttpPost]
        [Route("ResetPassword")]
        public ResponseDataModel<bool> ResetPassword(ChangePasswordVM changePasswordVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.Data = false;
            try
            {
                if (changePasswordVM.EmployeeId > 0 && !string.IsNullOrEmpty(changePasswordVM.PassWord))
                {
                    string encryptedPwd = CommonMethod.Encrypt(changePasswordVM.PassWord, psSult);

                    tbl_Employee data = _db.tbl_Employee.Where(x => x.EmployeeId == changePasswordVM.EmployeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();

                    if (data != null)
                    {
                        data.Password = encryptedPwd;
                        data.UpdatedBy = changePasswordVM.EmployeeId;
                        data.UpdatedDate = DateTime.UtcNow;
                        response.IsError = false;
                        response.Data = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.InvalidUserName);
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.UserNamePasswordRequired);
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
        [Route("ValidateFingerprint")]
        public ResponseDataModel<string> ValidateFingerprint(EmployeeFirgerprintVM fingerprintVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                if (fingerprintVM.EmployeeId > 0)
                {

                    tbl_EmployeeFingerprint data = _db.tbl_EmployeeFingerprint.Where(x => x.ISOCode == fingerprintVM.ISOCode
                                                                    && x.EmployeeId == fingerprintVM.EmployeeId).FirstOrDefault();

                    if (data != null)
                    {
                        tbl_Employee objEmp = _db.tbl_Employee.Where(x => x.EmployeeId == fingerprintVM.EmployeeId).FirstOrDefault();
                        string EmployeeFullName = objEmp.FirstName + " " + objEmp.LastName;

                        response.IsError = false;
                        response.Data = "Great.. Fingerprint Matched.. This fingerprint is of " + EmployeeFullName;
                    }
                    else
                    {
                        response.Data = "Oops.. Fingerprint not match..";

                        response.IsError = true;
                        response.AddError("Oops.. Fingerprint not match..");
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError("EmployeeId not found");
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