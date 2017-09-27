using BetManager.Core.Domains.Tips;
using BetManager.Web.Mvc;
using BetManager.Web.Mvc.Common;

namespace BetManager.Web.ApiControllers.Predictions.Generate
{
    public class PredictionGenerateHandler : IModelHandler<PredictionGenerateFilterViewModel>
    {
        private readonly ITipManager _tipManager;

        public PredictionGenerateHandler()
        {
            _tipManager = new TipManager();
        }

        public ModelHandlerResult Handle(PredictionGenerateFilterViewModel model)
        {
            var result = new ModelHandlerResult();
            _tipManager.PredictionGenerate();
            return result;
        }
    }
}