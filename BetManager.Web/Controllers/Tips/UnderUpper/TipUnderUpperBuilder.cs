using BetManager.Core.Domains.Tips;
using BetManager.Web.Mvc;
using BetManager.Web.Mvc.Common;
using System.Linq;

namespace BetManager.Web.Controllers.Tips.UnderUpper
{
    public class TipUnderUpperBuilder : IModelBuilder<TipUnderUpperViewModel, TipUnderUpperFilterViewModel>
    {
        private readonly ITipManager _tipManager;

        public TipUnderUpperBuilder()
        {
            _tipManager = new TipManager();
        }

        public ModelBuilderResult<TipUnderUpperViewModel> Build(TipUnderUpperFilterViewModel filter)
        {
            var model = new ModelBuilderResult<TipUnderUpperViewModel>();
            model.HttpStatusCode = System.Net.HttpStatusCode.OK;
            model.Model = new TipUnderUpperViewModel
            {
                Filter = filter,
                Rows = _tipManager.GetAll(filter).Select(x => new TipUnderUpperItemViewModel
                {
                    ID = x.ID,
                    DateStart = x.DateStart,
                    DisplayName = x.DisplayName,
                    Category = x.Category,
                    OneAndHalfMinus = x.OneAndHalfMinus * 100,
                    OneAndHalfPlus = x.OneAndHalfPlus * 100,
                    TwoAndHalfMinus = x.TwoAndHalfMinus * 100,
                    TwoAndHalfPlus = x.TwoAndHalfPlus * 100,

                    TwoAndHalfMinusOdd = x.TwoAndHalfMinusOdd,
                    TwoAndHalfPlusOdd = x.TwoAndHalfPlusOdd,
                    OneAndHalfPlusOdd = x.OneAndHalfPlusOdd,
                    OneAndHalfMinusOdd = x.OneAndHalfMinusOdd,

                    WinnerCode = x.WinnerCode,
                    Goals = x.HomeScoreCurrent.HasValue && x.AwayScoreCurrent.HasValue ? x.HomeScoreCurrent.Value + x.AwayScoreCurrent.Value : (int?)null
                }).ToList()
            };
            return model;
        }
    }
}