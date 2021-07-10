using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class UnAuthorizeAccessController : Controller
    {
        // GET: Admin/UnAuthorizeAccess
        public ActionResult Index()
        {
            return View();
        }
    }
}