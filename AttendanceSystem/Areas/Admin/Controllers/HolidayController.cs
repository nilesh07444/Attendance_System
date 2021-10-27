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
    public class HolidayController : Controller
    {
        // GET: Admin/Holiday   
        AttendanceSystemEntities _db;
        public HolidayController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index(int? startMonth = null, int? endMonth = null, int? year = null)
        {
            HolidayFilterVM holidayFilterVM = new HolidayFilterVM();
            try
            {
                if (startMonth.HasValue && endMonth.HasValue)
                {
                    holidayFilterVM.StartMonth = startMonth.Value;
                    holidayFilterVM.EndMonth = endMonth.Value;
                }

                if (year.HasValue)
                {
                    holidayFilterVM.Year = year.Value;
                }

                long companyId = clsAdminSession.CompanyId;
                holidayFilterVM.HolidayList = GetHolidatList(holidayFilterVM);
                holidayFilterVM.CalenderMonth = CommonMethod.GetCalenderMonthList();
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
                                 StartDate = hd.StartDate,
                                 EndDate = hd.EndDate,
                                 HolidayReason = hd.HolidayReason,
                                 Remark = hd.Remark,
                                 CompanyId = hd.CompanyId,
                                 IsActive = hd.IsActive,
                                 IsDeleted = hd.IsDeleted
                             }).FirstOrDefault();
            }
            else
            {
                HolidayVM.StartDate = System.DateTime.Today;
                HolidayVM.EndDate = System.DateTime.Today;
            }

            return View(HolidayVM);
        }

        [HttpPost]
        public ActionResult Add(HolidayVM HolidayVM)
        {
            try
            {
                long companyId = clsAdminSession.CompanyId;
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    if (HolidayVM.StartDate < today || HolidayVM.EndDate < today)
                    {
                        ModelState.AddModelError("", ErrorMessage.HolidayShouldNotBePastDays);
                        return View(HolidayVM);
                    }

                    if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == HolidayVM.StartDate.Month && x.Year == HolidayVM.StartDate.Year && (x.IsEmployeeDone || x.IsWorkerDone)))
                    {
                        ModelState.AddModelError("", ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyHoliday);
                        return View(HolidayVM);
                    }

                    bool isHolidayExist = CheckHolidayDate(HolidayVM.StartDate, HolidayVM.EndDate, HolidayVM.HolidayId);
                    if (isHolidayExist)
                    {
                        ModelState.AddModelError("", ErrorMessage.HolidayOnSameDateAlreadyExist);
                        return View(HolidayVM);
                    }

                    if (HolidayVM.StartDate.Month != HolidayVM.EndDate.Month)
                    {
                        ModelState.AddModelError("", ErrorMessage.HolidayStartAndEndDateShouldBeforSameMonth);
                        return View(HolidayVM);
                    }

                    if (HolidayVM.StartDate.Year != HolidayVM.EndDate.Year)
                    {
                        ModelState.AddModelError("", ErrorMessage.HolidayStartAndEndDateShouldBeforSameMonth);
                        return View(HolidayVM);
                    }

                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());

                    if (HolidayVM.HolidayId > 0)
                    {
                        tbl_Holiday objHoliday = _db.tbl_Holiday.Where(x => x.HolidayId == HolidayVM.HolidayId).FirstOrDefault();

                        if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == objHoliday.StartDate.Month && x.Year == objHoliday.StartDate.Year && (x.IsEmployeeDone || x.IsWorkerDone)))
                        {
                            ModelState.AddModelError("", ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyHoliday);
                            return View(HolidayVM);
                        }
                        objHoliday.StartDate = HolidayVM.StartDate;
                        objHoliday.EndDate = HolidayVM.EndDate;
                        objHoliday.Remark = HolidayVM.Remark;
                        objHoliday.HolidayReason = HolidayVM.HolidayReason;
                        objHoliday.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objHoliday.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        tbl_Holiday objHoliday = new tbl_Holiday();
                        objHoliday.StartDate = HolidayVM.StartDate;
                        objHoliday.EndDate = HolidayVM.EndDate;
                        objHoliday.HolidayReason = HolidayVM.HolidayReason;
                        objHoliday.Remark = HolidayVM.Remark;
                        objHoliday.IsActive = true;
                        objHoliday.CompanyId = companyId.ToString();
                        objHoliday.CreatedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objHoliday.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objHoliday.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objHoliday.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_Holiday.Add(objHoliday);
                    }

                    List<tbl_Leave> leaveList = (from lv in _db.tbl_Leave
                                                 join emp in _db.tbl_Employee on lv.UserId equals emp.EmployeeId
                                                 where emp.CompanyId == companyId
                                                 && (lv.StartDate >= HolidayVM.StartDate && lv.StartDate <= HolidayVM.EndDate
                                                 || lv.EndDate >= HolidayVM.StartDate && lv.EndDate <= HolidayVM.EndDate
                                                 || HolidayVM.StartDate >= lv.StartDate && HolidayVM.StartDate <= lv.EndDate
                                                 || HolidayVM.EndDate >= lv.StartDate && HolidayVM.EndDate <= lv.EndDate
                                                 )
                                                 select lv).ToList();

                    leaveList.ForEach(x => {
                        _db.tbl_Leave.Remove(x);
                    });

                    _db.SaveChanges();
                }
                else
                {
                    return View(HolidayVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return RedirectToAction("Index");
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

                    objHoliday.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                    objHoliday.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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
            long companyId = clsAdminSession.CompanyId;
            try
            {
               
                tbl_Holiday objHoliday = _db.tbl_Holiday.Where(x => x.HolidayId == HolidayId).FirstOrDefault();

                if (objHoliday == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == objHoliday.StartDate.Month && x.Year == objHoliday.StartDate.Year && (x.IsEmployeeDone || x.IsWorkerDone)))
                    {
                        ReturnMessage = "monthlyconvesrion";
                    }
                    else
                    {
                        long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                        objHoliday.IsDeleted = true;
                        objHoliday.ModifiedBy = (int)PaymentGivenBy.CompanyAdmin;
                        objHoliday.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.SaveChanges();

                        ReturnMessage = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        private bool CheckHolidayDate(DateTime startDate, DateTime endDate, long holidayId)
        {
            bool isExist = false;
            try
            {
                long companyId = clsAdminSession.CompanyId;
                isExist = _db.tbl_Holiday.Any(x => x.CompanyId == companyId.ToString() && !x.IsDeleted
                && (holidayId > 0 ? x.HolidayId != holidayId : true)
                && (
                (startDate >= x.StartDate && endDate <= x.EndDate) ||
                (startDate >= x.StartDate && startDate <= x.EndDate) ||
                (endDate >= x.StartDate && endDate <= x.EndDate)
                ));
            }
            catch (Exception ex)
            {
                isExist = false;
            }

            return isExist;
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
                                           && (holidayFilterVM.StartMonth > 0 && holidayFilterVM.EndMonth > 0 ? (
                                           hd.StartDate.Month >= holidayFilterVM.StartMonth
                                           && hd.StartDate.Month <= holidayFilterVM.EndMonth) : true)
                                           && hd.StartDate.Year == holidayFilterVM.Year
                                           select new HolidayVM
                                           {
                                               HolidayId = hd.HolidayId,
                                               StartDate = hd.StartDate,
                                               EndDate = hd.EndDate,
                                               HolidayReason = hd.HolidayReason,
                                               Remark = hd.Remark,
                                               CompanyId = hd.CompanyId,
                                               IsActive = hd.IsActive,
                                               IsDeleted = hd.IsDeleted
                                           }).OrderByDescending(x => x.StartDate).ToList();

            return holidayList;
        }
    }
}