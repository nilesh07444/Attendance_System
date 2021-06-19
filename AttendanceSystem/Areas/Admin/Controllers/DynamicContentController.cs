using AttendanceSystem.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AttendanceSystem.Models;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class DynamicContentController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public DynamicContentController()
        {
            _db = new AttendanceSystemEntities();
        }
         
        public ActionResult Index()
        {
            List<DynamicContentVM> lstDyanamicContent = new List<DynamicContentVM>();

            try
            {
                lstDyanamicContent = (from d in _db.tbl_DynamicContent
                                      select new DynamicContentVM
                                      {
                                          DynamicContentId = d.DynamicContentId,
                                          DynamicContentType = d.DynamicContentType,
                                          ContentTitle = d.ContentTitle,
                                          ContentDescription = d.ContentDescription,
                                          SeqNo = d.SeqNo
                                      }).OrderBy(X => X.SeqNo).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(lstDyanamicContent);
        }

        public ActionResult Add()
        {
            DynamicContentVM contentVM = new DynamicContentVM();
            contentVM.DynamicContentTypeList = GetDynamicContentType();

            return View(contentVM);
        }

        [HttpPost]
        public ActionResult Add(DynamicContentVM contentVM)
        {

            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    tbl_DynamicContent objContent = new tbl_DynamicContent();
                    objContent.ContentTitle = contentVM.ContentTitle;
                    objContent.ContentDescription = contentVM.ContentDescription;
                    objContent.SeqNo = contentVM.SeqNo != null ? Convert.ToInt32(contentVM.SeqNo) : 0;
                    objContent.DynamicContentType = contentVM.DynamicContentType;
                    objContent.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    objContent.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.tbl_DynamicContent.Add(objContent);
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(contentVM);
        }
         
        public ActionResult Edit(int Id)
        {
            DynamicContentVM objDyanamicContent = new DynamicContentVM();

            try
            {

                objDyanamicContent = (from d in _db.tbl_DynamicContent
                                      select new DynamicContentVM
                                      {
                                          DynamicContentId = d.DynamicContentId,
                                          DynamicContentType = d.DynamicContentType,
                                          ContentTitle = d.ContentTitle,
                                          ContentDescription = d.ContentDescription,
                                          SeqNo = d.SeqNo
                                      }).Where(X => X.DynamicContentId == Id).FirstOrDefault();

                objDyanamicContent.DynamicContentTypeList = GetDynamicContentType();

            }
            catch (Exception ex)
            {
            }

            return View(objDyanamicContent);
        }

        [HttpPost]
        public ActionResult Edit(DynamicContentVM contentVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    tbl_DynamicContent objContent = _db.tbl_DynamicContent.Where(x => x.DynamicContentId == contentVM.DynamicContentId).FirstOrDefault();

                    objContent.ContentTitle = contentVM.ContentTitle;
                    objContent.ContentDescription = contentVM.ContentDescription;
                    objContent.SeqNo = contentVM.SeqNo != null ? Convert.ToInt32(contentVM.SeqNo) : 0;
                    objContent.DynamicContentType = contentVM.DynamicContentType;
                    objContent.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(contentVM);
        }

        private List<SelectListItem> GetDynamicContentType()
        {
            List<SelectListItem> lst = new List<SelectListItem>();

            lst.Add(new SelectListItem { Value = "1", Text = "FAQ" });
            lst.Add(new SelectListItem { Value = "2", Text = "Privacy Policy" });
            lst.Add(new SelectListItem { Value = "3", Text = "Terms & Condition" });
            lst.Add(new SelectListItem { Value = "4", Text = "How To Use" });

            return lst;
        }

        [HttpPost]
        public string DeleteDynamicContent(int DynamicContentId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_DynamicContent objDynamicContent = _db.tbl_DynamicContent.Where(x => x.DynamicContentId == DynamicContentId).FirstOrDefault();

                if (objDynamicContent == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_DynamicContent.Remove(objDynamicContent);
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