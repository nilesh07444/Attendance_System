using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel.WebAPI;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
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
        }

        [HttpPost]
        [Route("OwnPaymentList")]
        public ResponseDataModel<List<PaymentVM>> OwnPaymentList(PaymentFilterVM paymentFilterVM)
        {
            employeeId = base.UTI.EmployeeId;
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
                List<PaymentVM> paymentList = (from emp in _db.tbl_WorkerPayment
                                               where !emp.IsDeleted && emp.DebitAmount > 0
                                               && emp.PaymentDate >= workerPaymentFilterVM.StartDate && emp.PaymentDate <= workerPaymentFilterVM.EndDate
                                               && (workerPaymentFilterVM.EmployeeIds.Count > 0 ? workerPaymentFilterVM.EmployeeIds.Contains(emp.UserId) : true)
                                               select new PaymentVM
                                               {
                                                   EmployeePaymentId = emp.WorkerPaymentId,
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
            employeeId = base.UTI.EmployeeId;
            ResponseDataModel<PaymentVM> response = new ResponseDataModel<PaymentVM>();
            response.IsError = false;
            try
            {
                PaymentVM paymentDetails = (from emp in _db.tbl_WorkerPayment
                                            where !emp.IsDeleted && emp.UserId == employeeId
                                            && emp.WorkerPaymentId == id
                                            select new PaymentVM
                                            {

                                                EmployeePaymentId = emp.WorkerPaymentId,
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
            employeeId = base.UTI.EmployeeId;
            companyId = base.UTI.CompanyId;
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

                if (!_db.tbl_Attendance.Any(x => x.UserId == employeeId && x.InDateTime != null && x.OutDateTime == null))
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.YourAttendanceNotTakenYetYouCanNotAssignWorker);
                }

                if (paymentVM.PaymentType != (int)EmployeePaymentType.Salary && paymentVM.PaymentType != (int)EmployeePaymentType.Extra)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.PaymentTypeWrong);
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
                        tbl_WorkerPayment objWorkerPayment = new tbl_WorkerPayment();
                        objWorkerPayment.CompanyId = companyId;
                        objWorkerPayment.UserId = paymentVM.UserId;
                        objWorkerPayment.PaymentDate = CommonMethod.CurrentIndianDateTime(); //paymentVM.PaymentDate;
                        objWorkerPayment.CreditOrDebitText = ErrorMessage.Debit;
                        objWorkerPayment.DebitAmount = paymentVM.DebitAmount;
                        objWorkerPayment.PaymentType = paymentVM.PaymentType;
                        objWorkerPayment.Month = objWorkerPayment.PaymentDate.Month;
                        objWorkerPayment.Year = objWorkerPayment.PaymentDate.Year;
                        objWorkerPayment.CreatedBy = employeeId;
                        objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objWorkerPayment.ModifiedBy = employeeId;
                        objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_WorkerPayment.Add(objWorkerPayment);
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

        [HttpPost]
        [Route("WorkerPaymentReport")]
        public ResponseDataModel<List<PaymentReportVM>> WorkerPaymentReport(WorkerPaymentReportFilterVM paymentReportFilterVM)
        {
            ResponseDataModel<List<PaymentReportVM>> response = new ResponseDataModel<List<PaymentReportVM>>();
            response.IsError = false;

            try
            {
                if (paymentReportFilterVM.EmployeeId == 0)
                {
                    response.IsError = true;
                    response.AddError(ErrorMessage.EmployeeIdIsNotValid);
                }

                if (!response.IsError)
                {

                    var startMonthParam = new SqlParameter
                    {
                        ParameterName = "StartMonth",
                        Value = paymentReportFilterVM.StartMonth
                    };

                    var endMonthParam = new SqlParameter
                    {
                        ParameterName = "EndMonth",
                        Value = paymentReportFilterVM.EndMonth
                    };

                    var yearParam = new SqlParameter
                    {
                        ParameterName = "Year",
                        Value = paymentReportFilterVM.Year
                    };

                    var employeeIdParam = new SqlParameter
                    {
                        ParameterName = "EmployeeId",
                        Value = paymentReportFilterVM.EmployeeId
                    };

                    List<PaymentReportVM> paymentReport = _db.Database.SqlQuery<PaymentReportVM>("exec Usp_GetPaymentReport @StartMonth,@EndMonth,@Year,@EmployeeId ", startMonthParam, endMonthParam, yearParam, employeeIdParam).ToList<PaymentReportVM>();
                    response.Data = paymentReport;

                }
            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }


        [HttpPost]
        [Route("WorkerPaymentReportDownload")]
        public ResponseDataModel<string> WorkerPaymentReportDownload(WorkerPaymentReportFilterVM paymentReportFilterVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {

                ExcelPackage excel = new ExcelPackage();
                long companyId = base.UTI.CompanyId;
                bool hasrecord = false;
                var startMonthParam = new SqlParameter
                {
                    ParameterName = "StartMonth",
                    Value = paymentReportFilterVM.StartMonth
                };

                var endMonthParam = new SqlParameter
                {
                    ParameterName = "EndMonth",
                    Value = paymentReportFilterVM.EndMonth
                };

                var yearParam = new SqlParameter
                {
                    ParameterName = "Year",
                    Value = paymentReportFilterVM.Year
                };

                var employeeIdParam = new SqlParameter
                {
                    ParameterName = "EmployeeId",
                    Value = paymentReportFilterVM.EmployeeId
                };
                //Get student name of string type
                List<PaymentReportVM> paymentReport = _db.Database.SqlQuery<PaymentReportVM>("exec Usp_GetPaymentReport @StartMonth,@EndMonth,@Year,@EmployeeId ", startMonthParam, endMonthParam, yearParam, employeeIdParam).ToList<PaymentReportVM>();

                string employeeName = _db.tbl_Employee.Where(x => x.EmployeeId == paymentReportFilterVM.EmployeeId).Select(x => x.FirstName + "_" + x.LastName).FirstOrDefault();

                string StartMonthName = CommonMethod.GetEnumDescription((CalenderMonths)paymentReportFilterVM.StartMonth);
                string EndMonthName = CommonMethod.GetEnumDescription((CalenderMonths)paymentReportFilterVM.EndMonth);

                string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing", "Remark" };

                var workSheet = excel.Workbook.Worksheets.Add("Report");
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 20;
                workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Cells[1, 1].Value = "Worker Payment Report: " + employeeName + "_" + StartMonthName + " to " + EndMonthName + "_" + paymentReportFilterVM.Year;
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

                if (paymentReport != null && paymentReport.Count() > 0)
                {
                    hasrecord = true;
                    foreach (var payment in paymentReport)
                    {
                        workSheet.Cells[row1 + 2, 1].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 1].Value = payment.Date.ToString("dd-MM-yyyy");
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
                        workSheet.Cells[row1 + 2, 2].Value = payment.Opening;
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
                        workSheet.Cells[row1 + 2, 3].Value = payment.Credit;
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
                        workSheet.Cells[row1 + 2, 4].Value = payment.Debit;
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
                        workSheet.Cells[row1 + 2, 5].Value = payment.Closing;
                        workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 6].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 6].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 6].Value = payment.Remark;
                        workSheet.Cells[row1 + 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 6].AutoFitColumns(30, 70);
                        row1 = row1 + 1;
                    }
                }

                string guidstr = Guid.NewGuid().ToString();
                guidstr = guidstr.Substring(0, 5);

                string documentPath = HttpContext.Current.Server.MapPath(ErrorMessage.DocumentDirectoryPath);

                bool folderExists = Directory.Exists(documentPath);
                if (!folderExists)
                    Directory.CreateDirectory(documentPath);

                string flname = "WorkerPayment_" + employeeName + "_" + StartMonthName + " to " + EndMonthName + "_" + paymentReportFilterVM.Year + guidstr + ".xlsx";
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

        [HttpPost]
        [Route("PaymentReport")]
        public ResponseDataModel<List<PaymentReportVM>> PaymentReport(PaymentReportFilterVM paymentReportFilterVM)
        {
            employeeId = base.UTI.EmployeeId;
            ResponseDataModel<List<PaymentReportVM>> response = new ResponseDataModel<List<PaymentReportVM>>();
            response.IsError = false;

            try
            {

                var startMonthParam = new SqlParameter
                {
                    ParameterName = "StartMonth",
                    Value = paymentReportFilterVM.StartMonth
                };

                var endMonthParam = new SqlParameter
                {
                    ParameterName = "EndMonth",
                    Value = paymentReportFilterVM.EndMonth
                };

                var yearParam = new SqlParameter
                {
                    ParameterName = "Year",
                    Value = paymentReportFilterVM.Year
                };

                var employeeIdParam = new SqlParameter
                {
                    ParameterName = "EmployeeId",
                    Value = employeeId
                };
                //Get student name of string type
                List<PaymentReportVM> paymentReport = _db.Database.SqlQuery<PaymentReportVM>("exec Usp_GetPaymentReport @StartMonth,@EndMonth,@Year,@EmployeeId ", startMonthParam, endMonthParam, yearParam, employeeIdParam).ToList<PaymentReportVM>();
                response.Data = paymentReport;

            }
            catch (Exception ex)
            {
                response.IsError = true;
                response.AddError(ex.Message);
            }

            return response;
        }


        [HttpPost]
        [Route("PaymentReportDownload")]
        public ResponseDataModel<string> PaymentReportDownload(PaymentReportFilterVM paymentReportFilterVM)
        {
            ResponseDataModel<string> response = new ResponseDataModel<string>();
            try
            {
                employeeId = base.UTI.EmployeeId;
                ExcelPackage excel = new ExcelPackage();
                long companyId = base.UTI.CompanyId;
                bool hasrecord = false;
                var startMonthParam = new SqlParameter
                {
                    ParameterName = "StartMonth",
                    Value = paymentReportFilterVM.StartMonth
                };

                var endMonthParam = new SqlParameter
                {
                    ParameterName = "EndMonth",
                    Value = paymentReportFilterVM.EndMonth
                };

                var yearParam = new SqlParameter
                {
                    ParameterName = "Year",
                    Value = paymentReportFilterVM.Year
                };

                var employeeIdParam = new SqlParameter
                {
                    ParameterName = "EmployeeId",
                    Value = employeeId
                };
                //Get student name of string type
                List<PaymentReportVM> paymentReport = _db.Database.SqlQuery<PaymentReportVM>("exec Usp_GetPaymentReport @StartMonth,@EndMonth,@Year,@EmployeeId ", startMonthParam, endMonthParam, yearParam, employeeIdParam).ToList<PaymentReportVM>();



                string StartMonthName = CommonMethod.GetEnumDescription((CalenderMonths)paymentReportFilterVM.StartMonth);
                string EndMonthName = CommonMethod.GetEnumDescription((CalenderMonths)paymentReportFilterVM.EndMonth);

                string[] arrycolmns = new string[] { "Date", "Opening", "Credit", "Debit", "Closing", "Remark" };

                var workSheet = excel.Workbook.Worksheets.Add("Report");
                workSheet.Cells[1, 1].Style.Font.Bold = true;
                workSheet.Cells[1, 1].Style.Font.Size = 20;
                workSheet.Cells[1, 1].Style.VerticalAlignment = ExcelVerticalAlignment.Top;
                workSheet.Cells[1, 1].Value = "Payment Report: " + base.UTI.UserName + "_" + StartMonthName + " to " + EndMonthName + "_" + paymentReportFilterVM.Year;
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

                if (paymentReport != null && paymentReport.Count() > 0)
                {
                    hasrecord = true;
                    foreach (var payment in paymentReport)
                    {
                        workSheet.Cells[row1 + 2, 1].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 1].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 1].Value = payment.Date.ToString("dd-MM-yyyy");
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
                        workSheet.Cells[row1 + 2, 2].Value = payment.Opening;
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
                        workSheet.Cells[row1 + 2, 3].Value = payment.Credit;
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
                        workSheet.Cells[row1 + 2, 4].Value = payment.Debit;
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
                        workSheet.Cells[row1 + 2, 5].Value = payment.Closing;
                        workSheet.Cells[row1 + 2, 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 5].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 5].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 5].AutoFitColumns(30, 70);

                        workSheet.Cells[row1 + 2, 6].Style.Font.Bold = false;
                        workSheet.Cells[row1 + 2, 6].Style.Font.Size = 12;
                        workSheet.Cells[row1 + 2, 6].Value = payment.Remark;
                        workSheet.Cells[row1 + 2, 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                        workSheet.Cells[row1 + 2, 6].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        workSheet.Cells[row1 + 2, 6].Style.WrapText = true;
                        workSheet.Cells[row1 + 2, 6].AutoFitColumns(30, 70);
                        row1 = row1 + 1;
                    }
                }

                string guidstr = Guid.NewGuid().ToString();
                guidstr = guidstr.Substring(0, 5);

                string documentPath = HttpContext.Current.Server.MapPath(ErrorMessage.DocumentDirectoryPath);

                bool folderExists = Directory.Exists(documentPath);
                if (!folderExists)
                    Directory.CreateDirectory(documentPath);

                string flname = "Payment_" + base.UTI.UserName + "_" + StartMonthName + " to " + EndMonthName + "_" + paymentReportFilterVM.Year + guidstr + ".xlsx";
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
