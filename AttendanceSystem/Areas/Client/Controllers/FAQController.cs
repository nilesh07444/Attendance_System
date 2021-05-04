using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class FAQController : Controller
    {
        AttendanceSystemEntities _db;
        public FAQController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<tbl_DynamicContent> lstContent = _db.tbl_DynamicContent.Where(x => x.DynamicContentType == (int)DynamicContents.FAQ).OrderBy(x => x.SeqNo).ToList();
            ViewData["lstContent"] = lstContent;
            return View();
        }
    }
}