﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace GrinGlobal.Zone.Helpers
{
    /// <summary>
    /// Class to get all the values ​​embedded in the settings file.
    /// </summary>
    public class SettingsHelp
    {
        #region public Attribute
        /// <summary>
        /// Get all inputs that corresponds to the formId provided.
        /// </summary>
        /// <returns>
        /// The IEnumerable XElements that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public IEnumerable<XElement> Fields { get { return fields; } }
        /// <summary>
        /// Get all inputs that corresponds to the serverId provided.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Server { get { return server; } }
        /// <summary>
        /// Get all inputs that corresponds to the moduleId provided.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Module { get { return module; } }
        /// <summary>
        /// Get all inputs that corresponds to the formId provided.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Form { get { return form; } }
        /// <summary>
        /// Get the tag named Parameters, that corresponds to the action.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Parameter { get { return parameter; } }
        /// <summary>
        /// Get the tag named Columns, that corresponds to the action.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Column { get { return column; } }
        /// <summary>
        /// Get the tag named extendedProperties, that corresponds to the action.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement ExtendedPropertie { get { return extendedPropertie; } }
        /// <summary>
        /// Get the dataview name from the parameters
        /// </summary>
        /// <returns>
        /// The dataview name
        /// </returns>
        public string DataViewName { get { return dataViewName; } }
        /// <summary>
        /// Get the value from the DataViewAction
        /// </summary>
        /// <returns>
        /// Get IEnumerable<XElement> from Dataview Action defined in the settings
        /// </returns>
        public IEnumerable<XElement> DataViewAction { get { return dataViewAction; } }
        public DataTable DataTableAllServer { get { return dataTableAllServer; } }
        #region static 
        public string SYSTEM_ACTION_DATAVIEW { get { return "systemActionDataview"; } }
        public string SYSTEM_ACTION_DATAVIEW_VALUE { get { return "systemActionDataviewValue"; } }
        public string SYSTEM_COLUMN { get { return "systemColumn"; } }
        public string SETTING_NAME_DATAVIEW { get { return "dataviewName"; } }
        public string SETTING_NAME_GENERIC_ID { get { return "id"; } }
        public string SETTING_NAME_ACTIONVALUE { get { return "actionValue"; } }
        public string SETTING_NAME_ACTIONVALUE_NAME { get { return "name"; } }
        public string SETTING_NAME_ACTIONVALUE_VALUE { get { return "value"; } }
        public string SETTING_NAME_ACTIONVALUE_TYPE { get { return "type"; } }
        public string SETTING_NAME_ACTIONDATAVIEW { get { return "actionDataview"; } }
        public string SETTING_NAME_SERVER { get { return "server"; } }
        public string SETTING_NAME_MODULE { get { return "module"; } }
        public string SETTING_NAME_FORM { get { return "form"; } }
        public string SETTING_NAME_FIELD { get { return "field"; } }
        public string SETTING_NAME_ACTIONS { get { return "actions"; } }
        public string SETTING_NAME_URL { get { return "url"; } }
        public string SETTING_NAME_PARAMETERS { get { return "parameters"; } }
        public string SETTING_NAME_COLUMN { get { return "column"; } }
        public string SETTING_NAME_COLUMNS { get { return "columns"; } }
        public string SETTING_NAME_EXTENDEDPROPERTIES { get { return "extendedProperties"; } }
        public string SETTING_NAME_SUPPRESSEXCEPTIONS{ get { return "suppressExceptions"; } }
        public string SETTING_NAME_OFFSET { get { return "offset"; } }
        public string SETTING_NAME_LIMIT { get { return "limit"; } }
        public string SETTING_NAME_OPTIONS { get { return "options"; } }
        public string SETTING_NAME_SEPARATOR { get { return "separator"; } }
        public string SETTING_NAME_ASSIGNMENT { get { return "assignment"; } }
        public string SETTING_NAME_GLOBAL { get { return "global"; } }
        public string SETTING_NAME_CATALOGUES { get { return "catalogues"; } }
        public string SETTING_NAME_GROUPNAME { get { return "groupName"; } }
        public string SETTING_NAME_JAVASCRIPT { get { return "javascript"; } }
        public string SETTING_NAME_MAXJSONLENGTH { get { return "maxJsonLength"; } }
        public string GRINGLOBAL_PROPERTIES_NAME_ISVISIBLE { get { return "is_visible"; } }



        public XElement GlobalCatalogue { get { return globalCatalogue; } }
        public XElement GlobalParameter { get { return globalParameter; } }
        public int GlobalMaxJsonLength  { get { return globalMaxJsonLength; } }

        #endregion
        #endregion
        #region private Attribute
        private string fileXnml { get { return System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~"), "Setting.xml"); } }
        private XElement xmlElement { get { return XElement.Load(fileXnml); } }
        private IEnumerable<XElement> fields;
        private XElement server;
        private XElement module;
        private XElement form;
        private XElement parameter;
        private XElement column;
        private XElement extendedPropertie;
        private IEnumerable<XElement> dataViewAction;
        private string dataViewName;
        private Dictionary<string, string> columnVariables;
        private XElement globalCatalogue;
        private XElement globalParameter;
        private int globalMaxJsonLength;
        private DataTable dataTableAllServer;
        #endregion
        #region Constructor
        /// <summary>
        /// To find the all tags that corresponds to the <paramref name="serverId">, <param name="moduleId"> and <param name="formId"> provided.
        /// </summary>
        /// <param name="serverId">String that identifies the server the Grind-Global</param>
        /// <param name="moduleId">String that identifies the module for the aplication</param>
        /// <param name="formId">String that identifies the form for the aplication</param>
        public SettingsHelp(string serverId, string moduleId, string formId)
        {
            LoadServerCombo();
            GetXElement(serverId, moduleId, formId);
            LoadColumnVariable();
        }

        public SettingsHelp()
        {
            LoadServerCombo();
        }
        #endregion
        #region private Methods 

        private void GetXElement(string serverId, string moduleId, string formId)
        {
            server = xmlElement.Elements(SETTING_NAME_SERVER)
                                          .Where(w => (string)w.Attribute(SETTING_NAME_GENERIC_ID) == serverId)
                                          .SingleOrDefault();
            XElement global = server.Elements(SETTING_NAME_GLOBAL).SingleOrDefault();
            XElement javaS = global.Elements(SETTING_NAME_JAVASCRIPT).SingleOrDefault();
            module = (from el in server.Elements(SETTING_NAME_MODULE)
                               where (string)el.Attribute(SETTING_NAME_GENERIC_ID) == moduleId
                               select el).SingleOrDefault();
            form = (from el in module.Elements(SETTING_NAME_FORM)
                             where (string)el.Attribute(SETTING_NAME_GENERIC_ID) == formId
                             select el).SingleOrDefault();
            fields = from fi in form.Descendants(SETTING_NAME_FIELD) select fi;
            XElement action = form.Elements(SETTING_NAME_ACTIONS).SingleOrDefault();
            parameter = action.Elements(SETTING_NAME_PARAMETERS).SingleOrDefault();
            column = action.Elements(SETTING_NAME_COLUMNS).SingleOrDefault();
            extendedPropertie = action.Elements(SETTING_NAME_EXTENDEDPROPERTIES).SingleOrDefault();
            dataViewName = parameter.Element(SETTING_NAME_DATAVIEW).Value;
            dataViewAction = from el in extendedPropertie.Descendants(SETTING_NAME_ACTIONDATAVIEW) select el;
            globalCatalogue = global.Elements(SETTING_NAME_CATALOGUES).FirstOrDefault();
            globalParameter = global.Elements(SETTING_NAME_PARAMETERS).FirstOrDefault();
            string maxjson = javaS.Elements(SETTING_NAME_MAXJSONLENGTH).FirstOrDefault().Value.ToString();
            globalMaxJsonLength = string.IsNullOrEmpty(maxjson)?int.MaxValue:int.Parse(maxjson);
        }

        private void LoadColumnVariable()
        {
            columnVariables = new Dictionary<string, string>();
            foreach (XElement col in GetColumn_Attribute(SYSTEM_COLUMN))
            {
                columnVariables.Add(col.Attribute(SYSTEM_COLUMN).Value.ToString(), col.Value.ToString().Trim());
            }
        }
        #region dataview action
        private DataRow _GetElement(DataRow dr, string key, string value, string type)
        {
            switch (type)
            {
                case "dateTimeNow":
                    dr[key] = DateTime.Now;
                    break;
                case "const":
                    dr[key] = value;
                    break;
            }
            return dr;
        }
        #endregion

        private void LoadServerCombo()
        {
            dataTableAllServer = new DataTable();
            dataTableAllServer.Columns.Add("id", typeof(int));
            dataTableAllServer.Columns.Add("name", typeof(string));
            List<XElement> servers = xmlElement.Elements(SETTING_NAME_SERVER).ToList();
            foreach (XElement serv in servers)
            {
                DataRow dat = dataTableAllServer.NewRow();
                dat["id"] = Int32.Parse(serv.Attribute("id").Value);
                dat["name"] = serv.Attribute("name").Value;
                dataTableAllServer.Rows.Add(dat);
            }
        }
        #endregion
        #region public methods
        /// <summary>
        /// Find the specific column from the action, columns
        /// </summary>
        /// <param name="name">Name of column to find is automatic cast to uppercase</param>
        /// <returns>
        /// Get the column XElement or null in case not found
        /// </returns>
        public XElement GetColumn_Name(string name)
        {
            XElement col = (from e in column.Descendants(SETTING_NAME_COLUMN) where e.Value.Trim().ToUpper() == name.ToUpper() select e).DefaultIfEmpty(null).FirstOrDefault();
            return col;
        }
        /// <summary>
        /// Find all column that and spesific atribute
        /// </summary>
        /// <param name="attribute">Name to Attribute for search</param>
        /// <returns>
        /// Get the list XElement or list empty in case not found
        /// </returns>
        public List<XElement> GetColumn_Attribute(string attribute)
        {
            List<XElement> cols = (from e in column.Descendants(SETTING_NAME_COLUMN) where e.Attribute(attribute) != null select e).ToList();
            return cols;
        }
        /// <summary>
        /// Complete the DataRow with parameter from setting
        /// </summary>
        /// <param name="dr">DataRow to fill with setting</param>
        /// <param name="idDataViewAction">id the node from DataViewAction</param>
        /// <returns>
        /// Return the DataRow with values
        /// </returns>
        public DataRow GetRowAction(DataRow dr, string idDataViewAction)
        {
            foreach (XElement act in dataViewAction)
            {
                string id = (act.Attribute(SETTING_NAME_GENERIC_ID) != null) ? act.Attribute(SETTING_NAME_GENERIC_ID).Value.ToString().Trim() : "";
                if (idDataViewAction == id)
                {
                    IEnumerable<XElement> values = from el in act.Descendants(SETTING_NAME_ACTIONVALUE) select el;
                    foreach (XElement val in values)
                    {
                        string key = val.Attribute(SETTING_NAME_ACTIONVALUE_NAME).Value.ToString().Trim();
                        string value = val.Attribute(SETTING_NAME_ACTIONVALUE_VALUE).Value.ToString().Trim();
                        string type = val.Attribute(SETTING_NAME_ACTIONVALUE_TYPE).Value.ToString().Trim();
                        dr = _GetElement(dr, key, value, type);
                    }
                }
            }
            return dr;
        }
        /// <summary>
        /// Find the value form variable in javascript in the setting
        /// </summary>
        /// <param name="variableName">Value to find in the array the javaScriptVar</param>
        /// <returns>
        /// the value of name of variable or "" in case not found
        /// </returns>
        public string GetColumnVariable (string variableName)
        {
            return (columnVariables.ContainsKey(variableName))? columnVariables[variableName] : "";
        }
        /// <summary>
        /// Get the value of attribute that relacionate to the search attribute 
        /// </summary>
        /// <param name="attributeSearch">The attribute name to match with attributeSearchValue</param>
        /// <param name="attributeSearchValue">Value to match in the settings</param>
        /// <returns>
        /// Return the node in the attribute to find, if that the attribute search not found return null element
        /// </returns>
        public XElement GetNodeAction(string attributeSearch,string attributeSearchValue)
        {
            XElement node = null;
            foreach (XElement act in dataViewAction)
            {
                string value = (act.Attribute(attributeSearch) != null) ? act.Attribute(attributeSearch).Value.ToString().Trim() : "";
                if(attributeSearchValue == value)
                {
                    node = act;
                }
            }
            return node;
        }
        /// <summary>
        /// Get the node from the xml setting the name actionDataviews 
        /// </summary>
        /// <param name="idAction"> attribute id to find the node</param>
        /// <returns>XElement with node</returns>
        public XElement GetNodeAction(string idAction)
        {
            XElement node = null;
            foreach (XElement act in dataViewAction)
            {
                string value = (act.Attribute(SETTING_NAME_GENERIC_ID) != null) ? act.Attribute(SETTING_NAME_GENERIC_ID).Value.ToString().Trim() : "";
                if (idAction == value)
                {
                    node = act;
                }
            }
            return node;
        }
        /// <summary>
        /// Get any node with attribute idSystemDataviewValue  
        /// </summary>
        /// <param name="idSystemDataviewValue">Id to find node</param>
        /// <returns>XElement node find</returns>
        public XElement GetDataviewValueVariable(string idSystemDataviewValue)
        {
            XElement node = null;
            foreach (XElement act in dataViewAction)
            {
                IEnumerable<XElement> nodes = from var in act.Elements(SETTING_NAME_ACTIONVALUE)
                                              where (string)var.Attribute(SYSTEM_ACTION_DATAVIEW_VALUE) == idSystemDataviewValue
                                              select var;
                if((nodes.Count() > 0))
                {
                    node = nodes.First();
                }
                
            }
            return  node;
        }
        #endregion
    }
}