using AttendanceSystem.Helper;
using System.Web.Mvc;
using static AttendanceSystem.ViewModel.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
    [PageAccess]
    public class DashboardController : Controller
    {
       
        public DashboardController()
        {

        }
        public ActionResult Index()
        {
            return View();
        }

       
    }
}