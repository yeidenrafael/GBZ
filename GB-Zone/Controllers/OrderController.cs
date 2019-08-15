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
            string serverId = Session["server"].ToString();

            GrinGlobalSoapHelp sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            ViewData["dataViewName"] = sopH.DataViewName;
            DataSet datS = new DataSet();
            return View(datS);
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
            ViewData["dataViewName"] = sopH.DataViewName;
            ViewData["parameters"] = sopH.GetStringParameter(parameter);
            ViewData[COLUMN_NAME_CHECK_LIST] = sopH.JavascriptVariables.ContainsKey(COLUMN_NAME_CHECK_LIST)?sopH.JavascriptVariables[COLUMN_NAME_CHECK_LIST]:"";
            DataSet datS = new DataSet();
            DataTable dt = sopH.GetData(parameter);
            datS.Tables.Add(dt.Copy());
            return View(datS);
        }

        public ActionResult GridView(string serverId, string moduleId, string formId, string viewName, string parameters)
        {
            GrinGlobalSoapHelp sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["parameters"] = parameters;
            ViewData["dataViewName"] = sopH.DataViewName;
            ViewData[COLUMN_NAME_CHECK_LIST] = sopH.JavascriptVariables.ContainsKey(COLUMN_NAME_CHECK_LIST) ? sopH.JavascriptVariables[COLUMN_NAME_CHECK_LIST] : "";
            DataSet datS = new DataSet();
            DataTable dt = sopH.GetData(parameters);
            datS.Tables.Add(dt.Copy());
            return PartialView("_GridViewSearch", datS);
        }


    }
}