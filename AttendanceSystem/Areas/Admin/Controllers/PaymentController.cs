using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class PaymentController : Controller
    {
        // GET: Admin/Payment
        AttendanceSystemEntities _db;
        string enviornment;
        public PaymentController()
        {
            _db = new AttendanceSystemEntities();
            enviornment = ConfigurationManager.AppSettings["Environment"].ToString();
        }
        public ActionResult Index(int? userRole = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            PaymentFilterVM paymentFilterVM = new PaymentFilterVM();
            if (userRole.HasValue)
            {
                paymentFilterVM.UserRole = userRole.Value;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                paymentFilterVM.StartDate = startDate.Value;
                paymentFilterVM.EndDate = endDate.Value;
            }

            try
            {
                List<SelectListItem> employeePaymentTypeList = GetEmployeePaymentTypeList();

                long companyId = clsAdminSession.CompanyId;
                paymentFilterVM.PaymentList = (from empp in _db.tbl_EmployeePayment
                                               join emp in _db.tbl_Employee on empp.UserId equals emp.EmployeeId
                                               where !emp.IsDeleted && emp.CompanyId == companyId && !empp.IsDeleted && empp.CreditOrDebitText.ToLower() == "debit"
                                               && empp.PaymentDate >= paymentFilterVM.StartDate && empp.PaymentDate <= paymentFilterVM.EndDate
                                               && (paymentFilterVM.UserRole.HasValue ? emp.AdminRoleId == paymentFilterVM.UserRole.Value : true)
                                               select new PaymentVM
                                               {

                                                   EmployeePaymentId = empp.EmployeePaymentId,
                                                   UserId = empp.UserId,
                                                   PaymentDate = empp.PaymentDate,
                                                   EmployeeCode = emp.EmployeeCode,
                                                   UserName = emp.FirstName + " " + emp.LastName,
                                                   DebitAmount = empp.DebitAmount,
                                                   CreditAmount = empp.CreditAmount,
                                                   PaymentType = empp.PaymentType,
                                               }).OrderByDescending(x => x.EmployeePaymentId).ToList();

                paymentFilterVM.PaymentList.ForEach(x =>
                {
                    x.PaymentTypeText = employeePaymentTypeList.Where(z => z.Value == x.PaymentType.ToString()).Select(c => c.Text).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
            }

            paymentFilterVM.UserRoleList = GetUserRoleList();
            return View(paymentFilterVM);
        }

        public ActionResult Add(long id)
        {
            PaymentVM paymentVM = new PaymentVM();
            if (id > 0)
            {
                paymentVM = (from empp in _db.tbl_EmployeePayment
                             join emp in _db.tbl_Employee on empp.UserId equals emp.EmployeeId
                             where !empp.IsDeleted && empp.EmployeePaymentId == id
                             select new PaymentVM
                             {
                                 EmployeePaymentId = empp.EmployeePaymentId,
                                 UserId = empp.UserId,
                                 PaymentDate = empp.PaymentDate,
                                 EmployeeCode = emp.EmployeeCode,
                                 UserName = emp.FirstName + " " + emp.LastName,
                                 DebitAmount = empp.DebitAmount,
                                 PaymentType = empp.PaymentType,
                                 Remarks = empp.Remarks
                             }).FirstOrDefault();
            }

            paymentVM.EmployeeList = GetEmployeeList();
            paymentVM.EmployeePaymentTypeList = GetEmployeePaymentTypeList();
            return View(paymentVM);
        }

        [HttpPost]
        public ActionResult Add(PaymentVM paymentVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = Int64.Parse(clsAdminSession.CompanyId.ToString());

                    #region validation
                    if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == paymentVM.PaymentDate.Month && (x.IsEmployeeDone || x.IsWorkerDone)))
                    {
                        ModelState.AddModelError("", ErrorMessage.MonthlyConvesrionCompletedYouCanNotAddOrModifyPaymentDetails);
                        paymentVM.EmployeeList = GetEmployeeList();
                        paymentVM.EmployeePaymentTypeList = GetEmployeePaymentTypeList();
                        return View(paymentVM);
                    }
                    #endregion


                    if (paymentVM.EmployeePaymentId > 0)
                    {
                        tbl_EmployeePayment objEmployeePayment = _db.tbl_EmployeePayment.Where(x => x.EmployeePaymentId == paymentVM.EmployeePaymentId).FirstOrDefault();

                        objEmployeePayment.PaymentDate = paymentVM.PaymentDate;
                        objEmployeePayment.DebitAmount = paymentVM.DebitAmount;
                        objEmployeePayment.PaymentType = paymentVM.PaymentType;
                        objEmployeePayment.Remarks = paymentVM.Remarks;
                        objEmployeePayment.ModifiedBy = LoggedInUserId;
                        objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                        objEmployeePayment.CompanyId = companyId;
                        objEmployeePayment.UserId = paymentVM.UserId;
                        objEmployeePayment.PaymentDate = paymentVM.PaymentDate;
                        objEmployeePayment.CreditOrDebitText = "Debit";
                        objEmployeePayment.DebitAmount = paymentVM.DebitAmount;
                        objEmployeePayment.CreditAmount = 0;
                        objEmployeePayment.PaymentType = paymentVM.PaymentType;
                        objEmployeePayment.Remarks = paymentVM.Remarks;
                        objEmployeePayment.CreatedBy = LoggedInUserId;
                        objEmployeePayment.CreatedDate = DateTime.UtcNow;
                        objEmployeePayment.ModifiedBy = LoggedInUserId;
                        objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                        _db.tbl_EmployeePayment.Add(objEmployeePayment);
                    }
                    _db.SaveChanges();
                }
                else
                {
                    paymentVM.EmployeeList = GetEmployeeList();
                    return View(paymentVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return RedirectToAction("Index");
        }

        private List<SelectListItem> GetUserRoleList()
        {
            long[] adminRole = new long[] { (long)AdminRoles.CompanyAdmin, (long)AdminRoles.SuperAdmin };
            List<SelectListItem> lst = (from ms in _db.mst_AdminRole
                                        where !adminRole.Contains(ms.AdminRoleId)
                                        select new SelectListItem
                                        {
                                            Text = ms.AdminRoleName,
                                            Value = ms.AdminRoleId.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetEmployeeList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetEmployeePaymentTypeList()
        {
            string[] paymentTypeArr = Enum.GetNames(typeof(EmployeePaymentType));
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
        public string DeletePayment(int employeePaymentId)
        {
            string ReturnMessage = string.Empty;

            try
            {
                long companyId = clsAdminSession.CompanyId;
                tbl_EmployeePayment objEmployeePayment = _db.tbl_EmployeePayment.Where(x => x.EmployeePaymentId == employeePaymentId).FirstOrDefault();

                if (objEmployeePayment == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    #region validation
                    if (_db.tbl_Conversion.Any(x => x.CompanyId == companyId && x.Month == objEmployeePayment.PaymentDate.Month && (x.IsEmployeeDone || x.IsWorkerDone)))
                    {
                        ReturnMessage = "convertioncomplete";
                    }
                    #endregion
                    if (string.IsNullOrEmpty(ReturnMessage))
                    {
                        long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                        objEmployeePayment.IsDeleted = true;
                        objEmployeePayment.ModifiedBy = LoggedInUserId;
                        objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();

                        ReturnMessage = "success";
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = ex.Message.ToString();
                ReturnMessage = "exception";
            }

            return ReturnMessage;
        }

        public JsonResult VerifyEmployeeMobileNo(long employeeId)
        {
            int status = 0;
            string errorMessage = string.Empty;
            string otp = string.Empty;
            try
            {
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                string mobileNo = objEmployee.MobileNo;

                if (!string.IsNullOrEmpty(mobileNo))
                {
                    #region Send SMS

                    Random random = new Random();
                    int num = random.Next(555555, 999999);

                    int SmsId = (int)SMSType.PaymentOTP;
                    string msg = CommonMethod.GetSmsContent(SmsId);

                    Regex regReplace = new Regex("{#var#}");
                    msg = regReplace.Replace(msg, objEmployee.FirstName + " " + objEmployee.LastName, 1);
                    msg = regReplace.Replace(msg, num.ToString(), 1);

                    msg = msg.Replace("\r\n", "\n");

                    var json = CommonMethod.SendSMSWithoutLog(msg, mobileNo);

                    if (json.Contains("invalidnumber"))
                    {
                        status = 0;
                        errorMessage = ErrorMessage.InvalidMobileNo;
                    }
                    else
                    {
                        status = 1;
                        otp = num.ToString();
                    }

                    #endregion

                }
                else
                {
                    status = 0;
                    errorMessage = ErrorMessage.InvalidMobileNo;
                }
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, Otp = otp, ErrorMessage = errorMessage, SetOtp = clsAdminSession.SetOtp }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DeletedPayment(int? userRole = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            PaymentFilterVM paymentFilterVM = new PaymentFilterVM();
            if (userRole.HasValue)
            {
                paymentFilterVM.UserRole = userRole.Value;
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                paymentFilterVM.StartDate = startDate.Value;
                paymentFilterVM.EndDate = endDate.Value;
            }

            try
            {
                List<SelectListItem> employeePaymentTypeList = GetEmployeePaymentTypeList();

                long companyId = clsAdminSession.CompanyId;
                paymentFilterVM.PaymentList = (from empp in _db.tbl_EmployeePayment
                                               join emp in _db.tbl_Employee on empp.UserId equals emp.EmployeeId
                                               where emp.CompanyId == companyId && empp.IsDeleted
                                               && empp.PaymentDate >= paymentFilterVM.StartDate && empp.PaymentDate <= paymentFilterVM.EndDate
                                               && (paymentFilterVM.UserRole.HasValue ? emp.AdminRoleId == paymentFilterVM.UserRole.Value : true)
                                               select new PaymentVM
                                               {

                                                   EmployeePaymentId = empp.EmployeePaymentId,
                                                   UserId = empp.UserId,
                                                   PaymentDate = empp.PaymentDate,
                                                   EmployeeCode = emp.EmployeeCode,
                                                   UserName = emp.FirstName + " " + emp.LastName,
                                                   CreditAmount = empp.CreditAmount,
                                                   DebitAmount = empp.DebitAmount,
                                                   PaymentType = empp.PaymentType,
                                               }).OrderByDescending(x => x.EmployeePaymentId).ToList();

                paymentFilterVM.PaymentList.ForEach(x =>
                {
                    x.PaymentTypeText = employeePaymentTypeList.Where(z => z.Value == x.PaymentType.ToString()).Select(c => c.Text).FirstOrDefault();
                });
            }
            catch (Exception ex)
            {
            }

            paymentFilterVM.UserRoleList = GetUserRoleList();
            return View(paymentFilterVM);
        }

        public JsonResult GetEmployeePendingSalary(long employeeId)
        {
            int status = 0;
            string errorMessage = string.Empty;
            decimal? pendingSalary = 0;
            try
            {
                tbl_Employee objEmployee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();

                if (objEmployee.AdminRoleId == (int)AdminRoles.Worker)
                {
                    pendingSalary = _db.tbl_WorkerPayment.Any(x => x.UserId == employeeId) ? _db.tbl_WorkerPayment.Where(x => x.UserId == employeeId).Select(x => (x.CreditAmount.HasValue ? x.CreditAmount.Value : 0) - (x.DebitAmount.HasValue ? x.DebitAmount.Value : 0)).Sum() : 0;
                }
                else
                {
                    pendingSalary = _db.tbl_EmployeePayment.Any(x => x.UserId == employeeId) ? _db.tbl_EmployeePayment.Where(x => x.UserId == employeeId).Select(x => (x.CreditAmount.HasValue ? x.CreditAmount.Value : 0) - (x.DebitAmount.HasValue ? x.DebitAmount.Value : 0)).Sum() : 0;
                }
                status = 1;
            }
            catch (Exception ex)
            {
                status = 0;
                errorMessage = ex.Message.ToString();
            }

            return Json(new { Status = status, PendingSalary = pendingSalary, ErrorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
        }
    }
}