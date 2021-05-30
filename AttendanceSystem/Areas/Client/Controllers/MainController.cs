using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class MainController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public string HomeDirectoryPath = "";
        public string ServiceDirectoryPath = "";
        public MainController()
        {
            _db = new AttendanceSystemEntities();
            HomeDirectoryPath = ErrorMessage.HomeDirectoryPath;
            ServiceDirectoryPath = ErrorMessage.ServiceDirectoryPath;
        }
        public ActionResult Index()
        {
            List<HomeImageVM> lstHomeImages = new List<HomeImageVM>();
            List<PackageVM> lstAccountPackages = new List<PackageVM>();
            List<SMSPackageVM> lstSMSPackages = new List<SMSPackageVM>();
            List<OurClientVM> lstOurClients = new List<OurClientVM>();
            List<TestimonialVM> lstTestimonials = new List<TestimonialVM>();

            try
            {

                tbl_Setting objGensetting = _db.tbl_Setting.FirstOrDefault();

                //
                //string ToEmail = "prajapati.nileshbhai@gmail.com";

                //string FromEmail = objGensetting.SMTPFromEmailId;
                //string Subject = "Your Registration is Successful - Contract Book";
                //string bodyhtml = "Following are the detail:<br/>";
                //bodyhtml += "===============================<br/>";
                //bodyhtml += "Username: UN/16052021/1" + "<br/>";
                //bodyhtml += "Password: 12345" + "<br/>";

                //CommonMethod.SendEmail(ToEmail, FromEmail, Subject, bodyhtml);
                //

                lstHomeImages = (from hi in _db.tbl_HomeImage
                                 where hi.IsActive
                                 select new HomeImageVM
                                 {
                                     HomeImageId = hi.HomeImageId,
                                     HomeImageName = hi.HomeImageName,
                                     HeadingText1 = hi.HeadingText1,
                                     HeadingText2 = hi.HeadingText2,
                                     IsActive = hi.IsActive,
                                 }).OrderByDescending(x => x.HomeImageId).ToList();

                lstAccountPackages = (from pck in _db.tbl_Package
                                      where !pck.IsDeleted && pck.IsActive
                                      select new PackageVM
                                      {
                                          PackageId = pck.PackageId,
                                          PackageName = pck.PackageName,
                                          Amount = pck.Amount,
                                          PackageDescription = pck.PackageDescription,
                                          AccessDays = pck.AccessDays,
                                          IsActive = pck.IsActive,
                                          PackageImage = pck.PackageImage,
                                          NoOfSMS = pck.NoOfSMS,
                                          NoOfEmployee = pck.NoOfEmployee,
                                          PackageColorCode = pck.PackageColorCode,
                                          PackageFontIcon = pck.PackageFontIcon
                                      }).OrderByDescending(x => x.PackageId).ToList();

                lstSMSPackages = (from pck in _db.tbl_SMSPackage
                                  where !pck.IsDeleted && pck.IsActive
                                  select new SMSPackageVM
                                  {
                                      SMSPackageId = pck.SMSPackageId,
                                      PackageName = pck.PackageName,
                                      PackageAmount = pck.PackageAmount,
                                      AccessDays = pck.AccessDays,
                                      IsActive = pck.IsActive,
                                      NoOfSMS = pck.NoOfSMS,
                                      PackageColorCode = pck.PackageColorCode,
                                      PackageFontIcon = pck.PackageFontIcon
                                  }).OrderByDescending(x => x.SMSPackageId).ToList();

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

                if (objGensetting != null)
                {
                    if (!string.IsNullOrEmpty(objGensetting.HomeImage))
                    {
                        ViewBag.HomeFirstImage = HomeDirectoryPath + objGensetting.HomeImage;
                    }
                    if (!string.IsNullOrEmpty(objGensetting.HomeImage2))
                    {
                        ViewBag.HomeImage2 = HomeDirectoryPath + objGensetting.HomeImage2;
                    }
                    if (!string.IsNullOrEmpty(objGensetting.ServiceImage))
                    {
                        ViewBag.ServiceImage = ServiceDirectoryPath + objGensetting.ServiceImage;
                    }
                }

                ViewData["lstHomeImages"] = lstHomeImages;
                ViewData["lstAccountPackages"] = lstAccountPackages;
                ViewData["lstSMSPackages"] = lstSMSPackages;
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