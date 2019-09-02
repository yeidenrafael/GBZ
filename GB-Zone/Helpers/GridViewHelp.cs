using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace GrinGlobal.Zone.Helpers
{
    public class GridViewHelp
    {


        public DataTable InsertRows(List<string> keysToInsert, DataTable ds)
        {
            foreach (string key in keysToInsert)
            {
                ds.Rows.Add(key);
            }
            return ds;
        }

        public DataTable UpdateColumn(string columnName, Dictionary<string, string> newValues, DataTable ds)
        {
            foreach (string item in newValues.Keys)
            {
                var row = ds.Rows.Find(item);
                row[columnName] = newValues[item];
            }
            return ds;
        }
        public DataTable AddRow(string columnName, string columnKey, string index, string newValue, DataTable dt)
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
        public string DataTableToJSONWithJavaScriptSerializer(DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }
    }
}