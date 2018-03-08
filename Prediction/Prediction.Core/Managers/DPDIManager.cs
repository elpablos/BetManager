using Prediction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prediction.Core.Managers
{
    public class DPDIManager : DixonManager
    {
        public DPDIManager(IList<GameMatch> matches, IList<GameTeam> teams) : base(matches, teams)
        {
        }

        /// <summary>
        /// Suma všech pravděpodobností v logaritmu
        /// </summary>
        /// <returns></returns>
        public override double SumMaximumLikehood()
        {
            double ret = 0;
            double prefix = (1 - P);
            foreach (var match in Matches)
            {
                if (match.HomeScore < match.AwayScore)
                {
                    ret += Math.Log(prefix * AwayProbability(match));
                }
                else if (match.HomeScore > match.AwayScore)
                {
                    ret += Math.Log(prefix * HomeProbability(match));
                }
                else
                {
                    ret += Math.Log(prefix * DrawProbability(match));
                }
            }

            MaximumLikehoodValue = ret;
            return ret;
        }

        public override double DrawProbability(GameMatch match)
        {
            double ret = 0;
            var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);

            if (homeTeam == null || awayTeam == null)
            {
                throw new Exception(string.Format("match: {0}\t{1}-{2}\t{3}-{4}", match.Id, match.HomeTeamId, match.AwayTeamId, homeTeam == null, awayTeam == null));
            }

            for (int i = 0; i < PropLength; i++)
            {
                ret += Poisson(i, LambdaHome(homeTeam, awayTeam))
                    * Poisson(i, LambdaAway(homeTeam, awayTeam));
                if (match.HomeScore < Thetas.Count)
                {
                    ret += P * Thetas[match.HomeScore];
                }
            }

            return ret;
        }

        protected override double LambdaHome(GameTeam homeTeam, GameTeam awayTeam)
        {
            return homeTeam.HomeAttack * awayTeam.AwayAttack * homeTeam.Gamma * Gamma * Mi;
        }

        protected override double LambdaAway(GameTeam homeTeam, GameTeam awayTeam)
        {
            return awayTeam.HomeAttack * homeTeam.AwayAttack * Mi;
        }
    }
}
