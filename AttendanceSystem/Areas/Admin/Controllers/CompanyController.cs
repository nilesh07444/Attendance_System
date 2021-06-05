using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class CompanyController : Controller
    {
        AttendanceSystemEntities _db;
        string psSult;
        public string companyGSTDirectoryPath = "";
        public string companyPanDirectoryPath = "";
        public string companyLogoDirectoryPath = "";
        public string companyRegisterProofDirectoryPath = "";
        public string aadharCardDirectoryPath = "";
        public string panCardDirectoryPath = "";
        public string CompanyAdminProfileDirectoryPath = "";
        long loggedInUserId ;
        string enviornment;
        string defaultPassword;
        public CompanyController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            companyGSTDirectoryPath = ErrorMessage.CompanyGSTDirectoryPath;
            companyPanDirectoryPath = ErrorMessage.CompanyPanCardDirectoryPath;
            companyLogoDirectoryPath = ErrorMessage.CompanyLogoDirectoryPath;
            companyRegisterProofDirectoryPath = ErrorMessage.CompanyRegisterProofDirectoryPath;
            aadharCardDirectoryPath = ErrorMessage.AdharcardDirectoryPath;
            panCardDirectoryPath = ErrorMessage.PancardDirectoryPath;
            CompanyAdminProfileDirectoryPath = ErrorMessage.ProfileDirectoryPath;
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
            loggedInUserId = clsAdminSession.UserID;
            defaultPassword = ConfigurationManager.AppSettings["DefaultPassword"].ToString();
        }
        // GET: Admin/Company
        public ActionResult Registered()
        {
            List<RegisteredCompanyVM> registeredCompanyVM = new List<RegisteredCompanyVM>();
            try
            {
                registeredCompanyVM = (from cp in _db.tbl_Company
                                       join ct in _db.mst_CompanyType on cp.CompanyTypeId equals ct.CompanyTypeId
                                       where cp.IsActive
                                       select new RegisteredCompanyVM
                                       {
                                           CompanyId = cp.CompanyId,
                                           CompanyTypeText = ct.CompanyTypeName,
                                           CompanyName = cp.CompanyName,
                                           City = cp.City,
                                           State = cp.State,
                                           GSTNo = cp.GSTNo,
                                           CompanyLogoImage = cp.CompanyLogoImage,
                                           CompanyCode = cp.CompanyCode
                                       }).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(registeredCompanyVM);
        }

        public ActionResult Requests(int? status)
        {
            CompanyRequestFilterVM companyRequestFilterVM = new CompanyRequestFilterVM();
            try
            {
                int[] companyStatusArr = new int[] { (int)CompanyRequestStatus.Pending, (int)CompanyRequestStatus.Reject, (int)CompanyRequestStatus.Accept };
                if (status.HasValue)
                {
                    companyStatusArr = new int[] { status.Value };
                    companyRequestFilterVM.RequestStatus = status.Value;
                }

                companyRequestFilterVM.companyRequest = (from cp in _db.tbl_CompanyRequest
                                                         join ct in _db.mst_CompanyType on cp.CompanyTypeId equals ct.CompanyTypeId
                                                         where companyStatusArr.Contains(cp.RequestStatus)
                                                         select new CompanyRequestVM
                                                         {
                                                             CompanyRequestId = cp.CompanyRequestId,
                                                             CompanyTypeText = ct.CompanyTypeName,
                                                             CompanyName = cp.CompanyName,
                                                             CompanyEmailId = cp.CompanyEmailId,
                                                             CompanyAdminFirstName = cp.CompanyAdminFirstName,
                                                             CompanyAdminLastName = cp.CompanyAdminLastName,
                                                             CompanyAdminMobileNo = cp.CompanyAdminMobileNo,
                                                             CompanyAdminCity = cp.CompanyAdminCity,
                                                             CompanyAdminState = cp.CompanyAdminState,
                                                             RequestStatus = cp.RequestStatus
                                                         }).ToList();

                if (companyRequestFilterVM.companyRequest != null && companyRequestFilterVM.companyRequest.Count > 0)
                {
                    companyRequestFilterVM.companyRequest.ForEach(req => {

                        if (req.RequestStatus == (int)CompanyRequestStatus.Pending)
                        {
                            req.RequestStatusText = "Pending";
                        }
                        else if (req.RequestStatus == (int)CompanyRequestStatus.Accept)
                        {
                            req.RequestStatusText = "Accept";
                        }
                        else if (req.RequestStatus == (int)CompanyRequestStatus.Reject)
                        {
                            req.RequestStatusText = "Reject";
                        }
                    });
                }

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRequestFilterVM);
        }

        public ActionResult Edit(long id)
        {
            CompanyRequestVM companyRequestVM = new CompanyRequestVM();
            try
            {
                companyRequestVM = (from cp in _db.tbl_CompanyRequest
                                    join ct in _db.mst_CompanyType on cp.CompanyTypeId equals ct.CompanyTypeId
                                    where cp.CompanyRequestId == id
                                    select new CompanyRequestVM
                                    {
                                        CompanyRequestId = cp.CompanyRequestId,
                                        CompanyTypeId = cp.CompanyTypeId,
                                        CompanyName = cp.CompanyName,
                                        CompanyEmailId = cp.CompanyEmailId,
                                        CompanyContactNo = cp.CompanyContactNo,
                                        CompanyAlternateContactNo = cp.CompanyAlternateContactNo,
                                        CompanyGSTNo = cp.CompanyGSTNo,
                                        CompanyGSTPhoto = cp.CompanyGSTPhoto,
                                        CompanyPanNo = cp.CompanyPanNo,
                                        CompanyPanPhoto = cp.CompanyPanPhoto,
                                        CompanyAddress = cp.CompanyAddress,
                                        CompanyPincode = cp.CompanyPincode,
                                        CompanyCity = cp.CompanyCity,
                                        CompanyState = cp.CompanyState,
                                        CompanyLogoImage = cp.CompanyLogoImage,
                                        CompanyRegisterProofImage = cp.CompanyRegisterProofImage,
                                        CompanyDescription = cp.CompanyDescription,
                                        CompanyWebisteUrl = cp.CompanyWebisteUrl,
                                        CompanyAdminPrefix = cp.CompanyAdminPrefix,
                                        CompanyAdminFirstName = cp.CompanyAdminFirstName,
                                        CompanyAdminMiddleName = cp.CompanyAdminMiddleName,
                                        CompanyAdminLastName = cp.CompanyAdminLastName,
                                        dtCompanyAdminDOB = cp.CompanyAdminDOB,
                                        CompanyAdminEmailId = cp.CompanyAdminEmailId,
                                        CompanyAdminMobileNo = cp.CompanyAdminMobileNo,
                                        CompanyAdminAlternateMobileNo = cp.CompanyAdminAlternateMobileNo,
                                        CompanyAdminDesignation = cp.CompanyAdminDesignation,
                                        CompanyAdminAddress = cp.CompanyAdminAddress,
                                        CompanyAdminPincode = cp.CompanyAdminPincode,
                                        CompanyAdminCity = cp.CompanyAdminCity,
                                        CompanyAdminState = cp.CompanyAdminState,
                                        CompanyAdminProfilePhoto = cp.CompanyAdminProfilePhoto,
                                        CompanyAdminAadharCardNo = cp.CompanyAdminAadharCardNo,
                                        CompanyAdminAadharCardPhoto = cp.CompanyAdminAadharCardPhoto,
                                        CompanyAdminPanCardPhoto = cp.CompanyAdminPanCardPhoto,
                                        CompanyAdminPanCardNo = cp.CompanyAdminPanCardNo,
                                        RequestStatus = cp.RequestStatus,
                                        RejectReason = cp.RejectReason,
                                        FreeAccessDays = cp.FreeAccessDays,
                                        CompanyTypeText = ct.CompanyTypeName
                                    }).FirstOrDefault();

                companyRequestVM.CompanyAdminDOB = companyRequestVM.dtCompanyAdminDOB.ToString("dd MMM yyyy");
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRequestVM);
        }
        [HttpPost]
        public ActionResult Edit(CompanyRequestVM companyRequestVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    tbl_CompanyRequest objCompanyReq = _db.tbl_CompanyRequest.FirstOrDefault(x => x.CompanyRequestId == companyRequestVM.CompanyRequestId);
                    objCompanyReq.RequestStatus = companyRequestVM.RequestStatus;
                    objCompanyReq.RejectReason = companyRequestVM.RejectReason;
                    objCompanyReq.ModifiedBy = loggedInUserId;
                    objCompanyReq.ModifiedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    if (companyRequestVM.RequestStatus == (int)CompanyRequestStatus.Accept)
                    {

                        tbl_Company objcomp = new tbl_Company();


                        objcomp.CompanyTypeId = objCompanyReq.CompanyTypeId;
                        objcomp.CompanyName = objCompanyReq.CompanyName;
                        objcomp.CompanyCode = objCompanyReq.CompanyName;
                        objcomp.EmailId = objCompanyReq.CompanyEmailId;
                        objcomp.ContactNo = objCompanyReq.CompanyContactNo;
                        objcomp.AlternateContactNo = objCompanyReq.CompanyAlternateContactNo;
                        objcomp.Address = objCompanyReq.CompanyAddress;
                        objcomp.Pincode = objCompanyReq.CompanyPincode;
                        objcomp.City = objCompanyReq.CompanyCity;
                        objcomp.State = objCompanyReq.CompanyState;
                        objcomp.GSTNo = objCompanyReq.CompanyGSTNo;
                        objcomp.GSTPhoto = objCompanyReq.CompanyGSTPhoto;
                        objcomp.PanNo = objCompanyReq.CompanyPanNo;
                        objcomp.PanPhoto = objCompanyReq.CompanyPanPhoto;
                        objcomp.CompanyLogoImage = objCompanyReq.CompanyLogoImage;
                        objcomp.RegisterProofImage = objCompanyReq.CompanyRegisterProofImage;
                        objcomp.Description = objCompanyReq.CompanyDescription;
                        objcomp.WebisteUrl = objCompanyReq.CompanyWebisteUrl;
                        objcomp.FreeAccessDays = objCompanyReq.FreeAccessDays;
                        objcomp.IsTrialMode = true;
                        objcomp.TrialExpiryDate = DateTime.Now.AddDays(objCompanyReq.FreeAccessDays);
                        objcomp.IsActive = true;
                        objcomp.CreatedBy = loggedInUserId;
                        objcomp.CreatedDate = DateTime.UtcNow;
                        objcomp.ModifiedBy = loggedInUserId;
                        objcomp.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_Company.Add(objcomp);
                        _db.SaveChanges();

                        long companyId = objcomp.CompanyId;

                        //Generate company code for company and user table 
                        string companyCode = getCompanyCodeFormat(companyId, objCompanyReq.CompanyName);
                        objcomp.CompanyCode = companyCode;

                        //Update company id in company request table.
                        objCompanyReq.CompanyId = companyId;
                        _db.SaveChanges();


                        tbl_AdminUser objAdminUser = new tbl_AdminUser();
                        objAdminUser.AdminUserRoleId = (int)AdminRoles.CompanyAdmin;
                        objAdminUser.CompanyId = objCompanyReq.CompanyId;
                        objAdminUser.Prefix = objCompanyReq.CompanyAdminPrefix;
                        objAdminUser.FirstName = objCompanyReq.CompanyAdminFirstName;
                        objAdminUser.MIddleName = objCompanyReq.CompanyAdminMiddleName;
                        objAdminUser.LastName = objCompanyReq.CompanyAdminLastName;
                        objAdminUser.UserName = companyCode;
                        objAdminUser.Password = CommonMethod.Encrypt(defaultPassword, psSult);
                        objAdminUser.DOB = objCompanyReq.CompanyAdminDOB;
                        objAdminUser.EmailId = objCompanyReq.CompanyAdminEmailId;
                        objAdminUser.MobileNo = objCompanyReq.CompanyAdminMobileNo;
                        objAdminUser.AlternateMobileNo = objCompanyReq.CompanyAdminAlternateMobileNo;
                        objAdminUser.Address = objCompanyReq.CompanyAdminAddress;
                        objAdminUser.Pincode = objCompanyReq.CompanyAdminPincode;
                        objAdminUser.Designation = string.IsNullOrEmpty(objCompanyReq.CompanyAdminDesignation) ? CommonMethod.GetEnumDescription(AdminRoles.CompanyAdmin) : objCompanyReq.CompanyAdminDesignation;
                        objAdminUser.City = objCompanyReq.CompanyAdminCity;
                        objAdminUser.State = objCompanyReq.CompanyAdminState;
                        objAdminUser.ProfilePhoto= objCompanyReq.CompanyAdminProfilePhoto;
                        objAdminUser.AadharCardNo = objCompanyReq.CompanyAdminAadharCardNo;
                        objAdminUser.PanCardNo = objCompanyReq.CompanyAdminPanCardNo;
                        objAdminUser.AadharCardPhoto = objCompanyReq.CompanyAdminAadharCardPhoto;
                        objAdminUser.PanCardPhoto = objCompanyReq.CompanyAdminPanCardPhoto;
                        objAdminUser.IsActive = true;
                        objAdminUser.CreatedBy = loggedInUserId;
                        objAdminUser.CreatedDate = DateTime.UtcNow;
                        objAdminUser.ModifiedBy = loggedInUserId;
                        objAdminUser.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_AdminUser.Add(objAdminUser);
                        _db.SaveChanges();
                    }
                }
                else
                {
                    return View(companyRequestVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return RedirectToAction("Requests");
        }
        public ActionResult Renew()
        {
            List<CompanyRenewPaymentVM> companyRenewPaymentVM = new List<CompanyRenewPaymentVM>();
            try
            {
                companyRenewPaymentVM = (from cp in _db.tbl_CompanyRenewPayment
                                         join cm in _db.tbl_Company on cp.CompanyId equals cm.CompanyId
                                         select new CompanyRenewPaymentVM
                                         {
                                             CompanyRegistrationPaymentId = cp.CompanyRegistrationPaymentId,
                                             CompanyId = cp.CompanyId,
                                             CompanyName = cm.CompanyName,
                                             Amount = cp.Amount,
                                             PaymentFor = cp.PaymentFor,
                                             PaymentGatewayResponseId = cp.PaymentGatewayResponseId,
                                             StartDate = cp.StartDate,
                                             EndDate = cp.EndDate,
                                             AccessDays = cp.AccessDays,
                                             PackageId = cp.PackageId,
                                             PackageName = cp.PackageName
                                         }).OrderByDescending(x => x.CompanyRegistrationPaymentId).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRenewPaymentVM);
        }

        public ActionResult EditCompany(long id)
        {
            CompanyRequestVM registeredCompanyVM = new CompanyRequestVM();
            try
            {
                registeredCompanyVM = (from cp in _db.tbl_Company
                                       join ct in _db.mst_CompanyType on cp.CompanyTypeId equals ct.CompanyTypeId
                                       join emp in _db.tbl_AdminUser on cp.CompanyCode equals emp.UserName
                                       where cp.CompanyId == id
                                       select new CompanyRequestVM
                                       {
                                           CompanyId = cp.CompanyId,
                                           CompanyTypeId = cp.CompanyTypeId,
                                           CompanyName = cp.CompanyName,
                                           CompanyEmailId = cp.EmailId,
                                           CompanyContactNo = cp.ContactNo,
                                           CompanyAlternateContactNo = cp.AlternateContactNo,
                                           CompanyGSTNo = cp.GSTNo,
                                           CompanyGSTPhoto = cp.GSTPhoto,
                                           CompanyPanNo = cp.PanNo,
                                           CompanyPanPhoto = cp.PanPhoto,
                                           CompanyAddress = cp.Address,
                                           CompanyPincode = cp.Pincode,
                                           CompanyCity = cp.City,
                                           CompanyState = cp.State,
                                           CompanyLogoImage = cp.CompanyLogoImage,
                                           CompanyRegisterProofImage = cp.RegisterProofImage,
                                           CompanyDescription = cp.Description,
                                           CompanyWebisteUrl = cp.WebisteUrl,
                                           CompanyAdminPrefix = emp.Prefix,
                                           CompanyAdminFirstName = emp.FirstName,
                                           CompanyAdminMiddleName = emp.MIddleName,
                                           CompanyAdminLastName = emp.LastName,
                                           CompanyAdminEmailId = emp.EmailId,
                                           dtCompanyAdminDOB = emp.DOB,
                                           CompanyAdminMobileNo = emp.MobileNo,
                                           CompanyAdminAlternateMobileNo = emp.AlternateMobileNo,
                                           CompanyAdminDesignation = emp.Designation,
                                           CompanyAdminAddress = emp.Address,
                                           CompanyAdminPincode = emp.Pincode,
                                           CompanyAdminCity = emp.City,
                                           CompanyAdminState = emp.State,
                                           CompanyAdminProfilePhoto = emp.ProfilePhoto,
                                           CompanyAdminAadharCardNo = emp.AadharCardNo,
                                           CompanyAdminAadharCardPhoto = emp.AadharCardPhoto,
                                           CompanyAdminPanCardPhoto = emp.PanCardPhoto,
                                           CompanyAdminPanCardNo = emp.PanCardNo,
                                           FreeAccessDays = cp.FreeAccessDays,
                                           CompanyTypeText = ct.CompanyTypeName
                                       }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(registeredCompanyVM);
        }
        [HttpPost]
        public ActionResult EditCompany(CompanyRequestVM companyRequestVM,
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
                        if (string.IsNullOrEmpty(companyRequestVM.CompanyLogoImage))
                        {
                            ModelState.AddModelError("CompanyPhotoFile", ErrorMessage.ImageRequired);
                            return View(companyRequestVM);
                        }
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
                        if (string.IsNullOrEmpty(companyRequestVM.CompanyRegisterProofImage))
                        {
                            ModelState.AddModelError("CompanyRegisterProofImageFile", ErrorMessage.ImageRequired);
                            return View(companyRequestVM);
                        }
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
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("CompanyAdminProfilePhotoFile", ErrorMessage.SelectOnlyImage);
                            return View(companyRequestVM);
                        }

                        // Save file in folder
                        profileFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyAdminProfilePhotoFile.FileName);
                        CompanyAdminProfilePhotoFile.SaveAs(companyAdminProfileDirectoryPath + profileFileName);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(companyRequestVM.CompanyAdminProfilePhoto))
                        {
                            ModelState.AddModelError("CompanyAdminProfilePhotoFile", ErrorMessage.ImageRequired);
                            return View(companyRequestVM);
                        }
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
                        if (string.IsNullOrEmpty(companyRequestVM.CompanyAdminAadharCardPhoto))
                        {
                            ModelState.AddModelError("CompanyAdminAadharCardPhotoFile", ErrorMessage.ImageRequired);
                            return View(companyRequestVM);
                        }
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
                        if (string.IsNullOrEmpty(companyRequestVM.CompanyAdminPanCardPhoto))
                        {
                            ModelState.AddModelError("CompanyAdminPanCardPhotoFile", ErrorMessage.ImageRequired);
                            return View(companyRequestVM);
                        }
                    }
                    #endregion PancardImage



                    #region Create Company Request

                    tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyRequestVM.CompanyId).FirstOrDefault();
                    bool isCompanyNameChanged = objCompany.CompanyName != companyRequestVM.CompanyName;
                    objCompany.CompanyTypeId = Convert.ToInt64(companyRequestVM.CompanyTypeId);
                    objCompany.CompanyName = companyRequestVM.CompanyName;
                    objCompany.EmailId = companyRequestVM.CompanyEmailId;
                    objCompany.ContactNo = companyRequestVM.CompanyContactNo;
                    objCompany.AlternateContactNo = companyRequestVM.CompanyAlternateContactNo;
                    objCompany.GSTNo = companyRequestVM.CompanyGSTNo;
                    objCompany.GSTPhoto = !string.IsNullOrEmpty(companyGstFileName) ? companyGstFileName : objCompany.GSTPhoto;
                    objCompany.PanNo = companyRequestVM.CompanyPanNo;
                    objCompany.PanPhoto = !string.IsNullOrEmpty(companyPanCardFileName) ? companyPanCardFileName : objCompany.PanPhoto; ;
                    objCompany.Address = companyRequestVM.CompanyAddress;
                    objCompany.Pincode = companyRequestVM.CompanyPincode;
                    objCompany.City = companyRequestVM.CompanyCity;
                    objCompany.State = companyRequestVM.CompanyState;
                    objCompany.CompanyLogoImage = !string.IsNullOrEmpty(companyLogoFileName) ? companyLogoFileName : objCompany.CompanyLogoImage;
                    objCompany.RegisterProofImage = !string.IsNullOrEmpty(companyRegisterProofFileName) ? companyRegisterProofFileName : objCompany.RegisterProofImage;
                    objCompany.Description = companyRequestVM.CompanyDescription;
                    objCompany.WebisteUrl = companyRequestVM.CompanyWebisteUrl;
                    objCompany.ModifiedBy = loggedInUserId;
                    objCompany.ModifiedDate = DateTime.UtcNow;

                    string companyCode = string.Empty;
                    if (isCompanyNameChanged)
                    {
                        companyCode = getCompanyCodeFormat(objCompany.CompanyId, companyRequestVM.CompanyName);
                        objCompany.CompanyCode = companyCode;
                    }
                    _db.SaveChanges();

                    tbl_AdminUser objUser = _db.tbl_AdminUser.Where(x => x.UserName == objCompany.CompanyCode).FirstOrDefault();
                    objUser.Prefix = companyRequestVM.CompanyAdminPrefix;
                    objUser.FirstName = companyRequestVM.CompanyAdminFirstName;
                    objUser.MIddleName = companyRequestVM.CompanyAdminMiddleName;
                    objUser.LastName = companyRequestVM.CompanyAdminLastName;
                    objUser.EmailId = companyRequestVM.CompanyAdminEmailId;

                    DateTime dob_date = DateTime.ParseExact(companyRequestVM.CompanyAdminDOB, "dd/MM/yyyy", null);

                    objUser.DOB = dob_date; // companyRequestVM.CompanyAdminDOB;
                    objUser.MobileNo = companyRequestVM.CompanyAdminMobileNo;
                    objUser.AlternateMobileNo = companyRequestVM.CompanyAdminAlternateMobileNo;
                    objUser.Designation = companyRequestVM.CompanyAdminDesignation;
                    objUser.Address = companyRequestVM.CompanyAdminAddress;
                    objUser.Pincode = companyRequestVM.CompanyAdminPincode;
                    objUser.City = companyRequestVM.CompanyAdminCity;
                    objUser.State = companyRequestVM.CompanyAdminState;
                    objUser.ProfilePhoto= profileFileName;
                    objUser.AadharCardNo = companyRequestVM.CompanyAdminAadharCardNo;
                    objUser.AadharCardPhoto = companyAdminAdharCardFileName;
                    objUser.PanCardPhoto = companyAdminPancardFileName;
                    objUser.PanCardNo = companyRequestVM.CompanyAdminPanCardNo;
                    objUser.ModifiedBy = loggedInUserId;
                    objUser.ModifiedDate = DateTime.UtcNow;
                    if (isCompanyNameChanged)
                    {
                        objUser.UserName = companyCode;
                    }
                    _db.SaveChanges();

                    #endregion

                    return RedirectToAction("Registered");
                }
                else
                {
                    return View(companyRequestVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

        }

        public JsonResult VerifyCopmany(int companyId)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string otp = string.Empty;
            try
            {
                string mobileNo = (from cmp in _db.tbl_Company
                                   join emp in _db.tbl_AdminUser on cmp.CompanyCode equals emp.UserName
                                   select emp.MobileNo).FirstOrDefault();
                if (!string.IsNullOrEmpty(mobileNo))
                {
                    Random random = new Random();
                    int num = random.Next(555555, 999999);
                    if (enviornment != "Development")
                    {
                        string msg = "Your Otp code for Login is " + num;
                        var json = CommonMethod.SuperAdminSendSMS(msg, mobileNo, loggedInUserId);
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
                    }
                    else
                    {
                        status = 1;
                        otp = num.ToString();
                    }
                }
                else
                {
                    status = 0;
                    errorMessage = ErrorMessage.MobileNoNotFoundForTheCompany;
                }
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, Otp = otp, ErrorMessage = errorMessage, SetOtp = clsAdminSession.SetOtp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View(long id)
        {
            CompanyRequestVM companyRequestVM = new CompanyRequestVM();
            try
            {
                companyRequestVM = (from cp in _db.tbl_Company
                                    join ct in _db.mst_CompanyType on cp.CompanyTypeId equals ct.CompanyTypeId
                                    join emp in _db.tbl_AdminUser on cp.CompanyCode equals emp.UserName
                                    where cp.CompanyId == id
                                    select new CompanyRequestVM
                                    {
                                        CompanyId = cp.CompanyId,
                                        CompanyTypeId = cp.CompanyTypeId,
                                        CompanyName = cp.CompanyName,
                                        CompanyEmailId = cp.EmailId,
                                        CompanyContactNo = cp.ContactNo,
                                        CompanyAlternateContactNo = cp.AlternateContactNo,
                                        CompanyGSTNo = cp.GSTNo,
                                        CompanyGSTPhoto = cp.GSTPhoto,
                                        CompanyPanNo = cp.PanNo,
                                        CompanyPanPhoto = cp.PanPhoto,
                                        CompanyAddress = cp.Address,
                                        CompanyPincode = cp.Pincode,
                                        CompanyCity = cp.City,
                                        CompanyState = cp.State,
                                        CompanyLogoImage = cp.CompanyLogoImage,
                                        CompanyRegisterProofImage = cp.RegisterProofImage,
                                        CompanyDescription = cp.Description,
                                        CompanyWebisteUrl = cp.WebisteUrl,
                                        CompanyAdminPrefix = emp.Prefix,
                                        CompanyAdminFirstName = emp.FirstName,
                                        CompanyAdminMiddleName = emp.MIddleName,
                                        CompanyAdminLastName = emp.LastName,
                                        dtCompanyAdminDOB = emp.DOB,
                                        CompanyAdminEmailId = emp.EmailId,
                                        CompanyAdminMobileNo = emp.MobileNo,
                                        CompanyAdminAlternateMobileNo = emp.AlternateMobileNo,
                                        CompanyAdminDesignation = emp.Designation,
                                        CompanyAdminAddress = emp.Address,
                                        CompanyAdminPincode = emp.Pincode,
                                        CompanyAdminCity = emp.City,
                                        CompanyAdminState = emp.State,
                                        CompanyAdminProfilePhoto = emp.ProfilePhoto,
                                        CompanyAdminAadharCardNo = emp.AadharCardNo,
                                        CompanyAdminAadharCardPhoto = emp.AadharCardPhoto,
                                        CompanyAdminPanCardPhoto = emp.PanCardPhoto,
                                        CompanyAdminPanCardNo = emp.PanCardNo,
                                        FreeAccessDays = cp.FreeAccessDays,
                                        CompanyTypeText = ct.CompanyTypeName
                                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRequestVM);
        }

        private string getCompanyCodeFormat(long companyId, string companyName)
        {
            string companyCode = string.Empty;
            try
            {
                string companyNameWithoutSpeChar = Regex.Replace(companyName, @"[^0-9a-zA-Z]+", "");
                string first2CharOfCompanyName = companyNameWithoutSpeChar.ToUpper().Substring(0, 2);
                companyCode = first2CharOfCompanyName + "/" + DateTime.Now.ToString("ddMMyyyy") + "/" + companyId;
            }
            catch (Exception ex)
            {
            }
            return companyCode;
        }



    }
}