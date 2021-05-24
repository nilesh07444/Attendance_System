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
    public class EmployeeRatingController : Controller
    {
        AttendanceSystemEntities _db;
        // GET: Admin/EmployeeRating
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
                                                             && er.RateMonth >= EmployeeRatingFilterVM.StartMonth
                                                             && er.RateMonth <= EmployeeRatingFilterVM.EndMonth
                                                             && er.RateYear == EmployeeRatingFilterVM.Year
                                                             && (EmployeeRatingFilterVM.UserRole.HasValue ? emp.AdminRoleId == EmployeeRatingFilterVM.UserRole.Value : true)
                                                             && (!string.IsNullOrEmpty(EmployeeRatingFilterVM.EmployeeCode) ? emp.EmployeeCode == EmployeeRatingFilterVM.EmployeeCode : true)
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
                                                             }).OrderByDescending(x => x.EmployeeRatingId).ToList();
            }
            catch (Exception ex)
            {
            }
            EmployeeRatingFilterVM.UserRoleList = GetUserRoleList();
            EmployeeRatingFilterVM.CalenderMonth = CommonMethod.GetCalenderMonthList();
            return View(EmployeeRatingFilterVM);
        }

        public ActionResult Add()
        {
            EmployeeRatingVM EmployeeRatingVM = new EmployeeRatingVM();
            EmployeeRatingVM.EmployeeList = GetEmployeeList();
            return View(EmployeeRatingVM);
        }

        [HttpPost]
        public ActionResult Add(EmployeeRatingVM employeeRatingVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {

                    bool isExist = _db.tbl_EmployeeRating.Any(x => x.EmployeeId == employeeRatingVM.EmployeeId && x.RateYear == employeeRatingVM.RateYear && x.RateMonth == employeeRatingVM.RateMonth);
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

                        tbl_EmployeeRating objEmployeeRating = new tbl_EmployeeRating();
                        objEmployeeRating.EmployeeId = employeeRatingVM.EmployeeId;
                        objEmployeeRating.RateMonth = employeeRatingVM.RateMonth;
                        objEmployeeRating.RateYear = employeeRatingVM.RateYear;
                        objEmployeeRating.BehaviourRate = employeeRatingVM.BehaviourRate;
                        objEmployeeRating.RegularityRate = employeeRatingVM.RegularityRate;
                        objEmployeeRating.WorkRate = employeeRatingVM.WorkRate;
                        objEmployeeRating.Remarks = employeeRatingVM.Remarks;
                        objEmployeeRating.CreatedBy = LoggedInUserId;
                        objEmployeeRating.CreatedDate = DateTime.UtcNow;
                        objEmployeeRating.ModifiedBy = LoggedInUserId;
                        objEmployeeRating.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_EmployeeRating.Add(objEmployeeRating);

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
                                            Text = emp.FirstName + " " + emp.LastName,
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

    }
}