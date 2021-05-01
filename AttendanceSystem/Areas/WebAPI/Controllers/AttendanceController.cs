﻿using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class AttendanceController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public AttendanceController()
        {
            _db = new AttendanceSystemEntities();
        }

        [HttpPost]
        [Route("AddAttendance")]
        public ResponseDataModel<bool> AddAttendance(LeaveVM leaveVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                #region Validation
                if (leaveVM.StartDate == null || leaveVM.EndDate == null)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.LeaveDateRequired);
                }

                if (leaveVM.StartDate < DateTime.Now.Date || leaveVM.EndDate < DateTime.Now.Date)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.LeaveDateCanBeFutureDateOnly);
                }

                if (leaveVM.EndDate < leaveVM.StartDate)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.StartDateCanNotBeGreaterThanEndDate);
                }

                if (string.IsNullOrEmpty(leaveVM.LeaveReason))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.LeaveReasonRequired);
                }

                bool isLeaveExist = _db.tbl_Leave.Any(x => !x.IsDeleted && x.LeaveStatus != (int)LeaveStatus.Reject && x.StartDate >= leaveVM.StartDate && x.StartDate <= leaveVM.EndDate);
                if (isLeaveExist)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.LeaveOnSameDateAlreadyExist);
                }

                #endregion Validation

                tbl_Leave leaveObject = new tbl_Leave();
                leaveObject.UserId = base.UTI.EmployeeId;
                leaveObject.StartDate = leaveVM.StartDate;
                leaveObject.EndDate = leaveVM.EndDate;
                leaveObject.NoOfDays = Convert.ToDecimal((leaveVM.EndDate - leaveVM.StartDate).TotalDays + 1);
                leaveObject.LeaveStatus = (int)LeaveStatus.Pending;
                leaveObject.LeaveReason = leaveVM.LeaveReason;
                leaveObject.CreatedBy = base.UTI.EmployeeId;
                leaveObject.CreatedDate = DateTime.UtcNow;
                leaveObject.ModifiedBy = base.UTI.EmployeeId;
                leaveObject.ModifiedDate = DateTime.UtcNow;
                _db.tbl_Leave.Add(leaveObject);
                _db.SaveChanges();
                response.Data = true;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("LeaveList")]
        public ResponseDataModel<List<LeaveVM>> LeaveList(LeaveFilterVM leaveFilterVM)
        {
            ResponseDataModel<List<LeaveVM>> response = new ResponseDataModel<List<LeaveVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                List<LeaveVM> leaveList = (from lv in _db.tbl_Leave
                                           where !lv.IsDeleted
                                           && lv.UserId == employeeId
                                           && lv.StartDate >= leaveFilterVM.StartDate && lv.StartDate <= leaveFilterVM.EndDate
                                           && (leaveFilterVM.LeaveStatus.HasValue ? lv.LeaveStatus == leaveFilterVM.LeaveStatus.Value : true)
                                           select new LeaveVM
                                           {
                                               LeaveId = lv.LeaveId,
                                               EmployeeId = lv.UserId,
                                               StartDate = lv.StartDate,
                                               EndDate = lv.EndDate,
                                               NoOfDays = lv.NoOfDays,
                                               LeaveStatus = lv.LeaveStatus,
                                               RejectReason = lv.RejectReason,
                                               CancelledReason = lv.CancelledReason
                                           }).OrderByDescending(x => x.StartDate).ToList();

                response.Data = leaveList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("LeaveDetails")]
        public ResponseDataModel<LeaveVM> LeaveDetails(long leaveId)
        {
            ResponseDataModel<LeaveVM> response = new ResponseDataModel<LeaveVM>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                LeaveVM leaveDetails = (from lv in _db.tbl_Leave
                                           where !lv.IsDeleted
                                           && lv.LeaveId == leaveId
                                           select new LeaveVM
                                           {
                                               EmployeeId = lv.UserId,
                                               StartDate = lv.StartDate,
                                               EndDate = lv.EndDate,
                                               NoOfDays = lv.NoOfDays,
                                               LeaveStatus = lv.LeaveStatus,
                                               RejectReason = lv.RejectReason,
                                               CancelledReason = lv.CancelledReason
                                           }).FirstOrDefault();

                response.Data = leaveDetails;
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
