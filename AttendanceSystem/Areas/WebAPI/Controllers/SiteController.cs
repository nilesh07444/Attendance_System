using AttendanceSystem.Helper;
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

        [HttpGet]
        [Route("SiteLocationList")]
        public ResponseDataModel<List<ViewModel.SiteVM>> SiteLocationList()
        {
            ResponseDataModel<List<ViewModel.SiteVM>> response = new ResponseDataModel<List<ViewModel.SiteVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<ViewModel.SiteVM> siteList = (from st in _db.tbl_Site
                                                   where !st.IsDeleted && st.IsActive && st.CompanyId == companyId
                                                   && st.Latitude != null && st.Longitude != null
                                                   select new ViewModel.SiteVM
                                                   {
                                                       SiteId = st.SiteId,
                                                       SiteName = st.SiteName,
                                                       SiteDescription = st.SiteDescription,
                                                       IsActive = st.IsActive,
                                                       Latitude = st.Latitude,
                                                       Longitude = st.Longitude,
                                                       RadiousInMeter = st.RadiousInMeter
                                                   }).OrderByDescending(x => x.SiteName).ToList();

                response.Data = siteList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("PendingSiteLocationList")]
        public ResponseDataModel<List<ViewModel.SiteVM>> PendingSiteLocationList()
        {
            ResponseDataModel<List<ViewModel.SiteVM>> response = new ResponseDataModel<List<ViewModel.SiteVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<ViewModel.SiteVM> siteList = (from st in _db.tbl_Site
                                                   where !st.IsDeleted && st.IsActive && st.CompanyId == companyId
                                                   && st.Latitude == null && st.Longitude == null
                                                   select new ViewModel.SiteVM
                                                   {
                                                       SiteId = st.SiteId,
                                                       SiteName = st.SiteName,
                                                       SiteDescription = st.SiteDescription,
                                                       IsActive = st.IsActive
                                                   }).OrderByDescending(x => x.SiteName).ToList();

                response.Data = siteList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("AddOrUpdateSiteLocation")]
        public ResponseDataModel<bool> AddOrUpdateSiteLocation(ViewModel.SiteVM locationVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;
                int loggedInUserRoleId = base.UTI.RoleId;

                if (loggedInUserRoleId != (int)AdminRoles.Supervisor)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AccessDeniedOfSiteLocation);
                }

                if (!response.IsError)
                {
                    tbl_Site objSite = _db.tbl_Site.Where(x => x.SiteId == locationVM.SiteId && x.CompanyId == companyId).FirstOrDefault();
                    if (objSite != null)
                    {
                        objSite.Latitude = locationVM.Latitude;
                        objSite.Longitude = locationVM.Longitude;
                        objSite.RadiousInMeter = locationVM.RadiousInMeter;
                        objSite.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.SaveChanges();
                    }
                }
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
        [Route("GetSiteDetail/{Id}")]
        public ResponseDataModel<ViewModel.SiteVM> GetSiteDetail(long Id)
        {
            ResponseDataModel<ViewModel.SiteVM> response = new ResponseDataModel<ViewModel.SiteVM>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                ViewModel.SiteVM objSite = (from st in _db.tbl_Site
                                                   where !st.IsDeleted && st.CompanyId == companyId
                                                   && st.SiteId == Id
                                                   select new ViewModel.SiteVM
                                                   {
                                                       SiteId = st.SiteId,
                                                       SiteName = st.SiteName,
                                                       SiteDescription = st.SiteDescription,
                                                       IsActive = st.IsActive,
                                                       Latitude = st.Latitude,
                                                       Longitude = st.Longitude,
                                                       RadiousInMeter = st.RadiousInMeter
                                                   }).FirstOrDefault();
                response.Data = objSite;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("DeleteSiteLocation/{Id}")]
        public ResponseDataModel<bool> DeleteSiteLocation(long Id)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;
                int loggedInUserRoleId = base.UTI.RoleId;

                if (loggedInUserRoleId != (int)AdminRoles.Supervisor)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AccessDeniedOfSiteLocation);
                }

                if (!response.IsError)
                {
                    tbl_Site objSite = _db.tbl_Site.Where(x => x.SiteId == Id && x.CompanyId == companyId).FirstOrDefault();
                    if (objSite != null)
                    {
                        objSite.Latitude = null;
                        objSite.Longitude = null;
                        objSite.RadiousInMeter = null;
                        objSite.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.SaveChanges();
                    }
                }
                response.Data = true;
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
