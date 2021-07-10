using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class PackageController : Controller
    {
   
        AttendanceSystemEntities _db;
        public string packageDirectoryPath = "";
        public PackageController()
        {
            _db = new AttendanceSystemEntities();
            packageDirectoryPath = ErrorMessage.PackageDirectoryPath;
        }
        public ActionResult Index()
        {
            List<PackageVM> package = new List<PackageVM>();
            try
            {
                package = (from pck in _db.tbl_Package
                           where !pck.IsDeleted
                           select new PackageVM
                           {
                               PackageId = pck.PackageId,
                               PackageName = pck.PackageName,
                               Amount = pck.Amount,
                               PackageDescription = pck.PackageDescription,
                               AccessDays = pck.AccessDays,
                               IsActive = pck.IsActive,
                               PackageImage = pck.PackageImage,
                               NoOfSMS = pck.NoOfSMS,
                               NoOfEmployee = pck.NoOfEmployee,
                               PackageColorCode = pck.PackageColorCode,
                               PackageFontIcon = pck.PackageFontIcon
                           }).OrderByDescending(x => x.PackageId).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(package);
        }

        public ActionResult Add(long id)
        {
            PackageVM packageVM = new PackageVM();
            if (id > 0)
            {
                packageVM = (from pkg in _db.tbl_Package
                             where pkg.PackageId == id && !pkg.IsDeleted
                             select new PackageVM
                             {
                                 PackageId = pkg.PackageId,
                                 PackageName = pkg.PackageName,
                                 Amount = pkg.Amount,
                                 PackageDescription = pkg.PackageDescription,
                                 AccessDays = pkg.AccessDays,
                                 PackageImage = pkg.PackageImage,
                                 IsActive = pkg.IsActive,
                                 NoOfSMS = pkg.NoOfSMS,
                                 NoOfEmployee = pkg.NoOfEmployee,
                                 PackageColorCode = pkg.PackageColorCode,
                                 PackageFontIcon = pkg.PackageFontIcon
                             }).FirstOrDefault();
            }

            return View(packageVM);
        }

        [HttpPost]
        public ActionResult Add(PackageVM packageVM, HttpPostedFileBase PackageImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = (int)PaymentGivenBy.SuperAdmin;

                    string fileName = string.Empty;
                    string path = Server.MapPath(packageDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (PackageImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(PackageImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("PackageImageFile", ErrorMessage.SelectOnlyImage);
                            return View(packageVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(PackageImageFile.FileName);
                        PackageImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        if (packageVM.PackageId == 0)
                        {
                            ModelState.AddModelError("PackageImageFile", ErrorMessage.ImageRequired);
                            return View(packageVM);
                        }
                    }

                    if (packageVM.PackageId > 0)
                    {
                        tbl_Package objPackage = _db.tbl_Package.Where(x => x.PackageId == packageVM.PackageId).FirstOrDefault();
                        objPackage.PackageImage = PackageImageFile != null ? fileName : objPackage.PackageImage;
                        objPackage.PackageName = packageVM.PackageName;
                        objPackage.Amount = packageVM.Amount;
                        objPackage.PackageDescription = packageVM.PackageDescription;
                        objPackage.AccessDays = packageVM.AccessDays;
                        objPackage.NoOfSMS = packageVM.NoOfSMS;
                        objPackage.NoOfEmployee = packageVM.NoOfEmployee;
                        objPackage.PackageColorCode = packageVM.PackageColorCode;
                        objPackage.PackageFontIcon = packageVM.PackageFontIcon;
                        objPackage.ModifiedBy = LoggedInUserId;
                        objPackage.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_Package objPackage = new tbl_Package();
                        objPackage.PackageName = packageVM.PackageName;
                        objPackage.Amount = packageVM.Amount;
                        objPackage.PackageDescription = packageVM.PackageDescription;
                        objPackage.AccessDays = packageVM.AccessDays;
                        objPackage.PackageImage = fileName;
                        objPackage.IsActive = true;
                        objPackage.NoOfSMS = packageVM.NoOfSMS;
                        objPackage.NoOfEmployee = packageVM.NoOfEmployee;
                        objPackage.PackageColorCode = packageVM.PackageColorCode;
                        objPackage.PackageFontIcon = packageVM.PackageFontIcon;
                        objPackage.CreatedBy = LoggedInUserId;
                        objPackage.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objPackage.ModifiedBy = LoggedInUserId;
                        objPackage.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_Package.Add(objPackage);
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

            return View(packageVM);
        }

        public ActionResult View(int id)
        {
            PackageVM packageVM = new PackageVM();

            packageVM = (from pkg in _db.tbl_Package
                         where pkg.PackageId == id && !pkg.IsDeleted
                         select new PackageVM
                         {
                             PackageId = pkg.PackageId,
                             PackageName = pkg.PackageName,
                             Amount = pkg.Amount,
                             PackageDescription = pkg.PackageDescription,
                             AccessDays = pkg.AccessDays,
                             PackageImage = pkg.PackageImage,
                             IsActive = pkg.IsActive,
                             NoOfSMS = pkg.NoOfSMS,
                             NoOfEmployee = pkg.NoOfEmployee,
                             PackageColorCode = pkg.PackageColorCode,
                             PackageFontIcon = pkg.PackageFontIcon
                         }).FirstOrDefault();

            return View(packageVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Package objPackage = _db.tbl_Package.Where(x => x.PackageId == Id).FirstOrDefault();

                if (objPackage != null)
                {
                    long LoggedInUserId = (int)PaymentGivenBy.SuperAdmin;
                    if (Status == "Active")
                    {
                        objPackage.IsActive = true;
                    }
                    else
                    {
                        objPackage.IsActive = false;
                    }

                    objPackage.ModifiedBy = LoggedInUserId;
                    objPackage.ModifiedDate = CommonMethod.CurrentIndianDateTime();

                    _db.SaveChanges();
                    ReturnMessage = ErrorMessage.Success;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = ErrorMessage.Exception;
            }

            return ReturnMessage;
        }

        [HttpPost]
        public string DeletePackage(int PackageId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Package objPackage = _db.tbl_Package.Where(x => x.PackageId == PackageId).FirstOrDefault();

                if (objPackage == null)
                {
                    ReturnMessage = ErrorMessage.NotFound;
                }
                else
                {
                    _db.tbl_Package.Remove(objPackage);
                    _db.SaveChanges();

                    ReturnMessage = ErrorMessage.Success;
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = ErrorMessage.Exception;
            }

            return ReturnMessage;
        }
    }
}