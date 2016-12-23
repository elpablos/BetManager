using Dixon.Library.Managers;
using Microsoft.SolverFoundation.Services;
using System;

namespace Dixon.Library.Solvers
{
    public class DixonColesSolver2 : IDixonColesSolver
    {
        private readonly IDixonManager _DixonManager;

        public DixonColesSolver2(IDixonManager dixonManager)
        {
            _DixonManager = dixonManager;
        }

        public double Solve(DateTime actualDate)
        {
            double result = 0;

            // solver init
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            

            Set matches = new Set(Domain.Integer, "matches");
            Set teams = new Set(Domain.Integer, "Teams");

            // parameters
            //Parameter dateStart = new Parameter(Domain.Any, "dateStart", matches);
            //dateStart.SetBinding(_DixonManager.Matches, "DateStart", "Id");

            Parameter homeScore = new Parameter(Domain.IntegerNonnegative, "homeScore", matches);
            homeScore.SetBinding(_DixonManager.Matches, "HomeScore", "Id");

            Parameter awayScore = new Parameter(Domain.IntegerNonnegative, "awayScore", matches);
            awayScore.SetBinding(_DixonManager.Matches, "AwayScore", "Id");

            Parameter homeTeamId = new Parameter(Domain.IntegerNonnegative, "homeTeamId", matches);
            homeTeamId.SetBinding(_DixonManager.Matches, "HomeTeamId", "Id");

            Parameter awayTeamId = new Parameter(Domain.IntegerNonnegative, "awayTeamId", matches);
            awayTeamId.SetBinding(_DixonManager.Matches, "AwayTeamId", "Id");

            Parameter epsilon = new Parameter(Domain.RealNonnegative, "epsilon");
            epsilon.SetBinding(_DixonManager.Epsilon);

            model.AddParameters(homeScore, awayScore, homeTeamId, awayTeamId, epsilon);

            // decisions
            Decision homeAttack = new Decision(Domain.RealRange(0, 3), "homeAttack", teams);
            homeAttack.SetBinding(_DixonManager.Teams, "HomeAttack", "Id");

            Decision awayAttack = new Decision(Domain.RealRange(0, 3), "awayAttack", teams);
            awayAttack.SetBinding(_DixonManager.Teams, "AwayAttack", "Id");

            Decision rho = new Decision(Domain.RealRange(0, 1), "rho");

            Decision gama = new Decision(Domain.RealRange(1, 2), "gama");

            model.AddDecisions(homeAttack, awayAttack, rho, gama);

            // constraints
            model.AddConstraint("homeAttackCount",
                Model.Sum(Model.ForEach(teams, t => homeAttack[t])) == _DixonManager.Teams.Count);
            model.AddConstraint("awayAttackCount",
                Model.Sum(Model.ForEach(teams, t => awayAttack[t])) == _DixonManager.Teams.Count);

            Goal sum = model.AddGoal("sum", GoalKind.Maximize,
                Model.Sum(_DixonManager.Sum(actualDate))
                );

            /*
            // home-attack
            var lambda = (match.HomeTeam.HomeAttack * match.AwayTeam.AwayAttack * Gama);
            // away-attack
            var mu = (match.HomeTeam.AwayAttack * match.AwayTeam.HomeAttack);

            return
                // casova fce
                match.TimeFunc(dateActual, Epsilon)
                // ln fce zavislosti tau
                * (Math.Log(DependenceTau(match, lambda, mu))
                // pocet golu domaciho + ln predikce golu domaciho
                + match.HomeScore * Math.Log(lambda)
                // minus predikce golu domaciho
                - lambda
                // pocet golu hosti + ln predikce golu hosti
                + match.AwayScore * Math.Log(mu)
                // minus predikce golu hosti
                - mu
                );
             */

            // solve
            context.Solve();
            context.PropagateDecisions();

            Console.WriteLine(String.Format("Sum consumption: {0:f2}", sum.ToDouble()));

            // navrat
            return result;
        }
    }
}

