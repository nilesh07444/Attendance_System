using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class MyProfileController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public MyProfileController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index()
        {
            AdminProfileVM objProfile = new AdminProfileVM();
            try
            {
                long loggedInUserId = clsAdminSession.UserID;
                int loggedInRoleID = clsAdminSession.RoleID;

                objProfile = (from p in _db.tbl_AdminUser
                              where p.AdminUserId == loggedInUserId
                              select new AdminProfileVM
                              {
                                  AdminUserId = p.AdminUserId,
                                  AdminUserRoleId = p.AdminUserRoleId,
                                  CompanyId = p.CompanyId,
                                  FirstName = p.FirstName,
                                  LastName = p.LastName,
                                  UserName = p.UserName,
                                  EmailId = p.EmailId,
                                  MobileNo = p.MobileNo,
                                  Address = p.Address,
                                  City = p.City,
                                  State = p.State,
                                  IsActive = p.IsActive,
                                  IsDeleted = p.IsDeleted,
                                  CreatedBy = p.CreatedBy,
                                  CreatedDate = p.CreatedDate,
                                  ModifiedBy = p.ModifiedBy,
                                  ModifiedDate = p.ModifiedDate,
                                  Prefix = p.Prefix, 
                                  AlternateMobileNo = p.AlternateMobileNo,
                                  AadharCardNo = p.AadharCardNo,
                                  PanCardNo = p.PanCardNo,
                                  PanCardPhoto = p.PanCardPhoto, 
                                  AadharCardPhoto = p.AadharCardPhoto
                              }).FirstOrDefault();

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