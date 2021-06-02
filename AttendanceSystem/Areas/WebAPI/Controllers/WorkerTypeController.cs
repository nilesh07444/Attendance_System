using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/WorkerType")]
    public class WorkerTypeController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public WorkerTypeController()
        {
            _db = new AttendanceSystemEntities();
        }
         
        [HttpGet]
        [Route("List")]
        public ResponseDataModel<List<ViewModel.WorkerTypeVM>> List()
        {
            ResponseDataModel<List<ViewModel.WorkerTypeVM>> response = new ResponseDataModel<List<ViewModel.WorkerTypeVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<ViewModel.WorkerTypeVM> WorkerTypeList = (from wt in _db.tbl_WorkerType
                                             where !wt.IsDeleted && wt.IsActive && wt.CompanyId == companyId
                                             select new ViewModel.WorkerTypeVM
                                             {

                                                 WorkerTypeId = wt.WorkerTypeId,
                                                 WorkerTypeName = wt.WorkerTypeName,
                                                 Description = wt.Description,
                                                 IsActive = wt.IsActive

                                             }).OrderByDescending(x => x.WorkerTypeId).ToList();

                response.Data = WorkerTypeList;
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
