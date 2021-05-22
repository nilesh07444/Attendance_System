using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class AboutController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public string HomeDirectoryPath = "";
        public AboutController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index()
        {
            List<OurClientVM> lstOurClients = new List<OurClientVM>();
            List<TestimonialVM> lstTestimonials = new List<TestimonialVM>();

            try
            {
                lstOurClients = (from s in _db.tbl_Sponsor
                                 where !s.IsDeleted && s.IsActive
                                 select new OurClientVM
                                 {
                                     SponsorId = s.SponsorId,
                                     SponsorName = s.SponsorName,
                                     SponsorImage = s.SponsorImage,
                                     SponsorLink = s.SponsorLink
                                 }).OrderByDescending(x => x.SponsorId).ToList();

                lstTestimonials = (from t in _db.tbl_Testimonial
                                   where t.IsActive
                                   select new TestimonialVM
                                   {
                                       TestimonialId = t.TestimonialId,
                                       CompanyName = t.CompanyName,
                                       CompanyPersonName = t.CompanyPersonName,
                                       FeedbackText = t.FeedbackText,
                                       IsActive = t.IsActive,
                                   }).OrderByDescending(x => x.TestimonialId).ToList();

                ViewData["lstOurClients"] = lstOurClients;
                ViewData["lstTestimonials"] = lstTestimonials;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View();
        }
    }
}