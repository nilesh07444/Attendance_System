using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class CompanyController : Controller
    {
        AttendanceSystemEntities _db;
        string psSult;
        public string GSTDirectoryPath = "";
        public string CompanyDirectoryPath = "";
        public string CancellationChequeDirectoryPath = "";
        public string PancardDirectoryPath = "";
        public string AdharcardDirectoryPath = "";
        string enviornment;
        public CompanyController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
            GSTDirectoryPath = ErrorMessage.GSTDirectoryPath;
            CompanyDirectoryPath = ErrorMessage.CompanyDirectoryPath;
            CancellationChequeDirectoryPath = ErrorMessage.CancellationChequeDirectoryPath;
            PancardDirectoryPath = ErrorMessage.PancardDirectoryPath;
            AdharcardDirectoryPath = ErrorMessage.AdharcardDirectoryPath;
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
        }
        // GET: Admin/Company
        public ActionResult Registered()
        {
            List<CompanyRequestVM> companyRequestVM = new List<CompanyRequestVM>();
            try
            {
                companyRequestVM = (from cp in _db.tbl_Company
                                    join ct in _db.mst_CompanyType on cp.CompanyTypeId equals ct.CompanyTypeId
                                    where cp.IsActive
                                    select new CompanyRequestVM
                                    {
                                        CompanyId = cp.CompanyId,
                                        CompanyTypeText = ct.CompanyTypeName,
                                        CompanyName = cp.CompanyName,
                                        City = cp.City,
                                        State = cp.State,
                                        GSTNo = cp.GSTNo,
                                        CompanyPhoto = cp.CompanyPhoto
                                    }).ToList();
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(companyRequestVM);
        }

        public ActionResult Requests(int? status)
        {
            CompanyRequestFilterVM companyRequestFilterVM = new CompanyRequestFilterVM();
            //List<CompanyRequestVM> companyRequestVM = new List<CompanyRequestVM>();
            try
            {
                int[] companyStatusArr = new int[] { (int)CompanyRequestStatus.Pending, (int)CompanyRequestStatus.Reject };
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
                                                             Firstname = cp.Firstname,
                                                             Lastname = cp.Lastname,
                                                             EmailId = cp.EmailId,
                                                             MobileNo = cp.MobileNo,
                                                             City = cp.City,
                                                             State = cp.State
                                                         }).ToList();
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
                                        Prefix = cp.Prefix,
                                        Firstname = cp.Firstname,
                                        Lastname = cp.Lastname,
                                        DateOfBirth = cp.DateOfBirth,
                                        EmailId = cp.EmailId,
                                        MobileNo = cp.MobileNo,
                                        AlternateMobileNo = cp.AlternateMobileNo,
                                        City = cp.City,
                                        State = cp.State,
                                        AadharCardNo = cp.AadharCardNo,
                                        GSTNo = cp.GSTNo,
                                        PanCardNo = cp.PanCardNo,
                                        PanCardPhoto = cp.PanCardPhoto,
                                        AadharCardPhoto = cp.AadharCardPhoto,
                                        GSTPhoto = cp.GSTPhoto,
                                        CompanyPhoto = cp.CompanyPhoto,
                                        CancellationChequePhoto = cp.CancellationChequePhoto,
                                        RequestStatus = cp.RequestStatus,
                                        RejectReason = cp.RejectReason,
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
        [HttpPost]
        public ActionResult Edit(CompanyRequestVM companyRequestVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    tbl_CompanyRequest objCompanyReq = _db.tbl_CompanyRequest.FirstOrDefault(x => x.CompanyRequestId == companyRequestVM.CompanyRequestId);
                    objCompanyReq.RequestStatus = companyRequestVM.RequestStatus;
                    objCompanyReq.RejectReason = companyRequestVM.RejectReason;
                    objCompanyReq.ModifiedBy = LoggedInUserId;
                    objCompanyReq.ModifiedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    if (companyRequestVM.RequestStatus == (int)CompanyRequestStatus.Accept)
                    {
                        string companyInitials = "UN/" + System.DateTime.Today.ToString("ddMMyyyy") + "/2";
                        tbl_Company objcomp = new tbl_Company();

                        objcomp.CompanyTypeId = objCompanyReq.CompanyTypeId;
                        objcomp.CompanyName = objCompanyReq.CompanyName;
                        objcomp.CompanyCode = companyInitials;
                        objcomp.City = objCompanyReq.City;
                        objcomp.State = objCompanyReq.State;
                        objcomp.GSTNo = objCompanyReq.GSTNo;
                        objcomp.GSTPhoto = objCompanyReq.GSTPhoto;
                        objcomp.CompanyPhoto = objCompanyReq.CompanyPhoto;
                        objcomp.CancellationChequePhoto = objCompanyReq.CancellationChequePhoto;
                        objcomp.FreeAccessDays = objCompanyReq.FreeAccessDays;
                        objcomp.IsActive = true;
                        objcomp.CreatedBy = LoggedInUserId;
                        objcomp.CreatedDate = DateTime.UtcNow;
                        objcomp.ModifiedBy = LoggedInUserId;
                        objcomp.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_Company.Add(objcomp);
                        _db.SaveChanges();

                        objCompanyReq.CompanyId = objcomp.CompanyId;
                        _db.SaveChanges();

                        tbl_AdminUser objAdminUser = new tbl_AdminUser();
                        objAdminUser.AdminUserRoleId = (int)AdminRoles.CompanyAdmin;
                        objAdminUser.CompanyId = objCompanyReq.CompanyId;
                        objAdminUser.Prefix = objCompanyReq.Prefix;
                        objAdminUser.FirstName = objCompanyReq.Firstname;
                        objAdminUser.LastName = objCompanyReq.Lastname;
                        objAdminUser.UserName = companyInitials;
                        objAdminUser.Password = CommonMethod.Encrypt(CommonMethod.RandomString(6, true), psSult);
                        objAdminUser.EmailId = objCompanyReq.EmailId;
                        objAdminUser.MobileNo = objCompanyReq.MobileNo;
                        objAdminUser.AlternateMobileNo = objCompanyReq.AlternateMobileNo;
                        objAdminUser.DateOfBirth = objCompanyReq.DateOfBirth;
                        objAdminUser.City = objCompanyReq.City;
                        objAdminUser.State = objCompanyReq.State;
                        objAdminUser.AadharCardNo = objCompanyReq.AadharCardNo;
                        objAdminUser.PanCardNo = objCompanyReq.PanCardNo;
                        objAdminUser.AadharCardPhoto = objCompanyReq.AadharCardPhoto;
                        objAdminUser.PanCardPhoto = objCompanyReq.PanCardPhoto;
                        objAdminUser.IsActive = true;
                        objAdminUser.CreatedBy = LoggedInUserId;
                        objAdminUser.CreatedDate = DateTime.UtcNow;
                        objAdminUser.ModifiedBy = LoggedInUserId;
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
                                           Prefix = emp.Prefix,
                                           Firstname = emp.FirstName,
                                           Lastname = emp.LastName,
                                           DateOfBirth = emp.DateOfBirth,
                                           EmailId = emp.EmailId,
                                           MobileNo = emp.MobileNo,
                                           AlternateMobileNo = emp.AlternateMobileNo,
                                           City = cp.City,
                                           State = cp.State,
                                           AadharCardNo = emp.AadharCardNo,
                                           GSTNo = cp.GSTNo,
                                           PanCardNo = emp.PanCardNo,
                                           PanCardPhoto = emp.PanCardPhoto,
                                           AadharCardPhoto = emp.AadharCardPhoto,
                                           GSTPhoto = cp.GSTPhoto,
                                           CompanyPhoto = cp.CompanyPhoto,
                                           CancellationChequePhoto = cp.CancellationChequePhoto,
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
        public ActionResult EditCompany(CompanyRequestVM registeredCompanyVM, HttpPostedFileBase PanCardPhotoFile,
            HttpPostedFileBase AadharCardPhotoFile,
            HttpPostedFileBase GSTPhotoFile,
            HttpPostedFileBase CompanyPhotoFile,
            HttpPostedFileBase CancellationChequePhotoFile)
        {
            try
            {
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
                            return View(registeredCompanyVM);
                        }

                        // Save file in folder
                        panCardFileName = Guid.NewGuid() + "-" + Path.GetFileName(PanCardPhotoFile.FileName);
                        PanCardPhotoFile.SaveAs(panCardPath + panCardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("PanCardPhotoFile", ErrorMessage.ImageRequired);
                        return View(registeredCompanyVM);
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
                            return View(registeredCompanyVM);
                        }

                        // Save file in folder
                        adharCardFileName = Guid.NewGuid() + "-" + Path.GetFileName(AadharCardPhotoFile.FileName);
                        AadharCardPhotoFile.SaveAs(adharCardPath + adharCardFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("AadharCardPhotoFile", ErrorMessage.ImageRequired);
                        return View(registeredCompanyVM);
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
                            return View(registeredCompanyVM);
                        }

                        // Save file in folder
                        gstFileName = Guid.NewGuid() + "-" + Path.GetFileName(GSTPhotoFile.FileName);
                        GSTPhotoFile.SaveAs(gstPath + gstFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("GSTPhotoFile", ErrorMessage.ImageRequired);
                        return View(registeredCompanyVM);
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
                            return View(registeredCompanyVM);
                        }

                        // Save file in folder
                        companyFileName = Guid.NewGuid() + "-" + Path.GetFileName(CompanyPhotoFile.FileName);
                        CompanyPhotoFile.SaveAs(companyPath + companyFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CompanyPhotoFile", ErrorMessage.ImageRequired);
                        return View(registeredCompanyVM);
                    }
                    #endregion CompanyImage

                    #region Cancel Cheque Image

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
                            return View(registeredCompanyVM);
                        }

                        // Save file in folder
                        chqFileName = Guid.NewGuid() + "-" + Path.GetFileName(CancellationChequePhotoFile.FileName);
                        CancellationChequePhotoFile.SaveAs(cancellationChqPath + chqFileName);
                    }
                    else
                    {
                        ModelState.AddModelError("CancellationChequePhotoFile", ErrorMessage.ImageRequired);
                        return View(registeredCompanyVM);
                    }

                    #endregion ChqImage

                    if (registeredCompanyVM.CompanyId > 0)
                    {

                        tbl_Company objcomp = _db.tbl_Company.Where(x => x.CompanyId == registeredCompanyVM.CompanyId).FirstOrDefault();

                        objcomp.City = registeredCompanyVM.City;
                        objcomp.State = registeredCompanyVM.State;
                        objcomp.GSTNo = registeredCompanyVM.GSTNo;
                        objcomp.GSTPhoto = GSTPhotoFile != null ? gstFileName : objcomp.GSTPhoto;
                        objcomp.CompanyPhoto = CompanyPhotoFile != null ? companyFileName : objcomp.CompanyPhoto;
                        objcomp.CancellationChequePhoto = CancellationChequePhotoFile != null ? chqFileName : objcomp.CancellationChequePhoto;
                        objcomp.FreeAccessDays = registeredCompanyVM.FreeAccessDays;
                        objcomp.ModifiedBy = LoggedInUserId;
                        objcomp.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_Company.Add(objcomp);
                        _db.SaveChanges();


                        tbl_AdminUser objAdminUser = _db.tbl_AdminUser.Where(x => x.UserName == registeredCompanyVM.CompanyCode).FirstOrDefault();
                        objAdminUser.Prefix = registeredCompanyVM.Prefix;
                        objAdminUser.FirstName = registeredCompanyVM.Firstname;
                        objAdminUser.LastName = registeredCompanyVM.Lastname;
                        objAdminUser.EmailId = registeredCompanyVM.EmailId;
                        objAdminUser.MobileNo = registeredCompanyVM.MobileNo;
                        objAdminUser.AlternateMobileNo = registeredCompanyVM.AlternateMobileNo;
                        objAdminUser.DateOfBirth = registeredCompanyVM.DateOfBirth;
                        objAdminUser.City = registeredCompanyVM.City;
                        objAdminUser.State = registeredCompanyVM.State;
                        objAdminUser.AadharCardNo = registeredCompanyVM.AadharCardNo;
                        objAdminUser.PanCardNo = registeredCompanyVM.PanCardNo;
                        objAdminUser.AadharCardPhoto = AadharCardPhotoFile != null ? adharCardFileName : objAdminUser.AadharCardPhoto; 
                        objAdminUser.PanCardPhoto = PanCardPhotoFile != null ? panCardFileName : objAdminUser.PanCardPhoto;
                        objAdminUser.ModifiedBy = LoggedInUserId;
                        objAdminUser.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                    }
                }
                else
                {
                    return View(registeredCompanyVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return RedirectToAction("Requests");
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
                    using (WebClient webClient = new WebClient())
                    {
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        if (enviornment != "Development")
                        {
                            string msg = "Your Otp code for Login is " + num;
                            msg = HttpUtility.UrlEncode(msg);
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", mobileNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
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

            return Json(new { Status = status, Otp = otp, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult View(long id)
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
                                        Prefix = cp.Prefix,
                                        Firstname = cp.Firstname,
                                        Lastname = cp.Lastname,
                                        DateOfBirth = cp.DateOfBirth,
                                        EmailId = cp.EmailId,
                                        MobileNo = cp.MobileNo,
                                        AlternateMobileNo = cp.AlternateMobileNo,
                                        City = cp.City,
                                        State = cp.State,
                                        AadharCardNo = cp.AadharCardNo,
                                        GSTNo = cp.GSTNo,
                                        PanCardNo = cp.PanCardNo,
                                        PanCardPhoto = cp.PanCardPhoto,
                                        AadharCardPhoto = cp.AadharCardPhoto,
                                        GSTPhoto = cp.GSTPhoto,
                                        CompanyPhoto = cp.CompanyPhoto,
                                        CancellationChequePhoto = cp.CancellationChequePhoto,
                                        RequestStatus = cp.RequestStatus,
                                        RejectReason = cp.RejectReason,
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
    }
}