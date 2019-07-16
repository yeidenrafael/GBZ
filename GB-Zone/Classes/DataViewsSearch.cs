using DevExpress.Web.Mvc;
using GrinGlobal.Zone.Helpers;
using GrinGlobal.Zone.Models;
//using IO.Swagger.Model;
using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;

namespace GrinGlobal.Zone.Classes
{
    public class DataViewsSearch
    {
        private string _value { get; set; }
        private string _cropId { get; set; }
        private string _viewSelected { get; set; }

        void DataviewSearch() { }

        public DataTable GetData(string serverId, string moduleId, string formId, string fieldId, string value)
        {
            //string cropId = serverId;

            int nBracket = 0;
            int nIndex = 0;
            string colName = string.Empty;

            //read the project
            /*XElement service = Settings.CropInfo(cropId)
                                  .Elements("modules")
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == viewSelected).FirstOrDefault();*/

            /*XElement service = Settings.Module(serverId, moduleId)
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();*/
            XElement service = Settings.Form(serverId, moduleId, formId)
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();

            //extract settings from Setting.xml
            string urlService = service.Parent.Parent.Parent.Attribute("url").Value.ToString();
            string dataviewName = service.Element("actions").Element("parameters").Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(service.Element("actions").Element("parameters").Element("suppressExceptions").Value);
            int offset = int.Parse(service.Element("actions").Element("parameters").Element("offset").Value);
            int limit = int.Parse(service.Element("actions").Element("parameters").Element("limit").Value);
            string options = service.Element("actions").Element("parameters").Element("options").Value;

            //put the value in the delimitedParameterList
            string delimitedParams = service.Element("actions").Element("parameters").Element("delimitedParameterList").Value;
            Char separator = (char) Convert.ToInt32(service.Element("actions").Element("parameters").Element("separator").Value);
                        
            var arrValue = value.Split(separator);

            while ((nBracket = delimitedParams.IndexOf("{0}", nBracket)) != -1)
            {
                delimitedParams = delimitedParams.Remove(nBracket, 3).Insert(nBracket, arrValue[nIndex]);
                nBracket++;
                nIndex++;
            }

            GGZoneModel ggZoneModel = new GGZoneModel();

            //invoke model requesting the datatable
            DataSet ds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, delimitedParams, offset, limit, options);
            
            //remove or add column
            foreach (DataColumn col in ds.Tables[dataviewName].Columns)
            {
                if (col.ExtendedProperties["is_visible"].ToString() == "N")
                {
                    col.ColumnMapping = MappingType.Hidden;
                }

                XElement column = (from e in service.Element("actions").Elements("columns").Descendants("column")
                                where e.Value.Trim().ToUpper() == col.ColumnName.Trim().ToUpper()
                                select e).FirstOrDefault();

                if (column != null)
                {
                    //create extendproperties
                    if (column.Attribute("header") != null && bool.Parse(column.Attribute("header").Value))
                    {
                        col.ExtendedProperties.Add("is_header", true);
                    }
                    if (column.Attribute("link") != null && bool.Parse(column.Attribute("link").Value))
                    {
                        //moduleRef="Inventory" formRef="gbz_get_inventory" fieldRef="intrid" colRef="inventory_number_part1"
                        col.ExtendedProperties.Add("moduleRef", column.Attribute("moduleRef").Value);
                        col.ExtendedProperties.Add("formRef", column.Attribute("formRef").Value);
                        col.ExtendedProperties.Add("fieldRef", column.Attribute("fieldRef").Value);
                        col.ExtendedProperties.Add("colRef", column.Attribute("colRef").Value);
                    }
                }

                if (col.ColumnName == "storage_location")
                {
                    col.ReadOnly = false;
                }

                /*if (col.ColumnName == "inventory_number" && formId == "gbz_get_boxes")
                {
                    col.ReadOnly = false;
                }*/
            }

            
            if(service.Element("actions").Element("extendedProperties").Element("masterDetail") != null)
            {
                ds.Tables[dataviewName].ExtendedProperties.Add("masterDetail", true);
                ds.Tables[dataviewName].ExtendedProperties.Add("actionName", service.Element("actions").Element("extendedProperties").Element("masterDetail").Attribute("actionName").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("moduleRef", service.Element("actions").Element("extendedProperties").Element("masterDetail").Attribute("moduleRef").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("formRef", service.Element("actions").Element("extendedProperties").Element("masterDetail").Attribute("formRef").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("fieldRef", service.Element("actions").Element("extendedProperties").Element("masterDetail").Attribute("fieldRef").Value);
                ds.Tables[dataviewName].ExtendedProperties.Add("colRef", service.Element("actions").Element("extendedProperties").Element("masterDetail").Attribute("colRef").Value);
            }

            return ds.Tables[dataviewName];

        }

        internal DataTable ReorderBox(string serverId, string moduleId, string formId, string fieldId, string value)
        {
            DataTable dt = GetData(serverId, moduleId, formId, fieldId, value);
            int count = 1;
            string fmt = "000";
            foreach (DataRow row in dt.Rows)
            {
                row["storage_location_part4"] = count.ToString(fmt);
                count++;
            }
            return dt;
        }

        internal List<InventoryItem> GetInventoryItems(string serverId, string moduleId, string formId, string fieldId, string value)
        {
            DataTable dataTableName = GetData(serverId, moduleId, formId, fieldId, value);

            List<InventoryItem> listName = dataTableName.AsEnumerable().Select(m => new InventoryItem()
            {
                EntryId = m.Field<string>("storage_location_part4"),
                InventoryNumber = m.Field<string>("inventory_number"),
            }).ToList();

            return listName;
        }

        public DataTable SaveData(string serverId, string moduleId, string formId, string fieldId, string value)
        {
            //string cropId = cropSelected;

            int nBracket = 0;
            int nIndex = 0;
            string colName = string.Empty;

            //read the project
            /*XElement service = Settings.CropInfo(cropId)
                                  .Elements("modules")
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == viewSelected).FirstOrDefault();*/

            /*XElement service = Settings.Module(serverId, moduleId)
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();*/
            XElement service = Settings.Form(serverId, moduleId, formId)
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();

            //extract settings from Setting.xml
            string urlService = service.Parent.Parent.Parent.Attribute("url").Value.ToString();
            string dataviewName = service.Element("actions").Element("parameters").Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(service.Element("actions").Element("parameters").Element("suppressExceptions").Value);
            int offset = int.Parse(service.Element("actions").Element("parameters").Element("offset").Value);
            int limit = int.Parse(service.Element("actions").Element("parameters").Element("limit").Value);
            string options = service.Element("actions").Element("parameters").Element("options").Value;

            //put the value in the delimitedParameterList
            string delimitedParams = service.Element("actions").Element("parameters").Element("delimitedParameterList").Value;
            Char separator = (char)Convert.ToInt32(service.Element("actions").Element("parameters").Element("separator").Value);

            var arrValue = value.Split(separator);

            while ((nBracket = delimitedParams.IndexOf("{0}", nBracket)) != -1)
            {
                delimitedParams = delimitedParams.Remove(nBracket, 3).Insert(nBracket, arrValue[nIndex]);
                nBracket++;
                nIndex++;
            }

            GGZoneModel ggZoneModel = new GGZoneModel();

            //invoke model requesting the datatable
            DataSet oldds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, delimitedParams, offset, limit, options);

            DataTable model = oldds.Tables[dataviewName];

            var primaryKey = model.PrimaryKey[0].ColumnName;

            DataRow[] dr = model.Select(primaryKey + " = " + GridViewExtension.GetEditValue<dynamic>(primaryKey));// EditorExtension.GetValue<object>(primaryKey).ToString().Replace("\"", ""));

            foreach (DataColumn col in model.Columns)
            {
                if (!col.ReadOnly)
                {
                    //dr[0][col.ColumnName] = GridViewExtension.GetEditValue<dynamic>(col.ColumnName);
                    
                    var val = GridViewExtension.GetEditValue<dynamic>(col.ColumnName);

                    if (val != null)
                    {
                        dr[0][col.ColumnName] = val;
                    }
                    
                }
            }

            //parsing storage location
            string val2 = GridViewExtension.GetEditValue<dynamic>("storage_location");
            //string val2 = EditorExtension.GetValue<object>("storage_location") as String;

            if (val2 != null)
            {
                //val2 = val2.Replace("\"", "");

                var arrValue2 = val2.Split(new char[] { '-' });

                dr[0]["storage_location_part1"] = arrValue2[0];
                dr[0]["storage_location_part2"] = arrValue2[1];
                dr[0]["storage_location_part3"] = arrValue2[2];
                dr[0]["storage_location_part4"] = arrValue2[3];
            }

            DataSet result = ggZoneModel.SaveData(urlService, suppressExceptions, oldds, options);
            
            /*
            DataSet ds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, delimitedParams, offset, limit, options);
            //remove or add column
            foreach (DataColumn col in ds.Tables[dataviewName].Columns)
            {
                if (col.ExtendedProperties["is_visible"].ToString() == "N")
                {
                    col.ColumnMapping = MappingType.Hidden;
                }

                XElement column = (from e in service.Element("actions").Elements("columns").Descendants("column")
                                   where e.Value.Trim().ToUpper() == col.ColumnName.Trim().ToUpper()
                                   select e).FirstOrDefault();

                if (column != null)
                {
                    //create extendproperties
                    if (column.Attribute("header") != null && bool.Parse(column.Attribute("header").Value))
                    {
                        col.ExtendedProperties.Add("is_header", true);
                    }
                    if (column.Attribute("link") != null && bool.Parse(column.Attribute("link").Value))
                    {
                        col.ExtendedProperties.Add("view_reference", column.Attribute("viewreference").Value);
                        col.ExtendedProperties.Add("col_reference", column.Attribute("colreference").Value);
                    }
                }
            }

            return ds.Tables[dataviewName];
            */
            return result.Tables[dataviewName];
        }

        public DataTable BatchSaveData(string serverId, string moduleId, string formId, string fieldId, string value)
        {
            //string cropId = cropSelected;

            int nBracket = 0;
            int nIndex = 0;
            string colName = string.Empty;

            //read the project
            /*XElement service = Settings.CropInfo(cropId)
                                  .Elements("modules")
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == viewSelected).FirstOrDefault();*/

            /*XElement service = Settings.Module(serverId, moduleId)
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();*/
            XElement service = Settings.Form(serverId, moduleId, formId)
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();

            //extract settings from Setting.xml
            string urlService = service.Parent.Parent.Parent.Attribute("url").Value.ToString();
            string dataviewName = service.Element("actions").Element("parameters").Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(service.Element("actions").Element("parameters").Element("suppressExceptions").Value);
            int offset = int.Parse(service.Element("actions").Element("parameters").Element("offset").Value);
            int limit = int.Parse(service.Element("actions").Element("parameters").Element("limit").Value);
            string options = service.Element("actions").Element("parameters").Element("options").Value;

            //put the value in the delimitedParameterList
            string delimitedParams = service.Element("actions").Element("parameters").Element("delimitedParameterList").Value;
            Char separator = (char)Convert.ToInt32(service.Element("actions").Element("parameters").Element("separator").Value);

            var arrValue = value.Split(separator);

            while ((nBracket = delimitedParams.IndexOf("{0}", nBracket)) != -1)
            {
                delimitedParams = delimitedParams.Remove(nBracket, 3).Insert(nBracket, arrValue[nIndex]);
                nBracket++;
                nIndex++;
            }

            GGZoneModel ggZoneModel = new GGZoneModel();

            //invoke model requesting the datatable
            DataSet oldds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, delimitedParams, offset, limit, options);

            //DataTable model = oldds.Tables[dataviewName];

            DataTable table = oldds.Tables[dataviewName]; //search.GetData(serverId, moduleId, formId, fieldId, value);//getDataTable();

            List<string> keysToInsert = GridViewExtension.GetBatchInsertValues<string>("storage_location_part4"/*table.Columns[0].ColumnName*/);
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
                        if (!column.ReadOnly)
                        {
                            row[column.ColumnName] = newValues[item];
                        }
                    }
                }

                var insertValues = GridViewExtension.GetBatchInsertValues<string>(column.ColumnName);
                if (insertValues != null && insertValues.Count> 0)
                //if (insertValues != null )
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
            
            
            DataSet result = ggZoneModel.SaveData(urlService, suppressExceptions, oldds, options);
            
            return result.Tables[dataviewName];
        }

        internal void UpdateInventorySource(string serverId, string moduleId, string formId, string fieldId, string value, string inventoryId)
        {
            /*XElement service = Settings.Module(serverId, moduleId)
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();*/
            XElement service = Settings.Form(serverId, moduleId, formId)
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();

            //extract settings from Setting.xml
            string urlService = service.Parent.Parent.Parent.Attribute("url").Value.ToString();
            bool suppressExceptions = bool.Parse(service.Element("actions").Element("parameters").Element("suppressExceptions").Value);
            string dataviewName = "gbz_get_order_request_item";
            string delimitedParameterList = ":orderrequestid=;:orderrequestitemid=" + value;
            int offset = int.Parse(service.Element("actions").Element("parameters").Element("offset").Value);
            int limit = int.Parse(service.Element("actions").Element("parameters").Element("limit").Value);
            string options = service.Element("actions").Element("parameters").Element("options").Value;

            GGZoneModel ggZoneModel = new GGZoneModel();

            //invoke model requesting the datatable
            DataSet ds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, delimitedParameterList, offset, limit, options);

            ds.Tables[dataviewName].Rows[0]["inventory_id"] = inventoryId;

            ggZoneModel.SaveData(urlService, suppressExceptions, ds, options);
        }

        //public BrapiResponseBrGermplasmV2TO GetGermplasmDetails(string cropId, int germplasmDbId)
        //{
        //    XElement service = Settings.Server(cropId);

        //    //extract settings from Setting.xml
        //    string crop = service.Attribute("name").Value.ToString();


        //    GGZoneModel ggZoneModel = new GGZoneModel();

        //    var result = ggZoneModel.GetGermplasmDetails(crop, germplasmDbId);

        //    return result;
        //}

        internal List<InventoryItem> NewBox(string serverId, string moduleId, string formId, string fieldId, List<InventoryItem> insert, string box)
        {
            int nBracket = 0;
            int nIndex = 0;
            string colName = string.Empty;

            //read the project
            XElement service = Settings.Form(serverId, moduleId, formId)
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == fieldId).FirstOrDefault();

            //extract settings from Setting.xml
            string urlService = service.Parent.Parent.Parent.Attribute("url").Value.ToString();
            string dataviewName = service.Element("actions").Element("parameters").Element("dataviewName").Value;
            bool suppressExceptions = bool.Parse(service.Element("actions").Element("parameters").Element("suppressExceptions").Value);
            int offset = int.Parse(service.Element("actions").Element("parameters").Element("offset").Value);
            int limit = int.Parse(service.Element("actions").Element("parameters").Element("limit").Value);
            string options = service.Element("actions").Element("parameters").Element("options").Value;

            //put the value in the delimitedParameterList
            string delimitedParams = service.Element("actions").Element("parameters").Element("delimitedParameterList").Value;
            Char separator = (char)Convert.ToInt32(service.Element("actions").Element("parameters").Element("separator").Value);

            string value = "";

            foreach (var inventoryItem in insert)
            {
                if (/*updateValues.IsValid(inventoryItem) && */inventoryItem.InventoryNumber != null)
                {
                    if (value == "")
                        value = inventoryItem.InventoryNumber;
                    else
                        value += "','" + inventoryItem.InventoryNumber;
                }
            }

            var arrValue = value.Split(separator);

            while ((nBracket = delimitedParams.IndexOf("{0}", nBracket)) != -1)
            {
                delimitedParams = delimitedParams.Remove(nBracket, 3).Insert(nBracket, arrValue[nIndex]);
                nBracket++;
                nIndex++;
            }

            GGZoneModel ggZoneModel = new GGZoneModel();

            //invoke model requesting the datatable
            DataSet ds = ggZoneModel.GetData(urlService, suppressExceptions, dataviewName, delimitedParams, offset, limit, options);

            DataTable model = ds.Tables[dataviewName];
            //string box = "";
            var inventoryItems = new List<InventoryItem>();
            var arrValue2 = box.ToUpper().Split(new char[] { '-' });

            foreach (var inventoryItem in insert)
            {
                if (/*updateValues.IsValid(inventoryItem) && */inventoryItem.InventoryNumber != null)
                {
                    DataRow[] dr = model.Select("inventory_number = '" + inventoryItem.InventoryNumber + "'");

                    //parsing storage location
                    //string val2 = inventoryItem.Box;

                    //if (val2 != null)
                    //{
                    //var arrValue2 = box.ToUpper().Split(new char[] { '-' });

                    dr[0]["storage_location_part1"] = arrValue2[0];
                    dr[0]["storage_location_part2"] = arrValue2[1];
                    dr[0]["storage_location_part3"] = arrValue2[2];
                    //}
                    dr[0]["storage_location_part4"] = inventoryItem.EntryId;//insert.IndexOf(inventoryItem) + 1;

                    inventoryItems.Add(inventoryItem);
                }
            }

            ggZoneModel.SaveData(urlService, suppressExceptions, ds, options);

            return inventoryItems;
        }
    }
}