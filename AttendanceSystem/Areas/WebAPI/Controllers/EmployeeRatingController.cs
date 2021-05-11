using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/EmployeeRating")]
    public class EmployeeRatingController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public EmployeeRatingController()
        {
            _db = new AttendanceSystemEntities();
        }

        [HttpPost]
        [Route("List")]
        public ResponseDataModel<List<EmployeeRatingVM>> List(CommonEmployeeRatingFilterVM EmployeeRatingFilterVM)
        {
            ResponseDataModel<List<EmployeeRatingVM>> response = new ResponseDataModel<List<EmployeeRatingVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long employeeRole = base.UTI.RoleId;
                long companyId = base.UTI.CompanyId;

                if (EmployeeRatingFilterVM.StartMonth < (int)Months.StartMonth || EmployeeRatingFilterVM.EndMonth < (int)Months.StartMonth)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MonthShouldBeFrom1To12);
                }

                if (EmployeeRatingFilterVM.StartMonth > (int)Months.EndMonth || EmployeeRatingFilterVM.EndMonth > (int)Months.EndMonth)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MonthShouldBeFrom1To12);
                }

                if (EmployeeRatingFilterVM.StartYear > DateTime.Now.Year || EmployeeRatingFilterVM.EndYear > DateTime.Now.Year)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.FutureYearNotAllowed);
                }
                List<EmployeeRatingVM> EmployeeRatingList = (from er in _db.tbl_EmployeeRating
                                                             join emp in _db.tbl_Employee on er.EmployeeId equals emp.EmployeeId
                                                             where !emp.IsDeleted &&
                                                            (employeeRole == (int)AdminRoles.CompanyAdmin ? emp.CompanyId == companyId : emp.EmployeeId == employeeId)
                                                             && er.RateMonth >= EmployeeRatingFilterVM.StartMonth
                                                             && er.RateYear >= EmployeeRatingFilterVM.StartYear
                                                             && er.RateMonth <= EmployeeRatingFilterVM.EndMonth
                                                             && er.RateYear <= EmployeeRatingFilterVM.EndYear
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
                                                             }).OrderByDescending(x => x.RateMonth).ToList();

                response.Data = EmployeeRatingList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("Detail/{id}")]
        public ResponseDataModel<EmployeeRatingVM> Detail(long id)
        {
            ResponseDataModel<EmployeeRatingVM> response = new ResponseDataModel<EmployeeRatingVM>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                EmployeeRatingVM EmployeeRatingDetails = (from er in _db.tbl_EmployeeRating
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
                                                          }).FirstOrDefault();

                response.Data = EmployeeRatingDetails;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("Top10Avg")]
        public ResponseDataModel<List<EmployeeRatingVM>> Top10Avg(CommonEmployeeRatingFilterVM EmployeeRatingFilterVM)
        {
            ResponseDataModel<List<EmployeeRatingVM>> response = new ResponseDataModel<List<EmployeeRatingVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long employeeRole = base.UTI.RoleId;
                long companyId = base.UTI.CompanyId;

                List<EmployeeRatingVM> EmployeeRatingList = (from er in _db.tbl_EmployeeRating
                                                             join emp in _db.tbl_Employee on er.EmployeeId equals emp.EmployeeId
                                                             where !emp.IsDeleted &&
                                                            (employeeRole == (int)AdminRoles.CompanyAdmin ? emp.CompanyId == companyId : emp.EmployeeId == employeeId)
                                                             && er.RateMonth >= EmployeeRatingFilterVM.StartMonth
                                                             && er.RateYear >= EmployeeRatingFilterVM.StartYear
                                                             && er.RateMonth <= EmployeeRatingFilterVM.EndMonth
                                                             && er.RateYear <= EmployeeRatingFilterVM.EndYear
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
                                                             }).OrderByDescending(x => (x.BehaviourRate+x.RegularityRate+x.WorkRate)/3).Take(10).ToList();

                response.Data = EmployeeRatingList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }
    }
}
