using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public LoginResponseVM Login(LoginRequestVM loginRequestVM)
        {
            LoginResponseVM response = new LoginResponseVM();
            try
            {

                IEnumerable<string> HeaderAccessKey;
                this.Request.Headers.TryGetValues("Authorization", out HeaderAccessKey);
                string accessKey = HeaderAccessKey.ToList().FirstOrDefault().ToString();

                if (!string.IsNullOrEmpty(loginRequestVM.UserName) && !string.IsNullOrEmpty(loginRequestVM.PassWord))
                {
                    string DecryptPassword = string.Empty;

                    var data = _db.tbl_AdminUser.Where(x => x.UserName == loginRequestVM.UserName && x.Password == loginRequestVM.PassWord).FirstOrDefault();

                    if (data != null)
                    {
                        string Name = data.FirstName + ' ' + data.LastName;
                        response.Status = (int)Status.Success;
                        UserTokenVM userToken = new UserTokenVM()
                        {
                            UserId = data.UserName,
                            Role = data.AdminUserRoleId.ToString(),
                            UserName = Name

                        };

                        JWTAccessTokenVM tokenVM = new JWTAccessTokenVM();
                        tokenVM = JWTAuthenticationHelper.GenerateToken(userToken);
                        response.access_token = tokenVM.Token;
                    }
                    else
                    {
                        response.Status = (int)Status.Failure;
                        response.ErrorMessage = ErrorMessage.YouAreNotAuthorized;
                    }
                }
                else
                {
                    response.Status = (int)Status.Failure;
                    response.ErrorMessage = ErrorMessage.UserNamePasswordRequired;
                }
            }
            catch (Exception ex)
            {
                response.Status = (int)Status.Failure;
                response.ErrorMessage = ex.Message;
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

    }
}