using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class LoginController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public LoginController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            return View();
        }
    }
}