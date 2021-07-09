using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class EmployeeRatingController : Controller
    {
        AttendanceSystemEntities _db;

        public EmployeeRatingController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index(int? startMonth = null, int? endMonth = null, int? year = null, int? userRole = null, string employeeCode = null)
        {
            EmployeeRatingFilterVM EmployeeRatingFilterVM = new EmployeeRatingFilterVM();
            if (userRole.HasValue)
            {
                EmployeeRatingFilterVM.UserRole = userRole.Value;
            }

            if (startMonth.HasValue && endMonth.HasValue)
            {
                EmployeeRatingFilterVM.StartMonth = startMonth.Value;
                EmployeeRatingFilterVM.EndMonth = endMonth.Value;
            }

            if (year.HasValue)
            {
                EmployeeRatingFilterVM.Year = year.Value;
            }

            if (!string.IsNullOrEmpty(employeeCode))
            {
                EmployeeRatingFilterVM.EmployeeCode = employeeCode;
            }

            try
            {
                long companyId = clsAdminSession.CompanyId;
                EmployeeRatingFilterVM.EmployeeRatingList = (from er in _db.tbl_EmployeeRating
                                                             join emp in _db.tbl_Employee on er.EmployeeId equals emp.EmployeeId
                                                             where !emp.IsDeleted && emp.CompanyId == companyId
                                                             && (EmployeeRatingFilterVM.StartMonth > 0 && EmployeeRatingFilterVM.EndMonth > 0 ? (er.RateMonth >= EmployeeRatingFilterVM.StartMonth
                                                             && er.RateMonth <= EmployeeRatingFilterVM.EndMonth) : true)
                                                             && er.RateYear == EmployeeRatingFilterVM.Year
                                                             && (EmployeeRatingFilterVM.UserRole.HasValue ? emp.AdminRoleId == EmployeeRatingFilterVM.UserRole.Value : true)
                                                             && (!string.IsNullOrEmpty(EmployeeRatingFilterVM.EmployeeCode) ? emp.EmployeeCode == EmployeeRatingFilterVM.EmployeeCode : true)
                                                             select new EmployeeRatingVM
                                                             {

                                                                 EmployeeRatingId = er.EmployeeRatingId,
                                                                 EmployeeId = er.EmployeeId,
                                                                 EmployeeCode = emp.EmployeeCode,
                                                                 EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                                                 RateMonth = er.RateMonth,
                                                                 RateYear = er.RateYear,
                                                                 BehaviourRate = er.BehaviourRate,
                                                                 RegularityRate = er.RegularityRate,
                                                                 WorkRate = er.WorkRate,
                                                                 Remarks = er.Remarks,
                                                                 CreatedDate = er.CreatedDate,
                                                                 AvgRate = SqlFunctions.StringConvert((new decimal[] { er.BehaviourRate, er.RegularityRate, er.WorkRate }).Average(), 4, 2)
                                                             }).OrderByDescending(x => x.EmployeeRatingId).ToList();

                EmployeeRatingFilterVM.EmployeeRatingList.ForEach(x =>
                {
                    x.RateMonthText = CommonMethod.GetEnumDescription((CalenderMonths)x.RateMonth);
                });
            }
            catch (Exception ex)
            {
            }
            EmployeeRatingFilterVM.UserRoleList = GetUserRoleList();
            EmployeeRatingFilterVM.CalenderMonth = CommonMethod.GetCalenderMonthList();
            return View(EmployeeRatingFilterVM);
        }

        public ActionResult Add(long id)
        {
            EmployeeRatingVM employeeRatingVM = new EmployeeRatingVM();

            if (id > 0)
            {
                employeeRatingVM = (from er in _db.tbl_EmployeeRating
                                    where er.EmployeeRatingId == id
                                    select new EmployeeRatingVM
                                    {
                                        EmployeeRatingId = er.EmployeeRatingId,
                                        EmployeeId = er.EmployeeId,
                                        RateMonth = er.RateMonth,
                                        RateYear = er.RateYear,
                                        BehaviourRate = er.BehaviourRate,
                                        RegularityRate = er.RegularityRate,
                                        WorkRate = er.WorkRate,
                                        Remarks = er.Remarks,
                                        CreatedDate = er.CreatedDate
                                    }).FirstOrDefault();
            }
            employeeRatingVM.EmployeeList = GetEmployeeList();
            return View(employeeRatingVM);
        }

        [HttpPost]
        public ActionResult Add(EmployeeRatingVM employeeRatingVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    bool isExist = _db.tbl_EmployeeRating.Any(x => x.EmployeeId == employeeRatingVM.EmployeeId
                    && x.RateYear == employeeRatingVM.RateYear
                    && x.RateMonth == employeeRatingVM.RateMonth
                    && (employeeRatingVM.EmployeeRatingId > 0 ? x.EmployeeRatingId != employeeRatingVM.EmployeeRatingId : true)
                    );
                    if (isExist)
                    {
                        ModelState.AddModelError(" ", ErrorMessage.EmployeeRatingAlreadyExist);
                        employeeRatingVM.EmployeeList = GetEmployeeList();
                        return View(employeeRatingVM);
                    }
                    else
                    {
                        long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                        long companyId = Int64.Parse(clsAdminSession.CompanyId.ToString());

                        if (employeeRatingVM.EmployeeRatingId > 0)
                        {
                            tbl_EmployeeRating objEmployeeRating = _db.tbl_EmployeeRating.Where(x => x.EmployeeRatingId == employeeRatingVM.EmployeeRatingId).FirstOrDefault();
                            if ((CommonMethod.CurrentIndianDateTime() - objEmployeeRating.CreatedDate).TotalDays > 7)
                            {
                                ModelState.AddModelError(" ", ErrorMessage.EmployeeRatingCanNotModifyAfter7Days);
                                employeeRatingVM.EmployeeList = GetEmployeeList();
                                return View(employeeRatingVM);
                            }
                            objEmployeeRating.RateMonth = employeeRatingVM.RateMonth;
                            objEmployeeRating.RateYear = employeeRatingVM.RateYear;
                            objEmployeeRating.BehaviourRate = employeeRatingVM.BehaviourRate;
                            objEmployeeRating.RegularityRate = employeeRatingVM.RegularityRate;
                            objEmployeeRating.WorkRate = employeeRatingVM.WorkRate;
                            objEmployeeRating.Remarks = employeeRatingVM.Remarks;
                            objEmployeeRating.ModifiedBy = LoggedInUserId;
                            objEmployeeRating.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        }
                        else
                        {
                            tbl_EmployeeRating objEmployeeRating = new tbl_EmployeeRating();
                            objEmployeeRating.EmployeeId = employeeRatingVM.EmployeeId;
                            objEmployeeRating.RateMonth = employeeRatingVM.RateMonth;
                            objEmployeeRating.RateYear = employeeRatingVM.RateYear;
                            objEmployeeRating.BehaviourRate = employeeRatingVM.BehaviourRate;
                            objEmployeeRating.RegularityRate = employeeRatingVM.RegularityRate;
                            objEmployeeRating.WorkRate = employeeRatingVM.WorkRate;
                            objEmployeeRating.Remarks = employeeRatingVM.Remarks;
                            objEmployeeRating.CreatedBy = LoggedInUserId;
                            objEmployeeRating.CreatedDate = CommonMethod.CurrentIndianDateTime();
                            objEmployeeRating.ModifiedBy = LoggedInUserId;
                            objEmployeeRating.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                            _db.tbl_EmployeeRating.Add(objEmployeeRating);
                        }
                        _db.SaveChanges();
                    }
                }
                else
                {
                    employeeRatingVM.EmployeeList = GetEmployeeList();
                    return View(employeeRatingVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return RedirectToAction("Index");
        }


        private List<SelectListItem> GetUserRoleList()
        {
            long[] adminRole = new long[] { (long)AdminRoles.CompanyAdmin, (long)AdminRoles.SuperAdmin };
            List<SelectListItem> lst = (from ms in _db.mst_AdminRole
                                        where !adminRole.Contains(ms.AdminRoleId)
                                        select new SelectListItem
                                        {
                                            Text = ms.AdminRoleName,
                                            Value = ms.AdminRoleId.ToString()
                                        }).ToList();
            return lst;
        }



        private List<SelectListItem> GetEmployeeList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

        public ActionResult View(long id)
        {
            EmployeeRatingVM employeeRatingVM = new EmployeeRatingVM();

            employeeRatingVM = (from er in _db.tbl_EmployeeRating
                                join emp in _db.tbl_Employee on er.EmployeeId equals emp.EmployeeId
                                where er.EmployeeRatingId == id
                                select new EmployeeRatingVM
                                {
                                    EmployeeRatingId = er.EmployeeRatingId,
                                    EmployeeId = er.EmployeeId,
                                    EmployeeName = emp.FirstName + " " + emp.LastName,
                                    RateMonth = er.RateMonth,
                                    RateYear = er.RateYear,
                                    BehaviourRate = er.BehaviourRate,
                                    RegularityRate = er.RegularityRate,
                                    WorkRate = er.WorkRate,
                                    Remarks = er.Remarks,
                                    CreatedDate = er.CreatedDate,
                                }).FirstOrDefault();

            employeeRatingVM.RateMonthText = CommonMethod.GetEnumDescription((CalenderMonths)employeeRatingVM.RateMonth);
            return View(employeeRatingVM);
        }

        [HttpPost]
        public string DeleteEmployeeRating(int employeeRatingId)
        {
            string ReturnMessage = "";

            try
            {
                tbl_EmployeeRating objEmployeeRating = _db.tbl_EmployeeRating.Where(x => x.EmployeeRatingId == employeeRatingId).FirstOrDefault();

                if (objEmployeeRating == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    if ((CommonMethod.CurrentIndianDateTime() - objEmployeeRating.CreatedDate).TotalDays > 7)
                    {
                        ReturnMessage = "cannotmodify";
                    }
                    else
                    {

                        _db.tbl_EmployeeRating.Remove(objEmployeeRating);
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
    }
}