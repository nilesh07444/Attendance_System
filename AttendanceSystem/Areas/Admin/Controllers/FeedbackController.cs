using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using AttendanceSystem.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class FeedbackController : Controller
    {
        // GET: Admin/Feedback
        AttendanceSystemEntities _db;
        int loggedInUserRoleId;
        long companyId;
        long loggedInUserId;
        public FeedbackController()
        {
            _db = new AttendanceSystemEntities();
            loggedInUserRoleId = clsAdminSession.RoleID;
            companyId = clsAdminSession.CompanyId;
            loggedInUserId = clsAdminSession.UserID;
        }
        public ActionResult Index(int? feedbackType = null, int? feedbackStatus = null)
        {
            FeedBackFilterVM feedBackFilterVM = new FeedBackFilterVM();

            if (feedbackType.HasValue)
                feedBackFilterVM.FeedbackType = feedbackType.Value;
            if(feedbackStatus.HasValue)
                feedBackFilterVM.FeedbackStatus = feedbackStatus.Value;

            try
            {

                List<SelectListItem> feedBackTypeList = GetFeedbackTypeList();
                List<SelectListItem> feedBackStatusList = GetFeedbackStatusList();

                feedBackFilterVM.FeedBackList = (from fb in _db.tbl_Feedback
                                                 join cmp in _db.tbl_Company on fb.CompanyId equals cmp.CompanyId
                                                 where fb.IsActive && !fb.IsDeleted
                                                 && (loggedInUserRoleId == (int)AdminRoles.CompanyAdmin ? fb.CompanyId == companyId : true)
                                                 && (feedBackFilterVM.FeedbackType.HasValue ? fb.FeedbackType == feedBackFilterVM.FeedbackType.Value : true)
                                                 && (feedBackFilterVM.FeedbackStatus.HasValue ? fb.FeedbackStatus == feedBackFilterVM.FeedbackStatus.Value : true)
                                                 select new FeedbackVM
                                                 {
                                                     FeedbackId = fb.FeedbackId,
                                                     CompanyId = fb.CompanyId,
                                                     CompanyName = cmp.CompanyName,
                                                     FeedbackType = fb.FeedbackType,
                                                     FeedbackStatus = fb.FeedbackStatus,
                                                     FeedbackText = fb.FeedbackText,
                                                     Remarks = fb.Remarks,
                                                     SuperAdminFeedbackText = fb.SuperAdminFeedbackText,
                                                     IsDeleted = fb.IsDeleted,
                                                     IsActive = fb.IsActive
                                                 }).ToList();

                feedBackFilterVM.FeedBackList.ForEach(x =>
                {
                    x.FeedbackTypeText = feedBackTypeList.Where(z => z.Value == x.FeedbackType.ToString()).Select(z => z.Text).FirstOrDefault();
                    x.FeedbackStatusText = feedBackStatusList.Where(z => z.Value == x.FeedbackStatus.ToString()).Select(z => z.Text).FirstOrDefault();
                });

                feedBackFilterVM.FeedBackTypeList = feedBackTypeList;
                feedBackFilterVM.FeedBackStatusList = feedBackStatusList;

            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(feedBackFilterVM);
        }

        public ActionResult Add()
        {
            FeedbackVM feedbackVM = new FeedbackVM();
            feedbackVM.CompanyId = clsAdminSession.CompanyId;
            feedbackVM.FeedBackTypeList = GetFeedbackTypeList();
            return View(feedbackVM);
        }

        [HttpPost]
        public ActionResult Add(FeedbackVM feedbackVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    long companyId = clsAdminSession.CompanyId;

                    tbl_Feedback objfeedback = new tbl_Feedback();
                    objfeedback.CompanyId = companyId;
                    objfeedback.FeedbackType = feedbackVM.FeedbackType;
                    objfeedback.FeedbackStatus = (int)FeedbackStatus.Pending;
                    objfeedback.FeedbackText = feedbackVM.FeedbackText;
                    objfeedback.Remarks = feedbackVM.Remarks;
                    objfeedback.IsActive = true;
                    objfeedback.CreatedBy = LoggedInUserId;
                    objfeedback.CreatedDate = DateTime.UtcNow;
                    objfeedback.ModifiedBy = LoggedInUserId;
                    objfeedback.ModifiedDate = DateTime.UtcNow;
                    _db.tbl_Feedback.Add(objfeedback);

                    _db.SaveChanges();



                }
                else
                {
                    feedbackVM.FeedBackTypeList = GetFeedbackTypeList();
                    return View(feedbackVM);
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(long id)
        {
            FeedbackVM feedbackVM = new FeedbackVM();
            try
            {

                feedbackVM = (from fb in _db.tbl_Feedback
                              join cmp in _db.tbl_Company on fb.CompanyId equals cmp.CompanyId
                              where fb.IsActive && !fb.IsDeleted && fb.FeedbackId == id
                              && (loggedInUserRoleId == (int)AdminRoles.CompanyAdmin ? fb.CompanyId == companyId : true)
                              select new FeedbackVM
                              {
                                  FeedbackId = fb.FeedbackId,
                                  CompanyId = fb.CompanyId,
                                  CompanyName = cmp.CompanyName,
                                  FeedbackType = fb.FeedbackType,
                                  FeedbackStatus = fb.FeedbackStatus,
                                  FeedbackText = fb.FeedbackText,
                                  Remarks = fb.Remarks,
                                  SuperAdminFeedbackText = fb.SuperAdminFeedbackText,
                                  IsDeleted = fb.IsDeleted,
                                  IsActive = fb.IsActive
                              }).FirstOrDefault();


                feedbackVM.FeedbackTypeText = CommonMethod.GetEnumDescription((FeedbackType)feedbackVM.FeedbackType);
                feedbackVM.FeedBackStatusList = GetFeedbackStatusList();
                feedbackVM.FeedBackStatusList.RemoveAt(0);//remove pending status from list
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(feedbackVM);
        }

        [HttpPost]
        public ActionResult Edit(FeedbackVM feedbackVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    if (feedbackVM.FeedbackId > 0)
                    {
                        tbl_Feedback objfeedback = _db.tbl_Feedback.FirstOrDefault(x => x.FeedbackId == feedbackVM.FeedbackId);
                        objfeedback.FeedbackStatus = feedbackVM.FeedbackStatus;
                        objfeedback.SuperAdminFeedbackText = feedbackVM.SuperAdminFeedbackText;
                        objfeedback.ModifiedBy = loggedInUserId;
                        objfeedback.ModifiedDate = DateTime.UtcNow;
                        _db.SaveChanges();
                    }
                    return RedirectToAction("Index");

                }
                else
                {
                    feedbackVM.FeedBackStatusList = GetFeedbackStatusList();
                    feedbackVM.FeedBackStatusList.RemoveAt(0);//remove pending status from list
                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(feedbackVM);
        }

        public ActionResult View(long id)
        {
            FeedbackVM feedbackVM = new FeedbackVM();
            try
            {

                feedbackVM = (from fb in _db.tbl_Feedback
                              join cmp in _db.tbl_Company on fb.CompanyId equals cmp.CompanyId
                              where fb.IsActive && !fb.IsDeleted && fb.FeedbackId == id
                              && (loggedInUserRoleId == (int)AdminRoles.CompanyAdmin ? fb.CompanyId == companyId : true)
                              select new FeedbackVM
                              {
                                  FeedbackId = fb.FeedbackId,
                                  CompanyId = fb.CompanyId,
                                  CompanyName = cmp.CompanyName,
                                  FeedbackType = fb.FeedbackType,
                                  FeedbackStatus = fb.FeedbackStatus,
                                  FeedbackText = fb.FeedbackText,
                                  Remarks = fb.Remarks,
                                  SuperAdminFeedbackText = fb.SuperAdminFeedbackText,
                                  IsDeleted = fb.IsDeleted,
                                  IsActive = fb.IsActive,
                                  CreatedDate = fb.CreatedDate
                              }).FirstOrDefault();


                feedbackVM.FeedbackStatusText = CommonMethod.GetEnumDescription((FeedbackStatus)feedbackVM.FeedbackStatus);
                feedbackVM.FeedbackTypeText = CommonMethod.GetEnumDescription((FeedbackType)feedbackVM.FeedbackType);
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }
            return View(feedbackVM);
        }

        private List<SelectListItem> GetFeedbackTypeList()
        {
            string[] feedbackTypeArr = Enum.GetNames(typeof(FeedbackType));
            var listfeedbackType = feedbackTypeArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listfeedbackType
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }

        private List<SelectListItem> GetFeedbackStatusList()
        {
            string[] feedbackStatusArr = Enum.GetNames(typeof(FeedbackStatus));
            var listfeedbackStatus = feedbackStatusArr.Select((value, key) => new { value, key }).ToDictionary(x => x.key + 1, x => x.value);

            List<SelectListItem> lst = (from pt in listfeedbackStatus
                                        select new SelectListItem
                                        {
                                            Text = pt.Value,
                                            Value = pt.Key.ToString()
                                        }).ToList();
            return lst;
        }
    }
}