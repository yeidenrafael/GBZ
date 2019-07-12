using System;
using System.Web.Mvc;
using GrinGlobal.Zone.Classes;

namespace GrinGlobal.Zone.Controllers
{
    public class BatchEditController : Controller
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

            return View(search.GetData(serverId, moduleId, formId, fieldId, value));
        }

        [HttpPost]
        public ActionResult Reorder(FormCollection formdata)
        {
            DataViewsSearch search = new DataViewsSearch();

            string serverId = Session["server"].ToString();
            string moduleId = formdata["moduleId"];
            string formId = formdata["formId"];
            string fieldId = formdata["fieldId"];
            string value = formdata["value"];

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            return View("Index", search.ReorderBox(serverId, moduleId, formId, fieldId, value));
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

        public ActionResult BatchUpdateAction(string serverId, string moduleId, string formId, string fieldId, string value, string viewName)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            try
            {
                search.BatchSaveData(serverId, moduleId, formId, fieldId, value);
            }
            catch (Exception e)
            {
                Guid d = Guid.NewGuid();
                log.Fatal(Guid.NewGuid(), e);

                ViewData["EditError"] = String.Format(e.Message);
            }

            return PartialView(viewName, search.GetData(serverId, moduleId, formId, fieldId, value));

            /*DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;
            ViewData["value"] = value;

            ViewData["germplasmDbId"] = 0;

            DataTable table = search.GetData(serverId, moduleId, formId, fieldId, value);//getDataTable();

            List<string> keysToInsert = GridViewExtension.GetBatchInsertValues<string>(table.Columns[0].ColumnName);
            if (keysToInsert != null)
                //Data.InsertRows(keysToInsert);
                Console.WriteLine("Inserting");
            foreach (DataColumn column in table.Columns)
            {
                var newValues = GridViewExtension.GetBatchUpdateValues<string, string>(column.ColumnName); // S is key field type, T is the column type

                if (newValues != null && newValues.Count > 0)
                {
                    //Data.UpdateColumn(column.ColumnName, newValues);
                    foreach (string item in newValues.Keys)
                    {
                        var row = table.Rows.Find(item);
                        row[column.ColumnName] = newValues[item];
                    }
                }

                var insertValues = GridViewExtension.GetBatchInsertValues<string>(column.ColumnName);
                if (insertValues != null)
                {
                    Dictionary<string, string> dictionary = keysToInsert.ToDictionary(x => x, x => insertValues[keysToInsert.IndexOf(x)]);
                    //Data.UpdateColumn(column.ColumnName, dictionary);
                    Console.WriteLine("Updating");
                }
            }

            var deleteValues = GridViewExtension.GetBatchDeleteKeys<string>();
            if (deleteValues != null && deleteValues.Count != 0)
                //Data.RemoveRows(deleteValues);
                Console.WriteLine("Removing");

            return PartialView("_GridViewPartial", table);*/
        }

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
                log.Fatal(Guid.NewGuid(), e);

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

        public ActionResult GetGermplasmDetails(string serverId, string germplasmDbId)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["server"] = serverId;
            ViewData["germplasmDbId"] = germplasmDbId;

            return PartialView("_LoadOnDemand", search.GetGermplasmDetails(serverId, Int32.Parse(germplasmDbId)));
        }

    }
}