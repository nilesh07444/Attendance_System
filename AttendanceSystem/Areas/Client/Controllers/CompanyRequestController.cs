using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
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
        public string CancellationChequeDirectoryPath = "";

        public CompanyRequestController()
        {
            _db = new AttendanceSystemEntities();
            companyGSTDirectoryPath = ErrorMessage.CompanyGSTDirectoryPath;
            companyPanDirectoryPath = ErrorMessage.CompanyPanCardDirectoryPath;
            companyLogoDirectoryPath = ErrorMessage.CompanyLogoDirectoryPath;
            companyRegisterProofDirectoryPath = ErrorMessage.CompanyRegisterProofDirectoryPath;
            aadharCardDirectoryPath = ErrorMessage.AdharcardDirectoryPath;
            panCardDirectoryPath = ErrorMessage.PancardDirectoryPath;
            CancellationChequeDirectoryPath = ErrorMessage.CancellationChequeDirectoryPath;
        }
        // GET: Client/CompanyRequest
        public ActionResult Index()
        {
            CompanyRequestVM companyRequestVM = new CompanyRequestVM();
            companyRequestVM.CompanyTypeList = GetCompanyType();
            return View(companyRequestVM);
        }

        [HttpPost]
        public ActionResult Index(CompanyRequestVM companyRequestVM,
            HttpPostedFileBase CompanyGSTPhotoFile,
            HttpPostedFileBase CompanyPanPhotoFile,
            HttpPostedFileBase CompanyLogoImageFile,
            HttpPostedFileBase CompanyRegisterProofImageFile,
            HttpPostedFileBase CompanyCancellationChequePhotoFile,
            HttpPostedFileBase CompanyAdminAadharCardPhotoFile,
            HttpPostedFileBase CompanyAdminPanCardPhotoFile)
        {
            try
            {

                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    //long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    string companyGstFileName = string.Empty, companyPanCardFileName = string.Empty, companyLogoFileName = string.Empty, companyRegisterProofFileName = string.Empty,
                        chqFileName = string.Empty, companyAdminAdharCardFileName = string.Empty, companyAdminPancardFileName = string.Empty;
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
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyGSTPhotoFile", ErrorMessage.SelectOnlyImage);
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
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyPanPhotoFile", ErrorMessage.SelectOnlyImage);
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
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyPhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyLogoFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyLogoImageFile.FileName);
                        CompanyLogoImageFile.SaveAs(companyLogoPath + companyLogoFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyPhotoFile", ErrorMessage.ImageRequired);
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
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyRegisterProofImageFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyRegisterProofFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyRegisterProofImageFile.FileName);
                        CompanyRegisterProofImageFile.SaveAs(companyRegisterProofPath + companyRegisterProofFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyRegisterProofImageFile", ErrorMessage.ImageRequired);
                        return View(companyRequestVM);
                    }
                    #endregion CompanyRegisterProofImage

                    #region Cancel Cheque Image

                    if (CompanyCancellationChequePhotoFile != null)
                    {
                        string cancellationChqPath = Server.MapPath(CancellationChequeDirectoryPath);

                        folderExists = Directory.Exists(cancellationChqPath);
                        if (!folderExists)
                            Directory.CreateDirectory(cancellationChqPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyCancellationChequePhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyCancellationChequePhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        chqFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyCancellationChequePhotoFile.FileName);
                        CompanyCancellationChequePhotoFile.SaveAs(cancellationChqPath + chqFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyCancellationChequePhotoFile", ErrorMessage.ImageRequired);
                        return View(companyRequestVM);
                    }

                    #endregion ChqImage

                    #region AdharCardImage
                    if (CompanyAdminAadharCardPhotoFile != null)
                    {
                        string adharCardPath = Server.MapPath(aadharCardDirectoryPath);

                        folderExists = Directory.Exists(adharCardPath);
                        if (!folderExists)
                            Directory.CreateDirectory(adharCardPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyAdminAadharCardPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyAdminAadharCardPhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyAdminAdharCardFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyAdminAadharCardPhotoFile.FileName);
                        CompanyAdminAadharCardPhotoFile.SaveAs(adharCardPath + companyAdminAdharCardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyAdminAadharCardPhotoFile", ErrorMessage.ImageRequired);
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
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyAdminPanCardPhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyAdminPancardFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyAdminPanCardPhotoFile.FileName);
                        CompanyAdminPanCardPhotoFile.SaveAs(panCardPath + companyAdminPancardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyAdminPanCardPhotoFile", ErrorMessage.ImageRequired);
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

                    tbl_CompanyRequest objCompany = new tbl_CompanyRequest();
                    objCompany.CompanyTypeId = Convert.ToInt64(companyRequestVM.CompanyTypeId);
                    objCompany.CompanyName = companyRequestVM.CompanyName;
                    objCompany.CompanyEmailId = companyRequestVM.CompanyEmailId;
                    objCompany.CompanyContactNo = companyRequestVM.CompanyContactNo;
                    objCompany.CompanyAlternateContactNo = companyRequestVM.CompanyAlternateContactNo;
                    objCompany.CompanyGSTNo = companyRequestVM.CompanyGSTNo;
                    objCompany.CompanyGSTPhoto = companyGstFileName;
                    objCompany.CompanyPanNo = companyRequestVM.CompanyPanNo;
                    objCompany.CompanyPanPhoto = companyPanCardFileName;
                    objCompany.CompanyAddress = companyRequestVM.CompanyAddress;
                    objCompany.CompanyPincode = companyRequestVM.CompanyPincode;
                    objCompany.CompanyCity = companyRequestVM.CompanyCity;
                    objCompany.CompanyState = companyRequestVM.CompanyState;
                    objCompany.CompanyLogoImage = companyLogoFileName;
                    objCompany.CompanyRegisterProofImage = companyRegisterProofFileName;
                    objCompany.CompanyDescription = companyRequestVM.CompanyDescription;
                    objCompany.CompanyWebisteUrl = companyRequestVM.CompanyWebisteUrl;
                    objCompany.CompanyCancellationChequePhoto = chqFileName;
                    objCompany.CompanyAdminPrefix = companyRequestVM.CompanyAdminPrefix;
                    objCompany.CompanyAdminFirstName = companyRequestVM.CompanyAdminFirstName;
                    objCompany.CompanyAdminMiddleName = companyRequestVM.CompanyAdminMiddleName;
                    objCompany.CompanyAdminLastName = companyRequestVM.CompanyAdminLastName;
                    objCompany.CompanyAdminEmailId = companyRequestVM.CompanyAdminEmailId;
                    objCompany.CompanyAdminMobileNo = companyRequestVM.CompanyAdminMobileNo;
                    objCompany.CompanyAdminAlternateMobileNo = companyRequestVM.CompanyAdminAlternateMobileNo;
                    objCompany.CompanyAdminDesignation = companyRequestVM.CompanyAdminDesignation;
                    objCompany.CompanyAdminAddress = companyRequestVM.CompanyAdminAddress;
                    objCompany.CompanyAdminPincode = companyRequestVM.CompanyAdminPincode;
                    objCompany.CompanyAdminCity = companyRequestVM.CompanyAdminCity;
                    objCompany.CompanyAdminState = companyRequestVM.CompanyAdminState;
                    objCompany.CompanyAdminAadharCardNo = companyRequestVM.CompanyAdminAadharCardNo;
                    objCompany.CompanyAdminAadharCardPhoto = companyAdminAdharCardFileName;
                    objCompany.CompanyAdminPanCardPhoto = companyAdminPancardFileName;
                    objCompany.CompanyAdminPanCardNo = companyRequestVM.CompanyAdminPanCardNo;
                    objCompany.RequestStatus = (int)CompanyRequestStatus.Pending;
                    objCompany.FreeAccessDays = freeAccessDays;
                    objCompany.IsDeleted = false;
                    objCompany.CreatedBy = -1; 
                    objCompany.CreatedDate = DateTime.UtcNow;
                    objCompany.ModifiedBy = -1; 
                    objCompany.ModifiedDate = DateTime.UtcNow;
                    _db.tbl_CompanyRequest.Add(objCompany);

                    _db.SaveChanges();

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

    }
}