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
        public string HomeDirectoryPath = "";
        public string PancardDirectoryPath = "";
        public string AdharcardDirectoryPath = "";
        public string GSTDirectoryPath = "";
        public string CompanyDirectoryPath = "";
        public string CancellationChequeDirectoryPath = "";

        public CompanyRequestController()
        {
            _db = new AttendanceSystemEntities();
            PancardDirectoryPath = ErrorMessage.PancardDirectoryPath;
            AdharcardDirectoryPath = ErrorMessage.AdharcardDirectoryPath;
            GSTDirectoryPath = ErrorMessage.GSTDirectoryPath;
            CompanyDirectoryPath = ErrorMessage.CompanyDirectoryPath;
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
        public ActionResult Index(CompanyRequestVM companyRequestVM, HttpPostedFileBase PanCardPhotoFile, HttpPostedFileBase AadharCardPhotoFile, HttpPostedFileBase GSTPhotoFile, HttpPostedFileBase CompanyPhotoFile, HttpPostedFileBase CancellationChequePhotoFile)
        {
            try
            {
                int freeAccessDays=Convert.ToInt32(ConfigurationManager.AppSettings["CompanyFreeAccessDays"].ToString());
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    string panCardFileName = string.Empty, adharCardFileName = string.Empty, gstFileName = string.Empty, companyFileName = string.Empty, chqFileName = string.Empty;
                    bool folderExists = false;

                    #region PancardImage
                    if (PanCardPhotoFile != null)
                    {
                        string panCardPath = Server.MapPath(PancardDirectoryPath);
                        folderExists = Directory.Exists(panCardPath);
                        if (!folderExists)
                            Directory.CreateDirectory(panCardPath);

                        // Image file validation
                        string ext = Path.GetExtension(PanCardPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("PanCardPhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        panCardFileName = Guid.NewGuid() + "-" + Path.GetFileName(PanCardPhotoFile.FileName);
                        PanCardPhotoFile.SaveAs(panCardPath + panCardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("PanCardPhotoFile", ErrorMessage.ImageRequired);
                        return View(companyRequestVM);
                    }
                    #endregion PancardImage

                    #region AdharCardImage
                    if (AadharCardPhotoFile != null)
                    {
                        string adharCardPath = Server.MapPath(AdharcardDirectoryPath);

                        folderExists = Directory.Exists(adharCardPath);
                        if (!folderExists)
                            Directory.CreateDirectory(adharCardPath);

                        // Image file validation
                        string ext = Path.GetExtension(AadharCardPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("AadharCardPhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        adharCardFileName = Guid.NewGuid() + "-" + Path.GetFileName(AadharCardPhotoFile.FileName);
                        AadharCardPhotoFile.SaveAs(adharCardPath + adharCardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("AadharCardPhotoFile", ErrorMessage.ImageRequired);
                        return View(companyRequestVM);
                    }
                    #endregion AdharCardImage

                    #region GSTImage
                    if (GSTPhotoFile != null)
                    {
                        string gstPath = Server.MapPath(GSTDirectoryPath);

                        folderExists = Directory.Exists(gstPath);
                        if (!folderExists)
                            Directory.CreateDirectory(gstPath);

                        // Image file validation
                        string ext = Path.GetExtension(GSTPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("GSTPhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        gstFileName = Guid.NewGuid() + "-" + Path.GetFileName(GSTPhotoFile.FileName);
                        GSTPhotoFile.SaveAs(gstPath + gstFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("GSTPhotoFile", ErrorMessage.ImageRequired);
                        return View(companyRequestVM);
                    }
                    #endregion GSTImage

                    #region CompanyImage
                    if (CompanyPhotoFile != null)
                    {
                        string companyPath = Server.MapPath(CompanyDirectoryPath);

                        folderExists = Directory.Exists(companyPath);
                        if (!folderExists)
                            Directory.CreateDirectory(companyPath);

                        // Image file validation
                        string ext = Path.GetExtension(CompanyPhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyPhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        companyFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyPhotoFile.FileName);
                        CompanyPhotoFile.SaveAs(companyPath + companyFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyPhotoFile", ErrorMessage.ImageRequired);
                        return View(companyRequestVM);
                    }
                    #endregion CompanyImage

                    #region ChqImage
                    if (CancellationChequePhotoFile != null)
                    {
                        string cancellationChqPath = Server.MapPath(CancellationChequeDirectoryPath);

                        folderExists = Directory.Exists(cancellationChqPath);
                        if (!folderExists)
                            Directory.CreateDirectory(cancellationChqPath);

                        // Image file validation
                        string ext = Path.GetExtension(CancellationChequePhotoFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CancellationChequePhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        chqFileName = Guid.NewGuid() + "-" + Path.GetFileName(CancellationChequePhotoFile.FileName);
                        CancellationChequePhotoFile.SaveAs(cancellationChqPath + chqFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CancellationChequePhotoFile", ErrorMessage.ImageRequired);
                        return View(companyRequestVM);
                    }
                    #endregion ChqImage


                    tbl_CompanyRequest objCompany = new tbl_CompanyRequest();


                    objCompany.CompanyTypeId = companyRequestVM.CompanyTypeId;
                    objCompany.CompanyName = companyRequestVM.CompanyName;
                    objCompany.Prefix = companyRequestVM.Prefix;
                    objCompany.Firstname = companyRequestVM.Firstname;
                    objCompany.Lastname = companyRequestVM.Lastname;
                    objCompany.DateOfBirth = companyRequestVM.DateOfBirth;
                    objCompany.EmailId = companyRequestVM.EmailId;
                    objCompany.MobileNo = companyRequestVM.MobileNo;
                    objCompany.AlternateMobileNo = companyRequestVM.AlternateMobileNo;
                    objCompany.City = companyRequestVM.City;
                    objCompany.State = companyRequestVM.State;
                    objCompany.AadharCardNo = companyRequestVM.AadharCardNo;
                    objCompany.GSTNo = companyRequestVM.GSTNo;
                    objCompany.PanCardNo = companyRequestVM.PanCardNo;
                    objCompany.PanCardPhoto = panCardFileName;
                    objCompany.AadharCardPhoto = adharCardFileName;
                    objCompany.GSTPhoto = gstFileName;
                    objCompany.CompanyPhoto = companyFileName;
                    objCompany.CancellationChequePhoto = chqFileName;
                    objCompany.RequestStatus = ErrorMessage.RunningStatusPending;
                    objCompany.FreeAccessDays = freeAccessDays;
                    objCompany.IsDeleted = false;
                    objCompany.CreatedBy = LoggedInUserId;
                    objCompany.CreatedDate = DateTime.UtcNow;
                    objCompany.ModifiedBy = LoggedInUserId;
                    objCompany.ModifiedDate = DateTime.UtcNow;
                    _db.tbl_CompanyRequest.Add(objCompany);

                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return RedirectToAction("ThankYou");
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
    }
}