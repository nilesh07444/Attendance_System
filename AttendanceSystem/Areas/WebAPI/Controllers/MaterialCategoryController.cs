using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/MaterialCategory")]
    public class MaterialCategoryController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;

        public MaterialCategoryController()
        {
            _db = new AttendanceSystemEntities();
        }


        [HttpGet]
        [Route("List")]
        public ResponseDataModel<List<ViewModel.MaterialCategoryVM>> List()
        {
            ResponseDataModel<List<ViewModel.MaterialCategoryVM>> response = new ResponseDataModel<List<ViewModel.MaterialCategoryVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<ViewModel.MaterialCategoryVM> MaterialCategoryList = (from st in _db.tbl_MaterialCategory
                                             where !st.IsDeleted && st.CompanyId == companyId
                                             select new ViewModel.MaterialCategoryVM
                                             {

                                                 MaterialCategoryId = st.MaterialCategoryId,
                                                 MaterialCategoryName = st.MaterialCategoryName,
                                                 Description = st.Description,
                                                 IsActive = st.IsActive

                                             }).OrderByDescending(x => x.MaterialCategoryId).ToList();

                response.Data = MaterialCategoryList;
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
