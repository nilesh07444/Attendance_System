using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using AttendanceSystem.ViewModel.WebAPI.ViewModel;
using System;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class SettingController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        private string psSult = string.Empty;
        public SettingController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
        }


        [HttpPost]
        [Route("ResetPassword")]
        public ResponseDataModel<bool> ResetPassword(ResetPasswordVM resetPasswordVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.Data = false;
            try
            {
                if (resetPasswordVM.EmployeeId > 0 && !string.IsNullOrEmpty(resetPasswordVM.CurrentPassWord) && !string.IsNullOrEmpty(resetPasswordVM.NewPassWord))
                {
                    string encryptedPwd = CommonMethod.Encrypt(resetPasswordVM.CurrentPassWord, psSult);
                    string encryptedNewPwd = CommonMethod.Encrypt(resetPasswordVM.NewPassWord, psSult);

                    tbl_Employee data = _db.tbl_Employee.Where(x => x.EmployeeId == resetPasswordVM.EmployeeId && x.Password == encryptedPwd && x.IsActive && !x.IsDeleted).FirstOrDefault();

                    if (data != null)
                    {
                        data.Password = encryptedNewPwd;
                        data.UpdatedBy = resetPasswordVM.EmployeeId;
                        data.UpdatedDate = DateTime.UtcNow;
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
        public ResponseDataModel<AuthenticateVM> GetMyProfile(int employeeId)
        {
            ResponseDataModel<AuthenticateVM> response = new ResponseDataModel<AuthenticateVM>();
            AuthenticateVM authenticateVM = new AuthenticateVM();

            try
            {
                var data = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId && x.IsActive && !x.IsDeleted).FirstOrDefault();
                if (data != null)
                {
                    UserTokenVM userToken = new UserTokenVM()
                    {
                        UserId = data.EmployeeCode,
                        Role = data.AdminRoleId.ToString(),
                        UserName = data.FirstName + " " + data.LastName
                    };

                    JWTAccessTokenVM tokenVM = new JWTAccessTokenVM();
                    tokenVM = JWTAuthenticationHelper.GenerateToken(userToken);
                    authenticateVM.Access_token = tokenVM.Token;
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
                    authenticateVM.ProfilePicture = data.ProfilePicture;
                    authenticateVM.EmploymentCategory = data.EmploymentCategory;
                    response.Data = authenticateVM;
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
    }
}