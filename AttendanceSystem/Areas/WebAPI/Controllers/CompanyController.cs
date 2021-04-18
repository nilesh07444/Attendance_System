using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class CompanyController : BaseUserController
    {
        [HttpGet]
        [Route("TestMethod")]
        public string TestMethod()
        {
            string psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            string password = "12345";
            string encryptedPwd = CommonMethod.Encrypt(password, psSult);

            return encryptedPwd;
        }
    }
}