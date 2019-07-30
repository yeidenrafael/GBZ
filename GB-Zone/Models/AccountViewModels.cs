using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel;
using GrinGlobal.Zone.GGService;
using GrinGlobal.Zone.Helpers;

namespace GrinGlobal.Zone.Models
{
    public class AccountViewModels
    {

        [Required]
        public string Server { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "UserName")]        
        public string UserName { get; set; }

        public System.Data.DataTable validateGG(string server,string userName, string password) {



            try
            {

            
            string url = String.Empty;
                var result = Settings.Server(server);
                          
           

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