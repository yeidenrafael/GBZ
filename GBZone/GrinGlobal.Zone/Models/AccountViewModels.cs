using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using System.ServiceModel;
using GrinGlobal.Zone.GGService;

namespace GrinGlobal.Zone.Models
{
    public class AccountViewModels
    {

        [Required]
        public string Crop { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "UserName")]        
        public string UserName { get; set; }

        public System.Data.DataTable validateGG(string crop,string userName, string password) {



            try
            {

            XElement xml = (new GrinGlobal.Zone.Helpers.Settings()).xmlElement;
            string url = String.Empty;
            var result = (from el in xml.Elements("Crop")
                          where (string)el.Attribute("id") == crop
                          select el
                          ).FirstOrDefault();
           

            if (result != null)
            {
                url = result.Attribute("url").Value.ToString();
                BasicHttpBinding bid = null;
                EndpointAddress address = null;
                GrinGlobal.Zone.Helpers.ProxyServer proxy = new Helpers.ProxyServer();
                proxy.SetClientEndpoints(url, ref bid, ref address);
                using (var client = new GUISoapClient(bid, address))
                {

                    var serviceResult = client.ValidateLogin(false, userName, password);              

                    return serviceResult.Tables[1] != null ? serviceResult.Tables[1] : null;
                }
            }
            else {
                return null;
            }
            }
            catch {
                return null;
            }

        }
    }
}