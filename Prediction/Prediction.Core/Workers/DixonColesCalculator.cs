using Prediction.Core.Models;
using Prediction.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Prediction.Core.Solvers;

namespace Prediction.Core.Workers
{
    public class DixonColesCalculator
    {
        private IGamePredictionService predictionService;

        public DateTime? KsiStart { get; set; }
        public SolverTypeEnum? Type { get; set; }
        public int PropLength { get; set; }
        public double Ksi { get; set; }

        public DixonColesCalculator(IGamePredictionService predictionService)
        {
            this.predictionService = predictionService;
        }

        public double Calculate(ICollection<DataInput> inputs)
        {
            double result = 0;
            var teams = new HashSet<GameTeam>();

            foreach (var input in inputs)
            {
                // home team
                teams.Add(new GameTeam
                {
                    Id = input.HomeTeamId,
                    DisplayName = input.HomeTeam
                });

                teams.Add(new GameTeam
                {
                    Id = input.AwayTeamId,
                    DisplayName = input.AwayTeam
                });
            }

            var predicts = predictionService.GetAll(Ksi, Type);
            foreach (var predict in predicts)
            {
                var matches = new List<GameMatch>();

                #region Prepare data

                var filteredInputs = inputs.Where(x => (x.DateStart.Date - predict.DatePredict.Date).Days == 0).ToList();
                foreach (var input in filteredInputs)
                {
                    var match = new GameMatch
                    {
                        Id = input.Id,
                        HomeTeamId = input.HomeTeamId,
                        AwayTeamId = input.AwayTeamId,
                        HomeScore = input.HomeScore,
                        AwayScore = input.AwayScore,
                        DateStart = input.DateStart,
                    };

                    match.HomeTeam = teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
                    match.AwayTeam = teams.FirstOrDefault(x => x.Id == match.AwayTeamId);
                    match.Days = (predict.DatePredict - match.DateStart).Days;
                    matches.Add(match);
                }

                // matches = matches.ToList();

                #endregion

                var dixonManager = predictionService.GetById(predict.Id, matches, teams.ToList());
                dixonManager.KsiStart = KsiStart;
                dixonManager.PropLength = PropLength;
                result += dixonManager.SumMaximumLikehood();
            }

            return result;
        }
    }
}
