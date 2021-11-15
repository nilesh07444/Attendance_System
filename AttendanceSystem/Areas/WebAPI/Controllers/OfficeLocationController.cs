using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
 
namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    public class OfficeLocationController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db; 
        public OfficeLocationController()
        {
            _db = new AttendanceSystemEntities();
        }
         
        [HttpPost]
        [Route("ValidateOfficeLocationPassword")]
        public ResponseDataModel<bool> ValidateOfficeLocationPassword(LocationPasswordVM passwordVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;
                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();

                #region Validation

                int loggedInUserRoleId = base.UTI.RoleId;

                if (loggedInUserRoleId != (int)AdminRoles.CompanyAdmin)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AccessDeniedOfOfficeLocation);
                }

                if (!response.IsError && string.IsNullOrEmpty(passwordVM.OfficeLocationAccessPassword))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EnterOfficeLocationAccessPassword);
                }

                if (!response.IsError && string.IsNullOrEmpty(objCompany.OfficeLocationAccessPassword))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.OfficeLocationAccessPasswordStillNotSaved);
                }

                if (!response.IsError && objCompany != null && objCompany.OfficeLocationAccessPassword != passwordVM.OfficeLocationAccessPassword)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.InvalidOfficeLocationAccessPassword);
                }

                #endregion Validation
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }
            return response;
        }
         
        [HttpGet]
        [Route("OfficeLocationList")]
        public ResponseDataModel<List<OfficeLocationVM>> OfficeLocationList()
        {
            ResponseDataModel<List<OfficeLocationVM>> response = new ResponseDataModel<List<OfficeLocationVM>>();
            response.IsError = false;
            try
            {
                
                long companyId = base.UTI.CompanyId;

                List<OfficeLocationVM> officeLocationList = (from st in _db.tbl_OfficeLocation
                                                             where !st.IsDeleted && st.IsActive && st.CompanyId == companyId
                                                             && st.Latitude != null && st.Longitude != null
                                                             select new OfficeLocationVM
                                                             {
                                                                 OfficeLocationId = st.OfficeLocationId,
                                                                 OfficeLocationName = st.OfficeLocationName,
                                                                 OfficeLocationDescription = st.OfficeLocationDescription,
                                                                 IsActive = st.IsActive,
                                                                 Latitude = st.Latitude,
                                                                 Longitude = st.Longitude,
                                                                 RadiousInMeter = st.RadiousInMeter
                                                             }).OrderByDescending(x => x.OfficeLocationName).ToList();

                response.Data = officeLocationList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpGet]
        [Route("PendingOfficeLocationList")]
        public ResponseDataModel<List<OfficeLocationVM>> PendingOfficeLocationList()
        {
            ResponseDataModel<List<OfficeLocationVM>> response = new ResponseDataModel<List<OfficeLocationVM>>();
            response.IsError = false;
            try
            {
                
                long companyId = base.UTI.CompanyId;

                List<OfficeLocationVM> officeLocationList = (from st in _db.tbl_OfficeLocation
                                                             where !st.IsDeleted && st.IsActive && st.CompanyId == companyId
                                                             && st.Latitude == null && st.Longitude == null
                                                             select new OfficeLocationVM
                                                             {
                                                                 OfficeLocationId = st.OfficeLocationId,
                                                                 OfficeLocationName = st.OfficeLocationName,
                                                                 OfficeLocationDescription = st.OfficeLocationDescription,
                                                                 IsActive = st.IsActive,
                                                                 Latitude = st.Latitude,
                                                                 Longitude = st.Longitude,
                                                                 RadiousInMeter = st.RadiousInMeter
                                                             }).OrderByDescending(x => x.OfficeLocationName).ToList();

                response.Data = officeLocationList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("AddOrUpdateOfficeLocation")]
        public ResponseDataModel<bool> AddOrUpdateOfficeLocation(OfficeLocationVM locationVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            { 
                long companyId = base.UTI.CompanyId;
                int loggedInUserRoleId = base.UTI.RoleId;

                if (loggedInUserRoleId != (int)AdminRoles.CompanyAdmin)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AccessDeniedOfOfficeLocation);
                }

                if (!response.IsError)
                {
                    tbl_OfficeLocation objOfficeLocation = _db.tbl_OfficeLocation.Where(x => x.OfficeLocationId == locationVM.OfficeLocationId && x.CompanyId == companyId).FirstOrDefault();
                    if (objOfficeLocation != null)
                    {
                        objOfficeLocation.Latitude = locationVM.Latitude;
                        objOfficeLocation.Longitude = locationVM.Longitude;
                        objOfficeLocation.RadiousInMeter = locationVM.RadiousInMeter;
                        objOfficeLocation.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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
        [Route("GetOfficeLocationDetail/{Id}")]
        public ResponseDataModel<OfficeLocationVM> GetOfficeLocationDetail(long Id)
        {
            ResponseDataModel<OfficeLocationVM> response = new ResponseDataModel<OfficeLocationVM>();
            response.IsError = false;

            try
            { 
                long companyId = base.UTI.CompanyId;

                OfficeLocationVM objOfficeLocation = (from st in _db.tbl_OfficeLocation
                                            where !st.IsDeleted && st.CompanyId == companyId
                                            && st.OfficeLocationId == Id
                                            select new OfficeLocationVM
                                            {
                                                OfficeLocationId = st.OfficeLocationId,
                                                OfficeLocationName = st.OfficeLocationName,
                                                OfficeLocationDescription = st.OfficeLocationDescription,
                                                IsActive = st.IsActive,
                                                Latitude = st.Latitude,
                                                Longitude = st.Longitude,
                                                RadiousInMeter = st.RadiousInMeter
                                            }).FirstOrDefault();

                response.Data = objOfficeLocation;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("DeleteOfficeLocation/{Id}")]
        public ResponseDataModel<bool> DeleteOfficeLocation(long Id)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;
                int loggedInUserRoleId = base.UTI.RoleId;

                if (loggedInUserRoleId != (int)AdminRoles.CompanyAdmin)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.AccessDeniedOfOfficeLocation);
                }

                if (!response.IsError)
                {
                    tbl_OfficeLocation objOfficeLocation = _db.tbl_OfficeLocation.Where(x => x.OfficeLocationId == Id && x.CompanyId == companyId).FirstOrDefault();
                    if (objOfficeLocation != null)
                    {
                        objOfficeLocation.Latitude = null;
                        objOfficeLocation.Longitude = null;
                        objOfficeLocation.RadiousInMeter = null;
                        objOfficeLocation.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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