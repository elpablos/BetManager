using BetManager.Core.Domains.Tips;
using BetManager.Web.Mvc;
using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.ApiControllers.Tips.Progress
{
    public class ProgressTipBuilder : IModelBuilder<IEnumerable<ProgressTipViewModel>, ProgressTipFilterViewModel>
    {
        private readonly ITipManager _tipManager;

        public ProgressTipBuilder()
        {
            _tipManager = new TipManager();
        }

        public ModelBuilderResult<IEnumerable<ProgressTipViewModel>> Build(ProgressTipFilterViewModel input)
        {
            var result = new ModelBuilderResult<IEnumerable<ProgressTipViewModel>>();
            result.Model = _tipManager.GetAllProgress(input).Select(x => new ProgressTipViewModel
            {
                ID = x.ID,
                DatePredict = x.DatePredict,
                Home = x.Home,
                Away = x.Away
            }).ToList();

            return result;
        }
    }
}