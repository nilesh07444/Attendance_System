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
    public class OfficeLocationController : Controller
    {
        AttendanceSystemEntities _db;
        public OfficeLocationController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<OfficeLocationVM> lstLocations = new List<OfficeLocationVM>();
            try
            {
                long companyId = clsAdminSession.CompanyId;
                lstLocations = (from st in _db.tbl_OfficeLocation
                                where !st.IsDeleted && st.CompanyId == companyId
                                select new OfficeLocationVM
                                {
                                    OfficeLocationId = st.OfficeLocationId,
                                    OfficeLocationName = st.OfficeLocationName,
                                    OfficeLocationDescription = st.OfficeLocationDescription,
                                    IsActive = st.IsActive,
                                    Latitude = st.Latitude,
                                    Longitude = st.Longitude,
                                    RadiousInMeter = st.RadiousInMeter
                                }).OrderByDescending(x => x.OfficeLocationId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(lstLocations);
        }

        public ActionResult Add(long Id)
        {
            OfficeLocationVM OfficeLocationVM = new OfficeLocationVM();
            if (Id > 0)
            {
                OfficeLocationVM = (from st in _db.tbl_OfficeLocation
                                    where st.OfficeLocationId == Id && !st.IsDeleted
                                    select new OfficeLocationVM
                                    {
                                        OfficeLocationId = st.OfficeLocationId,
                                        OfficeLocationName = st.OfficeLocationName,
                                        OfficeLocationDescription = st.OfficeLocationDescription,
                                        IsActive = st.IsActive
                                    }).FirstOrDefault();
            }

            return View(OfficeLocationVM);
        }

        [HttpPost]
        public ActionResult Add(OfficeLocationVM OfficeLocationVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;

                    if (OfficeLocationVM.OfficeLocationId > 0)
                    {
                        tbl_OfficeLocation objOfficeLocation = _db.tbl_OfficeLocation.Where(x => x.OfficeLocationId == OfficeLocationVM.OfficeLocationId).FirstOrDefault();
                        objOfficeLocation.OfficeLocationName = OfficeLocationVM.OfficeLocationName;
                        objOfficeLocation.OfficeLocationDescription = OfficeLocationVM.OfficeLocationDescription;
                        objOfficeLocation.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objOfficeLocation.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_OfficeLocation objOfficeLocation = new tbl_OfficeLocation();
                        objOfficeLocation.CompanyId = companyId;
                        objOfficeLocation.OfficeLocationName = OfficeLocationVM.OfficeLocationName;
                        objOfficeLocation.OfficeLocationDescription = OfficeLocationVM.OfficeLocationDescription;
                        objOfficeLocation.IsActive = true;
                        objOfficeLocation.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objOfficeLocation.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objOfficeLocation.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objOfficeLocation.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_OfficeLocation.Add(objOfficeLocation);
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

            return View(OfficeLocationVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_OfficeLocation objOfficeLocation = _db.tbl_OfficeLocation.Where(x => x.OfficeLocationId == Id).FirstOrDefault();

                if (objOfficeLocation != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objOfficeLocation.IsActive = true;
                    }
                    else
                    {
                        objOfficeLocation.IsActive = false;
                    }

                    objOfficeLocation.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objOfficeLocation.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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
        public string DeleteOfficeLocation(int Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_OfficeLocation objOfficeLocation = _db.tbl_OfficeLocation.Where(x => x.OfficeLocationId == Id).FirstOrDefault();

                if (objOfficeLocation == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objOfficeLocation.IsDeleted = true;
                    objOfficeLocation.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objOfficeLocation.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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

        public JsonResult CheckOfficeLocationName(string officeLocationName)
        {
            bool isExist = false;
            try
            {

                isExist = _db.tbl_OfficeLocation.Any(x => !x.IsDeleted && x.CompanyId == clsAdminSession.CompanyId && x.OfficeLocationName == officeLocationName);
            }
            catch (Exception ex)
            {
                isExist = false;
            }

            return Json(new { Status = isExist }, JsonRequestBehavior.AllowGet);
        }
    }
}