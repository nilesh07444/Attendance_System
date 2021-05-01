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

                List<PaymentVM> leaveList = (from emp in _db.tbl_EmployeePayment
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
        [Route("Detail/{id}")]
        public ResponseDataModel<PaymentVM> Detail(long id)
        {
            ResponseDataModel<PaymentVM> response = new ResponseDataModel<PaymentVM>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;

                PaymentVM leaveDetails = (from emp in _db.tbl_EmployeePayment
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
