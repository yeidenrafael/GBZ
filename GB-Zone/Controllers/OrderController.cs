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
        private readonly string SYSTEM_COLUMNNAME_CHECK_LIST = "systemCheckListColumName";
        private readonly string SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW = "systemCheckToSaveAction";
        private readonly string SYSTEM_DATAVIEWACTIONNAME_ORDER_REQUEST_ITEM_ACTION = "systemOrderRequestItemAction";
        private readonly string SYSTEM_COLUMNNAME_CHECK_BEFORE = "systemCheckBefore";
        private readonly string SYSTEM_DATAVIEWACTIONVALUE_CHECK_HISTORY_ACTION = "systemCheckHistoryAction";
        private GrinGlobalSoapHelp sopH;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(SearchController));
        private int remainigElementToCheckCount = 0;
        private GridViewHelp gvH = new GridViewHelp();

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
            ViewData["titleComplement"] = parameter.ContainsKey(":orderrequestid") ? " [" + parameter[":orderrequestid"] + "]" : "";
            ViewData[SYSTEM_COLUMNNAME_CHECK_LIST] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_LIST);
            ViewData[SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW);
            ViewData[SYSTEM_COLUMNNAME_CHECK_BEFORE] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_BEFORE);
            DataSet datS = GetDataSetAction(ViewData["parameters"].ToString());
            datS = AddHistoryAction(datS);
            ViewData["jsonCheckLastSeccionItems"] = gvH.DataTableToJSONWithJavaScriptSerializer(datS.Tables[sopH.SetH.DataViewName]);
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

            ViewData[SYSTEM_COLUMNNAME_CHECK_LIST] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_LIST);
            ViewData[SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW);
            ViewData[SYSTEM_COLUMNNAME_CHECK_BEFORE] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_BEFORE);
            DataSet datS = GetDataSetAction(ViewData["parameters"].ToString());
            datS = AddHistoryAction(datS);
            ViewData["jsonCheckLastSeccionItems"] = gvH.DataTableToJSONWithJavaScriptSerializer(datS.Tables[sopH.SetH.DataViewName]);
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
            ViewData[SYSTEM_COLUMNNAME_CHECK_LIST] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_LIST);
            ViewData[SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW);
            ViewData[SYSTEM_COLUMNNAME_CHECK_BEFORE] = sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_BEFORE);
            DataSet datS = GetDataSetAction(ViewData["parameters"].ToString());
            datS = AddHistoryAction(datS);
            DataTable dt = datS.Tables[sopH.SetH.DataViewName];
            try
            {
                string columnKey = dt.PrimaryKey[0].ColumnName;
                foreach (DataColumn column in dt.Columns)
                {
                    if(column.ReadOnly == false)
                    {
                        var newValues = GridViewExtension.GetBatchUpdateValues<string, string>(column.ColumnName); // S is key field type, T is the column type
                        if (newValues != null && columnKey != column.ColumnName)
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
                }
                XElement nodeAction = sopH.SetH.GetNodeAction(sopH.SetH.SYSTEM_ACTION_DATAVIEW, SYSTEM_DATAVIEWACTIONNAME_ORDER_REQUEST_ITEM_ACTION);
                string idAction = nodeAction.Attribute(sopH.SetH.SETTING_NAME_GENERIC_ID).Value != null ? nodeAction.Attribute(sopH.SetH.SETTING_NAME_GENERIC_ID).Value.ToString() : "";
                string dataViewName = nodeAction.Element(sopH.SetH.SETTING_NAME_PARAMETERS).Element(sopH.SetH.SETTING_NAME_DATAVIEW).Value!= null? nodeAction.Element(sopH.SetH.SETTING_NAME_PARAMETERS).Element(sopH.SetH.SETTING_NAME_DATAVIEW).Value.ToString(): "";
                if (!string.IsNullOrEmpty(dataViewName))
                {
                    int ii = -1;
                    DataTable dtAct = datS.Tables[dataViewName];
                    string keyColumnAct = dtAct.PrimaryKey[0] != null ? dtAct.PrimaryKey[0].ColumnName : "";
                    string keyColum = dt.PrimaryKey[0] != null ? dt.PrimaryKey[0].ColumnName : "";
                    var query = from order in dt.AsEnumerable()
                                                 where order.Field<bool>(sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_TO_SAVE_DATAVIEW)) == true 
                                                 select order;
                    if (query.Count() > 0)
                    {
                        foreach (DataRow dtF in query)
                        {
                            DataRow dr = dtAct.NewRow();
                            dr[keyColumnAct] = ii--;
                            dr[keyColum] = dtF[keyColum].ToString();
                            dr = sopH.SetH.GetRowAction(dr, idAction);
                            dtAct.Rows.Add(dr);
                        }
                    }
                    DataSet newDaS = sopH.SaveDataAction(parameters,new Dictionary<string, string>() ,idAction, dtAct);
                }
            }
            catch (Exception e)
            {
                Guid d = Guid.NewGuid();
                log.Fatal(Guid.NewGuid(), e);
                ViewData["EditError"] = String.Format(e.Message);
            }
            DataSet newDatS = GetDataSetAction(ViewData["parameters"].ToString());
            newDatS = AddHistoryAction(newDatS);
            ViewData["jsonCheckLastSeccionItems"] = gvH.DataTableToJSONWithJavaScriptSerializer(newDatS.Tables[sopH.SetH.DataViewName]);
            return PartialView("_GridViewSearch", newDatS);
        }

        private DataSet AddHistoryAction (DataSet datS)
        {
            remainigElementToCheckCount = 0;
            string keyColumn = datS.Tables[sopH.SetH.DataViewName].PrimaryKey[0].ColumnName;
            XElement nodeAction = sopH.SetH.GetNodeAction(sopH.SetH.SYSTEM_ACTION_DATAVIEW, SYSTEM_DATAVIEWACTIONNAME_ORDER_REQUEST_ITEM_ACTION);
            string idAction = nodeAction.Attribute(sopH.SetH.SETTING_NAME_GENERIC_ID).Value != null ? nodeAction.Attribute(sopH.SetH.SETTING_NAME_GENERIC_ID).Value.ToString() : "";
            string dataViewName = nodeAction.Element(sopH.SetH.SETTING_NAME_PARAMETERS).Element(sopH.SetH.SETTING_NAME_DATAVIEW).Value != null ? nodeAction.Element(sopH.SetH.SETTING_NAME_PARAMETERS).Element(sopH.SetH.SETTING_NAME_DATAVIEW).Value.ToString() : "";
            XElement dataviewActionvalue = sopH.SetH.GetDataviewValueVariable(SYSTEM_DATAVIEWACTIONVALUE_CHECK_HISTORY_ACTION);
            string field = dataviewActionvalue.Attribute(sopH.SetH.SETTING_NAME_ACTIONVALUE_NAME).Value.ToString(); //action_name_code
            string value = dataviewActionvalue.Attribute(sopH.SetH.SETTING_NAME_ACTIONVALUE_VALUE).Value.ToString();
            var query = from order in datS.Tables[dataViewName].AsEnumerable()
                        where order.Field<string>(field) == value
                        select order;
            int max= 1;
            if (query.Count() > 0)
            {
                for (int i = 0; i < datS.Tables[sopH.SetH.DataViewName].Rows.Count; i++)
                {
                    int orderRequesItemId = Int32.Parse(datS.Tables[sopH.SetH.DataViewName].Rows[i][keyColumn].ToString());
                    var cell = (from t in query.AsEnumerable() where t.Field<int>(keyColumn) == orderRequesItemId && t.Field<string>(field) == value select t);
                    if (cell.Count() != 0)
                    {
                        datS.Tables[sopH.SetH.DataViewName].Rows[i][sopH.SetH.GetColumnVariable(SYSTEM_COLUMNNAME_CHECK_BEFORE)] = true;
                        remainigElementToCheckCount++;
                        max++;
                    }
                }
            }
            
            return datS;
        }

        private DataSet GetDataSetAction(string parameter)
        {
            DataSet datS = sopH.GetData(parameter);
            XElement nodeAction = sopH.SetH.GetNodeAction(sopH.SetH.SYSTEM_ACTION_DATAVIEW, SYSTEM_DATAVIEWACTIONNAME_ORDER_REQUEST_ITEM_ACTION);
            string idAction = nodeAction.Attribute(sopH.SetH.SETTING_NAME_GENERIC_ID).Value != null ? nodeAction.Attribute(sopH.SetH.SETTING_NAME_GENERIC_ID).Value.ToString() : "";
            datS.Tables.Add(sopH.GetDataActionOne(parameter, new Dictionary<string, string>(), idAction));
            datS = AddHistoryAction(datS);
            return datS;
        }
    }
}