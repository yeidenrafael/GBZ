using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using GrinGlobal.Zone.GGService;
using System.Data.Common;
using System.Dynamic;
using System.ComponentModel;
using System.Data;
using System.ServiceModel;

namespace GrinGlobal.Zone.Models
{

    

    /// <summary>
    /// Return the data information from JSON webservices of GrinGlobal 
    /// </summary>
    public class GGZoneModel
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(GGZoneModel));
     
      
        public  DataTable GetData(
                                  string url,  
                                  bool suppressExceptions,
                                  string dataviewName,
                                  string delimitedParameterList,
                                  int offset,
                                  int limit,
                                  string options) {

           
            BasicHttpBinding bid = null;
            EndpointAddress address = null;
            GrinGlobal.Zone.Helpers.ProxyServer proxy = new Helpers.ProxyServer();
            proxy.SetClientEndpoints(url,ref bid, ref address);

            //get user info

            string userName = HttpContext.Current.Session["username"].ToString();
            string password = HttpContext.Current.Session["userkey"].ToString();

            using (var client = new GUISoapClient(bid,address)) {
              
                var result = client.GetData(suppressExceptions,userName,password, dataviewName, delimitedParameterList, offset,limit,options);
                System.Data.DataTable dt = result.Tables[1];
               
                return dt;
            }
        }

      
    }
}