using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace GrinGlobal.Zone.Helpers
{
    /// <summary>
    /// Class to get all the values ​​embedded in the settings file.
    /// </summary>
    public class SettingsHelp
    {
        #region public Attribute
        /// <summary>
        /// Get all inputs that corresponds to the formId provided.
        /// </summary>
        /// <returns>
        /// The IEnumerable XElements that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public IEnumerable<XElement> Fields { get { return fields; } }
        /// <summary>
        /// Get all inputs that corresponds to the serverId provided.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Server { get { return server; } }
        /// <summary>
        /// Get all inputs that corresponds to the moduleId provided.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Module { get { return module; } }
        /// <summary>
        /// Get all inputs that corresponds to the formId provided.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Form { get { return form; } }
        /// <summary>
        /// Get the tag named Parameters, that corresponds to the action.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Parameter { get { return parameter; } }
        /// <summary>
        /// Get the tag named Columns, that corresponds to the action.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement Column { get { return column; } }
        /// <summary>
        /// Get the tag named extendedProperties, that corresponds to the action.
        /// </summary>
        /// <returns>
        /// The XElement that is obtained from the Xpath parameters provided to constructor
        /// </returns>
        public XElement ExtendedPropertie { get { return extendedPropertie; } }
        #endregion
        #region private Attribute
        private string fileXnml { get { return String.Concat(HttpContext.Current.Server.MapPath("~"), "/Setting.xml"); } }
        private XElement xmlElement { get { return XElement.Load(fileXnml); }         }
        private readonly string NAME_SERVER = "server";
        private readonly string NAME_MODULE = "module";
        private readonly string GENERIC_ID = "id";
        private readonly string NAME_FORM = "form";
        private readonly string NAME_FIELD = "field";
        private readonly string NAME_ACTIONS = "actions";
        private readonly string NAME_PARAMETERS = "parameters";
        private readonly string NAME_COLUMN = "column";
        private readonly string NAME_COLUMNS = "columns";
        private readonly string NAME_EXTENDED_PROPERTIES = "extendedProperties";
        private IEnumerable<XElement> fields;
        private XElement server;
        private XElement module;
        private XElement form;
        private XElement parameter;
        private XElement column;
        private XElement extendedPropertie;
        #endregion
        #region Constructor
        /// <summary>
        /// To find the all tags that corresponds to the <paramref name="serverId">, <param name="moduleId"> and <param name="formId"> provided.
        /// </summary>
        /// <param name="serverId">String that identifies the server the Grind-Global</param>
        /// <param name="moduleId">String that identifies the module for the aplication</param>
        /// <param name="formId">String that identifies the form for the aplication</param>
        public SettingsHelp(string serverId, string moduleId, string formId)
        {
            GetXElement(serverId, moduleId, formId);
        }
        #endregion
        #region private Methods 

        private void GetXElement(string serverId, string moduleId, string formId)
        {
            server = xmlElement.Elements(NAME_SERVER)
                                          .Where(w => (string)w.Attribute(GENERIC_ID) == serverId)
                                          .SingleOrDefault();
            module = (from el in server.Elements(NAME_MODULE)
                               where (string)el.Attribute(GENERIC_ID) == moduleId
                               select el).SingleOrDefault();
            form = (from el in module.Elements(NAME_FORM)
                             where (string)el.Attribute(GENERIC_ID) == formId
                             select el).SingleOrDefault();
            fields = from fi in form.Descendants(NAME_FIELD) select fi;
            XElement action = form.Elements(NAME_ACTIONS).SingleOrDefault();
            parameter = action.Elements(NAME_PARAMETERS).SingleOrDefault();
            column = action.Elements(NAME_COLUMNS).SingleOrDefault();
            extendedPropertie = action.Elements(NAME_EXTENDED_PROPERTIES).SingleOrDefault();
        }
        #endregion
        #region public methods
        /// <summary>
        /// Find the specific column from the action, columns
        /// </summary>
        /// <param name="name">Name of column to find is automatic cast to uppercase</param>
        /// <returns>
        /// Get the column XElement or null in case not found
        /// </returns>
        public XElement GetColumn_Name(string name)
        {
            XElement col = (from e in column.Descendants(NAME_COLUMN) where e.Value.Trim().ToUpper() == name.ToUpper() select e).DefaultIfEmpty(null).FirstOrDefault();
            return col;
        }
        #endregion
    }
}