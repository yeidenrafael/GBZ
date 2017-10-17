using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;


namespace GrinGlobal.Zone.Helpers
{
    public class Settings
    {
        /// <summary>
        /// Extract the XML Setting file
        /// </summary>
        private string fileXnml {

            get {
                return String.Concat(HttpContext.Current.Server.MapPath("~"), "/Setting.xml");
            }
        }
        public XElement xmlElement { get {
                return XElement.Load(fileXnml);

            } }
    }
}