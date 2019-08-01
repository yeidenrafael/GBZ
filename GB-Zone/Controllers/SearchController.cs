using System;
using System.Web.Mvc;
using GrinGlobal.Zone.Classes;

namespace GrinGlobal.Zone.Controllers
{
    public class SearchController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchController));
        // GET: Search
        public ActionResult Index(string moduleId, string formId)
        {
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formdata)
        {
            DataViewsSearch search = new DataViewsSearch();

            string serverId = Session["server"].ToString();
            string moduleId = formdata["moduleId"];
            string formId = formdata["formId"];
            string fieldId = formdata["radios"];
            string value = formdata[String.Format("text {0}", fieldId)];
            
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;
            
            return View(search.GetData(serverId, moduleId, formId, fieldId, value));
        }

        public ActionResult Index2(string serverId, string moduleId, string formId, string fieldId, string value)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            return View("Index", search.GetData(serverId, moduleId, formId, fieldId, value));
        }

        public ActionResult GridView(string serverId, string moduleId, string formId, string fieldId, string value, string viewName)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            ViewData["germplasmDbId"] = 0;

            return PartialView(viewName, search.GetData(serverId, moduleId, formId, fieldId, value));
        }
        /*
        public ActionResult GridViewSearchDetail(string serverId, string moduleId, string formId, string fieldId, string value)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            return PartialView("_GridViewSearchDetail", search.GetData(serverId, moduleId, formId, fieldId, value));
        }
        */
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate(string serverId, string moduleId, string formId, string fieldId, string value, string viewName)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            try
            {
                search.SaveData(serverId, moduleId, formId, fieldId, value);
            }
            catch (Exception e)
            {
                Guid d = Guid.NewGuid();
                log.Fatal(Guid.NewGuid(),e);
                
                ViewData["EditError"] = String.Format(e.Message);
            }

            return PartialView(viewName, search.GetData(serverId, moduleId, formId, fieldId, value));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdateInv(string serverId, string moduleId, string formId, string fieldId, string value, string viewName, string newInventory)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            try
            {
                search.UpdateInventorySource(serverId, moduleId, formId, fieldId, value, newInventory);
            }
            catch (Exception e)
            {
                Guid d = Guid.NewGuid();
                log.Fatal(Guid.NewGuid(), e);

                ViewData["EditError"] = String.Format(e.Message);
            }

            return PartialView(viewName, search.GetData(serverId, moduleId, formId, fieldId, value));
        }
    }
}