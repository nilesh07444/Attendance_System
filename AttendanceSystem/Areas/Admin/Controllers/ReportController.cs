using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class ReportController : Controller
    {
        AttendanceSystemEntities _db;
        public ReportController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SalesReport(DateTime? startDate = null, DateTime? endDate = null, long? companyId = null, int? reportType = null)
        {
            SalesReportFilterVM salesReportFilterVM = new SalesReportFilterVM();
            if (companyId.HasValue)
            {
                salesReportFilterVM.CompanyId = companyId.Value;
            }

            if (reportType.HasValue)
            {
                salesReportFilterVM.ReportType = reportType.Value;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                salesReportFilterVM.StartDate = startDate.Value;
                salesReportFilterVM.EndDate = endDate.Value;
            }


            List<SalesReportVM> AccountSalesReportList = new List<SalesReportVM>();
            List<SalesReportVM> SMSSalesReportList = new List<SalesReportVM>();
            List<SalesReportVM> EmployeeSalesReportList = new List<SalesReportVM>();

            if (salesReportFilterVM.ReportType == 0 || salesReportFilterVM.ReportType == (int)SalesReportType.Account)
            {
                AccountSalesReportList = (from apkg in _db.tbl_CompanyRenewPayment
                                          join cmp in _db.tbl_Company on apkg.CompanyId equals cmp.CompanyId
                                          where apkg.CreatedDate >= salesReportFilterVM.StartDate && apkg.CreatedDate <= salesReportFilterVM.EndDate
                                          && (salesReportFilterVM.CompanyId.HasValue ? apkg.CompanyId == salesReportFilterVM.CompanyId.Value : true)
                                          select new SalesReportVM
                                          {
                                              BuyDate = apkg.CreatedDate,
                                              CompanyId = apkg.CompanyId,
                                              CompanyName = cmp.CompanyName,
                                              PackageType = SalesReportType.Account.ToString(),
                                              PackageName = apkg.PackageName,
                                              PackageAmount = 0,
                                              GST = apkg.GSTPer,
                                              GSTAmount = 0,
                                              FinalAmount = apkg.Amount,
                                              TotalFreeEmployee = apkg.NoOfEmployee,
                                              FreeAccessDays = apkg.AccessDays,
                                              TotalFreeSMS = apkg.NoOfSMS
                                          }).ToList();

                AccountSalesReportList.ForEach(x =>
                {
                    x.PackageAmount = Math.Round((x.FinalAmount / (100 + x.GST)) * 100, 2);
                    x.GSTAmount = Math.Round(x.PackageAmount * x.GST / 100, 2);
                });
            }

            if (salesReportFilterVM.ReportType == 0 || salesReportFilterVM.ReportType == (int)SalesReportType.SMS)
            {
                SMSSalesReportList = (from apkg in _db.tbl_CompanySMSPackRenew
                                      join cmp in _db.tbl_Company on apkg.CompanyId equals cmp.CompanyId
                                      where apkg.CreatedDate >= salesReportFilterVM.StartDate && apkg.CreatedDate <= salesReportFilterVM.EndDate
                                      && (salesReportFilterVM.CompanyId.HasValue ? apkg.CompanyId == salesReportFilterVM.CompanyId.Value : true)
                                      select new SalesReportVM
                                      {
                                          BuyDate = apkg.CreatedDate,
                                          CompanyId = apkg.CompanyId,
                                          CompanyName = cmp.CompanyName,
                                          PackageType = SalesReportType.SMS.ToString(),
                                          PackageName = apkg.SMSPackageName,
                                          PackageAmount = apkg.PackageAmount,
                                          GST = 0,
                                          GSTAmount = 0,
                                          FinalAmount = apkg.PackageAmount,
                                          TotalFreeEmployee = 0,
                                          FreeAccessDays = apkg.AccessDays,
                                          TotalFreeSMS = (long)apkg.NoOfSMS
                                      }).ToList();

            }

            if (salesReportFilterVM.ReportType == 0 || salesReportFilterVM.ReportType == (int)SalesReportType.Employee)
            {
                EmployeeSalesReportList = (from apkg in _db.tbl_EmployeeBuyTransaction
                                           join cmp in _db.tbl_Company on apkg.CompanyId equals cmp.CompanyId
                                           where apkg.CreatedDate >= salesReportFilterVM.StartDate && apkg.CreatedDate <= salesReportFilterVM.EndDate
                                           && (salesReportFilterVM.CompanyId.HasValue ? apkg.CompanyId == salesReportFilterVM.CompanyId.Value : true)
                                           select new SalesReportVM
                                           {
                                               BuyDate = apkg.CreatedDate,
                                               CompanyId = apkg.CompanyId,
                                               CompanyName = cmp.CompanyName,
                                               PackageType = SalesReportType.Employee.ToString(),
                                               PackageName = string.Empty,
                                               PackageAmount = apkg.TotalPaidAmount,
                                               GST = 0,
                                               GSTAmount = 0,
                                               FinalAmount = apkg.TotalPaidAmount,
                                               TotalFreeEmployee = apkg.NoOfEmpToBuy,
                                               FreeAccessDays = 0,
                                               TotalFreeSMS = 0
                                           }).ToList();

            }
            salesReportFilterVM.SalesReportList = AccountSalesReportList.Union(SMSSalesReportList).Union(EmployeeSalesReportList).OrderBy(x => x.BuyDate).ToList();
            salesReportFilterVM.CompanyList = GetCompanyList();
            salesReportFilterVM.ReportTypeList = GetReportTypeList();
            return View(salesReportFilterVM);
        }

        private List<SelectListItem> GetReportTypeList()
        {
            string[] reportTypeArr = Enum.GetNames(typeof(SalesReportType));
            var listreportType = reportTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listreportType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetCompanyList()
        {
            List<SelectListItem> lst = (from cmp in _db.tbl_Company
                                        orderby cmp.CompanyId
                                        select new SelectListItem
                                        {
                                            Text = cmp.CompanyName + " (" + cmp.CompanyCode + ")",
                                            Value = cmp.CompanyId.ToString()
                                        }).ToList();
            return lst;
        }
    }
}