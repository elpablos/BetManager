using BetManager.Web.Controllers.Tips.Detail;
using BetManager.Web.Controllers.Tips.List;
using BetManager.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Controllers
{
    [Authorize]
    public class TipController : Mvc.BaseController
    {
        public ActionResult Index(TipListFilterViewModel filter)
        {
            //UpdateModel(filter);
            if (filter.DateFrom == null && filter.DateTo == null)
            {
                filter.SetDefault(UserIdentity);
            }
            sessionHelper.Remember(filter);
            var result = Handler.Get<TipListBuilder>().Build(filter);
            return View(result.Model);
        }

        public ActionResult Detail(int id)
        {
            var result = Handler.Get<DetailTipBuilder>().Build(id);
            return View(result.Model);
        }
    }
}