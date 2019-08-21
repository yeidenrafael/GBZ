using System;
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
        public string SYSTEM_C_SHARP_VAR { get { return "systemCSharpVar"; } }
        #endregion
        #region private Attribute
        private string fileXnml { get { return System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~"), "Setting.xml"); } }
        private string fileXnmlAction { get { return System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~"), "SettingAction.xml"); } }
        private XElement xmlElement { get { return XElement.Load(fileXnml); } }
        private XElement xmlelementAction { get { return XElement.Load(fileXnmlAction); } }
        private readonly string NAME_SERVER = "server";
        private readonly string NAME_MODULE = "module";
        private readonly string GENERIC_ID = "id";
        private readonly string NAME_FORM = "form";
        private readonly string NAME_FIELD = "field";
        private readonly string NAME_ACTIONS = "actions";
        private readonly string NAME_PARAMETERS = "parameters";
        private readonly string NAME_COLUMN = "column";
        private readonly string NAME_COLUMNS = "columns";
        private readonly string NAME_EXTENDED_PROPERTIES = "extendedProperties";
        private readonly string ATTRIBUTE_JAVASCRIPT_VAR = "systemJavaScriptVar";
        private IEnumerable<XElement> fields;
        private XElement server;
        private XElement module;
        private XElement form;
        private XElement parameter;
        private XElement column;
        private XElement extendedPropertie;
        private IEnumerable<XElement> dataViewAction;
        private string dataViewName;
        private Dictionary<string, string> javascriptVariables;
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
            GetXElement(serverId, moduleId, formId);
            LoadJavascriptVariable();
        }
        #endregion
        #region private Methods 

        private void GetXElement(string serverId, string moduleId, string formId)
        {
            server = xmlElement.Elements(NAME_SERVER)
                                          .Where(w => (string)w.Attribute(GENERIC_ID) == serverId)
                                          .SingleOrDefault();
            module = (from el in server.Elements(NAME_MODULE)
                               where (string)el.Attribute(GENERIC_ID) == moduleId
                               select el).SingleOrDefault();
            form = (from el in module.Elements(NAME_FORM)
                             where (string)el.Attribute(GENERIC_ID) == formId
                             select el).SingleOrDefault();
            fields = from fi in form.Descendants(NAME_FIELD) select fi;
            XElement action = form.Elements(NAME_ACTIONS).SingleOrDefault();
            parameter = action.Elements(NAME_PARAMETERS).SingleOrDefault();
            column = action.Elements(NAME_COLUMNS).SingleOrDefault();
            extendedPropertie = action.Elements(NAME_EXTENDED_PROPERTIES).SingleOrDefault();
            dataViewName = parameter.Element("dataviewName").Value;
            dataViewAction = from el in extendedPropertie.Descendants("actionDataview") select el;
        }

       

        private void LoadJavascriptVariable()
        {
            javascriptVariables = new Dictionary<string, string>();
            foreach (XElement col in GetColumn_Attribute(ATTRIBUTE_JAVASCRIPT_VAR))
            {
                javascriptVariables.Add(col.Attribute(ATTRIBUTE_JAVASCRIPT_VAR).Value.ToString(), col.Value.ToString().Trim());
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
            XElement col = (from e in column.Descendants(NAME_COLUMN) where e.Value.Trim().ToUpper() == name.ToUpper() select e).DefaultIfEmpty(null).FirstOrDefault();
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
            List<XElement> cols = (from e in column.Descendants(NAME_COLUMN) where e.Attribute(attribute) != null select e).ToList();
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
                string id = (act.Attribute("id") != null) ? act.Attribute("id").Value.ToString().Trim() : "";
                if (idDataViewAction == id)
                {
                    IEnumerable<XElement> values = from el in act.Descendants("actionValue") select el;
                    foreach (XElement val in values)
                    {
                        string key = val.Attribute("name").Value.ToString().Trim();
                        string value = val.Attribute("value").Value.ToString().Trim();
                        string type = val.Attribute("type").Value.ToString().Trim();
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
        public string GetJavascriptVariable (string variableName)
        {
            return (javascriptVariables.ContainsKey(variableName))? javascriptVariables[variableName] : "";
        }
        /// <summary>
        /// Get the value of attribute that relacionate to the search attribute 
        /// </summary>
        /// <param name="attributeSearch">The attribute name to match with attributeSearchValue</param>
        /// <param name="attributeSearchValue">Value to match in the settings</param>
        /// <returns>
        /// Return the node in the attribute to find, if that the attribute search not found return null element
        /// </returns>
        public XElement GeteNodeAction(string attributeSearch,string attributeSearchValue)
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

        public XElement GeteNodeAction(string idAction)
        {
            XElement node = null;
            foreach (XElement act in dataViewAction)
            {
                string value = (act.Attribute("id") != null) ? act.Attribute("id").Value.ToString().Trim() : "";
                if (idAction == value)
                {
                    node = act;
                }
            }
            return node;
        }
        #endregion
    }
}