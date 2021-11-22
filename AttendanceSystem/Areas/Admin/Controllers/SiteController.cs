using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class SiteController : Controller
    {
        // GET: Admin/Site
        AttendanceSystemEntities _db; 
        public SiteController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<SiteVM> Site = new List<SiteVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                Site = (from st in _db.tbl_Site
                        where !st.IsDeleted && st.CompanyId == companyId
                        select new SiteVM
                        {
                            SiteId = st.SiteId,
                            SiteName = st.SiteName,
                            SiteDescription = st.SiteDescription,
                            IsActive = st.IsActive,
                            Latitude = st.Latitude,
                            Longitude = st.Longitude,
                            RadiousInMeter = st.RadiousInMeter
                        }).OrderByDescending(x => x.SiteId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(Site);
        }

        public ActionResult Add(long id)
        {
            SiteVM SiteVM = new SiteVM();
            if (id > 0)
            {
                SiteVM = (from st in _db.tbl_Site
                          where st.SiteId == id && !st.IsDeleted
                          select new SiteVM
                          {
                              SiteId = st.SiteId,
                              SiteName = st.SiteName,
                              SiteDescription = st.SiteDescription,
                              IsActive = st.IsActive
                          }).FirstOrDefault();
            }

            return View(SiteVM);
        }

        [HttpPost]
        public ActionResult Add(SiteVM SiteVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;

                    if (SiteVM.SiteId > 0)
                    {
                        tbl_Site objSite = _db.tbl_Site.Where(x => x.SiteId == SiteVM.SiteId).FirstOrDefault();
                        objSite.SiteName = SiteVM.SiteName;
                        objSite.SiteDescription = SiteVM.SiteDescription;
                        objSite.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objSite.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_Site objSite = new tbl_Site();
                        objSite.CompanyId = companyId;
                        objSite.SiteName = SiteVM.SiteName;
                        objSite.SiteDescription = SiteVM.SiteDescription;
                        objSite.IsActive = true;
                        objSite.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objSite.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objSite.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objSite.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_Site.Add(objSite);
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

            return View(SiteVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Site objSite = _db.tbl_Site.Where(x => x.SiteId == Id).FirstOrDefault();

                if (objSite != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objSite.IsActive = true;
                    }
                    else
                    {
                        objSite.IsActive = false;
                    }

                    objSite.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objSite.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                    _db.SaveChanges();
                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeleteSite(int SiteId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Site objSite = _db.tbl_Site.Where(x => x.SiteId == SiteId).FirstOrDefault();

                if (objSite == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objSite.IsDeleted = true;
                    objSite.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objSite.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.SaveChanges();

                    ReturnMessage = "success";
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public JsonResult CheckSiteName(string siteName)
        {
            bool isExist = false;
            try
            {

                isExist = _db.tbl_Site.Any(x => !x.IsDeleted && x.CompanyId == clsAdminSession.CompanyId && x.SiteName == siteName);
            }
            catch (Exception ex)
            {
                isExist = false;
            }

            return Json(new { Status = isExist }, JsonRequestBehavior.AllowGet);
        }
    }
}