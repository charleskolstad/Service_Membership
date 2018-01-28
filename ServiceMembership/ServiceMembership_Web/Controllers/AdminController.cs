using ServiceMembership_Core;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ServiceMembership_Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        [AllowAnonymous, HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult RecoverPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost]
        public ActionResult RecoverPassword(RecoverModel model)
        {
            string message = UserManager.RecoverPassword(model);

            if (!string.IsNullOrEmpty(message))
                ModelState.AddModelError("", message);

            return View();
        }

        public JsonResult UsersGetAll()
        {
            List<UserInfo> userInfo = UserManager.GetAllUsers();
            return Json(userInfo, JsonRequestBehavior.AllowGet);
        }
    }
}