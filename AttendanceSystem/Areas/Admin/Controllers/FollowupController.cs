using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    [NoDirectAccess]
    public class FollowupController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public FollowupController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index(DateTime? ExpiryDate, int? PrimaryStatus, int? SecondaryStatus)
        {
            FollowupFilterVM followupFilterVM = new FollowupFilterVM();

            try
            {

                List<SelectListItem> followupStatusList = GetFollowupStatusList();

                if (!ExpiryDate.HasValue)
                {
                    followupFilterVM.ExpiryDate = CommonMethod.CurrentIndianDateTime();
                }
                else
                {
                    followupFilterVM.ExpiryDate = ExpiryDate;
                }

                if (PrimaryStatus.HasValue)
                {
                    followupFilterVM.PrimaryFollowupStatus = PrimaryStatus.Value;
                }

                if (SecondaryStatus.HasValue)
                {
                    followupFilterVM.SecondaryFollowupStatus = SecondaryStatus.Value;
                }


                List<int> actualStatusToMatch = new List<int>();

                if (PrimaryStatus != null)
                {
                    if (PrimaryStatus == (int)FollowupStatus.Open)
                    {
                        actualStatusToMatch.Add((int)FollowupStatus.Open);
                    }
                    else
                    {
                        if (SecondaryStatus == null)
                        {
                            actualStatusToMatch.Add((int)FollowupStatus.Interested);
                            actualStatusToMatch.Add((int)FollowupStatus.NotInterested);
                        }
                        else
                        {
                            actualStatusToMatch.Add((int)SecondaryStatus);
                        }
                    }
                }


                followupFilterVM.FollowupList = (from c in _db.tbl_Company
                                                 where !c.IsDeleted
                                                 && (followupFilterVM.ExpiryDate.HasValue && c.IsTrialMode ? (DbFunctions.TruncateTime(c.TrialExpiryDate) <= DbFunctions.TruncateTime(followupFilterVM.ExpiryDate)) : true)
                                                 && (followupFilterVM.ExpiryDate.HasValue && !c.IsTrialMode ? (DbFunctions.TruncateTime(c.AccountExpiryDate) <= DbFunctions.TruncateTime(followupFilterVM.ExpiryDate)) : true)
                                                 && (PrimaryStatus.HasValue ? actualStatusToMatch.Contains(c.FollowupStatus.Value) : true)
                                                 select new FollowupVM
                                                 {
                                                     CompanyId = c.CompanyId,
                                                     CompanyName = c.CompanyName,
                                                     CompanyCode = c.CompanyCode,
                                                     IsCompanyInTrialMode = c.IsTrialMode,
                                                     CompanyAccountExpiryDate = (c.IsTrialMode ? c.TrialExpiryDate : c.AccountExpiryDate),
                                                     FollowupStatus = c.FollowupStatus,
                                                     IsActive = c.IsActive,
                                                     NextFollowupDate = c.NextFollowupDate
                                                 }).OrderBy(x => x.CompanyAccountExpiryDate).ToList();

                followupFilterVM.FollowupList.ForEach(x =>
                {
                    x.FollowupStatusText = followupStatusList.Where(z => z.Value == x.FollowupStatus.ToString()).Select(c => c.Text).FirstOrDefault();
                });

                followupFilterVM.PrimaryStatusList = GetPrimaryFollowupStatusList();
                followupFilterVM.SecondaryStatusList = GetSecondaryFollowupStatusList();
            }
            catch (Exception ex)
            {
            }

            return View(followupFilterVM);
        }

        public ActionResult Add(long Id) // Id = CompanyId
        {
            List<FollowupVM> lstFollowups = new List<FollowupVM>();

            try
            {
                long companyId = Id;

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == companyId).FirstOrDefault();
                ViewBag.CompanyName = objCompany.CompanyName;

                lstFollowups = (from f in _db.tbl_Followup
                                where f.CompanyId == companyId
                                select new FollowupVM
                                {
                                    FollowupId = f.FollowupId,
                                    CompanyId = f.CompanyId,
                                    NextFollowupDate = f.NextFollowupDate,
                                    FollowupStatus = f.FollowupStatus,
                                    Description = f.Description,
                                    Remarks = f.Remarks,
                                    CreatedDate = f.CreatedDate,
                                }).OrderByDescending(x => x.CreatedDate).ToList();

                ViewBag.CompanyId = companyId;
            }
            catch (Exception ex)
            {
            }

            return View(lstFollowups);
        }

        [HttpPost]
        public string Add(CreateFollowupVM followupVM)
        {
            string msg = string.Empty;

            try
            {
                tbl_Followup objFollowup = new tbl_Followup();
                objFollowup.CompanyId = followupVM.CompanyId;
                objFollowup.NextFollowupDate = followupVM.NextFollowupDate;
                objFollowup.FollowupStatus = followupVM.FollowupStatus.Value;
                objFollowup.Description = followupVM.Description;
                objFollowup.Remarks = followupVM.Remarks;
                objFollowup.CreatedDate = CommonMethod.CurrentIndianDateTime();
                objFollowup.CreatedBy = (int)PaymentGivenBy.SuperAdmin;
                _db.tbl_Followup.Add(objFollowup);
                _db.SaveChanges();

                tbl_Company objCompany = _db.tbl_Company.Where(x => x.CompanyId == followupVM.CompanyId).FirstOrDefault();
                if (objCompany != null)
                {
                    objCompany.FollowupStatus = followupVM.FollowupStatus;
                    objCompany.NextFollowupDate = followupVM.FollowupStatus == (int)FollowupStatus.Open ? followupVM.NextFollowupDate : null;
                    _db.SaveChanges();
                }

                msg = "success";
            }
            catch (Exception ex)
            {
                msg = "exception";
            }

            return msg;
        }

        public ActionResult Reminders()
        {
            List<FollowupVM> followupReminderList = new List<FollowupVM>();
            try
            {
                DateTime currentDate = CommonMethod.CurrentIndianDateTime();
                followupReminderList = (from c in _db.tbl_Company
                                where !c.IsDeleted
                                && c.FollowupStatus == (int)FollowupStatus.Open 
                                && DbFunctions.TruncateTime(c.NextFollowupDate) <= DbFunctions.TruncateTime(currentDate)
                                select new FollowupVM
                                {
                                    CompanyId = c.CompanyId,
                                    CompanyName = c.CompanyName,
                                    CompanyCode = c.CompanyCode,
                                    IsCompanyInTrialMode = c.IsTrialMode,
                                    CompanyAccountExpiryDate = (c.IsTrialMode ? c.TrialExpiryDate : c.AccountExpiryDate),
                                    FollowupStatus = c.FollowupStatus,
                                    IsActive = c.IsActive,
                                    NextFollowupDate = c.NextFollowupDate
                                }).OrderBy(x => x.CompanyAccountExpiryDate).ToList();
            }
            catch (Exception ex)
            {
            }

            return View(followupReminderList);
        }

        private List<SelectListItem> GetFollowupStatusList()
        {
            string[] followupStatusArr = Enum.GetNames(typeof(FollowupStatus));
            var listFollowupStatus = followupStatusArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listFollowupStatus
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetPrimaryFollowupStatusList()
        {
            long[] secondaryStatuses = new long[] { (long)FollowupStatus.Interested, (long)FollowupStatus.NotInterested };

            string[] followupStatusArr = Enum.GetNames(typeof(FollowupStatus));
            var listFollowupStatus = followupStatusArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listFollowupStatus
                                        where !secondaryStatuses.Contains(pt.Key)
                                        select new SelectListItem
                                        {
                                            Text = CommonMethod.GetEnumDescription((FollowupStatus)pt.Key),
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetSecondaryFollowupStatusList()
        {
            long[] primaryStatuses = new long[] { (long)FollowupStatus.Open, (long)FollowupStatus.Close };

            string[] followupStatusArr = Enum.GetNames(typeof(FollowupStatus));
            var listFollowupStatus = followupStatusArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listFollowupStatus
                                        where !primaryStatuses.Contains(pt.Key)
                                        select new SelectListItem
                                        {
                                            Text = CommonMethod.GetEnumDescription((FollowupStatus)pt.Key),
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

    }
}