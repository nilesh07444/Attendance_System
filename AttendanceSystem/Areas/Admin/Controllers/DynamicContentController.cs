using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class DynamicContentController : Controller
    {
        // GET: Admin/DynamicContent
        public ActionResult Index()
        {
            return View();
        } 
    }
}