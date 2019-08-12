using GrinGlobal.Zone.Classes;
using GrinGlobal.Zone.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GrinGlobal.Zone.Controllers
{
    public class OrderController : Controller
    {
        private readonly string COLUMN_NAME_CHECK_LIST= "CheckListColumName";
        public ActionResult Index(string moduleId, string formId)
        {
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formdata)
        {
            string serverId = Session["server"].ToString();
            string moduleId = formdata["moduleId"];
            string formId = formdata["formId"];

            GrinGlobalSoapHelp sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            Dictionary<string, string> parameter = sopH.GetParameters(formdata);
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["parameters"] = sopH.GetStringParameter(parameter);
//            ViewData["viewName"] = 
            ViewData[COLUMN_NAME_CHECK_LIST] = sopH.JavascriptVariables.ContainsKey(COLUMN_NAME_CHECK_LIST)?sopH.JavascriptVariables[COLUMN_NAME_CHECK_LIST]:"";
            return View(sopH.GetData(parameter));
        }
        public ActionResult GridView(string serverId, string moduleId, string formId, string viewName, string parameters)
        {
            GrinGlobalSoapHelp sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["parameters"] = parameters;
            ViewData[COLUMN_NAME_CHECK_LIST] = sopH.JavascriptVariables.ContainsKey(COLUMN_NAME_CHECK_LIST) ? sopH.JavascriptVariables[COLUMN_NAME_CHECK_LIST] : "";
            return PartialView("_GridViewSearch", sopH.GetData(parameters));
        }
    }
}