using BetManager.Solver.Managers;
using BetManager.Solver.Models;
using Microsoft.SolverFoundation.Services;
using System;
using System.Diagnostics;

namespace BetManager.Solver.Solvers
{
    /// <summary>
    /// How to connect together
    /// https://msdn.microsoft.com/en-us/library/ff847512(v=vs.93).aspx
    /// </summary>
    public class DPSolver : IDixonColesSolver
    {
        private readonly IDixonManager _DixonManager;
        public string LastReport { get; private set; }

        public DPSolver(IDixonManager dixonManager)
        {
            _DixonManager = dixonManager;
        }

        public double Solve(DateTime actualDate)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();

            // solver init
            SolverContext context = SolverContext.GetContext();
            context.ClearModel();

            Model model = context.CreateModel();

            Set matches = new Set(Domain.Integer, "matches");
            Set teams = new Set(Domain.Integer, "Teams");
            Set facts = new Set(Domain.Integer, "facts");

            // parameters
            Parameter homeScore = new Parameter(Domain.IntegerNonnegative, "homeScore", matches, teams, teams);
            homeScore.SetBinding(_DixonManager.Matches, "HomeScore", "Id", "HomeTeamId", "AwayTeamId");

            Parameter awayScore = new Parameter(Domain.IntegerNonnegative, "awayScore", matches, teams, teams);
            awayScore.SetBinding(_DixonManager.Matches, "AwayScore", "Id", "HomeTeamId", "AwayTeamId");

            Parameter days = new Parameter(Domain.IntegerNonnegative, "days", matches, teams, teams);
            days.SetBinding(_DixonManager.Matches, "Days", "Id", "HomeTeamId", "AwayTeamId");

            Parameter homeTeamId = new Parameter(Domain.IntegerNonnegative, "homeTeamId", matches);
            homeTeamId.SetBinding(_DixonManager.Matches, "HomeTeamId", "Id");

            Parameter awayTeamId = new Parameter(Domain.IntegerNonnegative, "awayTeamId", matches);
            awayTeamId.SetBinding(_DixonManager.Matches, "AwayTeamId", "Id");

            var fact = new System.Collections.Generic.List<Factorial>();
            for (int i = 0; i <= _DixonManager.PropLength; i++)
            {
                fact.Add(new Factorial
                {
                    Id = i,
                    Value = MethodExtensions.LogFactorial(i) // Math.Log(DixonManager.Factorial(i))
                });
            }

            Parameter factorial = new Parameter(Domain.RealNonnegative, "factorial", facts);
            factorial.SetBinding(fact, "Value", "Id");

            Parameter ksi = new Parameter(Domain.Real, "ksi");
            ksi.SetBinding(_DixonManager.Ksi);

            model.AddParameters(homeScore, awayScore, days, homeTeamId, awayTeamId, factorial, ksi);

            // decisions
            Decision attack = new Decision(Domain.RealRange(0, 2), "attack", teams);
            attack.SetBinding(_DixonManager.Teams, "HomeAttack", "Id");

            Decision defence = new Decision(Domain.RealRange(0, 2), "defence", teams);
            defence.SetBinding(_DixonManager.Teams, "AwayAttack", "Id");

            // Decision rho = new Decision(Domain.RealRange(-1, 1), "rho");
            Decision gamma = new Decision(Domain.RealRange(0, 3), "gamma");
            Decision mi = new Decision(Domain.RealRange(0, 3), "mi");

            model.AddDecisions(attack, defence, gamma, mi);

            // constraints
            model.AddConstraint("homeAttackCount",
                Model.Sum(Model.ForEach(teams, t => attack[t])) == _DixonManager.Teams.Count);
            model.AddConstraint("awayAttackCount",
                Model.Sum(Model.ForEach(teams, t => defence[t])) == _DixonManager.Teams.Count);

            Goal sum = model.AddGoal("sum", GoalKind.Maximize,
                Model.Sum(
                   Model.ForEach(matches, match =>
                   Model.ForEachWhere(teams, h =>
                   Model.ForEachWhere(teams, a =>
                   {
                       // home-attack
                       var lambda = mi * attack[h] * defence[a] * gamma;

                       // away-attack
                       var mu = mi * attack[a] * defence[h];

                       //// funkce tau
                       //var tau = Model.If(Model.And(homeScore[match, h, a] == 0, awayScore[match, h, a] == 0),
                       //    1 - lambda * mu * rho,
                       //    Model.If(Model.And(homeScore[match, h, a] == 0, awayScore[match, h, a] == 1),
                       //    1 + lambda * rho,
                       //    Model.If(Model.And(homeScore[match, h, a] == 1, awayScore[match, h, a] == 0),
                       //    1 + mu * rho,
                       //    Model.If(Model.And(homeScore[match, h, a] == 1, awayScore[match, h, a] == 1),
                       //    1 - rho,
                       //    1))));

                       return
                       // casova fce
                       Model.Exp(-ksi * days[match, h, a] / 365.25)
                       *
                        (
                            (
                                -lambda + (homeScore[match, h, a] * Model.Log(lambda))
                            ) - factorial[homeScore[match, h, a]]
                            +
                            (
                                -mu + (awayScore[match, h, a] * Model.Log(mu))
                            ) - factorial[awayScore[match, h, a]]
                        );
                   }, a => awayTeamId[match] == a
                   ), h => homeTeamId[match] == h
                   ))));

            context.CheckModel();

            // solve
            var solution = context.Solve(new HybridLocalSearchDirective());
            LastReport = solution.GetReport().ToString();

            context.PropagateDecisions();

            watch.Stop();

            _DixonManager.Mi = mi.GetDouble();  // rho.GetDouble();
            _DixonManager.Gamma = gamma.GetDouble();
            _DixonManager.Summary = sum.ToDouble();
            _DixonManager.LastElapsed = watch.Elapsed;

            // navrat
            return _DixonManager.Summary;
        }

        public void Dispose()
        {
            // throw new NotImplementedException();
        }
    }
}
