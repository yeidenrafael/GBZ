using System;
using System.Web;
using GrinGlobal.Zone.GGService;
using System.Data;
using System.ServiceModel;
using IO.Swagger.Api;
using IO.Swagger.Model;

namespace GrinGlobal.Zone.Models
{
    /// <summary>
    /// Return the data information from JSON webservices of GrinGlobal 
    /// </summary>
    public class GGZoneModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GGZoneModel));

        public DataSet GetData(string url, bool suppressExceptions, string dataviewName, string delimitedParameterList, int offset, int limit, string options)
        {

            BasicHttpBinding bid = null;
            EndpointAddress address = null;
            Helpers.ProxyServer proxy = new Helpers.ProxyServer();
            proxy.SetClientEndpoints(url, ref bid, ref address);

            //get user info
            string userName = HttpContext.Current.Session["username"].ToString();
            string password = HttpContext.Current.Session["userkey"].ToString();

            using (var client = new GUISoapClient(bid, address))
            {

                DataSet ds = client.GetData(suppressExceptions, userName, password, dataviewName, delimitedParameterList, offset, limit, options);

                return ds;
            }
        }

        public DataSet SaveData(string url, bool suppressExceptions, DataSet ds, string options)
        {

            BasicHttpBinding bid = null;
            EndpointAddress address = null;
            Helpers.ProxyServer proxy = new Helpers.ProxyServer();
            proxy.SetClientEndpoints(url, ref bid, ref address);

            //get user info
            string userName = HttpContext.Current.Session["username"].ToString();
            string password = HttpContext.Current.Session["userkey"].ToString();

            using (var client = new GUISoapClient(bid, address))
            {

                DataSet result = client.SaveData(suppressExceptions, userName, password, ds, options);

                return result;
            }
        }

        public BrapiResponseBrGermplasmV2TO GetGermplasmDetails(string crop, int germplasmDbId)
        {
            //test brapi
            GermplasmApi gmApi = new GermplasmApi("http://asdev.cimmyt.org:8280/brapi/v1");
            
            var result = gmApi.GetGermplasmDetails(crop.ToLower(), germplasmDbId);
         
            return result;
        }

        /*
        public DataTable GetDataBusiness(bool suppressExceptions,
                                 string dataviewName,
                                 string delimitedParameterList,
                                 int offset,
                                 int limit,
                                 string options){

            //get user info
            string userName = HttpContext.Current.Session["username"].ToString();
            string password = HttpContext.Current.Session["userkey"].ToString();
            string cnn = string.Empty;

            GrinGlobal.Core.DataConnectionSpec obj = new GrinGlobal.Core.DataConnectionSpec("sqlserver", "Data Source=172.17.61.157;Database=gringlobal;User Id=gg_user;password=PA55w0rd!");
            
            //cnn = obj.ConnectionString;

            string token = GrinGlobal.Business.SecureData.Login(userName, password, obj);
            using (SecureData sd = new SecureData(suppressExceptions, token, obj))
            {
                var ds = sd.GetData(dataviewName, delimitedParameterList, offset, limit, options);
                DataTable dt = ds.Tables[1];
                return dt;
            }
        }

        public DataTable SaveDataBusiness(DataTable model,
                                 bool suppressExceptions,
                                 string dataviewName,
                                 string delimitedParameterList,
                                 int offset,
                                 int limit,
                                 string options)
        {

            //get user info
            string userName = HttpContext.Current.Session["username"].ToString();
            string password = HttpContext.Current.Session["userkey"].ToString();
            string cnn = string.Empty;

            GrinGlobal.Core.DataConnectionSpec obj = new GrinGlobal.Core.DataConnectionSpec("sqlserver", "Data Source=172.17.61.157;Database=gringlobal;User Id=gg_user;password=PA55w0rd!");

            //cnn = obj.ConnectionString;

            string token = GrinGlobal.Business.SecureData.Login(userName, password, obj);
            using (SecureData sd = new SecureData(suppressExceptions, token, obj))
            {
                var ds = sd.GetData(dataviewName, delimitedParameterList, offset, limit, options);

                ds.Tables[dataviewName].Merge(model);

                //string ops = "useuniquekeys=false;booldefaultfalse=false;onlyifallrequiredfieldsexist=false;altlanguageid=null;safeupdatesanddeletes=true;skipsearchengineupdates;false;insertonlylanguagedata=false;rowprogressinterval=100;ownerid=155410";

                sd.SaveData(ds, false, options);
                DataTable dt = ds.Tables[dataviewName];
                return dt;
            }
        }
        */
    }
}