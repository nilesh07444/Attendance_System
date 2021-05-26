using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
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
                                               where !emp.IsDeleted && emp.CompanyId == companyId && !empp.IsDeleted
                                               && empp.PaymentDate >= paymentFilterVM.StartDate && empp.PaymentDate <= paymentFilterVM.EndDate
                                               && (paymentFilterVM.UserRole.HasValue ? emp.AdminRoleId == paymentFilterVM.UserRole.Value : true)
                                               select new PaymentVM
                                               {

                                                   EmployeePaymentId = empp.EmployeePaymentId,
                                                   UserId = empp.UserId,
                                                   PaymentDate = empp.PaymentDate,
                                                   UserName = emp.FirstName + " " + emp.LastName,
                                                   Amount = empp.Amount,
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
                                 UserName = emp.FirstName + " " + emp.LastName,
                                 Amount = empp.Amount,
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

                    if (paymentVM.EmployeePaymentId > 0)
                    {
                        tbl_EmployeePayment objEmployeePayment = _db.tbl_EmployeePayment.Where(x => x.EmployeePaymentId == paymentVM.EmployeePaymentId).FirstOrDefault();

                        objEmployeePayment.PaymentDate = paymentVM.PaymentDate;
                        objEmployeePayment.Amount = paymentVM.Amount;
                        objEmployeePayment.PaymentType = paymentVM.PaymentType;
                        objEmployeePayment.Remarks = paymentVM.Remarks;
                        objEmployeePayment.ModifiedBy = LoggedInUserId;
                        objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                    }
                    else
                    {
                        tbl_EmployeePayment objEmployeePayment = new tbl_EmployeePayment();
                        objEmployeePayment.UserId = paymentVM.UserId;
                        objEmployeePayment.PaymentDate = paymentVM.PaymentDate;
                        objEmployeePayment.Amount = paymentVM.Amount;
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
                                            Text = emp.FirstName + " " + emp.LastName,
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
            string ReturnMessage = "";

            try
            {
                tbl_EmployeePayment objEmployeePayment = _db.tbl_EmployeePayment.Where(x => x.EmployeePaymentId == employeePaymentId).FirstOrDefault();

                if (objEmployeePayment == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objEmployeePayment.IsDeleted = true;
                    objEmployeePayment.ModifiedBy = LoggedInUserId;
                    objEmployeePayment.ModifiedDate = DateTime.UtcNow;
                    _db.SaveChanges();

                    ReturnMessage = "success";
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
                string mobileNo = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).Select(x => x.MobileNo).FirstOrDefault();
                if (!string.IsNullOrEmpty(mobileNo))
                {
                    using (WebClient webClient = new WebClient())
                    {
                        Random random = new Random();
                        int num = random.Next(555555, 999999);
                        if (enviornment != "Development")
                        {
                            string msg = "Your Otp code for Login is " + num;
                            msg = HttpUtility.UrlEncode(msg);
                            string url = CommonMethod.GetSMSUrl().Replace("--MOBILE--", mobileNo).Replace("--MSG--", msg);
                            var json = webClient.DownloadString(url);
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
                        }
                        else
                        {
                            status = 1;
                            otp = num.ToString();
                        }
                    }
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
                                                   UserName = emp.FirstName + " " + emp.LastName,
                                                   Amount = empp.Amount,
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
    }
}