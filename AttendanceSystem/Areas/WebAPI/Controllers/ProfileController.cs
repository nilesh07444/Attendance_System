using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using AttendanceSystem.ViewModel.WebAPI.ViewModel;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class ProfileController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        private string psSult = string.Empty;
        private string enviornment = string.Empty;
        string domainUrl = string.Empty;
        public ProfileController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
            domainUrl = ConfigurationManager.AppSettings["DomainUrl"].ToString();
        }

        [HttpPost]
        [Route("ChangePassword")]
        public ResponseDataModel<LoginResponseVM> ChangePassword(ProfileChangePasswordVM resetPasswordVM)
        {
            ResponseDataModel<LoginResponseVM> response = new ResponseDataModel<LoginResponseVM>();

            try
            {
                long employeeId = base.UTI.EmployeeId;

                LoginResponseVM loginResponseVM = new LoginResponseVM();

                if (!string.IsNullOrEmpty(resetPasswordVM.CurrentPassWord) && !string.IsNullOrEmpty(resetPasswordVM.NewPassword))
                {
                    string encryptedPwd = CommonMethod.Encrypt(resetPasswordVM.CurrentPassWord, psSult);
                    string encryptedNewPwd = CommonMethod.Encrypt(resetPasswordVM.NewPassword, psSult);
                    string encryptedConfirmPwd = CommonMethod.Encrypt(resetPasswordVM.ConfirmPassword, psSult);

                    if (encryptedNewPwd != encryptedConfirmPwd)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.NewPasswordAndConfirmPasswordMustBeSame);
                    }

                    if (!response.IsError)
                    {
                        tbl_Employee data = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId && x.Password == encryptedPwd && x.IsActive && !x.IsDeleted).FirstOrDefault();

                        if (data != null)
                        {
                            Random random = new Random();
                            int num = random.Next(555555, 999999);
                             
                            int SmsId = (int)SMSType.ChangePasswordOTP;
                            string msg = CommonMethod.GetSmsContent(SmsId);

                            Regex regReplace = new Regex("{#var#}");
                            msg = regReplace.Replace(msg, data.FirstName + " " + data.LastName, 1);
                            msg = regReplace.Replace(msg, num.ToString(), 1);

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
                             
                            loginResponseVM.EmployeeId = employeeId;
                            loginResponseVM.IsFingerprintEnabled = data.IsFingerprintEnabled;

                            response.Data = loginResponseVM;
                        }
                        else
                        {
                            response.IsError = true;
                            response.AddError(ErrorMessage.InvalidPassword);
                        }
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CurrentAndNewBothPasswordRequired);
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
        [Route("SetPassword")]
        public ResponseDataModel<bool> SetPassword(SetPasswordVM setPasswordVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.Data = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                if (!string.IsNullOrEmpty(setPasswordVM.NewPassword))
                {
                    string encryptedPwd = CommonMethod.Encrypt(setPasswordVM.NewPassword, psSult);

                    tbl_Employee data = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();

                    if (data != null)
                    {
                        data.Password = encryptedPwd;
                        data.UpdatedBy = employeeId;
                        data.UpdatedDate = CommonMethod.CurrentIndianDateTime();
                        response.IsError = false;
                        response.Data = true;
                        _db.SaveChanges();
                    }
                    else
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.InvalidPassword);
                    }
                }
                else
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.CurrentAndNewBothPasswordRequired);
                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }
         
        [Route("GetMyProfile"), HttpGet]
        public ResponseDataModel<AuthenticateVM> GetMyProfile()
        {
            ResponseDataModel<AuthenticateVM> response = new ResponseDataModel<AuthenticateVM>();
            AuthenticateVM authenticateVM = new AuthenticateVM();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                tbl_Employee data = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    tbl_Company companyObj = _db.tbl_Company.Where(x => x.CompanyId == data.CompanyId).FirstOrDefault();
                    string companyTypeName = _db.mst_CompanyType.Where(x => x.CompanyTypeId == companyObj.CompanyTypeId).Select(x => x.CompanyTypeName).FirstOrDefault();
                    authenticateVM.EmployeeId = data.EmployeeId;
                    authenticateVM.CompanyId = data.CompanyId;
                    authenticateVM.CompanyName = companyObj.CompanyName;
                    authenticateVM.CompanyTypeId = companyObj.CompanyTypeId;
                    authenticateVM.CompanyTypeText = companyTypeName;
                    authenticateVM.RoleId = data.AdminRoleId;
                    authenticateVM.RoleName = CommonMethod.GetEnumDescription((AdminRoles)data.AdminRoleId);
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
                    authenticateVM.ProfilePicture = domainUrl + ErrorMessage.EmployeeDirectoryPath + data.ProfilePicture;
                    authenticateVM.EmploymentCategory = data.EmploymentCategory;
                    authenticateVM.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)data.EmploymentCategory);
                    authenticateVM.IsFingerprintEnabled = data.IsFingerprintEnabled;
                    authenticateVM.IsLeaveForward = data.IsLeaveForward;
                    authenticateVM.State = string.Empty;
                    authenticateVM.Pincode = string.Empty;
                    authenticateVM.MonthlySalaryPrice = data.MonthlySalaryPrice;
                    authenticateVM.ExtraPerHourPrice = data.ExtraPerHourPrice;
                    authenticateVM.NoOfFreeLeavePerMonth = data.NoOfFreeLeavePerMonth;
                    authenticateVM.PerCategoryPrice = data.PerCategoryPrice;
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

        [Route("ResendOtp"), HttpGet]
        public ResponseDataModel<LoginResponseVM> ResendOtp()
        {
            ResponseDataModel<LoginResponseVM> response = new ResponseDataModel<LoginResponseVM>();
            LoginResponseVM loginResponseVM = new LoginResponseVM();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                var data = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    response.IsError = false;
                    loginResponseVM.IsFingerprintEnabled = data.IsFingerprintEnabled;
                    loginResponseVM.EmployeeId = data.EmployeeId;

                    Random random = new Random();
                    int num = random.Next(555555, 999999);

                    int SmsId = (int)SMSType.ChangePasswordOTP;
                    string msg = CommonMethod.GetSmsContent(SmsId);

                    Regex regReplace = new Regex("{#var#}");
                    msg = regReplace.Replace(msg, data.FirstName + " " + data.LastName, 1);
                    msg = regReplace.Replace(msg, num.ToString(), 1);

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
    }
}
