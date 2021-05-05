using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/Holiday")]
    public class HolidayController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public HolidayController()
        {
            _db = new AttendanceSystemEntities();
        }

        [HttpGet]
        [Route("List")]
        public ResponseDataModel<List<HolidayVM>> List(HolidayFilterVM holidayFilterVM)
        {
            ResponseDataModel<List<HolidayVM>> response = new ResponseDataModel<List<HolidayVM>>();
            response.IsError = false;

            try
            {
                long companyid = base.UTI.CompanyId;

                List<HolidayVM> HolidayList = (from hd in _db.tbl_Holiday
                                               where !hd.IsDeleted && hd.CompanyId == companyid.ToString()
                                               && hd.StartDate >= holidayFilterVM.StartDate
                                               && hd.StartDate <= holidayFilterVM.EndDate
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

                response.Data = HolidayList;
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
        public ResponseDataModel<HolidayVM> Detail(long id)
        {
            ResponseDataModel<HolidayVM> response = new ResponseDataModel<HolidayVM>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                HolidayVM holidayDetails = (from hd in _db.tbl_Holiday
                                        where !hd.IsDeleted
                                        && hd.HolidayId == id
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

                response.Data = holidayDetails;
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
