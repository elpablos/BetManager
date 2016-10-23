using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using BetManager.Core.Domains.Tips;

namespace BetManager.Web.ApiControllers.Tips.Graph
{
    public class GraphTipBuilder : IModelBuilder<IEnumerable<GraphTipViewModel>, GraphTipFilterViewModel>
    {
        private readonly ITipManager _tipManager;

        public GraphTipBuilder()
        {
            _tipManager = new TipManager();
        }

        public ModelBuilderResult<IEnumerable<GraphTipViewModel>> Build(GraphTipFilterViewModel input)
        {
            var result = new ModelBuilderResult<IEnumerable<GraphTipViewModel>>();
            result.Model = _tipManager.GetGraph(input).Select(x => new GraphTipViewModel
            {
                DateStart = x.DateStart,
                Correct = x.Correct,
                Price = x.Price,
                Total = x.Total
            }).ToList();

            return result;
        }
    }
}