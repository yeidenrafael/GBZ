using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using GrinGlobal.Zone.Models;
using System.Data;
using System.Web.Security;
using GrinGlobal.Zone.Helpers;

namespace GrinGlobal.Zone.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Login to App
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Login()
        {
            SettingsHelp setH = new SettingsHelp();
            return View(setH.DataTableAllServer);
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(AccountViewModels model, string returnUrl)
        {
            string errrMsg = "Invalid login attempt. Your user or password is wrong";

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", errrMsg);
                return View(model);
            }

            var userInformation = model.validateGG(model.Server, model.UserName, model.Password);
            if (userInformation != null) {
                DataRow rowUser = userInformation.Rows[0];
                if (rowUser["is_enabled"].ToString() == "Y")
                {
                    Session["server"] = model.Server;
                    Session["username"] = model.UserName;
                    Session["userkey"] = model.Password;
                    FormsAuthentication.SetAuthCookie(model.UserName, false);
                    return RedirectToAction("Index", "Home");
                }
                else {
                    ModelState.AddModelError("", "The user is disabled in GRIN-Global server");
                    return View(model);
                }
            }
            ModelState.AddModelError("", errrMsg);
            return View(model);
        }

        /// <summary>
        /// LogOut
        /// </summary>
        /// <returns></returns>
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

    }
}