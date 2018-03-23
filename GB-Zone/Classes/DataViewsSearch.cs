using DevExpress.Web.Mvc;
using GrinGlobal.Zone.Helpers;
using GrinGlobal.Zone.Models;
using IO.Swagger.Model;
using System;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace GrinGlobal.Zone.Classes
{
    public class DataViewsSearch
    {
        private string _value { get; set; }
        private string _cropId { get; set; }
        private string _viewSelected { get; set; }

        void DataviewSearch() { }

        public DataTable GetData(string value, string cropSelected, string viewSelected, string moduleId)
        {
            string cropId = cropSelected;

            int nBracket = 0;
            int nIndex = 0;
            string colName = string.Empty;

            //read the project
            /*XElement service = Settings.CropInfo(cropId)
                                  .Elements("modules")
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == viewSelected).FirstOrDefault();*/

            XElement service = Settings.Module(cropId, moduleId)
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == viewSelected).FirstOrDefault();

            //extract settings from Setting.xml
            string urlService = service.Parent.Parent.Parent.Attribute("url").Value.ToString();
            string dataviewName = service.Element("actions").Element("parameters").Element("dataviewName").Value;
            Boolean suppressExceptions = bool.Parse(service.Element("actions").Element("parameters").Element("suppressExceptions").Value);
            Int32 offset = int.Parse(service.Element("actions").Element("parameters").Element("offset").Value);
            Int32 limit = int.Parse(service.Element("actions").Element("parameters").Element("limit").Value);
            string options = service.Element("actions").Element("parameters").Element("options").Value;

            //put the value in the delimitedParameterList
            string delimitedParams = service.Element("actions").Element("parameters").Element("delimitedParameterList").Value;

            var arrValue = value.Split(new char[] { '-' });

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
                        col.ExtendedProperties.Add("view_reference", column.Attribute("viewreference").Value);
                        col.ExtendedProperties.Add("col_reference", column.Attribute("colreference").Value); 
                    }
                }

                if (col.ColumnName == "storage_location")
                {
                    col.ReadOnly = false;
                }
            }

            return ds.Tables[dataviewName];

        }

        public DataTable SaveData(string value, string cropSelected, string viewSelected, string moduleId)
        {
            string cropId = cropSelected;

            int nBracket = 0;
            int nIndex = 0;
            string colName = string.Empty;

            //read the project
            /*XElement service = Settings.CropInfo(cropId)
                                  .Elements("modules")
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == viewSelected).FirstOrDefault();*/

            XElement service = Settings.Module(cropId, moduleId)
                                  .Elements("form")
                                  .Elements("field")
                                  .Where(c => (string)c.Attribute("id") == viewSelected).FirstOrDefault();

            //extract settings from Setting.xml
            string urlService = service.Parent.Parent.Parent.Attribute("url").Value.ToString();
            string dataviewName = service.Element("actions").Element("parameters").Element("dataviewName").Value;
            Boolean suppressExceptions = bool.Parse(service.Element("actions").Element("parameters").Element("suppressExceptions").Value);
            Int32 offset = int.Parse(service.Element("actions").Element("parameters").Element("offset").Value);
            Int32 limit = int.Parse(service.Element("actions").Element("parameters").Element("limit").Value);
            string options = service.Element("actions").Element("parameters").Element("options").Value;

            //put the value in the delimitedParameterList
            string delimitedParams = service.Element("actions").Element("parameters").Element("delimitedParameterList").Value;

            var arrValue = value.Split(new char[] { '-' });

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

            //parsing storage location
            string val2 = EditorExtension.GetValue<object>("storage_location") as String;

            if (val2 != null)
            {
                val2 = val2.Replace("\"", "");

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

        public BrapiResponseBrGermplasmV2TO GetGermplasmDetails(string cropId, int germplasmDbId)
        {
            XElement service = Settings.CropInfo(cropId);

            //extract settings from Setting.xml
            string crop = service.Attribute("name").Value.ToString();


            GGZoneModel ggZoneModel = new GGZoneModel();

            var result = ggZoneModel.GetGermplasmDetails(crop, germplasmDbId);

            return result;
        }
    }
}