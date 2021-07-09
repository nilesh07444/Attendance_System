using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class MyProfileController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public MyProfileController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index()
        {
            CompanyRequestVM objProfile = new CompanyRequestVM();
            try
            {
                long loggedInUserId = clsAdminSession.UserID;
                int loggedInRoleID = clsAdminSession.RoleID;

                if (loggedInRoleID == (int)AdminRoles.SuperAdmin)
                {
                    objProfile = (from emp in _db.tbl_AdminUser 
                                  where emp.AdminUserId == loggedInUserId
                                  select new CompanyRequestVM
                                  {                                       
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
                                  }).FirstOrDefault();
                }
                else
                {

                    objProfile = (from emp in _db.tbl_AdminUser
                                  join cp in _db.tbl_Company on emp.CompanyId equals cp.CompanyId
                                  join ct in _db.mst_CompanyType on cp.CompanyTypeId equals ct.CompanyTypeId
                                  where emp.AdminUserId == loggedInUserId
                                  select new CompanyRequestVM
                                  {
                                      CompanyId = cp.CompanyId,
                                      CompanyTypeId = cp.CompanyTypeId,
                                      CompanyCode = cp.CompanyCode,
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
                                      CompanyDistrict = cp.District,
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
                                      CompanyAdminDateOfMarriageAnniversary = emp.DateOfMarriageAnniversary,
                                      CompanyAdminMobileNo = emp.MobileNo,
                                      CompanyAdminAlternateMobileNo = emp.AlternateMobileNo,
                                      CompanyAdminDesignation = emp.Designation,
                                      CompanyAdminAddress = emp.Address,
                                      CompanyAdminPincode = emp.Pincode,
                                      CompanyAdminCity = emp.City,
                                      CompanyAdminState = emp.State,
                                      CompanyAdminDistrict = emp.District,
                                      CompanyAdminProfilePhoto = emp.ProfilePhoto,
                                      CompanyAdminAadharCardNo = emp.AadharCardNo,
                                      CompanyAdminAadharCardPhoto = emp.AadharCardPhoto,
                                      CompanyAdminPanCardPhoto = emp.PanCardPhoto,
                                      CompanyAdminPanCardNo = emp.PanCardNo,
                                      FreeAccessDays = cp.FreeAccessDays,
                                      CompanyTypeText = ct.CompanyTypeName,
                                      EmployeeCode = emp.UserName
                                  }).FirstOrDefault();

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return View(objProfile);
        }

        public ActionResult Edit()
        {
            return View();
        }

    }
}