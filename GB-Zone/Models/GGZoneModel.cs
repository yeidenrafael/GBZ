using System;
using System.Web;
using GrinGlobal.Zone.GGService;
using System.Data;
using System.ServiceModel;
using System.Collections.Generic;

namespace GrinGlobal.Zone.Models
{
    /// <summary>
    /// Return the data information from Json of GrinGlobal 
    /// </summary>
    public class GGZoneModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GGZoneModel));
        /// <summary>
        /// Get all data from the specific dataview
        /// </summary>
        /// <param name="url">service url</param>
        /// <param name="suppressExceptions">allow show error</param>
        /// <param name="dataviewName">Name of dataview</param>
        /// <param name="delimitedParameterList">char to separte the parameters</param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public DataSet GetData(string url, bool suppressExceptions, string dataviewName, string delimitedParameterList, int offset, int limit, string options)
        {
            BasicHttpBinding bid = null;
            EndpointAddress address = null;
            Helpers.ProxyServer proxy = new Helpers.ProxyServer();
            proxy.SetClientEndpoints(url, ref bid, ref address);
            string userName = HttpContext.Current.Session["username"].ToString();//get user info
            string password = HttpContext.Current.Session["userkey"].ToString();
            using (var client = new GUISoapClient(bid, address))
            {
                DataSet ds = client.GetData(suppressExceptions, userName, password, dataviewName, delimitedParameterList, offset, limit, options);
                return ds;
            }
        }
        /// <summary>
        /// Get all the parameters of an already defined dataview
        /// </summary>
        /// <param name="url">service url</param>
        /// <param name="suppressExceptions">allow show error</param>
        /// <param name="dataviewName"> name of dataview</param>
        /// <returns>Dataset with dataview parameter information</returns>
        public DataSet GetParameters(string url, bool suppressExceptions, string dataviewName)
        {
            BasicHttpBinding bid = null;
            EndpointAddress address = null;
            Helpers.ProxyServer proxy = new Helpers.ProxyServer();
            proxy.SetClientEndpoints(url, ref bid, ref address);
            string userName = HttpContext.Current.Session["username"].ToString();//get user info
            string password = HttpContext.Current.Session["userkey"].ToString();
            using (var client = new GUISoapClient(bid, address))
            {
                DataSet ds = client.GetDataParameterTemplate(suppressExceptions, userName, password, dataviewName);
                return ds;
            }
        }
        /// <summary>
        /// Save data modified by external application
        /// </summary>
        /// <param name="url">service url</param>
        /// <param name="suppressExceptions">allow show error</param>
        /// <param name="ds">DataSet with value modification</param>
        /// <param name="options"></param>
        /// <returns>Data set with save status information</returns>
        public DataSet SaveData(string url, bool suppressExceptions, DataSet ds, string options)
        {
            BasicHttpBinding bid = null;
            EndpointAddress address = null;
            Helpers.ProxyServer proxy = new Helpers.ProxyServer();
            proxy.SetClientEndpoints(url, ref bid, ref address);
            string userName = HttpContext.Current.Session["username"].ToString();//get user info
            string password = HttpContext.Current.Session["userkey"].ToString();
            using (var client = new GUISoapClient(bid, address))
            {
                DataSet result = client.SaveData(suppressExceptions, userName, password, ds, options);
                return result;
            }
        }

        public IList<InventoryItem> GetInventoryItems() {
            var model = new List<InventoryItem>();
            return model;
        }
    }
}