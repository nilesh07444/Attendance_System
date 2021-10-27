using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/Site")]
    public class SiteController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public SiteController()
        {
            _db = new AttendanceSystemEntities();
        }
         
        [HttpGet]
        [Route("List")]
        public ResponseDataModel<List<ViewModel.SiteVM>> List()
        {
            ResponseDataModel<List<ViewModel.SiteVM>> response = new ResponseDataModel<List<ViewModel.SiteVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<ViewModel.SiteVM> siteList = (from st in _db.tbl_Site
                                             where !st.IsDeleted && st.CompanyId == companyId
                                             select new ViewModel.SiteVM
                                             {

                                                 SiteId = st.SiteId,
                                                 SiteName = st.SiteName,
                                                 SiteDescription = st.SiteDescription,
                                                 IsActive = st.IsActive

                                             }).OrderByDescending(x => x.SiteId).ToList();

                response.Data = siteList;
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
