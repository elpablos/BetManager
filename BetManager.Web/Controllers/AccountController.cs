using BetManager.Web.Controllers.Accounts.Login;
using BetManager.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Controllers
{
    public class AccountController : Controller
    {
        public AccountController()
        {

        }

        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel data)
        {
            if (ModelState.IsValid)
            {
                var result = Handler.Get<LoginHandler>().Handle(data);
                if ((bool)result.Data)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(data);
        }

        public ActionResult Logout()
        {
            Handler.Get<LoginHandler>().Logout();
            return RedirectToAction("Index", "Home");
        }
    }
}