using System;
using System.Web.Mvc;
using GrinGlobal.Zone.Classes;
using GrinGlobal.Zone.Models;
using DevExpress.Web.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace GrinGlobal.Zone.Controllers
{
    public class StorageController : Controller
    {
        // GET: Storage
        public ActionResult Index(string moduleId, string formId)
        {
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            Session["box"] = null;
            
            return View(getDataTable());
        }

        //GGZoneModel gbZoneModel = new GGZoneModel();
        /*
        [ValidateInput(false)]
        public ActionResult GridViewPartial(string command)
        {
            List<InventoryItem> model;
            if (Session["GridBound"] == null)
            {
                if (String.IsNullOrWhiteSpace(command))
                {
                    model = new List<InventoryItem>();

                }
                else
                {
                    Session["GridBound"] = command;
                    model = new List<InventoryItem>();
                }
            }
            else
            {
                model = new List<InventoryItem>();
            }
            return PartialView("_GridViewPartial", model);
        }
        */

        [ValidateInput(false)]
        public ActionResult GridViewPartial(string Parameter/*List<InventoryItem> model*/)
        {
            return PartialView("_GridViewPartial", getDataTable()/*search.GetInventoryItems(serverId, moduleId, formId, fieldId, value)*/);
            //return PartialView("_GridViewPartial", model);
        }

        [ValidateInput(false)]
        public ActionResult NewBox(string Parameter)
        {
            Session["box"] = Parameter;
            return GridViewPartial(Parameter /*new List<InventoryItem>()*/);
        }

        [ValidateInput(false)]
        public ActionResult UpdateBox(string Parameter)
        {
            return PartialView("_GridViewPartial", getDataTable());
            //GridViewPartial(search.GetInventoryItems(serverId, moduleId, formId, fieldId, value));
        }

        [ValidateInput(false)]
        public ActionResult BatchEditingUpdateModel(MVCxGridViewBatchUpdateValues<InventoryItem, object> updateValues)
        {
            DataViewsSearch search = new DataViewsSearch();

            string serverId = Session["server"].ToString();
            string moduleId = "Inventory";
            string formId = "gbz_boxes";
            string fieldId = "inventoryNumber";

            string box = Session["box"].ToString();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;

            /*
            foreach (var inventoryItem in updateValues.Insert)
            {
                if (updateValues.IsValid(product))
                    InsertProduct(inventoryItem, updateValues);
            }
            foreach (var product in updateValues.Update)
            {
                if (updateValues.IsValid(product))
                    UpdateProduct(product, updateValues);
            }
            foreach (var productID in updateValues.DeleteKeys)
            {
                DeleteProduct(productID, updateValues);
            }*/
            search.NewBox(serverId, moduleId, formId, fieldId, updateValues.Insert, box);
            //return PartialView("_GridViewPartial", search.NewBox(serverId, moduleId, formId, fieldId, updateValues.Insert, box));//GridViewPartial();
            return RedirectToAction("Index2", "Search", new { serverId, moduleId, formId = "gbz_get_inventory", fieldId = "storageLocation", value = box }); // This is going back to another page after the info is updated
        }

        public ActionResult BatchUpdateAction()
        {
            DataTable table = getDataTable();

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

            return PartialView("_GridViewPartial", table);
        }

        private DataTable getDataTable()
        {
            DataViewsSearch search = new DataViewsSearch();

            Session["box"] = "AL04-B05-02";// Parameter;

            string serverId = Session["server"].ToString();
            string moduleId = "Inventory";
            string formId = "gbz_get_boxes";
            string fieldId = "storageLocation";
            string value = Session["box"].ToString();

            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["viewName"] = fieldId;

            return search.GetData(serverId, moduleId, formId, fieldId, value);
        }

        /*
        protected void InsertProduct(InventoryItem product, MVCxGridViewBatchUpdateValues<InventoryItem, object> updateValues)
        {
            try
            {
                //NorthwindDataProvider.InsertProduct(product);
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(product, e.Message);
            }
        }
        protected void UpdateProduct(InventoryItem product, MVCxGridViewBatchUpdateValues<InventoryItem, object> updateValues)
        {
            try
            {
                //NorthwindDataProvider.UpdateProduct(product);
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(product, e.Message);
            }
        }
        protected void DeleteProduct(object productID, MVCxGridViewBatchUpdateValues<InventoryItem, object> updateValues)
        {
            try
            {
                //NorthwindDataProvider.DeleteProduct(productID);
            }
            catch (Exception e)
            {
                updateValues.SetErrorText(productID, e.Message);
            }
        }*/
    }
}