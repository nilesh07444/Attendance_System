using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class CompanyController : Controller
    {
        AttendanceSystemEntities _db;
        string psSult;
        public CompanyController()
        {
            _db = new AttendanceSystemEntities();
            psSult = ConfigurationManager.AppSettings["PasswordSult"].ToString();
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

    }
}