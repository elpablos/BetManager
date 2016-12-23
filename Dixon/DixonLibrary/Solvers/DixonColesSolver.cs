using Dixon.Library.Managers;
using Microsoft.SolverFoundation.Services;
using System;

namespace Dixon.Library.Solvers
{
    /// <summary>
    /// How to connect together
    /// https://msdn.microsoft.com/en-us/library/ff847512(v=vs.93).aspx
    /// </summary>
    public class DixonColesSolver : IDixonColesSolver
    {
        private readonly IDixonManager _DixonManager;

        public DixonColesSolver(IDixonManager dixonManager)
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
                Model.Sum(
                    Model.ForEach(matches, match =>
                    {
                        // home-attack
                        var lambda = Model.Sum(Model.ForEachWhere(teams, t => homeAttack[t], t => Model.Equal(t, homeTeamId[match])))
                        * Model.Sum(Model.ForEachWhere(teams, t => awayAttack[t], t => Model.Equal(t, awayTeamId[match]))) * gama;
                        // away-attack
                        var mu = Model.ForEachWhere(teams, t => Model.Sum(awayAttack[t]), t => t == homeTeamId[match])
                         * Model.ForEachWhere(teams, t => homeAttack[t], t => t == awayTeamId[match]);

                        mu = 1;
                        var tau = 1;
                        var dateDif = 200;

                        return
                        // casova fce
                        Model.Exp(-epsilon * dateDif)
                        // ln fce zavislosti tau
                        * (Model.Log(tau)
                        // pocet golu domaciho + ln predikce golu domaciho
                        + homeScore[match] * Model.Log(lambda)
                        // minus predikce golu domaciho
                        - lambda
                        // pocet golu hosti + ln predikce golu hosti
                        + awayScore[match] * Model.Log(mu)
                        // minus predikce golu hosti
                        - mu);
                    })
                ));

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

            context.CheckModel();

            // solve
            context.Solve();

            Console.WriteLine(String.Format("Sum consumption: {0:f2}", sum.ToDouble()));

            context.PropagateDecisions();

            // navrat
            return result;
        }
    }
}
