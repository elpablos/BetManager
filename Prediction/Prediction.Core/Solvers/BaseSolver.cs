using Microsoft.SolverFoundation.Services;
using Prediction.Core.Managers;
using Prediction.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Prediction.Core.Solvers
{
    public abstract class BaseSolver : IDixonColesSolver
    {
        public string LastReport { get; protected set; }

        protected readonly IDixonManager _DixonManager;

        public HybridLocalSearchDirective Directive { get; private set; }

        public BaseSolver(IDixonManager dixonManager)
        {
            _DixonManager = dixonManager;
            Directive = new HybridLocalSearchDirective();
        }

        public virtual void Dispose()
        {
        }

        protected virtual ICollection<GameMatch> FilterMatch()
        {
            var matchList = new List<GameMatch>();
            var teamList = new HashSet<GameTeam>();

            foreach (var match in _DixonManager.Matches)
            {
                if (match.Days <= 700)
                {
                    match.TimeValue = TimeFunc(match, _DixonManager.Ksi);
                    matchList.Add(match);

                    teamList.Add(match.HomeTeam);
                    teamList.Add(match.AwayTeam);
                }
            }

            var except = _DixonManager.Teams.Except(teamList).ToList();
            if (except.Count > 0)
            {
                foreach (var exc in except)
                {
                    _DixonManager.Teams.Remove(exc);
                }
            }

            return matchList;
        }

        protected virtual double TimeFunc(GameMatch match, double ksi)
        {
            // Model.Exp(-ksi * days[match, h, a] / 365.25)
            return Math.Exp(-ksi * match.Days / 365.25);
        }

        public abstract double Solve(DateTime actualDate);
    }
}
