using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class DynamicContentController : Controller
    {
        // GET: Client/DynamicContent
        public ActionResult Index()
        {
            return View();
        }

        private List<SelectListItem> GetDynamicContentType()
        {
            List<SelectListItem> lst = new List<SelectListItem>();
            lst.Add(new SelectListItem { Value = "1", Text = "FAQ" });
            lst.Add(new SelectListItem { Value = "2", Text = "Privacy Policy" });
            lst.Add(new SelectListItem { Value = "3", Text = "Terms & Condition" });            
            return lst;
        }

    }
}