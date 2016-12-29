using BetManager.Web.Controllers.Predictions.Detail;
using BetManager.Web.Controllers.Predictions.List;
using BetManager.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Controllers
{
    [Authorize]
    public class PredictionController : BaseController
    {
        /// <summary>
        /// Přehled odhadů
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public ActionResult Index(PredictionListFilterViewModel filter)
        {
            if (filter.DateFrom == null && filter.DateTo == null)
            {
                filter.SetDefault(UserIdentity);
            }
            sessionHelper.Remember(filter);
            var result = Handler.Get<PredictionListBuilder>().Build(filter);
            return View(result.Model);
        }

        public ActionResult Detail(int id)
        {
            var result = Handler.Get<PredictionDetailTipBuilder>().Build(id);
            return View(result.Model);
        }
    }
}