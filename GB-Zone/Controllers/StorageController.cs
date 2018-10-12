using System;
using System.Web.Mvc;
using GrinGlobal.Zone.Classes;
using System.Data;
using GrinGlobal.Zone.Models;
using DevExpress.Web.Mvc;

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

            return View(gbZoneModel.GetInventoryItems());
        }
        
        GGZoneModel gbZoneModel = new GGZoneModel();

        [ValidateInput(false)]
        public ActionResult GridViewPartial(string Parameter)
        {
            Session["box"] = Parameter;
            return PartialView("_GridViewPartial", gbZoneModel.GetInventoryItems());
        }

        public ActionResult UpdateInfo(string Parameter)
        {
            // Does some database query to update info passing lastName as a parameter
            return GridViewPartial(Parameter);//RedirectToAction("GridViewPartial"); // This is going back to another page after the info is updated
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialAddNew([ModelBinder(typeof(DevExpressEditorsBinder))] GrinGlobal.Zone.Models.InventoryItem item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to insert the new item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate([ModelBinder(typeof(DevExpressEditorsBinder))] InventoryItem item)
        {
            var model = new object[0];
            if (ModelState.IsValid)
            {
                try
                {
                    // Insert here a code to update the item in your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";
            return PartialView("_GridViewPartial", model);
        }
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialDelete(System.String EntryId)
        {
            var model = new object[0];
            if (EntryId != null)
            {
                try
                {
                    // Insert here a code to delete the item from your model
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            return PartialView("_GridViewPartial", model);
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
        }
    }
}