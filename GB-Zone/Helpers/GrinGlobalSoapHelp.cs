using GrinGlobal.Zone.Models;
using System.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using System.Collections;

namespace GrinGlobal.Zone.Helpers
{
    public class GrinGlobalSoapHelp
    {
        #region private attribute
        private SettingsHelp setH;
        private readonly string PARAMETER_COLUMN_NAME = "param_name";
        private readonly string PARAMETER_DATA_TABLE_NAME = "dv_param_info";

        
        #endregion
        #region public attribute
        /// <summary>
        /// Get all configuration from the file setting
        /// </summary>
        public SettingsHelp SetH { get { return setH; } }
        #endregion
        #region constructor
        public GrinGlobalSoapHelp(string serverId, string moduleId, string formId)
        {
            setH = new SettingsHelp(serverId, moduleId, formId);
        }
        #endregion
        #region private methods
        private DataSet InitParametersFromGrinGlobal(string urlService, string dataViewName, bool suppressExceptions)
        {
            GGZoneModel ggZoneModel = new GGZoneModel();
            return ggZoneModel.GetParameters(urlService, suppressExceptions, dataViewName);
        }
        #endregion
        #region public methods
        /// <summary>
        /// Convert the inputs from form in Dictionary constructed by the parameter necessary to soap service from GrinGlobal.
        /// </summary>
        /// <param name="dataform">All inputs in form</param>
        /// <returns>
        /// Dictionary with key = name of parameter in GrinGlobal, value = value in form with the same name that parameter in GrinGlobal
        /// </returns>
        public Dictionary<string, string> GetParameters( FormCollection dataform )
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();
            string dataViewName = setH.Parameter.Element(setH.SETTING_NAME_DATAVIEW).Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            DataTable dtTable = InitParametersFromGrinGlobal(urlService, dataViewName, suppressExceptions).Tables[PARAMETER_DATA_TABLE_NAME];
            foreach (DataRow dtRow in dtTable.Rows)
            {
                string field = dtRow[PARAMETER_COLUMN_NAME].ToString();
                if (!string.IsNullOrEmpty(field))
                {
                    string value = "";
                    if (dataform != null)
                    {
                        value = dataform[field] == null ? "" : dataform[field];
                    }
                    dic.Add(field,value );
                }
            }
            return dic;
        }
        public Dictionary<string, string> GetParameters(string dataviewName)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();
            bool suppressExceptions = bool.Parse(setH.Parameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            DataTable dtTable = InitParametersFromGrinGlobal(urlService, dataviewName, suppressExceptions).Tables[PARAMETER_DATA_TABLE_NAME];
            foreach (DataRow dtRow in dtTable.Rows)
            {
                dic.Add(dtRow[0].ToString(), "");
            }
            return dic;
        }
        /// <summary>
        /// Convert the string parameters <see cref="GetStringParameter"/> in Dictionary 
        /// </summary>
        /// <param name="parameters">String parameters</param>
        /// <returns>Dictionary with parameters</returns>
        public Dictionary<string, string> ParametersStringToDictionary(string parameters)
        {
            Dictionary<string, string> dat = new Dictionary<string, string>();
            Char separator = (char)Convert.ToInt32(setH.Parameter.Element(setH.SETTING_NAME_SEPARATOR).Value);
            Char assignment = (char)Convert.ToInt32(setH.Parameter.Element(setH.SETTING_NAME_ASSIGNMENT).Value);
            string[] datos = parameters.Split(separator);
            foreach(string dato in datos)
            {
                string[] items = dato.Split(assignment);
                if (items.Count() == 2)
                {
                    dat.Add(items[0], items[1]);
                }
                else
                {
                    dat.Add(items[0], "");
                }
            }
            return dat;
        }
        /// <summary>
        /// Main function to get the DataSet from SOAP service of GringGlobal
        /// </summary>
        /// <param name="param">The parameters obtained the form, the other parameters sent to the constructor</param>
        /// <returns>Dataset from SAOP GrinGlobal</returns>
        public DataSet GetData(string param)
        {
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();//extract settings from Setting.xml
            string dataviewName = setH.Parameter.Element(setH.SETTING_NAME_DATAVIEW).Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            int offset = int.Parse(setH.Parameter.Element(setH.SETTING_NAME_OFFSET).Value);
            int limit = int.Parse(setH.Parameter.Element(setH.SETTING_NAME_LIMIT).Value);
            string options = setH.Parameter.Element(setH.SETTING_NAME_OPTIONS).Value;
            string dataviewHead = setH.ExtendedPropertie.Element("head") == null ? "" : setH.ExtendedPropertie.Element("head").Element(setH.SETTING_NAME_DATAVIEW).Value.ToString();
            DataSet dat = _GetData(param, urlService, dataviewName, suppressExceptions, offset, limit, options);
            if (!string.IsNullOrEmpty(dataviewHead))
            {
                DataSet datH = _GetData(param, urlService, dataviewHead, suppressExceptions, offset, limit, options);
                dat.Tables.Add(datH.Tables[dataviewHead].Copy());
                dat.Tables[dataviewHead].TableName = "head";
            }
            return dat;
        }
        /// <summary>
        /// Convert the Dictionary in string to insert in SOAP GrinGlobal service
        /// </summary>
        /// <param name="dic">Dictionary with key = name of parameter in GrinGlobal, value = value in form with the same name that parameter in GrinGlobal</param>
        /// <returns>
        /// String with parameter thar Dictionary
        /// </returns>
        public string GetStringParameter(Dictionary<string, string> dic)
        {
            string parameter = "";
            Char separator = (char)Convert.ToInt32(setH.Parameter.Element(setH.SETTING_NAME_SEPARATOR).Value);
            Char assignment = (char)Convert.ToInt32(setH.Parameter.Element(setH.SETTING_NAME_ASSIGNMENT).Value);
            foreach (KeyValuePair<string, string> entry in dic)
            {
                parameter += entry.Key + assignment + entry.Value + separator;
            }
            return parameter;
        }
        /// <summary>
        /// Get the DataviewAction set in settings xml, by id and add new parameters form the query
        /// </summary>
        /// <param name="originalParameter">Parameters original from query in the GetData </param>
        /// <param name="newParameter">Dictionary with new parameters specific to the action Dataview</param>
        /// <param name="idAction">Id the node to find in the setting xml</param>
        /// <returns>DateTable with the result from SOAP service</returns>
        public DataTable GetDataActionOne(string originalParameter, Dictionary<string, string> newParameter, string idAction)
        {
            DataTable datos = new DataTable();
            XElement act = setH.GetNodeAction(idAction);
            XElement nodeParameter = act.Element(setH.SETTING_NAME_PARAMETERS);
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();
            string dataviewName = nodeParameter.Element(setH.SETTING_NAME_DATAVIEW).Value;
            bool suppressExceptions = bool.Parse(nodeParameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            int offset = int.Parse(nodeParameter.Element(setH.SETTING_NAME_OFFSET).Value);
            int limit = int.Parse(nodeParameter.Element(setH.SETTING_NAME_LIMIT).Value);
            string options = nodeParameter.Element(setH.SETTING_NAME_OPTIONS).Value;
            string parameters = MergeParameterAction(originalParameter, newParameter, urlService, dataviewName, suppressExceptions);
            return  _GetData(parameters, urlService, dataviewName, suppressExceptions, offset, limit, options).Tables[dataviewName].Copy();
        }
        /// <summary>
        /// Save the dataview  modifier by the user or the palicacion
        /// </summary>
        /// <param name="parameters">Parameters form query in the GetData </param>
        /// <param name="newDataTable">Data table with change to save</param>
        /// <returns>DataSet result from service SOAP in GrinGlobal</returns>
        public DataSet SaveData(string parameters, DataTable newDataTable)
        {
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();//extract settings from Setting.xml
            string dataviewName = setH.Parameter.Element(setH.SETTING_NAME_DATAVIEW).Value;
            bool suppressExceptions = bool.Parse(setH.Parameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            int offset = int.Parse(setH.Parameter.Element(setH.SETTING_NAME_OFFSET).Value);
            int limit = int.Parse(setH.Parameter.Element(setH.SETTING_NAME_LIMIT).Value);
            string options = setH.Parameter.Element(setH.SETTING_NAME_OPTIONS).Value;
            return _SaveData(parameters, urlService, dataviewName, suppressExceptions, offset, limit, options, newDataTable);
        }
        /// <summary>
        /// Save the actionDataview  modifier by the user or the palicacion
        /// </summary>
        /// <param name="originalParameter">Parameters original from query in the GetData</param>
        /// <param name="newParameter">Dictionary with new parameters specific to the action Dataview</param>
        /// <param name="idAction">Id the node to find in the setting xml</param>
        /// <param name="newDataTable">Data table with change to save</param>
        /// <returns>DataSet result from service SOAP in GrinGlobal</returns>
        public DataSet SaveDataAction(string originalParameter, Dictionary<string, string> newParameter, string idAction ,DataTable newDataTable)
        {
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();
            XElement nodeAction = setH.GetNodeAction(idAction);
            XElement nodeParameter = nodeAction.Element(setH.SETTING_NAME_PARAMETERS);
            string dataviewName = nodeParameter.Element(setH.SETTING_NAME_DATAVIEW).Value;
            bool suppressExceptions = bool.Parse(nodeParameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            int offset = int.Parse(nodeParameter.Element(setH.SETTING_NAME_OFFSET).Value);
            int limit = int.Parse(nodeParameter.Element(setH.SETTING_NAME_LIMIT).Value);
            string options = nodeParameter.Element(setH.SETTING_NAME_OPTIONS).Value;
            string parameters = MergeParameterAction(originalParameter, newParameter, urlService, dataviewName, suppressExceptions);
            return _SaveData(parameters, urlService, dataviewName, suppressExceptions, offset, limit, options, newDataTable);
        }

        public DataTable GetCategories(string groupName)
        {
            string dataviewName = setH.GlobalCatalogue.Element(setH.SETTING_NAME_DATAVIEW).Value;
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();//extract settings from Setting.xml
            bool suppressExceptions = bool.Parse(setH.Parameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            int offset = int.Parse(setH.Parameter.Element(setH.SETTING_NAME_OFFSET).Value);
            int limit = int.Parse(setH.Parameter.Element(setH.SETTING_NAME_LIMIT).Value);
            string options = setH.Parameter.Element(setH.SETTING_NAME_OPTIONS).Value;
            Dictionary<string, string> parm = GetParameters(dataviewName);
            XElement parametersX = setH.GlobalCatalogue.Element(setH.SETTING_NAME_PARAMETERS);
            string parameterGroup = parametersX.Element(setH.SETTING_NAME_GROUPNAME).Value;
            if (parm.ContainsKey(parameterGroup))
            {
                parm[parameterGroup] = groupName;
            }
            string delimitedParams = GetStringParameter(parm);
            DataSet ds = _GetData(delimitedParams, urlService, dataviewName, suppressExceptions, offset, limit, options);
            DataTable dataTableName = ds.Tables[dataviewName];
            dataTableName.TableName = groupName;
            return dataTableName;
        }

        #endregion
        #region private methods
        private DataSet _GetDataAction(string parameters, DataSet ds)
        {
            foreach (XElement act in setH.DataViewAction)
            {
                ds.Tables.Add(GetDataActionOneByOne(act, parameters));
            }
            return ds;
        }

        private DataTable GetDataActionOneByOne(XElement actX, string parameters)
        {
            string urlService = setH.Server.Attribute(setH.SETTING_NAME_URL).Value.ToString();
            XElement nodeParameter = actX.Element(setH.SETTING_NAME_PARAMETERS);
            string dataviewName = nodeParameter.Element(setH.SETTING_NAME_DATAVIEW).Value;
            bool suppressExceptions = bool.Parse(nodeParameter.Element(setH.SETTING_NAME_SUPPRESSEXCEPTIONS).Value);
            int offset = int.Parse(nodeParameter.Element(setH.SETTING_NAME_OFFSET).Value);
            int limit = int.Parse(nodeParameter.Element(setH.SETTING_NAME_LIMIT).Value);
            string options = nodeParameter.Element(setH.SETTING_NAME_OPTIONS).Value;
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

        private string MergeParameterAction(string originalParameter,  Dictionary<string, string> newParameter,string urlService, string dataviewName, bool suppressExceptions)
        {
            Dictionary<string, string> dat = ParametersStringToDictionary(originalParameter);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            string parameters = "";
            DataTable dtTable = InitParametersFromGrinGlobal(urlService, dataviewName, suppressExceptions).Tables[PARAMETER_DATA_TABLE_NAME];
            foreach (DataRow dtRow in dtTable.Rows)
            {
                string key = dtRow[PARAMETER_COLUMN_NAME].ToString();
                if (!string.IsNullOrEmpty(key))
                {
                    string value = "";
                    if (dat.ContainsKey(key))
                    {
                        value = dat[key];
                    }
                    if (newParameter.ContainsKey(key))
                    {
                        value = newParameter[key];
                    }
                    dic.Add(key, value);
                }
            }
            parameters = GetStringParameter(dic);
            return parameters;
        }
        #endregion
    }
}