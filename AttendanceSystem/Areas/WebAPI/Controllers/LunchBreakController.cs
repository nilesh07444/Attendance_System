using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class LunchBreakController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        public LunchBreakController()
        {
            _db = new AttendanceSystemEntities();
        }
        
        [HttpPost]
        [Route("Add")]
        public ResponseDataModel<bool> Add(LunchBreakLocationVM lunchbreakVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            DateTime today = CommonMethod.CurrentIndianDateTime();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;
                bool IsStartLunchMode = false;
                int NoOfLunchBreakAllowed = 1;

                #region Get Lunch Info

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                if (objCompany != null)
                {
                    NoOfLunchBreakAllowed = objCompany.NoOfLunchBreakAllowed != null ? objCompany.NoOfLunchBreakAllowed.Value : 1;
                }

                tbl_EmployeeLunchBreak existLunch = _db.tbl_EmployeeLunchBreak.Where(x => x.EmployeeId == employeeId).OrderByDescending(x => x.EmployeeLunchBreakId).FirstOrDefault();
                if (existLunch == null || existLunch.EndDateTime != null)
                {
                    IsStartLunchMode = true;
                }

                // Get today's taken lunch breaks of employee
                List<tbl_EmployeeLunchBreak> lstExistLunchs = _db.tbl_EmployeeLunchBreak.Where(x => x.EmployeeId == employeeId && DbFunctions.TruncateTime(x.StartDateTime) == DbFunctions.TruncateTime(today)).ToList();

                #endregion

                #region Validation

                if (IsStartLunchMode && lstExistLunchs != null && lstExistLunchs.Count >= NoOfLunchBreakAllowed)
                {
                    response.IsError = true;
                    response.AddError("Your Break Limit Reached, so you can not take lunch break more for today.");
                }

                #endregion Validation

                if (!response.IsError)
                {
                     
                    // Add or Update Lunch Break

                    if (IsStartLunchMode)
                    {
                        // Add new lunch

                        tbl_EmployeeLunchBreak lunchObject = new tbl_EmployeeLunchBreak();
                        lunchObject.EmployeeId = base.UTI.EmployeeId;

                        lunchObject.StartDateTime = CommonMethod.CurrentIndianDateTime();
                        lunchObject.StartLunchLatitude = lunchbreakVM.Latitude;
                        lunchObject.StartLunchLongitude = lunchbreakVM.Longitude;
                        lunchObject.StartLunchLocationFrom = lunchbreakVM.LocationFrom;

                        _db.tbl_EmployeeLunchBreak.Add(lunchObject);
                    }
                    else
                    {
                        // Update with End Lunch
                        existLunch.EndDateTime = CommonMethod.CurrentIndianDateTime();
                        existLunch.EndLunchLatitude = lunchbreakVM.Latitude;
                        existLunch.EndLunchLongitude = lunchbreakVM.Longitude;
                        existLunch.EndLunchLocationFrom = lunchbreakVM.LocationFrom;
                    }

                    _db.SaveChanges();

                    response.Data = true;
                }

            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }
          
        [Route("List"), HttpGet]
        public ResponseDataModel<List<EmployeeLunchBreakVM>> List()
        {
            ResponseDataModel<List<EmployeeLunchBreakVM>> response = new ResponseDataModel<List<EmployeeLunchBreakVM>>();
            List<EmployeeLunchBreakVM> lunchBreakVM = new List<EmployeeLunchBreakVM>();
            DateTime today = CommonMethod.CurrentIndianDateTime();

            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                lunchBreakVM = (from lunch in _db.tbl_EmployeeLunchBreak
                                join emp in _db.tbl_Employee on lunch.EmployeeId equals emp.EmployeeId
                                join role in _db.mst_AdminRole on emp.AdminRoleId equals role.AdminRoleId
                                where !emp.IsDeleted && emp.CompanyId == companyId && emp.EmployeeId == employeeId
                                && DbFunctions.TruncateTime(lunch.StartDateTime) == DbFunctions.TruncateTime(today)
                                select new EmployeeLunchBreakVM
                                {
                                    EmployeeLunchBreakId = lunch.EmployeeLunchBreakId,
                                    EmployeeId = lunch.EmployeeId,
                                    EmployeeName = emp.Prefix + " " + emp.FirstName + " " + emp.LastName,
                                    EmployeeCode = emp.EmployeeCode,
                                    StartDateTime = lunch.StartDateTime,
                                    StartLunchLocationFrom = lunch.StartLunchLocationFrom,
                                    StartLunchLatitude = lunch.StartLunchLatitude,
                                    StartLunchLongitude = lunch.StartLunchLongitude,

                                    EndDateTime = lunch.EndDateTime,
                                    EndLunchLocationFrom = lunch.EndLunchLocationFrom,
                                    EndLunchLatitude = lunch.EndLunchLatitude,
                                    EndLunchLongitude = lunch.EndLunchLongitude,

                                    EmployeeRole = role.AdminRoleName
                                }).OrderByDescending(x => x.StartDateTime).ToList();

                response.Data = lunchBreakVM;
                response.IsError = false;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message.ToString());
            }

            return response;
        }

    }
}