using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class DistrictController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public DistrictController()
        {
            _db = new AttendanceSystemEntities();
        }

        public ActionResult Index(long? state)
        {
            DistrictFilterVM districtFilterVM = new DistrictFilterVM();
            try
            {
                if (state.HasValue)
                    districtFilterVM.StateId = state.Value;

                districtFilterVM.DistrictList = (from d in _db.tbl_District
                                                 join s in _db.tbl_State on d.StateId equals s.StateId
                                                 where !d.IsDeleted && !s.IsDeleted
                                                 && districtFilterVM.StateId == null || d.StateId == districtFilterVM.StateId.Value
                                                 select new DistrictVM
                                                 {
                                                     DistrictId = d.DistrictId,
                                                     DistrictName = d.DistrictName,
                                                     StateId = d.StateId,
                                                     IsActive = d.IsActive,
                                                     CreatedDate = d.CreatedDate,
                                                     StateName = s.StateName,
                                                 }).OrderByDescending(x => x.CreatedDate).ToList();

                districtFilterVM.StateList = new List<SelectListItem>();

                var lstStates = GetStateList();
                if (lstStates != null && lstStates.Count > 0)
                {
                    districtFilterVM.StateList = lstStates;
                }
                 
            }
            catch (Exception ex)
            {
            }
            return View(districtFilterVM);
        }

        public ActionResult Add()
        {
            DistrictVM objDistrict = new DistrictVM();
            objDistrict.StateList = GetStateList();
            return View(objDistrict);
        }

        [HttpPost]
        public ActionResult Add(DistrictVM districtVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    List<string> lstDistricts = districtVM.DistrictName.Trim().Split(',').ToList();

                    if (lstDistricts != null && lstDistricts.Count > 0)
                    {
                        lstDistricts.ForEach(districtName =>
                        {
                            if (!string.IsNullOrEmpty(districtName))
                            {
                                districtName = districtName.Trim();

                                // Check duplicate District
                                tbl_District duplicateDistrict = _db.tbl_District.Where(x => x.DistrictName == districtName && x.StateId == districtVM.StateId && !x.IsDeleted).FirstOrDefault();
                                if (duplicateDistrict == null)
                                {
                                    tbl_District objDistrict = new tbl_District();
                                    objDistrict.DistrictName = districtName;
                                    objDistrict.StateId = districtVM.StateId;
                                    objDistrict.IsActive = true;
                                    objDistrict.CreatedDate = CommonMethod.CurrentIndianDateTime();
                                    objDistrict.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                                    _db.tbl_District.Add(objDistrict);
                                    _db.SaveChanges();
                                }
                            }
                        });
                    }


                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(districtVM);
        }

        public ActionResult Edit(long Id)
        {
            DistrictVM districtVM = new DistrictVM();
            try
            {
                districtVM = (from d in _db.tbl_District
                              where d.DistrictId == Id
                              select new DistrictVM
                              {
                                  DistrictId = d.DistrictId,
                                  DistrictName = d.DistrictName,
                                  StateId = d.StateId,
                                  IsActive = d.IsActive,
                                  CreatedDate = d.CreatedDate
                              }).FirstOrDefault();

                districtVM.StateList = GetStateList();
            }
            catch (Exception ex)
            {
            }
            return View(districtVM);
        }

        [HttpPost]
        public ActionResult Edit(DistrictVM districtVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    string districtName = districtVM.DistrictName.Trim();

                    // Check duplicate District
                    tbl_District duplicateDistrict = _db.tbl_District.Where(x => x.DistrictName == districtName && x.StateId == districtVM.StateId
                                                            && x.DistrictId != districtVM.DistrictId && !x.IsDeleted).FirstOrDefault();
                    if (duplicateDistrict != null)
                    {
                        districtVM.StateList = GetStateList();

                        ModelState.AddModelError("DistrictName", ErrorMessage.DistrictNameExist);
                        return View(districtVM);
                    }

                    tbl_District objDistrict = _db.tbl_District.Where(x => x.DistrictId == districtVM.DistrictId).FirstOrDefault();
                    objDistrict.DistrictName = districtName;
                    objDistrict.StateId = districtVM.StateId;
                    objDistrict.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(districtVM);
        }

        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_District objDistrict = _db.tbl_District.Where(x => x.DistrictId == Id).FirstOrDefault();

                if (objDistrict != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objDistrict.IsActive = true;
                    }
                    else
                    {
                        objDistrict.IsActive = false;
                    }

                    objDistrict.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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

        [HttpPost]
        public string DeleteDistrict(int Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_District objDistrict = _db.tbl_District.Where(x => x.DistrictId == Id && !x.IsDeleted).FirstOrDefault();

                if (objDistrict == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    objDistrict.IsDeleted = true;
                    objDistrict.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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

        private List<SelectListItem> GetStateList()
        {
            var StateList = (from s in _db.tbl_State
                             where s.IsActive && !s.IsDeleted
                             select new SelectListItem
                             {
                                 Text = s.StateName,
                                 Value = s.StateId.ToString()
                             }).OrderBy(x => x.Text).ToList();

            return StateList;
        }

    }
}