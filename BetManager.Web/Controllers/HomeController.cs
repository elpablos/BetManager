using BetManager.Core.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ITipManager _tipManager;

        public HomeController()
        {
            _tipManager = new TipManager();
        }

        // GET: Tip
        public ActionResult Index(DateTime? dateFrom, decimal? formFrom, double? oddFrom)
        {
            dateFrom = dateFrom ?? DateTime.Now;
            formFrom = formFrom ?? 30;
            oddFrom = oddFrom ?? 2.0;

            var tips = _tipManager.GetAll(new { @DateFrom = dateFrom, @Form = formFrom, @Odd = oddFrom });
            ViewBag.DateFrom = dateFrom;
            ViewBag.FormFrom = formFrom;
            ViewBag.OddFrom = oddFrom;
            return View(tips);
        }

        // GET: Tip/Details/5
        public ActionResult Detail(int id)
        {
            var tip = _tipManager.GetById(id);
            ViewBag.Title = "Tip :: " + tip.DisplayName;
            return View(tip);
        }
    }
}