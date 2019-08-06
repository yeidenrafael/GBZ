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
            string fieldId = formdata["radios"];
            string value = formdata[String.Format("text {0}", fieldId)];


            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;
            return View();
        }
    }
}