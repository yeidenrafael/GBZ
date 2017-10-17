using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using log4net;
using log4net.Config;
using System.Data;

using System.Xml;
using System.Xml.Linq;
namespace GrinGlobal.Zone.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(HomeController));
        Models.GGZoneModel ctx = new Models.GGZoneModel();
        /// <summary>
        /// Default view
        /// </summary>
        /// <returns></returns>
        ///   
        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// Prepare the connection between Data and View
        /// </summary>
        /// <param name="chooseSelected"></param>
        /// <param name="value"></param>
        /// <returns></returns>

        public ActionResult ShowView(string chooseSelected, string value)
        {


            string urlService = String.Empty;
            string cropId = Session["crop"] != null ? Session["crop"].ToString() : "01";

            string delimitedParams = String.Empty;
            int nBracket = 0;
            int nIndex = 0;
            string colName = String.Empty;
            //read the project
            XElement xml = (new GrinGlobal.Zone.Helpers.Settings()).xmlElement;

            var service = (from el in xml.Elements("Crop")
                           where (string)el.Attribute("id") == cropId.ToString()
                           select el
                          ).Elements("Services").Elements("Service").Where(c => (string)c.Attribute("id") == chooseSelected).FirstOrDefault();

            //extract the url from setting xml
            urlService = service.Parent.Parent.Attribute("url").Value.ToString();

            //put the value in the delimitedParameterList
            delimitedParams = service.Element("delimitedParameterList").Value;

            var arrValue = value.Split(new char[] { '-' });

            while ((nBracket = delimitedParams.IndexOf("{0}", nBracket)) != -1)
            {
                delimitedParams = delimitedParams.Remove(nBracket, 3).Insert(nBracket, arrValue[nIndex]);
                nBracket++;
                nIndex++;
            }


            //invoke model requesting the datatable
            DataTable dt = ctx.GetData(urlService,
                                       bool.Parse(service.Element("suppressExceptions").Value),
                                       service.Element("dataviewName").Value,
                                       delimitedParams,
                                       int.Parse(service.Element("offset").Value),
                                       int.Parse(service.Element("limit").Value),
                                       service.Element("options").Value
                                       );

            //remove or add column            
            foreach (DataColumn col in dt.Columns)
            {
                if (col.ExtendedProperties["is_visible"].ToString() == "N" )
                {
                    col.ColumnMapping = MappingType.Hidden;
                }

                XElement elm = (from e in service.Elements("columns").Descendants("column")
                                where e.Value.Trim().ToUpper() == col.ColumnName.Trim().ToUpper()
                                select e).FirstOrDefault();
                /*if (elm == null)
                {
                    col.ColumnMapping = MappingType.Hidden;
                }
                else*/
                if (elm != null)
                {
                    //create extendproperties
                    if (elm.Attribute("header") != null && bool.Parse(elm.Attribute("header").Value))
                    {
                        col.ExtendedProperties.Add("Header", "Header");
                    }
                    if (elm.Attribute("link") != null && bool.Parse(elm.Attribute("link").Value))
                    {
                        col.ExtendedProperties.Add("url", Url.Action("ShowView", "Home", new { chooseSelected = elm.Attribute("view").Value, value = String.Concat("{", elm.Attribute("colreference").Value, "}") }));
                    }
                }
            }


            //change the order of columns
            /*nIndex = 0;
            DataColumn setCol;
            foreach (var colView in service.Elements("columns").Descendants("column"))
            {
                colName = colView.Value;
                setCol = dt.Columns[colName.ToString().Trim()];
                setCol.SetOrdinal(nIndex);
                nIndex++;
            }*/

            ViewBag.ViewTitle = dt.ExtendedProperties["title"].ToString();
            //ViewBag.ViewDesc = dt.ExtendedProperties["description"].ToString();
            ViewData.Model = dt;
            return View("Index");
        }

        protected override void OnAuthorization(AuthorizationContext filterContext)
        {

            if (filterContext.HttpContext.Session["username"] == null && filterContext.HttpContext.Session["userkey"] == null)
            {
                filterContext.HttpContext.Response.Redirect(Url.Action("Logout", "Account"));
            }
            base.OnAuthorization(filterContext);
        }
    }
}