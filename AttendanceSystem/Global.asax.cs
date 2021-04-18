using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace AttendanceSystem
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var line = Environment.NewLine;
            Exception exception = Server.GetLastError();
            Server.ClearError();
            StringBuilder errormessage = new StringBuilder();
            errormessage.Append(exception.Message);
            errormessage.Append(line);
            errormessage.Append(exception.InnerException!=null? exception.InnerException.Message:string.Empty);
            errormessage.Append(line);
            errormessage.Append(exception.StackTrace);
            CommonMethod.AppLog(errormessage.ToString());
        }
    }
}
