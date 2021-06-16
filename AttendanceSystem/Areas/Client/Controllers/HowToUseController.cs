using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class HowToUseController : Controller
    {
        AttendanceSystemEntities _db;
        public HowToUseController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<tbl_DynamicContent> lstContent = _db.tbl_DynamicContent.Where(x => x.DynamicContentType == (int)DynamicContents.HowToUse).OrderBy(x => x.SeqNo).ToList();
            ViewData["lstContent"] = lstContent;

            var HeroImageName = _db.tbl_Setting.FirstOrDefault().HeroHowToUsePageImageName;
            ViewBag.HeroUrl = ErrorMessage.HeroDirectoryPath + HeroImageName;

            return View();
        }
    }
}