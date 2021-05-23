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
            List<OurClientVM> lstOurClients = new List<OurClientVM>();
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

                lstOurClients = (from s in _db.tbl_Sponsor
                                 where !s.IsDeleted && s.IsActive
                                 select new OurClientVM
                                 {
                                     SponsorId = s.SponsorId,
                                     SponsorName = s.SponsorName,
                                     SponsorImage = s.SponsorImage,
                                     SponsorLink = s.SponsorLink
                                 }).OrderByDescending(x => x.SponsorId).ToList();

                ViewData["lstOurClients"] = lstOurClients;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
    }
}