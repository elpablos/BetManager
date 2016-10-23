using BetManager.Web.ApiControllers.Tips.Graph;
using BetManager.Web.Controllers.Tips.List;
using BetManager.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BetManager.Web.ApiControllers
{
    [Authorize]
    public class TipController : ApiController
    {
      
        [HttpPost]
        [Route("api/graphtip")]
        public IEnumerable<GraphTipViewModel> GetGraphTip(GraphTipFilterViewModel filter)
        {
            var result = Handler.Get<GraphTipBuilder>().Build(filter);
            return result.Model;
        }

        [HttpPost]
        public IEnumerable<TipListItemViewModel> Index(TipListFilterViewModel filter)
        {
            var result = Handler.Get<TipListBuilder>().Build(filter);
            return result.Model.Rows;
        }
    }
}
