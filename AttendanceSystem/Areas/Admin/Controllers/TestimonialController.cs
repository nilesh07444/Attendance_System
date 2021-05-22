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
    public class TestimonialController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public TestimonialController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<TestimonialVM> lstTestimonial = new List<TestimonialVM>();
            try
            {
                lstTestimonial = (from t in _db.tbl_Testimonial
                                  select new TestimonialVM
                                  {
                                      TestimonialId = t.TestimonialId,
                                      CompanyName = t.CompanyName,
                                      CompanyPersonName = t.CompanyPersonName,
                                      FeedbackText = t.FeedbackText,
                                      IsActive = t.IsActive,
                                  }).OrderByDescending(x => x.TestimonialId).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View(lstTestimonial);
        }

        public ActionResult Add(long id)
        {
            TestimonialVM objTestimonial = new TestimonialVM();
            if (id > 0)
            {
                objTestimonial = (from t in _db.tbl_Testimonial
                                  where t.TestimonialId == id
                                  select new TestimonialVM
                                  {
                                      TestimonialId = t.TestimonialId,
                                      CompanyName = t.CompanyName,
                                      CompanyPersonName = t.CompanyPersonName,
                                      FeedbackText = t.FeedbackText,
                                      IsActive = t.IsActive,
                                  }).FirstOrDefault();
            }
            return View(objTestimonial);
        }

        [HttpPost]
        public ActionResult Add(TestimonialVM testimonialVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    if (testimonialVM.TestimonialId > 0)
                    {
                        tbl_Testimonial objTestimonial = _db.tbl_Testimonial.Where(x => x.TestimonialId == testimonialVM.TestimonialId).FirstOrDefault();
                        objTestimonial.CompanyName = testimonialVM.CompanyName;
                        objTestimonial.CompanyPersonName = testimonialVM.CompanyPersonName;
                        objTestimonial.FeedbackText = testimonialVM.FeedbackText;

                        objTestimonial.ModifiedBy = LoggedInUserId;
                        objTestimonial.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        tbl_Testimonial objTestimonial = new tbl_Testimonial();
                        objTestimonial.CompanyName = testimonialVM.CompanyName;
                        objTestimonial.CompanyPersonName = testimonialVM.CompanyPersonName;
                        objTestimonial.FeedbackText = testimonialVM.FeedbackText;
                        objTestimonial.IsActive = true;
                        objTestimonial.CreatedBy = LoggedInUserId;
                        objTestimonial.CreatedDate = DateTime.UtcNow;
                        objTestimonial.ModifiedBy = LoggedInUserId;
                        objTestimonial.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_Testimonial.Add(objTestimonial);
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

            return View(testimonialVM);
        }

        [HttpPost]
        public string DeleteTestimonial(int Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Testimonial objTestimonial = _db.tbl_Testimonial.Where(x => x.TestimonialId == Id).FirstOrDefault();

                if (objTestimonial == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    _db.tbl_Testimonial.Remove(objTestimonial);
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
                tbl_Testimonial objTestimonial = _db.tbl_Testimonial.Where(x => x.TestimonialId == Id).FirstOrDefault();

                if (objTestimonial != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objTestimonial.IsActive = true;
                    }
                    else
                    {
                        objTestimonial.IsActive = false;
                    }

                    objTestimonial.ModifiedBy = LoggedInUserId;
                    objTestimonial.ModifiedDate = DateTime.UtcNow;

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