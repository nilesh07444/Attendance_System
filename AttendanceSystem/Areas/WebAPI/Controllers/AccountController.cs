using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using AttendanceSystem.ViewModel.WebAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class AccountController : ApiController
    {

        private readonly AttendanceSystemEntities _db;
        private string psSult = string.Empty;
        public AccountController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        }

        [HttpPost]
        [Route("login")]
        public ResponseDataModel<LoginResponseVM> Login(LoginRequestVM loginRequestVM)
        {
            ResponseDataModel<LoginResponseVM> response = new ResponseDataModel<LoginResponseVM>();
            try
            {
                LoginResponseVM loginResponseVM = new LoginResponseVM();
                response.IsError = false;
                DateTime today = CommonMethod.CurrentIndianDateTime();
                long loggedInUserId = (int)PaymentGivenBy.CompanyAdmin;

                if (!string.IsNullOrEmpty(loginRequestVM.UserName) && !string.IsNullOrEmpty(loginRequestVM.PassWord))
                {
                    string encryptPassword = CommonMethod.Encrypt(loginRequestVM.PassWord, psSult);

                    var data = _db.tbl_Employee.Where(x => x.AdminRoleId != (int)AdminRoles.Worker && x.EmployeeCode == loginRequestVM.UserName && x.Password == encryptPassword && x.IsActive && !x.IsDeleted).FirstOrDefault();

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

                            tbl_CompanyRenewPayment companyPackage = _db.tbl_CompanyRenewPayment.Where(x => x.CompanyId == data.CompanyId && today >= x.StartDate && today < x.EndDate).FirstOrDefault();
                            //tbl_CompanySMSPackRenew companySMSPackage = _db.tbl_CompanySMSPackRenew.Where(x => x.CompanyId == data.CompanyId && today >= x.RenewDate && today < x.PackageExpiryDate).FirstOrDefault();

                            if (companyObj.IsTrialMode && companyObj.TrialExpiryDate < today && companyPackage == null)
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.CompanyTrailAccountIsExpired);
                            }
                            else if (!companyObj.IsTrialMode && companyObj.AccountExpiryDate < today && companyPackage == null)
                            {
                                response.IsError = true;
                                response.AddError(ErrorMessage.CompanyAccountIsExpired);
                            }
                            else if (!companyObj.IsTrialMode && companyObj.AccountExpiryDate < today && companyPackage != null)
                            {

                                companyPackage.StartDate = companyObj.AccountExpiryDate.Value.AddMinutes(1);
                                companyPackage.EndDate = companyObj.AccountExpiryDate.Value.AddDays(companyPackage.AccessDays);

                                companyObj.CurrentPackageId = companyPackage.CompanyRegistrationPaymentId;
                                companyObj.AccountStartDate = companyPackage.StartDate;
                                companyObj.AccountExpiryDate = companyPackage.EndDate;
                                companyObj.CurrentEmployeeAccess = companyPackage.NoOfEmployee;
                                _db.SaveChanges();

                                tbl_CompanySMSPackRenew objCompanySMSPackRenew = new tbl_CompanySMSPackRenew();
                                objCompanySMSPackRenew.CompanyId = companyObj.CompanyId;
                                objCompanySMSPackRenew.SMSPackageId = companyPackage.PackageId;
                                objCompanySMSPackRenew.SMSPackageName = companyPackage.PackageName;
                                objCompanySMSPackRenew.RenewDate = today;
                                objCompanySMSPackRenew.PackageAmount = companyPackage.Amount;
                                objCompanySMSPackRenew.AccessDays = companyPackage.AccessDays;
                                objCompanySMSPackRenew.PackageExpiryDate = today.AddDays(companyPackage.AccessDays);
                                objCompanySMSPackRenew.NoOfSMS = companyPackage.NoOfSMS;
                                objCompanySMSPackRenew.RemainingSMS = companyPackage.NoOfSMS;
                                objCompanySMSPackRenew.InvoiceNo = companyPackage.InvoiceNo;
                                objCompanySMSPackRenew.GSTPer = companyPackage.GSTPer;
                                objCompanySMSPackRenew.CreatedBy = loggedInUserId;
                                objCompanySMSPackRenew.CreatedDate = today;
                                objCompanySMSPackRenew.ModifiedBy = loggedInUserId;
                                objCompanySMSPackRenew.ModifiedDate = today;
                                _db.tbl_CompanySMSPackRenew.Add(objCompanySMSPackRenew);
                                _db.SaveChanges();

                                #region checkEmployee

                                List<tbl_Employee> totalEmployeeList = _db.tbl_Employee.Where(x => x.CompanyId == data.CompanyId && !x.IsDeleted).ToList();
                                int activeEmployee = totalEmployeeList.Where(x => x.IsActive).Count();
                                int allowedEmployees = companyPackage.NoOfEmployee;

                                if (allowedEmployees > activeEmployee)
                                {
                                    int employeeToActive = allowedEmployees - activeEmployee;
                                    if (employeeToActive > 0)
                                    {
                                        List<tbl_Employee> employeeListToBeActive = totalEmployeeList.Where(x => !x.IsActive).OrderBy(x => x.EmployeeId).Take(employeeToActive).ToList();
                                        if (employeeListToBeActive.Count > 0)
                                        {
                                            employeeListToBeActive.ForEach(x =>
                                            {
                                                x.IsActive = true;
                                                x.UpdatedBy = loggedInUserId;
                                                x.UpdatedDate = today;
                                                _db.SaveChanges();
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    int employeeToDeActive = activeEmployee - allowedEmployees;
                                    if (employeeToDeActive > 0)
                                    {
                                        List<tbl_Employee> employeeListToBeDeActive = totalEmployeeList.Where(x => x.IsActive).OrderBy(x => x.EmployeeId).Take(employeeToDeActive).ToList();
                                        if (employeeListToBeDeActive.Count > 0)
                                        {
                                            employeeListToBeDeActive.ForEach(x =>
                                            {
                                                x.IsActive = false;
                                                x.UpdatedBy = loggedInUserId;
                                                x.UpdatedDate = today;
                                                _db.SaveChanges();
                                            });
                                        }
                                    }
                                }

                                #endregion checkEmployee
                            }

                            if (!response.IsError)
                            {

                                loginResponseVM.IsFingerprintEnabled = data.IsFingerprintEnabled;
                                loginResponseVM.EmployeeId = data.EmployeeId;

                                if (!loginResponseVM.IsFingerprintEnabled)
                                {
                                    Random random = new Random();
                                    int num = random.Next(555555, 999999);

                                    int SmsId = (int)SMSType.EmployeeLoginOTP;
                                    string msg = CommonMethod.GetSmsContent(SmsId);
                                    msg = msg.Replace("{#var#}", num.ToString());
                                    msg = msg.Replace("\r\n", "\n");

                                    ResponseDataModel<string> smsResponse = CommonMethod.SendSMS(msg, data.MobileNo, data.CompanyId, data.EmployeeId, data.EmployeeCode, data.EmployeeId, companyObj.IsTrialMode);

                                    if (smsResponse.IsError)
                                    {
                                        response.IsError = true;
                                        response.ErrorData = smsResponse.ErrorData;
                                    }
                                    else
                                    {
                                        loginResponseVM.OTP = num.ToString();
                                    }

                                }
                            }
                            response.Data = loginResponseVM;
                        }
                        else
                        {
                            response.IsError = true;
                            response.AddError(ErrorMessage.CompanyDoesNotExist);
                        }

                    }
                    else
                    {
                        // Check Company Admin Login
                        var companyAdminLoginData = _db.tbl_AdminUser.Where(x => x.UserName == loginRequestVM.UserName && x.Password == encryptPassword
                                                        && x.AdminUserRoleId == (int)AdminRoles.CompanyAdmin).FirstOrDefault();

                        if (companyAdminLoginData != null)
                        {
                            if (!string.IsNullOrEmpty(companyAdminLoginData.MobileNo))
                            {
                                Random random = new Random();
                                int num = random.Next(555555, 999999);

                                int SmsId = (int)SMSType.SuperAdminLoginOTP;
                                string msg = CommonMethod.GetSmsContent(SmsId);
                                msg = msg.Replace("{#var#}", num.ToString());
                                msg = msg.Replace("\r\n", "\n");

                                string smsResponse = CommonMethod.SendSMSWithoutLog(msg, companyAdminLoginData.MobileNo);

                                loginResponseVM.OTP = num.ToString();
                                loginResponseVM.CompanyAdminId = companyAdminLoginData.AdminUserId;

                                response.Data = loginResponseVM;
                            }
                        }
                        else
                        {
                            response.IsError = true;
                            response.AddError(ErrorMessage.UserNameOrPasswordInvalid);
                        }
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
                        CompanyTypeId = company.CompanyTypeId,
                        IsTrailMode = company.IsTrialMode
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
                    authenticateVM.ProfilePicture = CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + data.ProfilePicture;
                    authenticateVM.EmploymentCategory = data.EmploymentCategory;
                    authenticateVM.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)data.EmploymentCategory);
                    authenticateVM.IsTrialMode = company.IsTrialMode;
                    authenticateVM.CompanyName = company.CompanyName;
                    response.Data = authenticateVM;

                    tbl_LoginHistory objLoginHistory = new tbl_LoginHistory();
                    objLoginHistory.EmployeeId = data.EmployeeId;
                    objLoginHistory.LoginDate = CommonMethod.CurrentIndianDateTime();
                    objLoginHistory.LocationFrom = authenticateRequestVM.LocationFrom;
                    objLoginHistory.SiteId = data.CompanyId;
                    objLoginHistory.Latitude = authenticateRequestVM.Latitude;
                    objLoginHistory.Longitude = authenticateRequestVM.Longitude;
                    objLoginHistory.CreatedBy = data.EmployeeId;
                    objLoginHistory.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    objLoginHistory.ModifiedBy = data.EmployeeId;
                    objLoginHistory.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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

        [Route("AdminAuthenticate"), HttpPost]
        public ResponseDataModel<AuthenticateVM> AdminAuthenticate(CompanyAdminAuthenticateRequestVM authenticateRequestVM)
        {
            ResponseDataModel<AuthenticateVM> response = new ResponseDataModel<AuthenticateVM>();
            AuthenticateVM authenticateVM = new AuthenticateVM();

            try
            {
                var data = _db.tbl_AdminUser.Where(x => x.AdminUserId == authenticateRequestVM.CompanyAdminId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    tbl_Company company = _db.tbl_Company.Where(x => x.CompanyId == data.CompanyId).FirstOrDefault();
                    UserTokenVM userToken = new UserTokenVM()
                    {                        
                        RoleId = (int)AdminRoles.CompanyAdmin,
                        UserName = data.FirstName + " " + data.LastName,
                        CompanyId = (long)data.CompanyId,
                        CompanyTypeId = company.CompanyTypeId,
                        IsTrailMode = company.IsTrialMode,
                        CompanyAdminId = data.AdminUserId
                    };

                    JWTAccessTokenVM tokenVM = new JWTAccessTokenVM();
                    tokenVM = JWTAuthenticationHelper.GenerateToken(userToken); 
                    authenticateVM.Access_token = tokenVM.Token; 

                    authenticateVM.RoleId = (int)AdminRoles.CompanyAdmin;
                    authenticateVM.CompanyId = (long)data.CompanyId;
                    authenticateVM.CompanyTypeId = company.CompanyTypeId;
                    authenticateVM.Prefix = data.Prefix;
                    authenticateVM.FirstName = data.FirstName;
                    authenticateVM.LastName = data.LastName;
                    authenticateVM.Email = data.EmailId; 
                    authenticateVM.Password = data.Password;
                    authenticateVM.MobileNo = data.MobileNo;
                    authenticateVM.AlternateMobile = data.AlternateMobileNo;
                    authenticateVM.Address = data.Address;
                    authenticateVM.City = data.City;
                    authenticateVM.Designation = data.Designation;
                    authenticateVM.Dob = data.DOB; 
                    //authenticateVM.ProfilePicture = CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + data.ProfilePhoto; 
                    authenticateVM.IsTrialMode = company.IsTrialMode;
                    authenticateVM.CompanyName = company.CompanyName;
                    response.Data = authenticateVM; 
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

                    Random random = new Random();
                    int num = random.Next(555555, 999999);

                    int SmsId = (int)SMSType.EmployeeLoginOTP;
                    string msg = CommonMethod.GetSmsContent(SmsId);
                    msg = msg.Replace("{#var#}", num.ToString());
                    msg = msg.Replace("\r\n", "\n");

                    var json = CommonMethod.SendSMSWithoutLog(msg, data.MobileNo);

                    if (json.Contains("invalidnumber"))
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.InvalidMobileNo);
                    }
                    else
                    {
                        loginResponseVM.OTP = num.ToString();
                    }

                    response.Data = loginResponseVM;
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
                    tbl_Company companyObj = _db.tbl_Company.Where(x => x.CompanyId == data.CompanyId).FirstOrDefault();


                    Random random = new Random();
                    int num = random.Next(555555, 999999);

                    int SmsId = (int)SMSType.ForgetPasswordOTP;
                    string msg = CommonMethod.GetSmsContent(SmsId);

                    Regex regReplace = new Regex("{#var#}");
                    msg = regReplace.Replace(msg, data.FirstName + " " + data.LastName, 1);
                    msg = regReplace.Replace(msg, num.ToString(), 1);

                    msg = msg.Replace("\r\n", "\n");

                    //var json = CommonMethod.SendSMS(msg, data.MobileNo);
                    ResponseDataModel<string> smsResponse = CommonMethod.SendSMS(msg, data.MobileNo, data.CompanyId, data.EmployeeId, data.EmployeeCode, data.EmployeeId, companyObj.IsTrialMode);
                    if (smsResponse.IsError)
                    {
                        response.IsError = true;
                        response.ErrorData = smsResponse.ErrorData;
                    }
                    else
                    {
                        loginResponseVM.OTP = num.ToString();
                    }

                    response.Data = loginResponseVM;
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
                        data.UpdatedDate = CommonMethod.CurrentIndianDateTime();
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
        public ResponseDataModel<string> ValidateFingerprint(EmployeeFingerprintVM fingerprintVM)
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

        [HttpGet]
        [Route("GetEmployeeFingerPrintList/{Id}")]
        public ResponseDataModel<List<EmployeeFingerprintVM>> GetEmployeeFingerPrintList(long Id)
        {
            ResponseDataModel<List<EmployeeFingerprintVM>> response = new ResponseDataModel<List<EmployeeFingerprintVM>>();
            try
            {
                if (Id > 0)
                {
                    List<EmployeeFingerprintVM> lstEmployeeFingerPrints = (from f in _db.tbl_EmployeeFingerprint
                                                                           where f.EmployeeId == Id
                                                                           select new EmployeeFingerprintVM
                                                                           {
                                                                               EmployeeId = f.EmployeeId,
                                                                               ISOCode = f.ISOCode
                                                                           }).ToList();
                    response.Data = lstEmployeeFingerPrints;
                }
                else
                {
                    response.IsError = true;
                    response.AddError("Employee Id not found");
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