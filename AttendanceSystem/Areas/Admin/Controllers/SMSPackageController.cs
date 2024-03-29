﻿using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class SMSPackageController : Controller
    {
        // GET: Admin/SMSPackage
        AttendanceSystemEntities _db;
        public string SMSPackageDirectoryPath = "";
        public SMSPackageController()
        {
            _db = new AttendanceSystemEntities();
            SMSPackageDirectoryPath = ErrorMessage.SMSPackageDirectoryPath;
        }
        public ActionResult Index()
        {
            List<SMSPackageVM> SMSPackage = new List<SMSPackageVM>();
            try
            {
                SMSPackage = (from pck in _db.tbl_SMSPackage
                              where !pck.IsDeleted
                              select new SMSPackageVM
                              {
                                  SMSPackageId = pck.SMSPackageId,
                                  PackageName = pck.PackageName,
                                  PackageAmount = pck.PackageAmount,
                                  AccessDays = pck.AccessDays,
                                  IsActive = pck.IsActive,
                                  PackageImage = pck.PackageImage,
                                  NoOfSMS = pck.NoOfSMS,
                                  PackageColorCode = pck.PackageColorCode,
                                  PackageFontIcon = pck.PackageFontIcon,
                                  Description = pck.Description
                              }).OrderByDescending(x => x.SMSPackageId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(SMSPackage);
        }

        public ActionResult Add(long id)
        {
            SMSPackageVM SMSPackageVM = new SMSPackageVM();
            if (id > 0)
            {
                SMSPackageVM = (from pkg in _db.tbl_SMSPackage
                                where pkg.SMSPackageId == id && !pkg.IsDeleted
                                select new SMSPackageVM
                                {
                                    SMSPackageId = pkg.SMSPackageId,
                                    PackageName = pkg.PackageName,
                                    PackageAmount = pkg.PackageAmount,
                                    NoOfSMS = pkg.NoOfSMS,
                                    AccessDays = pkg.AccessDays,
                                    PackageImage = pkg.PackageImage,
                                    IsActive = pkg.IsActive,
                                    PackageColorCode = pkg.PackageColorCode,
                                    PackageFontIcon = pkg.PackageFontIcon,
                                    Description = pkg.Description
                                }).FirstOrDefault();
            }

            return View(SMSPackageVM);
        }

        [HttpPost]
        public ActionResult Add(SMSPackageVM SMSPackageVM, HttpPostedFileBase PackageImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = (int)PaymentGivenBy.SuperAdmin;

                    string fileName = string.Empty;
                    string path = Server.MapPath(SMSPackageDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (PackageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(PackageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("PackageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(SMSPackageVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(PackageImageFile.FileName);
                        PackageImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        if (SMSPackageVM.SMSPackageId == 0)
                        {
                            ModelState.AddModelError("PackageImageFile", ErrorMessage.ImageRequired);
                            return View(SMSPackageVM);
                        }
                    }

                    if (SMSPackageVM.SMSPackageId > 0)
                    {
                        tbl_SMSPackage objSMSPackage = _db.tbl_SMSPackage.Where(x => x.SMSPackageId == SMSPackageVM.SMSPackageId).FirstOrDefault();
                        objSMSPackage.PackageImage = PackageImageFile != null ? fileName : objSMSPackage.PackageImage;
                        objSMSPackage.PackageName = SMSPackageVM.PackageName;
                        objSMSPackage.PackageAmount = SMSPackageVM.PackageAmount;
                        objSMSPackage.NoOfSMS = SMSPackageVM.NoOfSMS;
                        objSMSPackage.AccessDays = SMSPackageVM.AccessDays;
                        objSMSPackage.PackageColorCode = SMSPackageVM.PackageColorCode;
                        objSMSPackage.PackageFontIcon = SMSPackageVM.PackageFontIcon;
                        objSMSPackage.Description = SMSPackageVM.Description;
                        objSMSPackage.ModifiedBy = LoggedInUserId;
                        objSMSPackage.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_SMSPackage objSMSPackage = new tbl_SMSPackage();
                        objSMSPackage.PackageName = SMSPackageVM.PackageName;
                        objSMSPackage.PackageAmount = SMSPackageVM.PackageAmount;
                        objSMSPackage.NoOfSMS = SMSPackageVM.NoOfSMS;
                        objSMSPackage.AccessDays = SMSPackageVM.AccessDays;
                        objSMSPackage.PackageImage = fileName;
                        objSMSPackage.PackageColorCode = SMSPackageVM.PackageColorCode;
                        objSMSPackage.PackageFontIcon = SMSPackageVM.PackageFontIcon;
                        objSMSPackage.Description = SMSPackageVM.Description;
                        objSMSPackage.IsActive = true;
                        objSMSPackage.CreatedBy = LoggedInUserId;
                        objSMSPackage.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objSMSPackage.ModifiedBy = LoggedInUserId;
                        objSMSPackage.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_SMSPackage.Add(objSMSPackage);
                    }
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(SMSPackageVM);
        }

        public ActionResult View(int id)
        {
            SMSPackageVM SMSPackageVM = new SMSPackageVM();

            SMSPackageVM = (from pkg in _db.tbl_SMSPackage
                            where pkg.SMSPackageId == id && !pkg.IsDeleted
                            select new SMSPackageVM
                            {
                                SMSPackageId = pkg.SMSPackageId,
                                PackageName = pkg.PackageName,
                                PackageAmount = pkg.PackageAmount,
                                NoOfSMS = pkg.NoOfSMS,
                                AccessDays = pkg.AccessDays,
                                PackageImage = pkg.PackageImage,
                                IsActive = pkg.IsActive,
                                PackageColorCode = pkg.PackageColorCode,
                                PackageFontIcon = pkg.PackageFontIcon,
                                Description = pkg.Description
                            }).FirstOrDefault();

            return View(SMSPackageVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_SMSPackage objSMSPackage = _db.tbl_SMSPackage.Where(x => x.SMSPackageId == Id).FirstOrDefault();

                if (objSMSPackage != null)
                {
                    long LoggedInUserId = (int)PaymentGivenBy.SuperAdmin;
                    if (Status == "Active")
                    {
                        objSMSPackage.IsActive = true;
                    }
                    else
                    {
                        objSMSPackage.IsActive = false;
                    }

                    objSMSPackage.ModifiedBy = LoggedInUserId;
                    objSMSPackage.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                    _db.SaveChanges();
                    ReturnMessage = ErrorMessage.Success;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = ErrorMessage.Exception;
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeleteSMSPackage(int SMSPackageId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_SMSPackage objSMSPackage = _db.tbl_SMSPackage.Where(x => x.SMSPackageId == SMSPackageId).FirstOrDefault();

                if (objSMSPackage == null)
                {
                    ReturnMessage = ErrorMessage.NotFound;
                }
                else
                {
                    long LoggedInUserId = (int)PaymentGivenBy.SuperAdmin;
                    objSMSPackage.IsDeleted = true;
                    objSMSPackage.ModifiedBy = LoggedInUserId;
                    objSMSPackage.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.SaveChanges();

                    ReturnMessage = ErrorMessage.Success;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = ErrorMessage.Exception;
            }

            return ReturnMessage;
        }
    }
}