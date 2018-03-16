﻿using System;
using System.Web.Mvc;
using GrinGlobal.Zone.Classes;
using DevExpress.Web.Mvc;

namespace GrinGlobal.Zone.Controllers
{
    public class SearchController : Controller
    {

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchController));
        // GET: Search
        public ActionResult Index(string moduleId)
        {
            ViewData["moduleId"] = moduleId;

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection formdata)
        {
            DataViewsSearch search = new DataViewsSearch();

            string viewName = formdata["radios"];
            string value = formdata[String.Format("text {0}", viewName)];
            string crop = Session["crop"].ToString();
            string moduleId = formdata["moduleId"];// "Module1";

            ViewData["viewName"] = viewName;
            ViewData["value"] = value;
            ViewData["crop"] = crop;
            ViewData["moduleId"] = moduleId;

            return View(search.GetData(value, crop, viewName, moduleId));
        }

        public ActionResult Index2(string crop, string value, string viewName, string moduleId)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["viewName"] = viewName;
            ViewData["value"] = value;
            ViewData["crop"] = crop;
            ViewData["moduleId"] = moduleId;

            return View("Index", search.GetData(value, crop, viewName, moduleId));
        }

        public ActionResult GridViewSearch(string crop, string value, string viewName, string moduleId)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["viewName"] = viewName;
            ViewData["value"] = value;
            ViewData["crop"] = crop;
            ViewData["moduleId"] = moduleId;

            return PartialView("_GridViewSearch", search.GetData(value, crop, viewName, moduleId));
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewPartialUpdate(string crop, string value, string viewName, string moduleId)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["viewName"] = viewName;
            ViewData["value"] = value;
            ViewData["crop"] = crop;
            ViewData["moduleId"] = moduleId;

            try
            {
                //throw new NotImplementedException();
                search.SaveData(value, crop, viewName, moduleId);
            }
            catch (Exception e)
            {
                Guid d = Guid.NewGuid();
                log.Fatal(Guid.NewGuid(),e);
                
                ViewData["EditError"] = String.Format("Something were wrong!!\nPlease contact your system administrator.");
            }

            return PartialView("_GridViewSearch", search.GetData(value, crop, viewName, moduleId));
            /*
            DataTable model = search.GetData(value, crop, viewName);

            DataTable updatedModel = null;

            if (ModelState.IsValid)
            {
                try
                {
                    //foreach (DataRow dr in model.Rows)
                    //{
                    DataRow[] dr = model.Select("inventory_id = " + EditorExtension.GetValue<object>("inventory_id").ToString().Replace("\"", ""));

                    foreach (DataColumn col in model.Columns)
                    {
                        if (!col.ReadOnly)
                        {
                            string val = EditorExtension.GetValue<object>(col.ColumnName) as String;

                            if (val != null)
                            {
                                val = val.Replace("\"", "");
                                dr[0][col.ColumnName] = val;
                            }
                        }
                    }
                    //}

                    updatedModel = search.SaveData(model, value, crop, viewName);
                }
                catch (Exception e)
                {
                    ViewData["EditError"] = e.Message;
                }
            }
            else
                ViewData["EditError"] = "Please, correct all errors.";

            return PartialView("_GridViewSearch", updatedModel);
            */

        }

        /*
        [HttpPost, ValidateInput(false)]
        public ActionResult GridViewSearchUpdate(MVCxGridViewBatchUpdateValues<object, object> values, string crop, string value, string viewName, string moduleId)
        {
            DataViewsSearch search = new DataViewsSearch();

            ViewData["viewName"] = viewName;
            ViewData["value"] = value;
            ViewData["crop"] = crop;

            return GridViewSearch(crop, value, viewName, moduleId);
        }
        */
    }
}