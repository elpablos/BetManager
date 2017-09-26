using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using BetManager.Core.Domains.Tips;

namespace BetManager.Web.Controllers.Tips.List
{
    public class TipListBuilder : IModelBuilder<TipListViewModel, TipListFilterViewModel>
    {
        private readonly ITipManager _tipManager;

        public TipListBuilder()
        {
            _tipManager = new TipManager();
        }

        public ModelBuilderResult<TipListViewModel> Build(TipListFilterViewModel filter)
        {
            var model = new ModelBuilderResult<TipListViewModel>();
            model.HttpStatusCode = System.Net.HttpStatusCode.OK;
            model.Model = new TipListViewModel
            {
                Filter = filter,
                Rows = _tipManager.GetAll(filter).Select(x => new TipListItemViewModel
                {
                    ID = x.ID,
                    DateStart = x.DateStart,
                    DisplayName = x.DisplayName,
                    Category = x.Category,
                    PredictTip = x.PredictTip,
                    Away = x.Away * 100,
                    Draw = x.Draw * 100,
                    Home = x.Home * 100,
                    FirstValue = x.FirstValue,
                    Xvalue = x.Xvalue,
                    SecondValue = x.SecondValue,
                    Odd = x.Odd,
                    WinnerCode = x.WinnerCode
                }).ToList()
            };
            return model;
        }
    }
}