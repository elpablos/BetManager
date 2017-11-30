using Prediction.Core.Models;
using System.Collections.Generic;

namespace Prediction.Core.Managers
{
    public class DPManager : DixonManager
    {
        public DPManager(IList<GameMatch> matches, IList<GameTeam> teams) : base(matches, teams)
        {
        }

        protected override double LambdaHome(GameTeam homeTeam, GameTeam awayTeam)
        {
            return homeTeam.HomeAttack * awayTeam.AwayAttack * Gamma * Mi;
        }

        protected override double LambdaAway(GameTeam homeTeam, GameTeam awayTeam)
        {
            return awayTeam.HomeAttack * homeTeam.AwayAttack * Mi;
        }
    }
}
