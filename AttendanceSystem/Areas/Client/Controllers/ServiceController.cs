using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class ServiceController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public string ServiceDirectoryPath = "";
        public ServiceController()
        {
            _db = new AttendanceSystemEntities();
            ServiceDirectoryPath = ErrorMessage.ServiceDirectoryPath;
        }

        public ActionResult Index()
        {

            try
            {
                tbl_Setting objGensetting = _db.tbl_Setting.FirstOrDefault();

                if (objGensetting != null)
                {
                    if (!string.IsNullOrEmpty(objGensetting.ServiceImage))
                    {
                        ViewBag.ServiceImage = ServiceDirectoryPath + objGensetting.ServiceImage;
                    }
                }

                var HeroImageName = _db.tbl_Setting.FirstOrDefault().HeroServicePageImageName;
                ViewBag.HeroUrl = ErrorMessage.HeroDirectoryPath + HeroImageName;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
    }
}