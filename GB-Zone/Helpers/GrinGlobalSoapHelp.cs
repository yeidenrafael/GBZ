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
        private DataSet parameterGrinGlobal;
        
        #endregion
        #region public attribute
        /// <summary>
        /// DataSet getting from  DataView parameters in Grin Global 
        /// </summary>
        public DataSet ParameterGrinGlobal { get { return parameterGrinGlobal; } }
        /// <summary>
        /// Get all configuration from the file setting
        /// </summary>
        public SettingsHelp SetH { get { return setH; } }
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
            string dataViewName = setH.Parameter.Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element("suppressExceptions").Value);
            parameterGrinGlobal = ggZoneModel.GetParameters(urlService, suppressExceptions, dataViewName);
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

        public DataSet GetData(string param)
        {
            string urlService = setH.Server.Attribute("url").Value.ToString();//extract settings from Setting.xml
            string dataviewName = setH.Parameter.Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element("suppressExceptions").Value);
            int offset = int.Parse(setH.Parameter.Element("offset").Value);
            int limit = int.Parse(setH.Parameter.Element("limit").Value);
            string options = setH.Parameter.Element("options").Value;
            return _GetData(param, urlService, dataviewName, suppressExceptions, offset, limit, options);
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


        public DataSet GetDataAction(string parameters,string idAction = "")
        {
            DataSet ds = GetData(parameters);
            return _GetDataAction(parameters, ds, idAction);
        }

        public DataSet SaveData(string parameters, DataTable newDataTable)
        {
            string urlService = setH.Server.Attribute("url").Value.ToString();//extract settings from Setting.xml
            string dataviewName = setH.Parameter.Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element("suppressExceptions").Value);
            int offset = int.Parse(setH.Parameter.Element("offset").Value);
            int limit = int.Parse(setH.Parameter.Element("limit").Value);
            string options = setH.Parameter.Element("options").Value;
            return _SaveData(parameters, urlService, dataviewName, suppressExceptions, offset, limit, options, newDataTable);
        }

        public DataSet SaveDataAction(string parameters,string idAction ,DataTable newDataTable)
        {
            string urlService = setH.Server.Attribute("url").Value.ToString();
            XElement nodeAction = setH.GeteNodeAction(idAction);
            XElement nodeParameter = nodeAction.Element("parameters");
            string dataviewName = nodeParameter.Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(nodeParameter.Element("suppressExceptions").Value);
            int offset = int.Parse(nodeParameter.Element("offset").Value);
            int limit = int.Parse(nodeParameter.Element("limit").Value);
            string options = nodeParameter.Element("options").Value;
            return _SaveData(parameters, urlService, dataviewName, suppressExceptions, offset, limit, options, newDataTable);
        }
        #endregion
        #region private methods
        private DataSet _GetDataAction(string parameters, DataSet ds, string idAction = "")
        {
            if(idAction == "")
            {
                foreach (XElement act in setH.DataViewAction)
                {
                    ds.Tables.Add(GetDataActionOBO(act, parameters));
                }
            }
            else
            {
                ds.Tables.Add(GetDataActionOBO(setH.GeteNodeAction(idAction), parameters));
            }
            return ds;
        }

        private DataTable GetDataActionOBO(XElement actX, string parameters)
        {
            string urlService = setH.Server.Attribute("url").Value.ToString();
            XElement nodeParameter = actX.Element("parameters");
            string dataviewName = nodeParameter.Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(nodeParameter.Element("suppressExceptions").Value);
            int offset = int.Parse(nodeParameter.Element("offset").Value);
            int limit = int.Parse(nodeParameter.Element("limit").Value);
            string options = nodeParameter.Element("options").Value;
            return _GetData(parameters, urlService, dataviewName, suppressExceptions, offset, limit, options).Tables[dataviewName].Copy();
        }

        private DataSet _GetData(string parameters, string urlService, string dataviewName, bool suppressExceptions, int offset, int limit, string options)
        {
            GGZoneModel ggZoneModel = new GGZoneModel();
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
            return ds;
        }

        private DataSet _SaveData(string parameters, string urlService, string dataviewName, bool suppressExceptions, int offset, int limit, string options, DataTable newDataTable)
        {
            GGZoneModel ggZoneModel = new GGZoneModel();
            DataSet ds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, parameters, offset, limit, options);//invoke model requesting the datatable
            ds.Tables.Remove(dataviewName);
            ds.Tables.Add(newDataTable.Copy());
            DataSet result = ggZoneModel.SaveData(urlService, suppressExceptions, ds, options);
            return result;
        }
        #endregion
    }
}