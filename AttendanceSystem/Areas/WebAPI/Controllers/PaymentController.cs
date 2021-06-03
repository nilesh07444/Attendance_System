using AttendanceSystem.Helper;
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
        long employeeId;
        long companyId;
        long employeeRoleId;
        public PaymentController()
        {
            _db = new AttendanceSystemEntities();
            employeeId = base.UTI.EmployeeId;
            companyId = base.UTI.CompanyId;
            employeeRoleId = base.UTI.RoleId;
        }

        [HttpPost]
        [Route("OwnPaymentList")]
        public ResponseDataModel<List<PaymentVM>> OwnPaymentList(PaymentFilterVM paymentFilterVM)
        {
            ResponseDataModel<List<PaymentVM>> response = new ResponseDataModel<List<PaymentVM>>();
            response.IsError = false;
            try
            {
                List<PaymentVM> paymentList = (from emp in _db.tbl_EmployeePayment
                                               where !emp.IsDeleted && emp.DebitAmount > 0
                                               && emp.PaymentDate >= paymentFilterVM.StartDate && emp.PaymentDate <= paymentFilterVM.EndDate
                                               && emp.UserId == employeeId
                                               select new PaymentVM
                                               {
                                                   EmployeePaymentId = emp.EmployeePaymentId,
                                                   UserId = emp.UserId,
                                                   PaymentDate = emp.PaymentDate,
                                                   DebitAmount = emp.DebitAmount,
                                                   PaymentType = emp.PaymentType,
                                                   Remarks = emp.Remarks
                                               }).OrderByDescending(x => x.EmployeePaymentId).ToList();

                paymentList.ForEach(x =>
                {
                    x.PaymentTypeText = x.PaymentType.HasValue ? CommonMethod.GetEnumDescription((EmployeePaymentType)x.PaymentType.Value) : string.Empty;
                });
                response.Data = paymentList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("WorkerPaymentList")]
        public ResponseDataModel<List<PaymentVM>> WorkerPaymentList(WorkerPaymentFilterVM workerPaymentFilterVM)
        {
            ResponseDataModel<List<PaymentVM>> response = new ResponseDataModel<List<PaymentVM>>();
            response.IsError = false;
            try
            {
                List<PaymentVM> paymentList = (from emp in _db.tbl_EmployeePayment
                                               where !emp.IsDeleted && emp.DebitAmount > 0
                                               && emp.PaymentDate >= workerPaymentFilterVM.StartDate && emp.PaymentDate <= workerPaymentFilterVM.EndDate
                                               && (workerPaymentFilterVM.EmployeeIds.Count > 0 ? workerPaymentFilterVM.EmployeeIds.Contains(emp.UserId) : true)
                                               select new PaymentVM
                                               {
                                                   EmployeePaymentId = emp.EmployeePaymentId,
                                                   UserId = emp.UserId,
                                                   PaymentDate = emp.PaymentDate,
                                                   DebitAmount = emp.DebitAmount,
                                                   PaymentType = emp.PaymentType,
                                                   Remarks = emp.Remarks
                                               }).OrderByDescending(x => x.EmployeePaymentId).ToList();

                paymentList.ForEach(x =>
                {
                    x.PaymentTypeText = x.PaymentType.HasValue ? CommonMethod.GetEnumDescription((EmployeePaymentType)x.PaymentType.Value) : string.Empty;
                });
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
                PaymentVM paymentDetails = (from emp in _db.tbl_EmployeePayment
                                            where !emp.IsDeleted && emp.UserId == employeeId
                                            && emp.EmployeePaymentId == id
                                            select new PaymentVM
                                            {

                                                EmployeePaymentId = emp.EmployeePaymentId,
                                                UserId = emp.UserId,
                                                PaymentDate = emp.PaymentDate,
                                                DebitAmount = emp.DebitAmount,
                                                PaymentType = emp.PaymentType,
                                                Remarks = emp.Remarks
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
                if (paymentVM.UserId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmployeeIdIsNotValid);
                }

                if (paymentVM.UserId > 0)
                {
                    bool isWorkerExist = _db.tbl_Employee.Any(x => x.EmployeeId == paymentVM.UserId && !x.IsDeleted && x.AdminRoleId == (int)AdminRoles.Worker && x.CompanyId == companyId);
                    if (!isWorkerExist)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.EmployeeDoesNotExist);
                    }

                    if (!response.IsError)
                    {
                        tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                        objEmployeePayment.CompanyId = companyId;
                        objEmployeePayment.UserId = paymentVM.UserId;
                        objEmployeePayment.PaymentDate = DateTime.UtcNow; //paymentVM.PaymentDate;
                        objEmployeePayment.CreditOrDebitText = "Debit";
                        objEmployeePayment.DebitAmount = paymentVM.DebitAmount;
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
