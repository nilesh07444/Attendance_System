using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class TermsConditionController : Controller
    {
        AttendanceSystemEntities _db;
        public TermsConditionController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<tbl_DynamicContent> lstContent = _db.tbl_DynamicContent.Where(x => x.DynamicContentType == (int)DynamicContents.TermsCondition).OrderBy(x => x.SeqNo).ToList();
            ViewData["lstContent"] = lstContent;

            var HeroImageName = _db.tbl_Setting.FirstOrDefault().HeroTermsConditionPageImageName;
            ViewBag.HeroUrl = ErrorMessage.HeroDirectoryPath + HeroImageName;

            return View();
        }
    }
}