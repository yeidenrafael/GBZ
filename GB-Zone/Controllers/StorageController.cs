using System.Web.Mvc;

namespace GrinGlobal.Zone.Controllers
{
    public class StorageController : Controller
    {
        // GET: Storage
        public ActionResult Index(string moduleId)
        {
            ViewData["moduleId"] = moduleId;

            return View();
        }
    }
}