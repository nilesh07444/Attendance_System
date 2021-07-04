﻿using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Client.Controllers
{
    public class CompanyRequestController : Controller
    {
        AttendanceSystemEntities _db;
        public string companyGSTDirectoryPath = "";
        public string companyPanDirectoryPath = "";
        public string companyLogoDirectoryPath = "";
        public string companyRegisterProofDirectoryPath = "";
        public string aadharCardDirectoryPath = "";
        public string panCardDirectoryPath = "";
        public string CompanyAdminProfileDirectoryPath = "";
        string enviornment;
        bool? setOtp;

        public CompanyRequestController()
        {
            _db = new AttendanceSystemEntities();
            companyGSTDirectoryPath = ErrorMessage.CompanyGSTDirectoryPath;
            companyPanDirectoryPath = ErrorMessage.CompanyPanCardDirectoryPath;
            companyLogoDirectoryPath = ErrorMessage.CompanyLogoDirectoryPath;
            companyRegisterProofDirectoryPath = ErrorMessage.CompanyRegisterProofDirectoryPath;
            aadharCardDirectoryPath = ErrorMessage.AdharcardDirectoryPath;
            panCardDirectoryPath = ErrorMessage.PancardDirectoryPath;
            CompanyAdminProfileDirectoryPath = ErrorMessage.ProfileDirectoryPath;
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
            setOtp = Convert.ToBoolean(ConfigurationManager.AppSettings["SetOtp"].ToString());
        }

        public ActionResult Index()
        {
            CompanyRequestVM companyRequestVM = new CompanyRequestVM();
            companyRequestVM.CompanyTypeList = GetCompanyType();

            var HeroImageName = _db.tbl_Setting.FirstOrDefault().HeroCompanyRequestPageImageName;
            ViewBag.HeroUrl = ErrorMessage.HeroDirectoryPath + HeroImageName;

            return View(companyRequestVM);
        }

        [HttpPost]
        public ActionResult Index(CompanyRequestVM companyRequestVM,
            HttpPostedFileBase CompanyGSTPhotoFile,
            HttpPostedFileBase CompanyPanPhotoFile,
            HttpPostedFileBase CompanyLogoImageFile,
            HttpPostedFileBase CompanyRegisterProofImageFile,
            HttpPostedFileBase CompanyAdminProfilePhotoFile,
            HttpPostedFileBase CompanyAdminAadharCardPhotoFile,
            HttpPostedFileBase CompanyAdminPanCardPhotoFile)
        {
            try
            {

                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    var HeroImageName = _db.tbl_Setting.FirstOrDefault().HeroCompanyRequestPageImageName;
                    ViewBag.HeroUrl = ErrorMessage.HeroDirectoryPath + HeroImageName;

                    #region validation

                    if (companyRequestVM.CompanyName.Replace(" ", string.Empty).Length < 2)
                    {
                        ModelState.AddModelError("CompanyName", ErrorMessage.CompanyNameMinimum2CharacterRequired);
                        companyRequestVM.CompanyTypeList = GetCompanyType();
                        return View(companyRequestVM);
                    }

                    #endregion validation

                    string companyGstFileName = string.Empty, companyPanCardFileName = string.Empty, companyLogoFileName = string.Empty, companyRegisterProofFileName = string.Empty,
                        profileFileName = string.Empty, companyAdminAdharCardFileName = string.Empty, companyAdminPancardFileName = string.Empty;
                    bool folderExists = false;

                    #region CompanyGST
                    if (CompanyGSTPhotoFile != null)
                    {
                        string companyGSTPath = Server.MapPath(companyGSTDirectoryPath);
                        folderExists = Directory.Exists(companyGSTPath);
                        if (!folderExists)
                            Directory.CreateDirectory(companyGSTPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyGSTPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP" && ext.ToUpper() != ".PDF")
                        {
                            ModelState.AddModelError("CompanyGSTPhotoFile", ErrorMessage.SelectOnlyImageOrPDF);
                            companyRequestVM.CompanyTypeList = GetCompanyType();
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyGstFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyGSTPhotoFile.FileName);
                        CompanyGSTPhotoFile.SaveAs(companyGSTPath + companyGstFileName);
                    }
                    #endregion CompanyGST

                    #region CompanyPancardImage
                    if (CompanyPanPhotoFile != null)
                    {
                        string companyPanCardPath = Server.MapPath(companyPanDirectoryPath);
                        folderExists = Directory.Exists(companyPanCardPath);
                        if (!folderExists)
                            Directory.CreateDirectory(companyPanCardPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyPanPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP" && ext.ToUpper() != ".PDF")
                        {
                            ModelState.AddModelError("CompanyPanPhotoFile", ErrorMessage.SelectOnlyImageOrPDF);
                            companyRequestVM.CompanyTypeList = GetCompanyType();
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyPanCardFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyPanPhotoFile.FileName);
                        CompanyPanPhotoFile.SaveAs(companyPanCardPath + companyPanCardFileName);
                    }
                    #endregion CompanyPancardImage

                    #region CompanyLogoImage
                    if (CompanyLogoImageFile != null)
                    {
                        string companyLogoPath = Server.MapPath(companyLogoDirectoryPath);

                        folderExists = Directory.Exists(companyLogoPath);
                        if (!folderExists)
                            Directory.CreateDirectory(companyLogoPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyLogoImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP" && ext.ToUpper() != ".PDF")
                        {
                            ModelState.AddModelError("CompanyPhotoFile", ErrorMessage.SelectOnlyImageOrPDF);
                            companyRequestVM.CompanyTypeList = GetCompanyType();
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyLogoFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyLogoImageFile.FileName);
                        CompanyLogoImageFile.SaveAs(companyLogoPath + companyLogoFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyLogoImageFile", ErrorMessage.ImageOrPdfRequired);
                        companyRequestVM.CompanyTypeList = GetCompanyType();
                        return View(companyRequestVM);
                    }
                    #endregion CompanyLogoImage

                    #region CompanyRegisterProofImage
                    if (CompanyRegisterProofImageFile != null)
                    {
                        string companyRegisterProofPath = Server.MapPath(companyRegisterProofDirectoryPath);

                        folderExists = Directory.Exists(companyRegisterProofPath);
                        if (!folderExists)
                            Directory.CreateDirectory(companyRegisterProofPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyRegisterProofImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP" && ext.ToUpper() != ".PDF")
                        {
                            ModelState.AddModelError("CompanyRegisterProofImageFile", ErrorMessage.SelectOnlyImageOrPDF);
                            companyRequestVM.CompanyTypeList = GetCompanyType();
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyRegisterProofFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyRegisterProofImageFile.FileName);
                        CompanyRegisterProofImageFile.SaveAs(companyRegisterProofPath + companyRegisterProofFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyRegisterProofImageFile", ErrorMessage.ImageOrPdfRequired);
                        companyRequestVM.CompanyTypeList = GetCompanyType();
                        return View(companyRequestVM);
                    }
                    #endregion CompanyRegisterProofImage

                    #region profileFileImage

                    if (CompanyAdminProfilePhotoFile != null)
                    {
                        string companyAdminProfileDirectoryPath = Server.MapPath(CompanyAdminProfileDirectoryPath);

                        folderExists = Directory.Exists(companyAdminProfileDirectoryPath);
                        if (!folderExists)
                            Directory.CreateDirectory(companyAdminProfileDirectoryPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyAdminProfilePhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP" && ext.ToUpper() != ".PDF")
                        {
                            ModelState.AddModelError("CompanyAdminProfilePhotoFile", ErrorMessage.SelectOnlyImageOrPDF);
                            companyRequestVM.CompanyTypeList = GetCompanyType();
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        profileFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyAdminProfilePhotoFile.FileName);
                        CompanyAdminProfilePhotoFile.SaveAs(companyAdminProfileDirectoryPath + profileFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyCancellationChequePhotoFile", ErrorMessage.ImageOrPdfRequired);
                        companyRequestVM.CompanyTypeList = GetCompanyType();
                        return View(companyRequestVM);
                    }

                    #endregion profileFileImage

                    #region AdharCardImage
                    if (CompanyAdminAadharCardPhotoFile != null)
                    {
                        string adharCardPath = Server.MapPath(aadharCardDirectoryPath);

                        folderExists = Directory.Exists(adharCardPath);
                        if (!folderExists)
                            Directory.CreateDirectory(adharCardPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyAdminAadharCardPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP" && ext.ToUpper() != ".PDF")
                        {
                            ModelState.AddModelError("CompanyAdminAadharCardPhotoFile", ErrorMessage.SelectOnlyImageOrPDF);
                            companyRequestVM.CompanyTypeList = GetCompanyType();
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyAdminAdharCardFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyAdminAadharCardPhotoFile.FileName);
                        CompanyAdminAadharCardPhotoFile.SaveAs(adharCardPath + companyAdminAdharCardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyAdminAadharCardPhotoFile", ErrorMessage.ImageOrPdfRequired);
                        companyRequestVM.CompanyTypeList = GetCompanyType();
                        return View(companyRequestVM);
                    }
                    #endregion AdharCardImage

                    #region PancardImage
                    if (CompanyAdminPanCardPhotoFile != null)
                    {
                        string panCardPath = Server.MapPath(panCardDirectoryPath);
                        folderExists = Directory.Exists(panCardPath);
                        if (!folderExists)
                            Directory.CreateDirectory(panCardPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyAdminPanCardPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP" && ext.ToUpper() != ".PDF")
                        {
                            ModelState.AddModelError("CompanyAdminPanCardPhotoFile", ErrorMessage.SelectOnlyImageOrPDF);
                            companyRequestVM.CompanyTypeList = GetCompanyType();
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyAdminPancardFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyAdminPanCardPhotoFile.FileName);
                        CompanyAdminPanCardPhotoFile.SaveAs(panCardPath + companyAdminPancardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyAdminPanCardPhotoFile", ErrorMessage.ImageOrPdfRequired);
                        companyRequestVM.CompanyTypeList = GetCompanyType();
                        return View(companyRequestVM);
                    }
                    #endregion PancardImage

                    #region Get Free access days

                    tbl_Setting objSetting = _db.tbl_Setting.FirstOrDefault();
                    int freeAccessDays;
                    if (objSetting != null)
                    {
                        freeAccessDays = objSetting.AccountFreeAccessDays.Value;
                    }
                    else
                    {
                        freeAccessDays = Convert.ToInt32(ConfigurationManager.AppSettings["CompanyFreeAccessDays"].ToString());
                    }

                    #endregion

                    #region Create Company Request
                      
                    DateTime dob_date = Convert.ToDateTime(companyRequestVM.CompanyAdminDOB.ToString());

                    tbl_CompanyRequest objCompany = new tbl_CompanyRequest();
                    objCompany.CompanyTypeId = Convert.ToInt64(companyRequestVM.CompanyTypeId);
                    objCompany.CompanyName = companyRequestVM.CompanyName.ToUpper();
                    objCompany.CompanyEmailId = companyRequestVM.CompanyEmailId;
                    objCompany.CompanyContactNo = companyRequestVM.CompanyContactNo;
                    objCompany.CompanyAlternateContactNo = companyRequestVM.CompanyAlternateContactNo;
                    objCompany.CompanyGSTNo = companyRequestVM.CompanyGSTNo.ToUpper();
                    objCompany.CompanyGSTPhoto = companyGstFileName;
                    objCompany.CompanyPanNo = companyRequestVM.CompanyPanNo.ToUpper();
                    objCompany.CompanyPanPhoto = companyPanCardFileName;
                    objCompany.CompanyAddress = companyRequestVM.CompanyAddress.ToUpper();
                    objCompany.CompanyPincode = companyRequestVM.CompanyPincode;
                    objCompany.CompanyCity = companyRequestVM.CompanyCity.ToUpper();
                    objCompany.CompanyState = companyRequestVM.CompanyState.ToUpper();
                    objCompany.CompanyDistrict = companyRequestVM.CompanyDistrict.ToUpper();
                    objCompany.CompanyLogoImage = companyLogoFileName;
                    objCompany.CompanyRegisterProofImage = companyRegisterProofFileName;
                    objCompany.CompanyDescription = companyRequestVM.CompanyDescription.ToUpper();
                    objCompany.CompanyWebisteUrl = companyRequestVM.CompanyWebisteUrl;
                    objCompany.CompanyAdminPrefix = companyRequestVM.CompanyAdminPrefix.ToUpper();
                    objCompany.CompanyAdminFirstName = companyRequestVM.CompanyAdminFirstName.ToUpper();
                    objCompany.CompanyAdminMiddleName = companyRequestVM.CompanyAdminMiddleName.ToUpper();
                    objCompany.CompanyAdminLastName = companyRequestVM.CompanyAdminLastName.ToUpper();
                    objCompany.CompanyAdminEmailId = companyRequestVM.CompanyAdminEmailId;
                    objCompany.CompanyAdminMobileNo = companyRequestVM.CompanyAdminMobileNo;
                    objCompany.CompanyAdminDOB = dob_date;
                    objCompany.CompanyAdminDateOfMarriageAnniversary = companyRequestVM.CompanyAdminDateOfMarriageAnniversary;
                    objCompany.CompanyAdminAlternateMobileNo = companyRequestVM.CompanyAdminAlternateMobileNo;
                    objCompany.CompanyAdminDesignation = companyRequestVM.CompanyAdminDesignation.ToUpper();
                    objCompany.CompanyAdminAddress = companyRequestVM.CompanyAdminAddress.ToUpper();
                    objCompany.CompanyAdminPincode = companyRequestVM.CompanyAdminPincode;
                    objCompany.CompanyAdminCity = companyRequestVM.CompanyAdminCity.ToUpper();
                    objCompany.CompanyAdminState = companyRequestVM.CompanyAdminState.ToUpper();
                    objCompany.CompanyAdminDistrict = companyRequestVM.CompanyAdminDistrict.ToUpper();
                    objCompany.CompanyAdminProfilePhoto = profileFileName;
                    objCompany.CompanyAdminAadharCardNo = companyRequestVM.CompanyAdminAadharCardNo;
                    objCompany.CompanyAdminAadharCardPhoto = companyAdminAdharCardFileName;
                    objCompany.CompanyAdminPanCardPhoto = companyAdminPancardFileName;
                    objCompany.CompanyAdminPanCardNo = companyRequestVM.CompanyAdminPanCardNo.ToUpper();
                    objCompany.RequestStatus = (int)CompanyRequestStatus.Pending;
                    objCompany.FreeAccessDays = freeAccessDays;
                    objCompany.IsDeleted = false;
                    objCompany.CreatedBy = -1;
                    objCompany.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    objCompany.ModifiedBy = -1;
                    objCompany.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.tbl_CompanyRequest.Add(objCompany);

                    _db.SaveChanges();


                    #endregion

                    #region Send SMS

                    int SmsId = (int)SMSType.CompanyRequest;
                    string msg = CommonMethod.GetSmsContent(SmsId);
                    Regex regReplace = new Regex("{#var#}");
                    msg = regReplace.Replace(msg, companyRequestVM.CompanyAdminFirstName + " " + companyRequestVM.CompanyAdminLastName, 1);
                    msg = msg.Replace("\r\n", "\n");

                    var json = CommonMethod.SendSMSWithoutLog(msg, companyRequestVM.CompanyAdminMobileNo);
                    if (json.Contains("invalidnumber"))
                    {
                        //status = 0;
                        //errorMessage = ErrorMessage.InvalidMobileNo;
                    }
                    else
                    {
                        //status = 1;
                        //otp = num.ToString();
                    }

                    #endregion

                    return RedirectToAction("ThankYou");
                }
                else
                {
                    companyRequestVM.CompanyTypeList = GetCompanyType();
                    return View(companyRequestVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

        }

        private List<SelectListItem> GetCompanyType()
        {

            List<SelectListItem> lst = (from ms in _db.mst_CompanyType
                                        select new SelectListItem
                                        {
                                            Text = ms.CompanyTypeName,
                                            Value = ms.CompanyTypeId.ToString()
                                        }).ToList();
            return lst;
        }

        public ActionResult ThankYou()
        {
            return View();
        }

        public JsonResult CheckCompanyName(string companyName)
        {
            bool isExist = false;
            try
            {

                isExist = _db.tbl_CompanyRequest.Any(x => x.CompanyName == companyName);
            }
            catch (Exception ex)
            {
                isExist = false;
            }

            return Json(new { Status = isExist }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerifyMobileNo(string mobileNo, string fullname)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string otp = string.Empty;
            try
            {
                #region Send SMS

                Random random = new Random();
                int num = random.Next(555555, 999999);

                int SmsId = (int)SMSType.EmployeeProfileEditOTP;
                string msg = CommonMethod.GetSmsContent(SmsId);

                Regex regReplace = new Regex("{#var#}");
                msg = regReplace.Replace(msg, fullname, 1);
                msg = regReplace.Replace(msg, num.ToString(), 1);

                msg = msg.Replace("\r\n", "\n");

                var json = CommonMethod.SendSMSWithoutLog(msg, mobileNo);

                if (json.Contains("invalidnumber"))
                {
                    status = 0;
                    errorMessage = ErrorMessage.InvalidMobileNo;
                }
                else
                {
                    status = 1;
                    otp = num.ToString();
                }

                #endregion
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, Otp = otp, ErrorMessage = errorMessage, SetOtp = setOtp }, JsonRequestBehavior.AllowGet);
        }
    }
}