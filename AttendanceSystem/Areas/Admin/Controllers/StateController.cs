using AttendanceSystem.Helper;
using AttendanceSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    public class StateController : Controller
    {
        private readonly AttendanceSystemEntities _db;
        public StateController()
        {
            _db = new AttendanceSystemEntities();
        }
        public ActionResult Index()
        {
            List<StateVM> lstStates = new List<StateVM>();
            try
            {
                lstStates = (from s in _db.tbl_State
                             where !s.IsDeleted
                             select new StateVM
                             {
                                 StateId = s.StateId,
                                 StateName = s.StateName,
                                 CountryId = s.CountryId,
                                 IsActive = s.IsActive,
                                 CreatedDate = s.CreatedDate
                             }).OrderByDescending(x => x.CreatedDate).ToList();
            }
            catch (Exception ex)
            {
            }
            return View(lstStates);
        }

        public ActionResult Add(long id)
        {
            StateVM StateVM = new StateVM();
            if (id > 0)
            {
                StateVM = (from st in _db.tbl_State
                           where st.StateId == id && !st.IsDeleted
                           select new StateVM
                           {
                               StateId = st.StateId,
                               StateName = st.StateName,
                               CountryId = st.CountryId,
                               IsActive = st.IsActive
                           }).FirstOrDefault();
            }

            return View(StateVM);
        }

        [HttpPost]
        public ActionResult Add(StateVM stateVM)
        {
            try
            {
                IEnumerable<ModelError> allErrors = ModelState.Values.SelectMany(v => v.Errors);
                if (ModelState.IsValid)
                {
                    if (stateVM.StateId > 0)
                    {
                        // Check duplicate State
                        tbl_State duplicateState = _db.tbl_State.Where(x => x.StateName == stateVM.StateName && x.StateId != stateVM.StateId && !x.IsDeleted).FirstOrDefault();
                        if (duplicateState != null)
                        {
                            ModelState.AddModelError("StateName", ErrorMessage.StateNameExist);
                            return View(stateVM);
                        }

                        tbl_State objState = _db.tbl_State.Where(x => x.StateId == stateVM.StateId).FirstOrDefault();
                        objState.StateName = stateVM.StateName;
                        objState.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                    }
                    else
                    {
                        // Check duplicate State
                        tbl_State duplicateState = _db.tbl_State.Where(x => x.StateName == stateVM.StateName && !x.IsDeleted).FirstOrDefault();
                        if (duplicateState != null)
                        {
                            ModelState.AddModelError("StateName", ErrorMessage.StateNameExist);
                            return View(stateVM);
                        }

                        tbl_State objState = new tbl_State();
                        objState.StateName = stateVM.StateName;
                        objState.CountryId = 1;
                        objState.IsActive = true;
                        objState.CreatedDate = CommonMethod.CurrentIndianDateTime();
                        objState.ModifiedDate = CommonMethod.CurrentIndianDateTime();
                        _db.tbl_State.Add(objState);
                    }
                    _db.SaveChanges();

                    return RedirectToAction("Index");

                }
            }
            catch (Exception ex)
            {
                string ErrorMessage = ex.Message.ToString();
                throw ex;
            }

            return View(stateVM);
        }
         
        [HttpPost]
        public string ChangeStatus(long Id, string Status)
        {
            string ReturnMessage = "";
            try
            {
                tbl_State objState = _db.tbl_State.Where(x => x.StateId == Id).FirstOrDefault();

                if (objState != null)
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    if (Status == "Active")
                    {
                        objState.IsActive = true;
                    }
                    else
                    {
                        objState.IsActive = false;
                    }
                     
                    objState.ModifiedDate = CommonMethod.CurrentIndianDateTime();

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
        public string DeleteState(int Id)
        {
            string ReturnMessage = "";

            try
            {
                tbl_State objState = _db.tbl_State.Where(x => x.StateId == Id && !x.IsDeleted).FirstOrDefault();

                if (objState == null)
                {
                    ReturnMessage = "notfound";
                }
                else
                {
                    long LoggedInUserId = Int64.Parse(clsAdminSession.UserID.ToString());
                    objState.IsDeleted = true; 
                    objState.ModifiedDate = CommonMethod.CurrentIndianDateTime();
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
         
    }
}