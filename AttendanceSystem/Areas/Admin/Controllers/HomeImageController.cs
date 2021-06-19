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
        public string HomeSliderDirectoryPath = "";
        public HomeImageController()
        {
            _db = new AttendanceSystemEntities();
            HomeDirectoryPath = ErrorMessage.HomeDirectoryPath;
            HomeSliderDirectoryPath = ErrorMessage.HomeSliderDirectoryPath;
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
                        objHome.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_HomeImage objHome = new tbl_HomeImage();
                        objHome.HeadingText1 = homeImageVM.HeadingText1;
                        objHome.HeadingText2 = homeImageVM.HeadingText2;
                        objHome.IsActive = true;
                        objHome.CreatedBy = LoggedInUserId;
                        objHome.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objHome.ModifiedBy = LoggedInUserId;
                        objHome.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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
                    objHome.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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

        public ActionResult Slider()
        {
            List<HomeSliderVM> homeslider = new List<HomeSliderVM>();
            try
            {
                homeslider = (from s in _db.tbl_HomeSlider
                              select new HomeSliderVM
                              {
                                  HomeSliderId = s.HomeSliderId,
                                  SliderImageName = s.SliderImageName,
                                  SliderType = s.SliderType,
                                  IsActive = s.IsActive,
                              }).OrderByDescending(x => x.HomeSliderId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(homeslider);
        }
 
        public ActionResult AddSlider(long Id)
        {
            HomeSliderVM homeSliderVM = new HomeSliderVM(); 
            if (Id > 0)
            {
                homeSliderVM = (from s in _db.tbl_HomeSlider
                               where s.HomeSliderId == Id
                               select new HomeSliderVM
                               {
                                   HomeSliderId = s.HomeSliderId,
                                   SliderImageName = s.SliderImageName,
                                   SliderType = s.SliderType,
                                   IsActive = s.IsActive,
                               }).FirstOrDefault();

                homeSliderVM.SliderTypeList = GetSliderTypeList();

            }
            else
                homeSliderVM.SliderTypeList = GetSliderTypeList();

            return View(homeSliderVM);
        }

        [HttpPost]
        public ActionResult AddSlider(HomeSliderVM homeSliderVM, HttpPostedFileBase SliderImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    #region Upload

                    string fileName = string.Empty;
                    string path = Server.MapPath(HomeSliderDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (SliderImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(SliderImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            homeSliderVM.SliderTypeList = GetSliderTypeList();
                            ModelState.AddModelError("SliderImageFile", ErrorMessage.SelectOnlyImage);
                            return View(homeSliderVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(SliderImageFile.FileName);
                        SliderImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        if (homeSliderVM.HomeSliderId == 0)
                        {
                            homeSliderVM.SliderTypeList = GetSliderTypeList();
                            ModelState.AddModelError("SliderImageFile", ErrorMessage.ImageRequired);
                            return View(homeSliderVM);
                        }
                    }

                    #endregion

                    if (homeSliderVM.HomeSliderId > 0)
                    {
                        tbl_HomeSlider objHomeSlider = _db.tbl_HomeSlider.Where(x => x.HomeSliderId == homeSliderVM.HomeSliderId).FirstOrDefault();

                        objHomeSlider.SliderImageName = SliderImageFile != null ? fileName : objHomeSlider.SliderImageName;
                        objHomeSlider.SliderType = homeSliderVM.SliderType;

                        objHomeSlider.ModifiedBy = LoggedInUserId;
                        objHomeSlider.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_HomeSlider objHomeSlider = new tbl_HomeSlider();
                        objHomeSlider.SliderImageName = fileName;
                        objHomeSlider.SliderType = homeSliderVM.SliderType;
                        objHomeSlider.IsActive = true;
                        objHomeSlider.CreatedBy = LoggedInUserId;
                        objHomeSlider.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objHomeSlider.ModifiedBy = LoggedInUserId;
                        objHomeSlider.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_HomeSlider.Add(objHomeSlider);
                    }
                    _db.SaveChanges();

                    return RedirectToAction("Slider");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(homeSliderVM);
        }

        [HttpPost]
        public string DeleteHomeSliderImage(long Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_HomeSlider objHome = _db.tbl_HomeSlider.Where(x => x.HomeSliderId == Id).FirstOrDefault();

                if (objHome == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_HomeSlider.Remove(objHome);
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
        public string ChangeHomeSliderImageStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_HomeSlider objHome = _db.tbl_HomeSlider.Where(x => x.HomeSliderId == Id).FirstOrDefault();

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
                    objHome.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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

        private List<SelectListItem> GetSliderTypeList()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "1", Text = "Slider 1" });
            lst.Add(new SelectListItem { Value = "2", Text = "Slider 2" });

            return lst;
        }
    }
}