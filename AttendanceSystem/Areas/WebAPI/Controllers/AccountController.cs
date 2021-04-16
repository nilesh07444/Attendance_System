using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttendanceSystem.Models;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class AccountController : ApiController
    {

        private readonly AttendanceSystemEntities _db;
        public AccountController()
        {
            _db = new AttendanceSystemEntities();
        }

        [Route("TestMethod"), HttpGet]
        public string TestMethod()
        {            
            return "Hello Nilesh";
        }


    }
}