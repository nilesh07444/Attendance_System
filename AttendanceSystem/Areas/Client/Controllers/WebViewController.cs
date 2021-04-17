using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class WebViewController : Controller
    {
        // GET: Client/WebView  
        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult TermsCondition()
        {
            return View();
        }

    }
}