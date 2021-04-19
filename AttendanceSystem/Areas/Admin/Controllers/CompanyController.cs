using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class CompanyController : Controller
    {
        // GET: Admin/Company
        public ActionResult Registered()
        {
            return View();
        }

        public ActionResult Requests()
        {
            return View();
        }

        public ActionResult Renew()
        {
            return View();
        }

    }
}