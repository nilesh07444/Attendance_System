using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{

    public class HolidayController : Controller
    {
        // GET: Admin/Holiday   
        AttendanceSystemEntities _db;
        public HolidayController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index(DateTime? startDate = null, DateTime? endDate = null)
        {
            HolidayFilterVM holidayFilterVM = new HolidayFilterVM();
            try
            {
                if (startDate.HasValue && endDate.HasValue)
                {
                    holidayFilterVM.StartDate = startDate.Value;
                    holidayFilterVM.EndDate = endDate.Value;
                }

                long companyId = clsAdminSession.CompanyId;
                holidayFilterVM.HolidayList = GetHolidatList(holidayFilterVM);
            }
            catch (Exception ex)
            {
            }
            return View(holidayFilterVM);
        }

        public ActionResult Add(long id)
        {
            HolidayVM HolidayVM = new HolidayVM();
            if (id > 0)
            {
                HolidayVM = (from hd in _db.tbl_Holiday
                             where hd.HolidayId == id && !hd.IsDeleted
                             select new HolidayVM
                             {
                                 HolidayId = hd.HolidayId,
                                 HolidayDate = hd.HolidayDate,
                                 HolidayReason = hd.HolidayReason,
                                 CompanyId = hd.CompanyId,
                                 IsActive = hd.IsActive,
                                 IsDeleted = hd.IsDeleted
                             }).FirstOrDefault();
            }

            return View(HolidayVM);
        }

        [HttpPost]
        public ActionResult Add(HolidayVM HolidayVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;
                    if (HolidayVM.HolidayId > 0)
                    {
                        tbl_Holiday objHoliday = _db.tbl_Holiday.Where(x => x.HolidayId == HolidayVM.HolidayId).FirstOrDefault();
                        objHoliday.HolidayDate = HolidayVM.HolidayDate;
                        objHoliday.HolidayReason = HolidayVM.HolidayReason;
                        objHoliday.ModifiedBy = LoggedInUserId;
                        objHoliday.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        tbl_Holiday objHoliday = new tbl_Holiday();
                        objHoliday.HolidayDate = HolidayVM.HolidayDate;
                        objHoliday.HolidayReason = HolidayVM.HolidayReason;
                        objHoliday.IsActive = true;
                        objHoliday.CompanyId = companyId.ToString();
                        objHoliday.CreatedBy = LoggedInUserId;
                        objHoliday.CreatedDate = DateTime.UtcNow;
                        objHoliday.ModifiedBy = LoggedInUserId;
                        objHoliday.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_Holiday.Add(objHoliday);
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

            return View(HolidayVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_Holiday objHoliday = _db.tbl_Holiday.Where(x => x.HolidayId == Id).FirstOrDefault();

                if (objHoliday != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objHoliday.IsActive = true;
                    }
                    else
                    {
                        objHoliday.IsActive = false;
                    }

                    objHoliday.ModifiedBy = LoggedInUserId;
                    objHoliday.ModifiedDate = DateTime.UtcNow;

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
        public string DeleteHoliday(int HolidayId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_Holiday objHoliday = _db.tbl_Holiday.Where(x => x.HolidayId == HolidayId).FirstOrDefault();

                if (objHoliday == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objHoliday.IsDeleted = true;
                    objHoliday.ModifiedBy = LoggedInUserId;
                    objHoliday.ModifiedDate = DateTime.UtcNow;
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

        public JsonResult CheckHolidayDate(DateTime date)
        {
            bool isExist = false;
            try
            {
                long companyId = clsAdminSession.CompanyId;
                isExist = _db.tbl_Holiday.Any(x => x.CompanyId == companyId.ToString() && !x.IsDeleted && x.HolidayDate == date);
            }
            catch (Exception ex)
            {
                isExist = false;
            }

            return Json(new { Status = isExist }, JsonRequestBehavior.AllowGet);
        }

        //public ActionResult SearchHoliday(DateTime startDate, DateTime endDate)
        //{
        //    HolidayFilterVM holidayFilterVM = new HolidayFilterVM();
        //    try
        //    {
        //        holidayFilterVM.StartDate = startDate;
        //        holidayFilterVM.EndDate = endDate;
        //        holidayFilterVM.HolidayList = GetHolidatList(holidayFilterVM);
        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //    return View("Index", holidayFilterVM);
        //}

        private List<HolidayVM> GetHolidatList(HolidayFilterVM holidayFilterVM)
        {
            long companyId = clsAdminSession.CompanyId;
            List<HolidayVM> holidayList = (from hd in _db.tbl_Holiday
                                           where !hd.IsDeleted && hd.CompanyId == companyId.ToString()
                                           && hd.HolidayDate >= holidayFilterVM.StartDate
                                           && hd.HolidayDate <= holidayFilterVM.EndDate
                                           select new HolidayVM
                                           {
                                               HolidayId = hd.HolidayId,
                                               HolidayDate = hd.HolidayDate,
                                               HolidayReason = hd.HolidayReason,
                                               CompanyId = hd.CompanyId,
                                               IsActive = hd.IsActive,
                                               IsDeleted = hd.IsDeleted
                                           }).OrderByDescending(x => x.HolidayDate).ToList();

            return holidayList;
        }
    }
}