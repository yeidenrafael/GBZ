using GrinGlobal.Zone.Helpers;
using System.Web.Mvc;
namespace GrinGlobal.Zone.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));
        Models.GGZoneModel ctx = new Models.GGZoneModel();
        /// <summary>
        /// Default view
        /// </summary>
        /// <returns></returns>
        ///   
        public ActionResult Index()
        {
            return View();
        }

 
       

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.Session["username"] == null && filterContext.HttpContext.Session["userkey"] == null)
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Logout", "Account"));
            }
            base.OnAuthorization(filterContext);
        }
    }
}