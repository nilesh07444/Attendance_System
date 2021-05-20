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
    public class OurClientController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public string OurClientDirectoryPath = "";
        public OurClientController()
        {
            _db = new AttendanceSystemEntities();
            OurClientDirectoryPath = ErrorMessage.OurClientDirectoryPath;
        }
        public ActionResult Index()
        {
            List<OurClientVM> lstOurClients = new List<OurClientVM>();
            try
            {
                lstOurClients = (from client in _db.tbl_Sponsor                                 
                                 select new OurClientVM
                                 {
                                     SponsorId = client.SponsorId,
                                     SponsorImage = client.SponsorImage,
                                     SponsorName = client.SponsorName,
                                     SponsorLink = client.SponsorLink,
                                     IsActive = client.IsActive,
                                 }).OrderByDescending(x => x.SponsorId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(lstOurClients);
        }

        public ActionResult Add(long id)
        {
            OurClientVM ourclientVM = new OurClientVM();
            if (id > 0)
            {
                ourclientVM = (from client in _db.tbl_Sponsor
                               where client.SponsorId == id
                               select new OurClientVM
                               {
                                   SponsorId = client.SponsorId,
                                   SponsorName = client.SponsorName,
                                   SponsorImage = client.SponsorImage,
                                   SponsorLink = client.SponsorLink,
                                   IsActive = client.IsActive,
                               }).FirstOrDefault();
            }
            return View(ourclientVM);
        }

        [HttpPost]
        public ActionResult Add(OurClientVM ourclientVM, HttpPostedFileBase SponsorImageFile)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    string fileName = string.Empty;
                    string path = Server.MapPath(OurClientDirectoryPath);

                    bool folderExists = Directory.Exists(path);
                    if (!folderExists)
                        Directory.CreateDirectory(path);

                    if (SponsorImageFile != null)
                    {
                        // Image file validation
                        string ext = Path.GetExtension(SponsorImageFile.FileName);
                        if (ext.ToUpper().Trim() != ".JPG" && ext.ToUpper() != ".PNG" && ext.ToUpper() != ".GIF" && ext.ToUpper() != ".JPEG" && ext.ToUpper() != ".BMP")
                        {
                            ModelState.AddModelError("SponsorImageFile", ErrorMessage.SelectOnlyImage);
                            return View(ourclientVM);
                        }

                        // Save file in folder
                        fileName = Guid.NewGuid() + "-" + Path.GetFileName(SponsorImageFile.FileName);
                        SponsorImageFile.SaveAs(path + fileName);
                    }
                    else
                    {
                        if (ourclientVM.SponsorId == 0)
                        {
                            ModelState.AddModelError("SponsorImageFile", ErrorMessage.ImageRequired);
                            return View(ourclientVM);
                        }
                    }

                    if (ourclientVM.SponsorId > 0)
                    {
                        tbl_Sponsor objOurClient = _db.tbl_Sponsor.Where(x => x.SponsorId == ourclientVM.SponsorId).FirstOrDefault();
                        objOurClient.SponsorImage = SponsorImageFile != null ? fileName : objOurClient.SponsorImage;
                        objOurClient.SponsorName = ourclientVM.SponsorName;
                        objOurClient.SponsorLink = ourclientVM.SponsorLink;

                        objOurClient.ModifiedBy = LoggedInUserId;
                        objOurClient.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        tbl_Sponsor objOurClient = new tbl_Sponsor();
                        objOurClient.SponsorImage = fileName;
                        objOurClient.SponsorName = ourclientVM.SponsorName;
                        objOurClient.SponsorLink = ourclientVM.SponsorLink;
                        objOurClient.IsActive = true;
                        objOurClient.CreatedBy = LoggedInUserId;
                        objOurClient.CreatedDate = DateTime.UtcNow;
                        objOurClient.ModifiedBy = LoggedInUserId;
                        objOurClient.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_Sponsor.Add(objOurClient);
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

            return View(ourclientVM);
        }

        [HttpPost]
        public string DeleteOurClient(int Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Sponsor objOurClient = _db.tbl_Sponsor.Where(x => x.SponsorId == Id).FirstOrDefault();

                if (objOurClient == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_Sponsor.Remove(objOurClient);
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
                tbl_Sponsor objOurClient = _db.tbl_Sponsor.Where(x => x.SponsorId == Id).FirstOrDefault();

                if (objOurClient != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objOurClient.IsActive = true;
                    }
                    else
                    {
                        objOurClient.IsActive = false;
                    }

                    objOurClient.ModifiedBy = LoggedInUserId;
                    objOurClient.ModifiedDate = DateTime.UtcNow;

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