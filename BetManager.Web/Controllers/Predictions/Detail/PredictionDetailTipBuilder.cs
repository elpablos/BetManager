using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;

namespace BetManager.Web.Controllers.Predictions.Detail
{
    public class PredictionDetailTipBuilder : IModelBuilder<PredictionDetailTipViewModel, int>
    {
        public PredictionDetailTipBuilder()
        {

        }

        public ModelBuilderResult<PredictionDetailTipViewModel> Build(int id)
        {
            throw new NotImplementedException();
        }
    }
}