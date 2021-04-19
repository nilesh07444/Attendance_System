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
    public class HomeImageController : Controller
    {
        // GET: Admin/HomeImage
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
                                  HomeImageName = hi.HomeImageName,
                                  HeadingText1 = hi.HeadingText1,
                                  HeadingText2 = hi.HeadingText2,
                                  IsActive = hi.IsActive,
                                  HomeImageFor = hi.HomeImageFor
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
                                   HomeImageName = hi.HomeImageName,
                                   HeadingText1 = hi.HeadingText1,
                                   HeadingText2 = hi.HeadingText2,
                                   IsActive = hi.IsActive,
                                   HomeImageFor = hi.HomeImageFor
                               }).FirstOrDefault();
            }
            homeImageVM.HomeImageForList = GetHomeImageFor();
            return View(homeImageVM);
        }

        [HttpPost]
        public ActionResult Add(HomeImageVM homeImageVM, HttpPostedFileBase HomeImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath(HomeDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (HomeImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(HomeImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("HomeImageFile", ErrorMessage.SelectOnlyImage);
                            return View(homeImageVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(HomeImageFile.FileName);
                        HomeImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        ModelState.AddModelError("HomeImageFile", ErrorMessage.ImageRequired);
                        return View(homeImageVM);
                    }

                    if (homeImageVM.HomeImageId > 0)
                    {
                        tbl_HomeImage objHome = _db.tbl_HomeImage.Where(x => x.HomeImageId == homeImageVM.HomeImageId).FirstOrDefault();
                        objHome.HomeImageName = fileName;
                        objHome.HomeImageFor = homeImageVM.HomeImageFor.HasValue ? homeImageVM.HomeImageFor.Value : (int)HomeImageFor.Website;
                        objHome.HeadingText1 = homeImageVM.HeadingText1;
                        objHome.HeadingText2 = homeImageVM.HeadingText2;

                        objHome.ModifiedBy = LoggedInUserId;
                        objHome.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        tbl_HomeImage objHome = new tbl_HomeImage();
                        objHome.HomeImageFor = homeImageVM.HomeImageFor.Value;
                        objHome.HeadingText1 = homeImageVM.HeadingText1;
                        objHome.HeadingText2 = homeImageVM.HeadingText2;
                        objHome.HomeImageName = fileName;
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

        public ActionResult Edit(int Id)
        {
            return View();
        }

        public ActionResult View(int Id)
        {
            return View();
        }
        private List<SelectListItem> GetHomeImageFor()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "1", Text = "Website" });
            lst.Add(new SelectListItem { Value = "2", Text = "Mobile App" });

            return lst;
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