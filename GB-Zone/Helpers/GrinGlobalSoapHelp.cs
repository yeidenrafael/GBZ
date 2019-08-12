using GrinGlobal.Zone.Models;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace GrinGlobal.Zone.Helpers
{
    public class GrinGlobalSoapHelp
    {
        #region private attribute
        private SettingsHelp setH;
        private readonly string PARAMETER_COLUMN_NAME = "param_name";
        private readonly string PARAMETER_DATA_TABLE_NAME = "dv_param_info";
        private readonly string ATTRIBUTE_JAVASCRIPT_VAR = "javaScriptVar";
        private DataSet parameterGrinGlobal;
        private Dictionary<string, string> javascriptVariables;
        #endregion
        #region public attribute
        /// <summary>
        /// DataSet getting from  DataView parameters in Grin Global 
        /// </summary>
        public DataSet ParameterGrinGlobal { get { return parameterGrinGlobal; } }
        public Dictionary<string,string> JavascriptVariables { get { return javascriptVariables; } }
        #endregion
        #region constructor
        public GrinGlobalSoapHelp(string serverId, string moduleId, string formId)
        {
            setH = new SettingsHelp(serverId, moduleId, formId);
            InitParametersFromGrinGlobal();
        }
        #endregion
        #region private methods
        private void InitParametersFromGrinGlobal()
        {
            GGZoneModel ggZoneModel = new GGZoneModel();
            string urlService = setH.Server.Attribute("url").Value.ToString();
            string dataviewName = setH.Parameter.Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element("suppressExceptions").Value);
            parameterGrinGlobal = ggZoneModel.GetParameters(urlService, suppressExceptions, dataviewName);
            LoadJavascriptVariable();
        }


        private void LoadJavascriptVariable()
        {
            javascriptVariables = new Dictionary<string, string>();
            foreach(XElement col in setH.GetColumn_Attribute(ATTRIBUTE_JAVASCRIPT_VAR))
            {
                javascriptVariables.Add(col.Attribute(ATTRIBUTE_JAVASCRIPT_VAR).Value.ToString(), col.Value.ToString().Trim());
            }
        }
        #endregion
        #region public methods
        public Dictionary<string, string> GetParameters(FormCollection dataform)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            DataTable dtTable = parameterGrinGlobal.Tables[PARAMETER_DATA_TABLE_NAME];
            foreach (DataRow dtRow in dtTable.Rows)
            {
                string field = dtRow[PARAMETER_COLUMN_NAME].ToString();
                if (!string.IsNullOrEmpty(field))
                {
                    string value = dataform[field] == null ? "" : dataform[field];
                    dic.Add(field,value );
                }
            }
            return dic;
        }

        public DataTable GetData(Dictionary<string, string> dic)
        {
            return _GetData(GetStringParameter(dic));
        }

        public DataTable GetData(string param)
        {
            return _GetData(param);
        }

        public DataTable _GetData(string parameters)
        {
            GGZoneModel ggZoneModel = new GGZoneModel();
            string urlService = setH.Server.Attribute("url").Value.ToString();//extract settings from Setting.xml
            string dataviewName = setH.Parameter.Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element("suppressExceptions").Value);
            int offset = int.Parse(setH.Parameter.Element("offset").Value);
            int limit = int.Parse(setH.Parameter.Element("limit").Value);
            string options = setH.Parameter.Element("options").Value;
            DataSet ds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, parameters, offset, limit, options);             //invoke model requesting the datatable
            foreach (DataColumn col in ds.Tables[dataviewName].Columns)
            {
                if (col.ExtendedProperties["is_visible"].ToString() == "N")
                {
                    col.ColumnMapping = MappingType.Hidden;
                }
                XElement column = setH.GetColumn_Name(col.ColumnName.Trim());
                if (column != null)
                {
                    if (column.Attribute("header") != null && bool.Parse(column.Attribute("header").Value))//create extendproperties
                    {
                        col.ExtendedProperties.Add("is_header", true);
                    }
                    if (column.Attribute("link") != null && bool.Parse(column.Attribute("link").Value))
                    {
                        col.ExtendedProperties.Add("moduleRef", column.Attribute("moduleRef").Value);
                        col.ExtendedProperties.Add("formRef", column.Attribute("formRef").Value);
                        col.ExtendedProperties.Add("fieldRef", column.Attribute("fieldRef").Value);
                        col.ExtendedProperties.Add("colRef", column.Attribute("colRef").Value);
                    }
                    if (column.Attribute("readOnly") != null)
                    {
                        col.ReadOnly = bool.Parse(column.Attribute("readOnly").Value);
                    }
                }
            }
            if (setH.ExtendedPropertie.Element("masterDetail") != null)
            {
                ds.Tables[dataviewName].ExtendedProperties.Add("masterDetail", true);
                ds.Tables[dataviewName].ExtendedProperties.Add("actionName", setH.ExtendedPropertie.Element("masterDetail").Attribute("actionName").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("moduleRef", setH.ExtendedPropertie.Element("masterDetail").Attribute("moduleRef").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("formRef", setH.ExtendedPropertie.Element("masterDetail").Attribute("formRef").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("fieldRef", setH.ExtendedPropertie.Element("masterDetail").Attribute("fieldRef").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("colRef", setH.ExtendedPropertie.Element("masterDetail").Attribute("colRef").Value);
            }
            return ds.Tables[dataviewName];
        }
        public string GetStringParameter(Dictionary<string, string> dic)
        {
            string parameter = "";
            Char separator = (char)Convert.ToInt32(setH.Parameter.Element("separator").Value);
            Char assignment = (char)Convert.ToInt32(setH.Parameter.Element("assignment").Value);
            foreach (KeyValuePair<string, string> entry in dic)
            {
                parameter += entry.Key + assignment + entry.Value + separator;
            }
            return parameter;
        }
        #endregion
    }
}