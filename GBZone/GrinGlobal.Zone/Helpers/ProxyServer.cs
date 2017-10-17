using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Web;
using System.Configuration;

namespace GrinGlobal.Zone.Helpers
{
    public class ProxyServer
    {
        
        public  void SetClientEndpoints(string url, ref BasicHttpBinding bid, ref EndpointAddress address)
        {
            const int MAXSIZE = 500000000;

            string host = HttpContext.Current.Request.Url.Authority;
            // Create a binding for HTTP.
            bid = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
            bid.Name = "basicHttpConf";
            bid.SendTimeout = TimeSpan.MaxValue;
            bid.MaxReceivedMessageSize = MAXSIZE;
            bid.ReaderQuotas.MaxNameTableCharCount = MAXSIZE;
            bid.MessageEncoding = WSMessageEncoding.Text;
            bid.Security.Transport.ClientCredentialType = HttpClientCredentialType.Ntlm;            
            address = new EndpointAddress(url);
        }
    }
}