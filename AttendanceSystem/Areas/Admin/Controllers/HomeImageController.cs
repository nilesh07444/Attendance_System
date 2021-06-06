using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class HomeImageController : Controller
    {
        AttendanceSystemEntities _db;
        public string HomeDirectoryPath = "";
        public HomeImageController()
        {
            _db = new AttendanceSystemEntities();
            HomeDirectoryPath = ErrorMessage.HomeDirectoryPath;
        }
        public ActionResult Index()
        {
            List<HomeImageVM> homeImages = new List<HomeImageVM>();
            try
            {
                homeImages = (from hi in _db.tbl_HomeImage
                              select new HomeImageVM
                              {
                                  HomeImageId = hi.HomeImageId, 
                                  HeadingText1 = hi.HeadingText1,
                                  HeadingText2 = hi.HeadingText2,
                                  IsActive = hi.IsActive,
                              }).OrderByDescending(x => x.HomeImageId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(homeImages);
        }

        public ActionResult Add(long id)
        {
            HomeImageVM homeImageVM = new HomeImageVM();
            if (id > 0)
            {
                homeImageVM = (from hi in _db.tbl_HomeImage
                               where hi.HomeImageId == id
                               select new HomeImageVM
                               {
                                   HomeImageId = hi.HomeImageId, 
                                   HeadingText1 = hi.HeadingText1,
                                   HeadingText2 = hi.HeadingText2,
                                   IsActive = hi.IsActive,
                               }).FirstOrDefault();
            }
            return View(homeImageVM);
        }

        [HttpPost]
        public ActionResult Add(HomeImageVM homeImageVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                     
                    if (homeImageVM.HomeImageId > 0)
                    {
                        tbl_HomeImage objHome = _db.tbl_HomeImage.Where(x => x.HomeImageId == homeImageVM.HomeImageId).FirstOrDefault();
                    
                        objHome.HeadingText1 = homeImageVM.HeadingText1;
                        objHome.HeadingText2 = homeImageVM.HeadingText2;

                        objHome.ModifiedBy = LoggedInUserId;
                        objHome.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        tbl_HomeImage objHome = new tbl_HomeImage();
                        objHome.HeadingText1 = homeImageVM.HeadingText1;
                        objHome.HeadingText2 = homeImageVM.HeadingText2; 
                        objHome.IsActive = true;
                        objHome.CreatedBy = LoggedInUserId;
                        objHome.CreatedDate = DateTime.UtcNow;
                        objHome.ModifiedBy = LoggedInUserId;
                        objHome.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_HomeImage.Add(objHome);
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

            return View(homeImageVM);
        }

        public ActionResult View(int Id)
        {
            return View();
        }

        [HttpPost]
        public string DeleteHomeImage(int HomeImageId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_HomeImage objHome = _db.tbl_HomeImage.Where(x => x.HomeImageId == HomeImageId).FirstOrDefault();

                if (objHome == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_HomeImage.Remove(objHome);
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
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_HomeImage objHome = _db.tbl_HomeImage.Where(x => x.HomeImageId == Id).FirstOrDefault();

                if (objHome != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objHome.IsActive = true;
                    }
                    else
                    {
                        objHome.IsActive = false;
                    }

                    objHome.ModifiedBy = LoggedInUserId;
                    objHome.ModifiedDate = DateTime.UtcNow;

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
    }
}