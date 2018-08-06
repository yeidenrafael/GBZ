﻿using System;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Linq;
using System.Xml.Linq;

namespace GrinGlobal.Zone.Helpers
{
    public static class Settings
    {
        /// <summary>
        /// Extract the XML Setting file
        /// </summary>
        private static string fileXnml
        {
            get
            {
                return String.Concat(HttpContext.Current.Server.MapPath("~"), "/Setting.xml");
            }
        }

        public static XElement xmlElement
        {
            get
            {
                return XElement.Load(fileXnml);
            }
        }
        public static XElement Server(string serverId) {

            XElement serverNode = xmlElement.Elements("server")
                                          .Where(w => (string)w.Attribute("id") == serverId)
                                          .SingleOrDefault();
             
            return serverNode;
        }
        public static XElement Module(string serverId, string moduleId)
        {

            XElement cropNode = xmlElement.Elements("server")
                                          .Where(w => (string)w.Attribute("id") == serverId)
                                          .SingleOrDefault();
            XElement module = (from el in cropNode.Elements("modules") where (string)el.Attribute("id") == moduleId select el).SingleOrDefault();
            return module;
        }
        public static IEnumerable<XElement> Fields(string serverId, string moduleId)
        {

            XElement cropNode = xmlElement.Elements("server")
                                          .Where(w => (string)w.Attribute("id") == serverId)
                                          .SingleOrDefault();
            IEnumerable<XElement> fields = (from el in cropNode.Elements("modules") where (string)el.Attribute("id") == moduleId select el).Elements("form").Elements("field");
            return fields;
        }
    }
}