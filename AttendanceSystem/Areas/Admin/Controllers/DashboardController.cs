using System.Web.Mvc;
using static AttendanceSystem.ViewModel.AccountModels;

namespace AttendanceSystem.Areas.Admin.Controllers
{
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