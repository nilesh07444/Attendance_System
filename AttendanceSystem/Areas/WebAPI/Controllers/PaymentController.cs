﻿using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/payment")]
    public class PaymentController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public PaymentController()
        {
            _db = new AttendanceSystemEntities();
        }


        [HttpGet]
        [Route("List")]
        public ResponseDataModel<List<PaymentVM>> List(PaymentFilterVM paymentFilterVM)
        {
            ResponseDataModel<List<PaymentVM>> response = new ResponseDataModel<List<PaymentVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                List<PaymentVM> paymentList = (from emp in _db.tbl_EmployeePayment
                                             where !emp.IsDeleted && emp.UserId == employeeId
                                             && emp.PaymentDate >= paymentFilterVM.StartDate && emp.PaymentDate <= paymentFilterVM.EndDate
                                             select new PaymentVM
                                             {

                                                 EmployeePaymentId = emp.EmployeePaymentId,
                                                 UserId = emp.UserId,
                                                 PaymentDate = emp.PaymentDate,
                                                 Amount = emp.Amount,
                                                 PaymentType = emp.PaymentType,

                                             }).OrderByDescending(x => x.EmployeePaymentId).ToList();

                response.Data = paymentList;
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
        public ResponseDataModel<PaymentVM> Detail(long id)
        {
            ResponseDataModel<PaymentVM> response = new ResponseDataModel<PaymentVM>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                PaymentVM paymentDetails = (from emp in _db.tbl_EmployeePayment
                                          where !emp.IsDeleted && emp.UserId == employeeId
                                          && emp.EmployeePaymentId == id
                                          select new PaymentVM
                                          {

                                              EmployeePaymentId = emp.EmployeePaymentId,
                                              UserId = emp.UserId,
                                              PaymentDate = emp.PaymentDate,
                                              Amount = emp.Amount,
                                              PaymentType = emp.PaymentType,

                                          }).FirstOrDefault();

                response.Data = paymentDetails;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("Add")]
        public ResponseDataModel<bool> Add(PaymentVM paymentVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                if (paymentVM.UserId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmployeeIdIsNotValid);
                }

                if (paymentVM.UserId > 0)
                {
                    bool isWorkerExist = _db.tbl_Employee.Any(x => x.EmployeeId != paymentVM.UserId && !x.IsDeleted && x.AdminRoleId != (int)AdminRoles.Worker && x.CompanyId == companyId);
                    if (isWorkerExist)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.EmployeeDoesNotExist);
                    }

                    if (!response.IsError)
                    {

                        tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                        objEmployeePayment.UserId = paymentVM.UserId;
                        objEmployeePayment.PaymentDate = paymentVM.PaymentDate;
                        objEmployeePayment.Amount = paymentVM.Amount;
                        objEmployeePayment.PaymentType = paymentVM.PaymentType;
                        objEmployeePayment.CreatedBy = employeeId;
                        objEmployeePayment.CreatedDate = DateTime.UtcNow;
                        objEmployeePayment.ModifiedBy = employeeId;
                        objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_EmployeePayment.Add(objEmployeePayment);
                        _db.SaveChanges();
                        response.Data = true;
                    }
                }
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
