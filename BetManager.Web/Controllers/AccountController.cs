using BetManager.Web.Controllers.Accounts.Detail;
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

        public ActionResult Detail()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = identity.Claims;
            string sid = claims.First(x => x.Type == ClaimTypes.Sid).Value;

            var result = Handler.Get<DetailAccountBuilder>().Build(Convert.ToInt32(sid));

            return View(result.Model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Detail(DetailAccountViewModel data)
        {
            if (ModelState.IsValid)
            {
                var result = Handler.Get<DetailAccountHandler>().Handle(data);
                if ((bool)result.Data)
                {
                    ViewBag.Message = "Změny proběhly v pořádku, pravděpodobně se projeví až při příštím spuštění";
                    Session.Clear();

                    return View(data);
                }
                else
                {
                    ModelState.AddModelError("", "Problém");
                }
            }
            return View(data);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginAccountViewModel data)
        {
            if (ModelState.IsValid)
            {
                var result = Handler.Get<LoginAccountHandler>().Handle(data);
                if ((bool)result.Data)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Neplatné přihlášení");
                }
            }

            return View(data);
        }

        public ActionResult Logout()
        {
            Handler.Get<LoginAccountHandler>().Logout();
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}