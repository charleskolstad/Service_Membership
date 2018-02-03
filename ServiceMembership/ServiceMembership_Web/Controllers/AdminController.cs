using ServiceMembership_Core;
using ServiceMembership_Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

        #region loginactions
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!String.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);

                return RedirectToAction("Index");
            }

            ViewBag.ReturnUrl = returnUrl;

            return View();
        }

        [AllowAnonymous, HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                Provider provider = new Provider();
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
            if (ModelState.IsValid)
            {
                Provider provider = new Provider();
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
                
                string message = UserManager.RecoverPassword(model);
                if (!string.IsNullOrEmpty(message))
                    ModelState.AddModelError("", message);
            }

            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Admin");
        }
        #endregion

        #region partialviews
        public ActionResult AllUsers()
        {
            return View();
        }
        #endregion

        #region 
        public JsonResult UsersGetAll()
        {
            List<UserInfo> userInfo = UserManager.GetAllUsers();
            return Json(userInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserGetByID(string userName)
        {
            UserInfo userInfo = UserManager.GetUserInfo(userName);
            return Json(userInfo, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ProfilesGetAll()
        {
            List<Profile> profiles = ProfileManager.GetAllProfiles();

            return Json(profiles, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public string CreateUser(UserInfo info)
        {
            string message = UserManager.CreateUser(info, Membership.GetUser().UserName);
            return message;            
        }

        public string DeleteUser(string userName)
        {
            string message = UserManager.DeleteUser(userName, Membership.GetUser().UserName);
            if (string.IsNullOrEmpty(message))
                return userName + " deleted.";
            else
                return message;
        }
        #endregion
    }
}