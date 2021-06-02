using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/worker")]
    public class WorkerController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public WorkerController()
        {
            _db = new AttendanceSystemEntities();
        }
         
        [HttpPost]
        [Route("AddFingerprint")]
        public ResponseDataModel<bool> AddFingerprint(EmployeeFirgerprintVM fingerprintVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {

                if (fingerprintVM.EmployeeId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmployeeIdIsNotValid);
                }

                if (fingerprintVM.EmployeeId > 0)
                {
                    int maximumEmployeeFingerprint = Convert.ToInt32(ConfigurationManager.AppSettings["MaximumEmployeeFingerprint"].ToString());

                    int workerFingerprintCount = _db.tbl_EmployeeFingerprint.Where(x => x.EmployeeId == fingerprintVM.EmployeeId).Count();
                    if (workerFingerprintCount >= maximumEmployeeFingerprint)
                    {
                        response.IsError = true;
                        response.AddError(ErrorMessage.WorkerFingerprintLimitReached);
                    }

                    if (!response.IsError)
                    {
                        tbl_EmployeeFingerprint objEmployeeFingerprint = new tbl_EmployeeFingerprint();
                        objEmployeeFingerprint.EmployeeId = fingerprintVM.EmployeeId;
                        objEmployeeFingerprint.ISOCode = fingerprintVM.ISOCode;
                        objEmployeeFingerprint.BitmapCode = fingerprintVM.BitmapCode;
                        _db.tbl_EmployeeFingerprint.Add(objEmployeeFingerprint);
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