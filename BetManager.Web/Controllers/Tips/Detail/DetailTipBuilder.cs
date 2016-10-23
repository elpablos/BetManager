using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using BetManager.Core.Domains.Tips;

namespace BetManager.Web.Controllers.Tips.Detail
{
    public class DetailTipBuilder : IModelBuilder<DetailTipViewModel, int>
    {
        private readonly ITipManager _tipManager;

        public DetailTipBuilder()
        {
            _tipManager = new TipManager();
        }

        public ModelBuilderResult<DetailTipViewModel> Build(int input)
        {
            var result = new ModelBuilderResult<DetailTipViewModel>();

            var tip = _tipManager.GetById(input);
            result.Model = new DetailTipViewModel
            {
                ID = tip.ID,
                AwayLastForm = tip.AwayLastForm,
                AwayLastGiven = tip.AwayLastGiven,
                AwayLastTaken = tip.AwayLastTaken,
                AwaySeasonCount = tip.AwaySeasonCount,
                AwaySeasonForm = tip.AwaySeasonForm,
                AwaySeasonGiven = tip.AwaySeasonGiven,
                AwaySeasonTaken= tip.AwaySeasonTaken,
                AwayTeam = tip.AwayTeam,
                Category = tip.Category,
                DateStart = tip.DateStart,
                DisplayName = tip.DisplayName,
                FirstValue = tip.FirstValue,
                Form = tip.Form,
                HomeLastForm = tip.HomeLastForm,
                HomeLastGiven = tip.HomeLastGiven,
                HomeLastTaken = tip.HomeLastTaken,
                HomeSeasonCount = tip.HomeSeasonCount,
                HomeSeasonForm = tip.HomeSeasonForm,
                HomeSeasonGiven = tip.HomeSeasonGiven,
                HomeSeasonTaken = tip.HomeSeasonTaken,
                HomeTeam = tip.HomeTeam,
                Odd = tip.Odd,
                Season = tip.Season,
                SecondValue = tip.SecondValue,
                Url = tip.Url,
                WinnerCode = tip.WinnerCode,
                Xvalue = tip.Xvalue
            };

            return result;
        }
    }
}