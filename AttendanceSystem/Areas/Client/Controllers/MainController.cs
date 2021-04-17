using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class MainController : Controller
    {
        // GET: Client/Main
        public ActionResult Index()
        {
            return View();
        }
    }
}