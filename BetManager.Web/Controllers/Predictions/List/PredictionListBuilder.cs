using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;

namespace BetManager.Web.Controllers.Predictions.List
{
    public class PredictionListBuilder : IModelBuilder<PredictionListViewModel, PredictionListFilterViewModel>
    {
        public PredictionListBuilder()
        {

        }

        public ModelBuilderResult<PredictionListViewModel> Build(PredictionListFilterViewModel input)
        {
            throw new NotImplementedException();
        }
    }
}