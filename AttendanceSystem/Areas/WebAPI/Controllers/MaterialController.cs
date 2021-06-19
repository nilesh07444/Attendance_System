using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.WebPages.Html;

namespace AttendanceSystem.Areas.WebAPI.Controllers
{
    [RoutePrefix("api/Material")]
    public class MaterialController : BaseUserController
    {
        private readonly AttendanceSystemEntities _db;
        long employeeId;
        long companyId;
        public MaterialController()
        {
            _db = new AttendanceSystemEntities();
            employeeId = base.UTI.EmployeeId;
            companyId = base.UTI.CompanyId;
        }


        [HttpPost]
        [Route("List")]
        public ResponseDataModel<List<MaterialVM>> List(MaterialFilterVM materialFilterVM)
        {
            ResponseDataModel<List<MaterialVM>> response = new ResponseDataModel<List<MaterialVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<MaterialVM> materialList = (from mt in _db.tbl_Material
                                                 join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                                 join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                                 where !mt.IsDeleted && mt.CompanyId == companyId
                                                 && (materialFilterVM.SiteId.HasValue ? mt.SiteId == materialFilterVM.SiteId : true)
                                                 && (materialFilterVM.MaterialCategoryId.HasValue ? mt.MaterialCategoryId == materialFilterVM.MaterialCategoryId : true)
                                                 && (materialFilterVM.Status.HasValue ? mt.InOut == materialFilterVM.Status : true)
                                                 && (materialFilterVM.StartDate.HasValue && materialFilterVM.EndDate.HasValue ? mt.MaterialDate >= materialFilterVM.StartDate && mt.MaterialDate <= materialFilterVM.EndDate : true)
                                                 select new MaterialVM
                                                 {
                                                     MaterialId = mt.MaterialId,
                                                     MaterialCategoryId = mt.MaterialCategoryId.Value,
                                                     MaterialCategoryText = mc.MaterialCategoryName,
                                                     MaterialDate = mt.MaterialDate,
                                                     SiteId = mt.SiteId,
                                                     SiteName = st.SiteName,
                                                     Qty = mt.Qty,
                                                     InOut = mt.InOut,
                                                     Remarks = mt.Remarks,
                                                     IsActive = mt.IsActive,
                                                 }).OrderByDescending(x => x.MaterialId).ToList();

                List<SelectListItem> materialInOutStatusList = GetMaterialStatusList();
                materialList.ForEach(x =>
                {
                    x.InOutText = materialInOutStatusList.Where(z => z.Value == x.InOut.ToString()).Select(c => c.Text).FirstOrDefault();
                });

                response.Data = materialList;
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
        public ResponseDataModel<bool> Add(MaterialVM materialVM)
        {
            ResponseDataModel<bool> response = new ResponseDataModel<bool>();
            response.IsError = false;
            response.Data = false;
            try
            {
                #region Validation
                if (materialVM.MaterialCategoryId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.LeaveDateRequired);
                }

                if (materialVM.SiteId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.SiteIsNotValid);
                }


                if (materialVM.Qty <= 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MaterialQtyNotValid);
                }

                if (materialVM.InOut <= 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.MaterialQtyNotValid);
                }

                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                #endregion Validation
                if (!response.IsError)
                {
                    tbl_Material objMaterial = new tbl_Material();
                    objMaterial.CompanyId = companyId;
                    objMaterial.MaterialCategoryId = materialVM.MaterialCategoryId;
                    objMaterial.MaterialDate = materialVM.MaterialDate;
                    objMaterial.SiteId = materialVM.SiteId;
                    objMaterial.Qty = materialVM.Qty;
                    objMaterial.InOut = materialVM.InOut;
                    objMaterial.Remarks = materialVM.Remarks;
                    objMaterial.IsActive = true;
                    objMaterial.CreatedBy = employeeId;
                    objMaterial.CreatedDate = CommonMethod.CurrentIndianDateTime();
                    objMaterial.ModifiedBy = employeeId;
                    objMaterial.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.tbl_Material.Add(objMaterial);
                    _db.SaveChanges();
                    response.Data = true;
                }
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
        public ResponseDataModel<MaterialVM> Detail(long id)
        {
            ResponseDataModel<MaterialVM> response = new ResponseDataModel<MaterialVM>();
            response.IsError = false;
            try
            {
                long companyId = base.UTI.CompanyId;

                MaterialVM materialVM = (from mt in _db.tbl_Material
                                         join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                         join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                         where !mt.IsDeleted && mt.CompanyId == companyId
                                         && mt.MaterialId == id
                                         select new MaterialVM
                                         {
                                             MaterialId = mt.MaterialId,
                                             MaterialCategoryId = mt.MaterialCategoryId.Value,
                                             MaterialCategoryText = mc.MaterialCategoryName,
                                             MaterialDate = mt.MaterialDate,
                                             SiteId = mt.SiteId,
                                             SiteName = st.SiteName,
                                             Qty = mt.Qty,
                                             InOut = mt.InOut,
                                             Remarks = mt.Remarks,
                                             IsActive = mt.IsActive,
                                         }).FirstOrDefault();

                response.Data = materialVM;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("DeletedList")]
        public ResponseDataModel<List<MaterialVM>> DeletedList(MaterialFilterVM materialFilterVM)
        {
            ResponseDataModel<List<MaterialVM>> response = new ResponseDataModel<List<MaterialVM>>();
            response.IsError = false;
            try
            {
                List<MaterialVM> materialList = (from mt in _db.tbl_Material
                                                 join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                                 join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                                 where mt.IsDeleted && mt.CompanyId == companyId
                                                 && (materialFilterVM.SiteId.HasValue ? mt.SiteId == materialFilterVM.SiteId : true)
                                                 && (materialFilterVM.MaterialCategoryId.HasValue ? mt.MaterialCategoryId == materialFilterVM.MaterialCategoryId : true)
                                                 && (materialFilterVM.Status.HasValue ? mt.InOut == materialFilterVM.Status : true)
                                                 && (materialFilterVM.StartDate.HasValue && materialFilterVM.EndDate.HasValue ? mt.MaterialDate >= materialFilterVM.StartDate && mt.MaterialDate <= materialFilterVM.EndDate : true)
                                                 select new MaterialVM
                                                 {
                                                     MaterialId = mt.MaterialId,
                                                     MaterialCategoryId = mt.MaterialCategoryId.Value,
                                                     MaterialCategoryText = mc.MaterialCategoryName,
                                                     MaterialDate = mt.MaterialDate,
                                                     SiteId = mt.SiteId,
                                                     SiteName = st.SiteName,
                                                     Qty = mt.Qty,
                                                     InOut = mt.InOut,
                                                     Remarks = mt.Remarks,
                                                     IsActive = mt.IsActive,
                                                 }).OrderByDescending(x => x.MaterialId).ToList();

                List<SelectListItem> materialInOutStatusList = GetMaterialStatusList();
                materialList.ForEach(x =>
                {
                    x.InOutText = materialInOutStatusList.Where(z => z.Value == x.InOut.ToString()).Select(c => c.Text).FirstOrDefault();
                });

                response.Data = materialList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        private List<SelectListItem> GetMaterialStatusList()
        {
            string[] paymentTypeArr = Enum.GetNames(typeof(MaterialStatus));
            var listpaymentType = paymentTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listpaymentType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        [HttpPost]
        [Route("Report")]
        public ResponseDataModel<List<MaterialReportVM>> Report(MaterialFilterVM materialFilterVM)
        {
            ResponseDataModel<List<MaterialReportVM>> response = new ResponseDataModel<List<MaterialReportVM>>();
            response.IsError = false;
            try
            {
                long employeeId = base.UTI.EmployeeId;
                long companyId = base.UTI.CompanyId;

                List<MaterialReportVM> materialList = (from mt in _db.tbl_Material
                                                       join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                                       join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                                       where !mt.IsDeleted && mt.CompanyId == companyId
                                                       && (materialFilterVM.SiteId.HasValue ? mt.SiteId == materialFilterVM.SiteId : true)
                                                       && (materialFilterVM.MaterialCategoryId.HasValue ? mt.MaterialCategoryId == materialFilterVM.MaterialCategoryId : true)
                                                       && (materialFilterVM.Status.HasValue ? mt.InOut == materialFilterVM.Status : true)
                                                       && (materialFilterVM.StartDate.HasValue && materialFilterVM.EndDate.HasValue ? mt.MaterialDate >= materialFilterVM.StartDate && mt.MaterialDate <= materialFilterVM.EndDate : true)
                                                       select new MaterialReportVM
                                                       {
                                                           MaterialId = mt.MaterialId,
                                                           MaterialCategoryId = mt.MaterialCategoryId.Value,
                                                           MaterialCategoryText = mc.MaterialCategoryName,
                                                           MaterialDate = mt.MaterialDate,
                                                           SiteId = mt.SiteId,
                                                           SiteName = st.SiteName,
                                                           Qty = mt.Qty,
                                                           Remarks = mt.Remarks,
                                                           InOut = mt.InOut
                                                       }).OrderByDescending(x => x.MaterialId).ToList();

                List<SelectListItem> materialInOutStatusList = GetMaterialStatusList();
                materialList.ForEach(x =>
                {
                    x.InWard = x.InOut == (int)MaterialStatus.In ? x.Qty : 0;
                    x.OutWard = x.InOut == (int)MaterialStatus.Out ? x.Qty : 0;
                });

                response.Data = materialList;
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }

        [HttpPost]
        [Route("ListDownload")]
        public ResponseDataModel<string> ListDownload(MaterialFilterVM materialFilterVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {

                ExcelPackage excel = new ExcelPackage();
                long companyId = base.UTI.CompanyId;
                bool hasrecord = false;
                List<MaterialVM> materialList = (from mt in _db.tbl_Material
                                                 join mc in _db.tbl_MaterialCategory on mt.MaterialCategoryId equals mc.MaterialCategoryId
                                                 join st in _db.tbl_Site on mt.SiteId equals st.SiteId
                                                 where !mt.IsDeleted && mt.CompanyId == companyId
                                                 && (materialFilterVM.SiteId.HasValue ? mt.SiteId == materialFilterVM.SiteId : true)
                                                 && (materialFilterVM.MaterialCategoryId.HasValue ? mt.MaterialCategoryId == materialFilterVM.MaterialCategoryId : true)
                                                 && (materialFilterVM.Status.HasValue ? mt.InOut == materialFilterVM.Status : true)
                                                 && (materialFilterVM.StartDate.HasValue && materialFilterVM.EndDate.HasValue ? mt.MaterialDate >= materialFilterVM.StartDate && mt.MaterialDate <= materialFilterVM.EndDate : true)
                                                 select new MaterialVM
                                                 {
                                                     MaterialId = mt.MaterialId,
                                                     MaterialCategoryId = mt.MaterialCategoryId.Value,
                                                     MaterialCategoryText = mc.MaterialCategoryName,
                                                     MaterialDate = mt.MaterialDate,
                                                     SiteId = mt.SiteId,
                                                     SiteName = st.SiteName,
                                                     Qty = mt.Qty,
                                                     InOut = mt.InOut,
                                                     Remarks = mt.Remarks,
                                                     IsActive = mt.IsActive,
                                                 }).OrderByDescending(x => x.MaterialId).ToList();

                List<SelectListItem> materialInOutStatusList = GetMaterialStatusList();
                materialList.ForEach(x =>
                {
                    x.InOutText = materialInOutStatusList.Where(z => z.Value == x.InOut.ToString()).Select(c => c.Text).FirstOrDefault();
                });

                string[] arrycolmns = new string[] { "Date", "Item Name", "Inward", "outward", "Remarks" };

                var workSheet = excel.Workbook.Worksheets.Add("Report");
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 20;
                workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Cells[1, 1].Value = "Material Sitewise Report: " + materialFilterVM.StartDate.Value.ToString("dd-MM-yyyy") + " to " + materialFilterVM.EndDate.Value.ToString("dd-MM-yyyy");
                for (var col = 1; col < arrycolmns.Length + 1; col++)
                {
                    workSheet.Cells[2, col].Style.Font.Bold = true;
                    workSheet.Cells[2, col].Style.Font.Size = 12;
                    workSheet.Cells[2, col].Value = arrycolmns[col - 1];
                    workSheet.Cells[2, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    workSheet.Cells[2, col].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    workSheet.Cells[2, col].AutoFitColumns(30, 70);
                    workSheet.Cells[2, col].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    workSheet.Cells[2, col].Style.WrapText = true;
                }

                int row1 = 1;

                if (materialList != null && materialList.Count() > 0)
                {
                    hasrecord = true;
                    foreach (var material in materialList)
                    {
                        workSheet.Cells[row1 + 2, 1].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 1].Value = material.MaterialDate.ToString("dd-MM-yyyy");
                        workSheet.Cells[row1 + 2, 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 1].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 1].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 2].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 2].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 2].Value = material.MaterialCategoryText;
                        workSheet.Cells[row1 + 2, 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 2].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 2].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 2].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 3].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 3].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 3].Value = material.InOut == (int)MaterialStatus.In ? material.Qty : 0;
                        workSheet.Cells[row1 + 2, 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 3].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 3].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 3].AutoFitColumns(30, 70);



                        workSheet.Cells[row1 + 2, 4].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 4].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 4].Value = material.InOut == (int)MaterialStatus.Out ? material.Qty : 0;
                        workSheet.Cells[row1 + 2, 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 4].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 4].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 4].AutoFitColumns(30, 70);


                        workSheet.Cells[row1 + 2, 5].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 5].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 5].Value = material.Remarks;
                        workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);
                        row1 = row1 + 1;
                    }
                }

                string guidstr = Guid.NewGuid().ToString();
                guidstr = guidstr.Substring(0, 5);

                string documentPath = HttpContext.Current.Server.MapPath(ErrorMessage.DocumentDirectoryPath);

                bool folderExists = Directory.Exists(documentPath);
                if (!folderExists)
                    Directory.CreateDirectory(documentPath);

                string flname = "MaterialSiteWiseReport_" + materialFilterVM.StartDate.Value.ToString("dd-MM-yyyy") + "_" + materialFilterVM.EndDate.Value.ToString("dd-MM-yyyy") + guidstr + ".xlsx";
                excel.SaveAs(new FileInfo(documentPath + flname));
                if (hasrecord == true)
                {
                    response.Data = CommonMethod.GetCurrentDomain() + ErrorMessage.DocumentDirectoryPath + flname;
                }
                else
                {
                    response.Data = "";
                }

            }
            catch (Exception ex)
            {
                response.AddError(ex.Message.ToString());
                return response;
            }

            return response;

        }
    }
}
