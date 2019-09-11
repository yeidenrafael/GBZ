using DevExpress.Web.Mvc;
using GrinGlobal.Zone.Classes;
using GrinGlobal.Zone.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using GrinGlobal.Zone.Helpers;

namespace GrinGlobal.Zone.Controllers
{
    public class BoxController : Controller
    {
        private string SETTING_COLUMN_DISABLE_READ_ONLY = "inventory_number"; // get by setting 
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchController));
        public const string EditErrorKey = "EditError";
        private GridViewHelp gvH = new GridViewHelp();
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
            value = "-" + value;//add de first value equal to ""
            DataTable ds = search.GetData(serverId, moduleId, formId, fieldId, value);
            ds.Columns[SETTING_COLUMN_DISABLE_READ_ONLY].ReadOnly = false;
            return View(ds);
        }
        public ActionResult GridView(string serverId, string moduleId, string formId, string fieldId, string value, string viewName)
        {
            DataViewsSearch search = new DataViewsSearch();
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;
            value = "-" + value;//add de first value equal to ""
            DataTable ds = search.GetData(serverId, moduleId, formId, fieldId, value);
            ds.Columns[SETTING_COLUMN_DISABLE_READ_ONLY].ReadOnly = false;
            return PartialView(viewName, ds);
        }

        public ActionResult BatchUpdateAction(string serverId, string moduleId, string formId, string fieldId, string value, string viewName)
        {
            DataViewsSearch search = new DataViewsSearch(); // Get parameter for set values in page to relation by setting
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;
            DataTable dt = new DataTable();
            try
            {
                string columnKey = "";
                dt = search.GetData(serverId, moduleId, formId, fieldId, "-" + value);
                dt.Columns[SETTING_COLUMN_DISABLE_READ_ONLY].ReadOnly = false;// change for automatic in getData
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ExtendedProperties["is_primary_key"].ToString() == "Y")
                    {
                        columnKey = column.ColumnName;
                    }
                }
                foreach (DataColumn column in dt.Columns)
                {
                    var newValues = GridViewExtension.GetBatchUpdateValues<string, string>(column.ColumnName); // S is key field type, T is the column type
                    if (newValues != null && SETTING_COLUMN_DISABLE_READ_ONLY != column.ColumnName)
                    {
                        foreach (var item in newValues)
                        {
                            dt = gvH.AddRow(column.ColumnName, columnKey, item.Key, item.Value, dt);
                        }
                    }
                    List<string> insertValues = GridViewExtension.GetBatchInsertValues<string>(column.ColumnName);
                    if (insertValues != null && insertValues.Count > 0)
                    {
                        int index = -1;
                        foreach (string insertV in insertValues.Where(s => !string.IsNullOrEmpty(s)).ToList())
                        {
                            dt = gvH.AddRow(column.ColumnName, columnKey, index.ToString(), insertV, dt);
                            index--;
                        }
                    }
                }
                search.BoxBatchSave(serverId, moduleId, formId, fieldId, dt, value);
            }
            catch (Exception e)
            {
                Guid d = Guid.NewGuid();
                log.Fatal(Guid.NewGuid(), e);
                string[] spearator = { ":line", "--->" };
                string[] lineas = e.Message.Split(spearator, StringSplitOptions.RemoveEmptyEntries);
                string error = "";
                if (lineas.Count() > 1)
                {
                    error = lineas[1];
                }
                if (string.IsNullOrEmpty(error))
                {
                    error = String.Format(e.Message);
                }
                ViewData[EditErrorKey] = error;
            }
            if (ViewData[EditErrorKey] == null)
            {
                return RedirectToAction("Index2", "Search", new { serverId, moduleId, formId = "gbz_get_inventory", fieldId = "storageLocation", value = value }); // This is going back to another page after the info is updated
            }
            else
            {
                DataTable ds = search.GetData(serverId, moduleId, formId, fieldId, "-" + value);
                ds.Columns[SETTING_COLUMN_DISABLE_READ_ONLY].ReadOnly = false;
                return PartialView("_GridViewSearch", ds);
            }
        }
    }
}