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
    public class WorkerAttendanceController : Controller
    {
        // GET: Admin/WorkerAttendance
        long companyId;
        AttendanceSystemEntities _db;
        public WorkerAttendanceController()
        {
            _db = new AttendanceSystemEntities();
            companyId = clsAdminSession.CompanyId;
        }
        public ActionResult Index(DateTime? attendanceDate, int? siteId, long? employeeId)
        {
            WorkerAttendanceFilterVM workerAttendanceFilterVM = new WorkerAttendanceFilterVM();
            try
            {
                companyId = clsAdminSession.CompanyId;
                if (attendanceDate.HasValue)
                {
                    workerAttendanceFilterVM.AttendanceDate = attendanceDate.Value;
                }
                if (siteId.HasValue)
                {
                    workerAttendanceFilterVM.SiteId = siteId.Value;
                }

                if (employeeId.HasValue)
                {
                    workerAttendanceFilterVM.EmployeeId = employeeId.Value;
                }

                workerAttendanceFilterVM.AttendanceList = (from at in _db.tbl_WorkerAttendance
                                                           join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                                           where emp.CompanyId == companyId
                                                           && at.AttendanceDate == workerAttendanceFilterVM.AttendanceDate
                                                           && (at.MorningSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.AfternoonSiteId == workerAttendanceFilterVM.SiteId
                                                           || at.EveningSiteId == workerAttendanceFilterVM.SiteId)
                                                           && (workerAttendanceFilterVM.EmployeeId.HasValue ? at.EmployeeId == workerAttendanceFilterVM.EmployeeId.Value : true)
                                                           select new WorkerAttendanceVM
                                                           {
                                                               AttendanceId = at.WorkerAttendanceId,
                                                               CompanyId = emp.CompanyId,
                                                               EmployeeId = emp.EmployeeId,
                                                               EmployeeCode = emp.EmployeeCode,
                                                               Name = emp.FirstName + " " + emp.LastName,
                                                               AttendanceDate = at.AttendanceDate,
                                                               EmploymentCategory = emp.EmploymentCategory,
                                                               IsMorning = (at.IsMorning && at.MorningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsAfternoon = (at.IsAfternoon && at.AfternoonSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               IsEvening = (at.IsEvening && at.EveningSiteId == workerAttendanceFilterVM.SiteId ? true : false),
                                                               ProfilePicture = emp.ProfilePicture
                                                           }).OrderByDescending(x => x.AttendanceDate).ToList();

                workerAttendanceFilterVM.AttendanceList.ForEach(x =>
                {
                    x.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)x.EmploymentCategory);
                    x.ProfilePicture = (!string.IsNullOrEmpty(x.ProfilePicture) ? CommonMethod.GetCurrentDomain() + ErrorMessage.EmployeeDirectoryPath + x.ProfilePicture : string.Empty);
                });
                workerAttendanceFilterVM.EmployeeList = GetWorkerList();
                workerAttendanceFilterVM.SiteList = GetSiteList();
            }
            catch (Exception ex)
            {
            }
            return View(workerAttendanceFilterVM);
        }

        private List<SelectListItem> GetSiteList()
        {
            companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from st in _db.tbl_Site
                                        where !st.IsDeleted && st.CompanyId == companyId
                                        select new SelectListItem
                                        {
                                            Text = st.SiteName,
                                            Value = st.SiteId.ToString()
                                        }).ToList();
            return lst;
        }

        [HttpPost]
        public JsonResult SearchWorker(string searchText)
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId

                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();


            return Json(lst.Select(c => new { Name = c.Text, ID = c.Value }).ToList(), JsonRequestBehavior.AllowGet);
        }

        private List<SelectListItem> GetWorkerList()
        {
            long companyId = clsAdminSession.CompanyId;
            List<SelectListItem> lst = (from emp in _db.tbl_Employee
                                        where !emp.IsDeleted && emp.CompanyId == companyId
                                        && emp.AdminRoleId == (int)AdminRoles.Worker
                                        select new SelectListItem
                                        {
                                            Text = emp.FirstName + " " + emp.LastName + " (" + emp.EmployeeCode + ")",
                                            Value = emp.EmployeeId.ToString()
                                        }).ToList();
            return lst;
        }

        public ActionResult AssignedWorkerList(int? siteId, DateTime? date, long? employeeId)
        {
            AssignedWorkerFilterVM assignedWorkerFilterVM = new AssignedWorkerFilterVM();
            try
            {
                companyId = clsAdminSession.CompanyId;

                if (siteId.HasValue && siteId.Value > 0)
                {
                    assignedWorkerFilterVM.SiteId = siteId.Value;
                }

                if (date.HasValue)
                {
                    assignedWorkerFilterVM.Date = date.Value;
                }

                if (employeeId.HasValue)
                {
                    assignedWorkerFilterVM.EmployeeId = employeeId.Value;
                }

                assignedWorkerFilterVM.AssignedWorkerList = (from emp in _db.tbl_Employee
                                                             join wk in _db.tbl_AssignWorker
                                                             on emp.EmployeeId equals wk.EmployeeId
                                                             join st in _db.tbl_Site on wk.SiteId equals st.SiteId
                                                             where !emp.IsDeleted && emp.CompanyId == companyId
                                                             && emp.AdminRoleId == (int)AdminRoles.Worker
                                                             && (assignedWorkerFilterVM.SiteId.HasValue && assignedWorkerFilterVM.SiteId.Value > 0 ? wk.SiteId == assignedWorkerFilterVM.SiteId.Value : true)
                                                             && wk.Date == assignedWorkerFilterVM.Date
                                                             select new AssignedWorkerVM
                                                             {
                                                                 EmployeeId = emp.EmployeeId,
                                                                 EmployeeCode = emp.EmployeeCode,
                                                                 EmployeeName = emp.FirstName + " " + emp.LastName,
                                                                 Date = wk.Date,
                                                                 SiteId = wk.SiteId,
                                                                 SiteName = st.SiteName,
                                                                 IsClosed = wk.IsClosed
                                                             }).ToList();

                assignedWorkerFilterVM.EmployeeList = GetWorkerList();
                assignedWorkerFilterVM.SiteList = GetSiteList();

            }
            catch (Exception ex)
            {

            }

            return View(assignedWorkerFilterVM);
        }

        public ActionResult Add(int siteId, DateTime date, long employeeId)
        {
            AddWorkerAttendanceVM addWorkerAttendanceVM = null;
            try
            {
                addWorkerAttendanceVM = (from at in _db.tbl_WorkerAttendance
                                         join emp in _db.tbl_Employee on at.EmployeeId equals emp.EmployeeId
                                         where at.AttendanceDate == date
                                         && at.EmployeeId == employeeId
                                         && !at.IsClosed
                                         && (at.MorningSiteId == siteId || at.AfternoonSiteId == siteId || at.EveningSiteId == siteId)
                                         select new AddWorkerAttendanceVM
                                         {
                                             AttendanceId = at.WorkerAttendanceId,
                                             AttendanceDate = at.AttendanceDate,
                                             EmployeeId = at.EmployeeId,
                                             SiteId = siteId,
                                             EmployeeName = emp.FirstName + " " + emp.LastName,
                                             EmployeeCode = emp.EmployeeCode,
                                             EmploymentCategoryId = emp.EmploymentCategory,
                                             PerCategoryPrice = emp.PerCategoryPrice,
                                             ExtraPerHourPrice = emp.ExtraPerHourPrice
                                         }).FirstOrDefault();



                if (addWorkerAttendanceVM == null)
                {
                    addWorkerAttendanceVM = new AddWorkerAttendanceVM();
                    addWorkerAttendanceVM.SiteId = siteId;
                    addWorkerAttendanceVM.AttendanceDate = date;
                    addWorkerAttendanceVM.EmployeeId = employeeId;

                    tbl_Employee objEmloyee = _db.tbl_Employee.Where(x => x.EmployeeId == employeeId).FirstOrDefault();
                    addWorkerAttendanceVM.EmployeeCode = objEmloyee.EmployeeCode;
                    addWorkerAttendanceVM.EmployeeName = objEmloyee.FirstName + " " + objEmloyee.LastName;
                    addWorkerAttendanceVM.EmploymentCategoryId = objEmloyee.EmploymentCategory;
                    addWorkerAttendanceVM.PerCategoryPrice = objEmloyee.PerCategoryPrice;
                    addWorkerAttendanceVM.ExtraPerHourPrice = objEmloyee.ExtraPerHourPrice;
                }

                addWorkerAttendanceVM.SiteName = _db.tbl_Site.Where(x => x.SiteId == siteId).Select(x => x.SiteName).FirstOrDefault();
                addWorkerAttendanceVM.WorkerAttendanceTypeList = GetAttendanceTypeList();
                addWorkerAttendanceVM.EmploymentCategoryText = CommonMethod.GetEnumDescription((EmploymentCategory)addWorkerAttendanceVM.EmploymentCategoryId);
                addWorkerAttendanceVM.PendingSalary = _db.tbl_WorkerPayment.Any(x => x.UserId == employeeId && x.PaymentType == (int)EmployeePaymentType.Salary) ? _db.tbl_WorkerPayment.Where(x => x.UserId == employeeId).Select(x => (x.CreditAmount.HasValue ? x.CreditAmount.Value : 0) - (x.DebitAmount.HasValue ? x.DebitAmount.Value : 0)).Sum() : 0;
            }
            catch (Exception ex)
            {

            }
            return View(addWorkerAttendanceVM);
        }

        private List<SelectListItem> GetAttendanceTypeList()
        {
            string[] workerAttendanceTypeArr = Enum.GetNames(typeof(WorkerAttendanceType));
            var listWorkerAttendanceType = workerAttendanceTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listWorkerAttendanceType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }


        [HttpPost]
        public ActionResult Add(AddWorkerAttendanceVM addWorkerAttendanceVM)
        {
            try
            {
                DateTime today = CommonMethod.CurrentIndianDateTime().Date;
                long employeeId = clsAdminSession.UserID;
                if (ModelState.IsValid)
                {
                    #region Validation
                    if (addWorkerAttendanceVM.SiteId == 0)
                    {
                        ModelState.AddModelError("", ErrorMessage.SiteRequired);
                    }
                    if (!_db.tbl_AssignWorker.Any(x => x.EmployeeId == addWorkerAttendanceVM.EmployeeId && x.SiteId == addWorkerAttendanceVM.SiteId && x.Date == today && !x.IsClosed))
                    {
                        ModelState.AddModelError("", ErrorMessage.WorketNotAssignedToday);
                    }


                    if (addWorkerAttendanceVM.AttendanceType != (int)WorkerAttendanceType.Morning
                        && addWorkerAttendanceVM.AttendanceType != (int)WorkerAttendanceType.Afternoon
                        && addWorkerAttendanceVM.AttendanceType != (int)WorkerAttendanceType.Evening)
                    {
                        ModelState.AddModelError("", ErrorMessage.InValidWorkerAttendanceType);
                    }

                    tbl_WorkerAttendance attendanceObject = _db.tbl_WorkerAttendance.Where(x => x.WorkerAttendanceId == addWorkerAttendanceVM.AttendanceId && x.EmployeeId == addWorkerAttendanceVM.EmployeeId && x.AttendanceDate == today && !x.IsClosed).FirstOrDefault();
                    if (attendanceObject == null && addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                    {
                        ModelState.AddModelError("", ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnMorningOrAfterNoon);
                    }

                    if (attendanceObject != null && !attendanceObject.IsMorning && attendanceObject.IsAfternoon && addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                    {
                        ModelState.AddModelError("", ErrorMessage.CanNotTakeMorningAttendanceAfterAfterNoonAttendance);
                    }

                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning
                        && attendanceObject != null && attendanceObject.IsMorning)
                    {
                        ModelState.AddModelError("", ErrorMessage.WorkerMorningAttendanceAlreadyDone);
                    }
                    else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Afternoon
                        && attendanceObject != null && attendanceObject.IsAfternoon)
                    {
                        ModelState.AddModelError("", ErrorMessage.WorkerAfternoonAttendanceAlreadyDone);
                    }
                    else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && attendanceObject != null && attendanceObject.IsEvening)
                    {
                        ModelState.AddModelError("", ErrorMessage.WorkerEveningAttendanceAlreadyDone);
                    }

                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && attendanceObject != null && !attendanceObject.IsAfternoon)
                    {
                        ModelState.AddModelError("", ErrorMessage.CanNotTakeEveningAttendanceWorkerNotPresentOnAfterNoon);
                    }


                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.HourlyBased && addWorkerAttendanceVM.NoOfHoursWorked == 0)
                    {
                        ModelState.AddModelError("", ErrorMessage.PleaseProvideNoOfHoursWorked);
                    }

                    if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening
                        && addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.UnitBased && addWorkerAttendanceVM.NoOfUnitWorked == 0)
                    {
                        ModelState.AddModelError("", ErrorMessage.PleaseProvideNoOfUnitWorked);
                    }

                    #endregion Validation
                    if (!ModelState.IsValid)
                    {
                        if (attendanceObject != null)
                        {
                            if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                            {
                                attendanceObject.IsEvening = true;
                                attendanceObject.EveningAttendanceBy = employeeId;
                                attendanceObject.EveningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.EveningSiteId = addWorkerAttendanceVM.SiteId;
                                //attendanceObject.EveningLatitude = addWorkerAttendanceVM.Latitude;
                                //attendanceObject.EveningLongitude = addWorkerAttendanceVM.Longitude;
                                //attendanceObject.EveningLocationFrom = addWorkerAttendanceVM.LocationFrom;
                                attendanceObject.IsClosed = true;
                                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.DailyBased || addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.MonthlyBased)
                                    attendanceObject.ExtraHours = addWorkerAttendanceVM.ExtraHours;

                                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.HourlyBased)
                                    attendanceObject.NoOfHoursWorked = addWorkerAttendanceVM.NoOfHoursWorked;

                                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.UnitBased)
                                    attendanceObject.NoOfHoursWorked = addWorkerAttendanceVM.NoOfUnitWorked;

                                tbl_AssignWorker assignedWorker = _db.tbl_AssignWorker.Where(x => x.SiteId == addWorkerAttendanceVM.SiteId && x.Date == today && x.EmployeeId == addWorkerAttendanceVM.EmployeeId && !x.IsClosed).FirstOrDefault();
                                if (assignedWorker != null)
                                {
                                    assignedWorker.IsClosed = true;
                                }

                            }
                            else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                            {
                                attendanceObject.IsAfternoon = true;
                                attendanceObject.AfternoonAttendanceBy = employeeId;
                                attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.AfternoonSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.AfternoonLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.AfternoonLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.AfternoonLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }
                            else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                            {
                                attendanceObject.IsMorning = true;
                                attendanceObject.MorningAttendanceBy = employeeId;
                                attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.MorningSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.MorningLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.MorningLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.MorningLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }


                            _db.SaveChanges();
                        }
                        else
                        {
                            attendanceObject = new tbl_WorkerAttendance();
                            attendanceObject.EmployeeId = addWorkerAttendanceVM.EmployeeId;
                            attendanceObject.AttendanceDate = today;
                            if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Afternoon)
                            {
                                attendanceObject.IsAfternoon = true;
                                attendanceObject.AfternoonAttendanceBy = employeeId;
                                attendanceObject.AfternoonAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.AfternoonSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.AfternoonLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.AfternoonLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.AfternoonLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }
                            else if (addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Morning)
                            {
                                attendanceObject.IsMorning = true;
                                attendanceObject.MorningAttendanceBy = employeeId;
                                attendanceObject.MorningAttendanceDate = CommonMethod.CurrentIndianDateTime();
                                attendanceObject.MorningSiteId = addWorkerAttendanceVM.SiteId;
                                attendanceObject.MorningLatitude = addWorkerAttendanceVM.Latitude;
                                attendanceObject.MorningLongitude = addWorkerAttendanceVM.Longitude;
                                attendanceObject.MorningLocationFrom = addWorkerAttendanceVM.LocationFrom;
                            }
                            _db.tbl_WorkerAttendance.Add(attendanceObject);
                            _db.SaveChanges();
                        }

                        if (addWorkerAttendanceVM.EmploymentCategoryId != (int)EmploymentCategory.MonthlyBased && addWorkerAttendanceVM.AttendanceType == (int)WorkerAttendanceType.Evening)
                        {
                            if (!_db.tbl_WorkerPayment.Any(x => x.UserId == attendanceObject.EmployeeId && x.AttendanceId == attendanceObject.WorkerAttendanceId))
                            {
                                tbl_WorkerPayment objWorkerPayment = new tbl_WorkerPayment();
                                objWorkerPayment.CompanyId = companyId;
                                objWorkerPayment.UserId = attendanceObject.EmployeeId;
                                objWorkerPayment.AttendanceId = attendanceObject.WorkerAttendanceId;
                                objWorkerPayment.PaymentDate = attendanceObject.AttendanceDate;
                                objWorkerPayment.PaymentType = (int)EmployeePaymentType.Salary;
                                objWorkerPayment.CreditOrDebitText = ErrorMessage.Credit;
                                objWorkerPayment.DebitAmount = addWorkerAttendanceVM.TodaySalary.HasValue && addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.DailyBased ? addWorkerAttendanceVM.TodaySalary.Value : 0;
                                objWorkerPayment.Remarks = ErrorMessage.AutoCreditOnEveningAttendance;
                                objWorkerPayment.Month = attendanceObject.AttendanceDate.Month;
                                objWorkerPayment.Year = attendanceObject.AttendanceDate.Year;
                                objWorkerPayment.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                objWorkerPayment.CreatedBy = employeeId;
                                objWorkerPayment.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                objWorkerPayment.ModifiedBy = employeeId;

                                if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.DailyBased)
                                {
                                    objWorkerPayment.CreditAmount = (attendanceObject.IsMorning && attendanceObject.IsAfternoon && attendanceObject.IsEvening ? (addWorkerAttendanceVM.PerCategoryPrice) : (addWorkerAttendanceVM.PerCategoryPrice / 2)) + (addWorkerAttendanceVM.ExtraPerHourPrice * addWorkerAttendanceVM.ExtraHours);
                                }
                                else if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.HourlyBased)
                                {
                                    objWorkerPayment.CreditAmount = addWorkerAttendanceVM.PerCategoryPrice * addWorkerAttendanceVM.NoOfHoursWorked;
                                }
                                else if (addWorkerAttendanceVM.EmploymentCategoryId == (int)EmploymentCategory.UnitBased)
                                {
                                    objWorkerPayment.CreditAmount = addWorkerAttendanceVM.PerCategoryPrice * addWorkerAttendanceVM.NoOfUnitWorked;
                                }
                                _db.tbl_WorkerPayment.Add(objWorkerPayment);
                                _db.SaveChanges();
                            }


                        }

                    }
                    else
                    {
                        addWorkerAttendanceVM.WorkerAttendanceTypeList = GetAttendanceTypeList();
                        return View(addWorkerAttendanceVM);
                    }
                }
                else
                {
                    addWorkerAttendanceVM.WorkerAttendanceTypeList = GetAttendanceTypeList();
                    return View(addWorkerAttendanceVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return RedirectToAction("AssignedWorkerList");
        }
    }
}