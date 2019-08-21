using DevExpress.Web.Mvc;
using GrinGlobal.Zone.Classes;
using GrinGlobal.Zone.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace GrinGlobal.Zone.Controllers
{
    public class OrderController : Controller
    {
        private readonly string COLUMN_NAME_CHECK_LIST= "CheckListColumName";
        private readonly string COLUMN_NAME_CHECK_TO_SAVE_DATAVIEW = "CheckToSaveAction";
        private readonly string DATAVIEW_ACTION_NAME_ORDER_REQUEST_ITEM = "orderRequestItemAction";
        private GrinGlobalSoapHelp sopH;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchController));

        public ActionResult Index(string moduleId, string formId)
        {
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            string serverId = Session["server"].ToString();

            sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            ViewData["dataViewName"] = sopH.SetH.DataViewName;
            DataSet datS = new DataSet();
            return View(datS);
        }

        [HttpPost]
        public ActionResult Index(FormCollection formdata)
        {
            string serverId = Session["server"].ToString();
            string moduleId = formdata["moduleId"];
            string formId = formdata["formId"];

            sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            Dictionary<string, string> parameter = sopH.GetParameters(formdata);
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["dataViewName"] = sopH.SetH.DataViewName;
            ViewData["parameters"] = sopH.GetStringParameter(parameter);
            ViewData[COLUMN_NAME_CHECK_LIST] = sopH.SetH.GetJavascriptVariable(COLUMN_NAME_CHECK_LIST);
            ViewData[COLUMN_NAME_CHECK_TO_SAVE_DATAVIEW] = sopH.SetH.GetJavascriptVariable(COLUMN_NAME_CHECK_TO_SAVE_DATAVIEW);
            DataSet datS = AddHistoryAction(sopH.GetDataAction(ViewData["parameters"].ToString()));
            
            return View(datS);
        }

        public ActionResult GridView(string serverId, string moduleId, string formId, string parameters)
        {
            sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["parameters"] = parameters;
            ViewData["dataViewName"] = sopH.SetH.DataViewName;
            ViewData[COLUMN_NAME_CHECK_LIST] = sopH.SetH.GetJavascriptVariable(COLUMN_NAME_CHECK_LIST);
            ViewData[COLUMN_NAME_CHECK_TO_SAVE_DATAVIEW] = sopH.SetH.GetJavascriptVariable(COLUMN_NAME_CHECK_TO_SAVE_DATAVIEW);
            DataSet datS = AddHistoryAction(sopH.GetDataAction(parameters));
            return PartialView("_GridViewSearch", datS);
        }

        public ActionResult BatchUpdateAction(string serverId, string moduleId, string formId, string parameters)
        {
            sopH = new GrinGlobalSoapHelp(serverId, moduleId, formId);
            ViewData["server"] = serverId;
            ViewData["moduleId"] = moduleId;
            ViewData["formId"] = formId;
            ViewData["parameters"] = parameters;
            ViewData["dataViewName"] = sopH.SetH.DataViewName;
            DataSet datS = AddHistoryAction(sopH.GetDataAction(parameters));
            DataTable dt = datS.Tables[sopH.SetH.DataViewName];
            try
            {
                string columnKey = "";
                foreach (DataColumn column in dt.Columns)
                {
                    if (column.ExtendedProperties["is_primary_key"].ToString() == "Y")
                    {
                        columnKey = column.ColumnName;
                    }
                }
                foreach (DataColumn column in dt.Columns)
                {
                    if(column.ReadOnly == false)
                    {
                        var newValues = GridViewExtension.GetBatchUpdateValues<string, string>(column.ColumnName); // S is key field type, T is the column type
                        if (newValues != null && columnKey != column.ColumnName)
                        {
                            foreach (var item in newValues)
                            {
                                dt = AddRow(column.ColumnName, columnKey, item.Key, item.Value, dt);
                            }
                        }
                        List<string> insertValues = GridViewExtension.GetBatchInsertValues<string>(column.ColumnName);
                        if (insertValues != null && insertValues.Count > 0)
                        {
                            int index = -1;
                            foreach (string insertV in insertValues.Where(s => !string.IsNullOrEmpty(s)).ToList())
                            {
                                dt = AddRow(column.ColumnName, columnKey, index.ToString(), insertV, dt);
                                index--;
                            }

                        }
                    }
                }
                XElement nodeAction = sopH.SetH.GeteNodeAction(sopH.SetH.SYSTEM_C_SHARP_VAR, DATAVIEW_ACTION_NAME_ORDER_REQUEST_ITEM);
                string idAction = nodeAction.Attribute("id").Value != null ? nodeAction.Attribute("id").Value.ToString() : "";
                string dataViewName = nodeAction.Element("parameters").Element("dataviewName").Value!= null? nodeAction.Element("parameters").Element("dataviewName").Value.ToString(): "";
                if (!string.IsNullOrEmpty(dataViewName))
                {
                    int ii = -1;
                    DataTable dtAct = datS.Tables[dataViewName];
                    string keyColumnAct = dtAct.PrimaryKey[0] != null ? dtAct.PrimaryKey[0].ColumnName : "";
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        bool dato = Convert.ToBoolean(dt.Rows[i][sopH.SetH.GetJavascriptVariable(COLUMN_NAME_CHECK_TO_SAVE_DATAVIEW)].ToString().ToLower());
                        if (dato == true)
                        {
                            DataRow dr = dtAct.NewRow();
                            dr[keyColumnAct] = ii--;
                            dr["order_request_item_id"] = dt.Rows[i]["order_request_item_id"].ToString();
                            //order_request_item_action - oria.started_date
                            dr = sopH.SetH.GetRowAction(dr, idAction);
                            dtAct.Rows.Add(dr);
                        }
                    }
                    DataSet newDaS = sopH.SaveDataAction(parameters, idAction, dtAct);
                }
            }
            catch (Exception e)
            {
                Guid d = Guid.NewGuid();
                log.Fatal(Guid.NewGuid(), e);
                ViewData["EditError"] = String.Format(e.Message);
            }
            return PartialView("_GridViewSearch", datS);
        }

        private DataSet AddHistoryAction (DataSet datS)
        {
            for (int i = 0; i < datS.Tables[sopH.SetH.DataViewName].Rows.Count; i++)
            {
                int orderRequesItemId = Int32.Parse(datS.Tables[sopH.SetH.DataViewName].Rows[i]["order_request_item_id"].ToString());
                var cell = (from t in datS.Tables["get_order_request_item_action"].AsEnumerable() where t.Field<int>("order_request_item_id") == orderRequesItemId select t);
                if (cell.Count() != 0)
                {
                    datS.Tables[sopH.SetH.DataViewName].Rows[i]["check_item"] = true;
                }
            }
            return datS;
        }

        private DataTable InsertRows(List<string> keysToInsert, DataTable ds)
        {
            foreach (string key in keysToInsert)
            {
                ds.Rows.Add(key);
            }
            return ds;
        }

        private DataTable UpdateColumn(string columnName, Dictionary<string, string> newValues, DataTable ds)
        {
            foreach (string item in newValues.Keys)
            {
                var row = ds.Rows.Find(item);
                row[columnName] = newValues[item];
            }
            return ds;
        }
        private DataTable AddRow(string columnName, string columnKey, string index, string newValue, DataTable dt)
        {
            DataRow dr = dt.Select(columnKey + "= " + index).DefaultIfEmpty(null).FirstOrDefault();
            if (dr != null)//Update value existe
            {
                dr[columnName] = newValue;
            }
            else// Add new row
            {
                dr = dt.NewRow();
                dr[columnKey] = index;
                dr[columnName] = newValue;
                dt.Rows.Add(dr);
            }
            return dt;
        }

    }
}