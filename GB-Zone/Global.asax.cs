using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace GrinGlobal.Zone
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            // ... 
            // Use HttpContext.Current to get a Web request processing helper 
            HttpServerUtility server = HttpContext.Current.Server;
            Exception exception = server.GetLastError();
            // Log an exception 
        }
       
    }
}
