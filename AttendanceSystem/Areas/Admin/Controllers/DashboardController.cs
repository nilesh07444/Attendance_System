using System.Web.Mvc;

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