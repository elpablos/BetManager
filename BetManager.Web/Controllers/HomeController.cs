using BetManager.Core.Domains.Tips;
using BetManager.Web.Controllers.Tips.List;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Controllers
{
    [Authorize]
    public class HomeController : Mvc.BaseController
    {
        private readonly ITipManager _tipManager;

        public HomeController()
        {
            _tipManager = new TipManager();
        }

        // GET: Tip
        public ActionResult Index(TipListFilterViewModel filter)
        {
            var vm = new TipListViewModel();
            vm.Filter = filter;
            // UpdateModel(filter);

            if (filter.DateFrom == null && filter.DateTo == null)
            {
                filter.SetDefaultDashboard();
            }

            sessionHelper.Remember(filter);
            return View(vm);
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