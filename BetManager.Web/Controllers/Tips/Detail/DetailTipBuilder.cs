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
                AwaySeasonTaken = tip.AwaySeasonTaken,
                AwayTeam = tip.AwayTeam,
                Category = tip.Category,
                DateStart = tip.DateStart,
                DisplayName = tip.DisplayName,
                FirstId = tip.FirstId,
                FirstValue = tip.FirstValue,
                PredictTip = tip.PredictTip,
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
                SecondId = tip.SecondId,
                SecondValue = tip.SecondValue,
                Url = tip.Url,
                WinnerCode = tip.WinnerCode,
                XId = tip.XId,
                Xvalue = tip.Xvalue,
                Home = tip.Home * 100,
                Draw = tip.Draw * 100,
                Away = tip.Away * 100,
                HomePercent = tip.HomePercent * 100,
                DrawPercent = tip.DrawPercent * 100,
                AwayPercent = tip.AwayPercent * 100,
                TwoAndHalfMinus = tip.TwoAndHalfMinus * 100,
                TwoAndHalfPlus = tip.TwoAndHalfPlus * 100,
                OneAndHalfMinus = tip.OneAndHalfMinus * 100,
                OneAndHalfPlus = tip.OneAndHalfPlus * 100,
                Poissons = _tipManager.GetDetailPoisson(new { id = tip.ID }).Select(x => new DetailTipPoissonViewModel
                {
                    DisplayName = x.DisplayName,
                    GoalFive = x.GoalFive,
                    GoalFour = x.GoalFour,
                    GoalOne = x.GoalOne,
                    GoalThree = x.GoalThree,
                    GoalTwo = x.GoalTwo,
                    GoalZero = x.GoalZero,
                    Score = x.Score,
                    Tip = x.Tip
                }).ToList(),
                PoissonHistories = _tipManager.GetAllPoissonHistory(new { id = tip.ID }).Select(x => new DetailTipPoissonViewModel
                {
                    DisplayName = x.DisplayName,
                    GoalFive = x.GoalFive,
                    GoalFour = x.GoalFour,
                    GoalOne = x.GoalOne,
                    GoalThree = x.GoalThree,
                    GoalTwo = x.GoalTwo,
                    GoalZero = x.GoalZero,
                    Score = x.Score,
                    Tip = x.Tip
                }).ToList(),
                Goals = _tipManager.GetDetailGoal(new { id = tip.ID }).Select(x => new DetailTipGoalViewModel
                {
                    Id = x.Id,
                    Prob = x.Prob
                }).ToList()
            };

            return result;
        }
    }
}