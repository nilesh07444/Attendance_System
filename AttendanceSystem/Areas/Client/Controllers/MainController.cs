﻿using AttendanceSystem.Helper;
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
        AttendanceSystemEntities _db;
        public string HomeDirectoryPath = "";
        public MainController()
        {
            _db = new AttendanceSystemEntities();
            HomeDirectoryPath = ErrorMessage.HomeDirectoryPath;
        }
        public ActionResult Index()
        {
            List<HomeImageVM> lstHomeImages = new List<HomeImageVM>();
            List<PackageVM> lstAccountPackages = new List<PackageVM>();
            List<SMSPackageVM> lstSMSPackages = new List<SMSPackageVM>();

            try
            {

                //
                //string ToEmail = "prajapati.nileshbhai@gmail.com";
                //tbl_Setting objGensetting = _db.tbl_Setting.FirstOrDefault();
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

                if (lstHomeImages != null)
                {
                    ViewBag.HomeFirstImage = HomeDirectoryPath + lstHomeImages.FirstOrDefault().HomeImageName;
                }

                ViewData["lstHomeImages"] = lstHomeImages;
                ViewData["lstAccountPackages"] = lstAccountPackages;
                ViewData["lstSMSPackages"] = lstSMSPackages; 
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View();
        }
    }
}