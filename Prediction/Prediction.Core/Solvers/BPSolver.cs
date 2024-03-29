﻿using Prediction.Core.Managers;
using Prediction.Core.Models;
using Microsoft.SolverFoundation.Services;
using System;
using System.Diagnostics;

namespace Prediction.Core.Solvers
{
    /// <summary>
    /// How to connect together
    /// https://msdn.microsoft.com/en-us/library/ff847512(v=vs.93).aspx
    /// </summary>
    public class BPSolver : BaseSolver
    {
        public BPSolver(IDixonManager dixonManager)
            :base (dixonManager)
        {
        }

        public override double Solve(DateTime actualDate)
        {
            Stopwatch watch = new Stopwatch();

            var matchList = FilterMatch();

            //var fact = new System.Collections.Generic.List<Factorial>();
            //for (int i = 0; i <= _DixonManager.PropLength; i++)
            //{
            //    fact.Add(new Factorial
            //    {
            //        Id = i,
            //        Value = MethodExtensions.LogFactorial(i) // opt
            //        // Value = MethodExtensions.Factorial(i) * 1.0 // non-opt
            //    });
            //}

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
            homeScore.SetBinding(matchList, "HomeScore", "Id", "HomeTeamId", "AwayTeamId");

            Parameter awayScore = new Parameter(Domain.IntegerNonnegative, "awayScore", matches, teams, teams);
            awayScore.SetBinding(matchList, "AwayScore", "Id", "HomeTeamId", "AwayTeamId");

            Parameter timeValues = new Parameter(Domain.RealNonnegative, "days", matches, teams, teams);
            timeValues.SetBinding(matchList, "TimeValue", "Id", "HomeTeamId", "AwayTeamId");

            Parameter homeTeamId = new Parameter(Domain.IntegerNonnegative, "homeTeamId", matches);
            homeTeamId.SetBinding(matchList, "HomeTeamId", "Id");

            Parameter awayTeamId = new Parameter(Domain.IntegerNonnegative, "awayTeamId", matches);
            awayTeamId.SetBinding(matchList, "AwayTeamId", "Id");

            //Parameter factorial = new Parameter(Domain.RealNonnegative, "factorial", facts);
            //factorial.SetBinding(fact, "Value", "Id");

            Parameter ksi = new Parameter(Domain.Real, "ksi");
            ksi.SetBinding(_DixonManager.Ksi);

            model.AddParameters(homeScore, awayScore, timeValues, homeTeamId, awayTeamId, ksi); // factorial

            // decisions
            Decision attack = new Decision(Domain.RealRange(0, 2), "attack", teams);
            attack.SetBinding(_DixonManager.Teams, "HomeAttack", "Id");

            Decision defence = new Decision(Domain.RealRange(0, 2), "defence", teams);
            defence.SetBinding(_DixonManager.Teams, "AwayAttack", "Id");

            Decision lambda = new Decision(Domain.RealRange(-2, 2), "lambda");
            Decision gamma = new Decision(Domain.RealRange(0, 3), "gamma");
            Decision mi = new Decision(Domain.RealRange(0, 3), "mi");

            model.AddDecisions(attack, defence, gamma, mi, lambda);

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
                       var lambdaHome = mi * attack[h] * defence[a] * gamma;

                       // away-attack
                       var lambdaAway = mi * attack[a] * defence[h];

                       // casova fce -nonopt (castecne zlogaritmovano)
                       //var opt = 
                       //(
                       //     homeScore[match, h, a] * Model.Log(lambdaHome) + awayScore[match, h, a] * Model.Log(lambdaAway) + (-lambdaHome - lambdaAway) +
                       //     Model.Log
                       //     (
                       //         1 + lambda * (Model.Exp(-homeScore[match, h, a]) - Model.Exp(-(1 - Model.Exp(-1)) * lambdaHome))
                       //         * (Model.Exp(-awayScore[match, h, a]) - Model.Exp(-(1 - Model.Exp(-1)) * lambdaAway))
                       //     )
                       //     -
                       //     (
                       //         factorial[homeScore[match, h, a]] + factorial[awayScore[match, h, a]]
                       //     )
                       // );

                       // casova fce -nonopt (znelogaritmovano)
                       //var nonopt =
                       //(
                       //     Model.Power(lambdaHome, homeScore[match, h, a]) * Model.Power(lambdaAway, awayScore[match, h, a]) * Model.Exp(-lambdaHome - lambdaAway)
                       //     *
                       //     (
                       //         (
                       //             1 + lambda * (Model.Exp(-homeScore[match, h, a]) - Model.Exp(-(1 - Model.Exp(-1)) * lambdaHome))
                       //             * (Model.Exp(-awayScore[match, h, a]) - Model.Exp(-(1 - Model.Exp(-1)) * lambdaAway))
                       //         )
                       //         /
                       //         (
                       //             factorial[homeScore[match, h, a]] * factorial[awayScore[match, h, a]]
                       //         )
                       //     )
                       //);

                       var fastopt =
                       (
                            homeScore[match, h, a] * Model.Log(lambdaHome) 
                            + awayScore[match, h, a] * Model.Log(lambdaAway) 
                            + (-lambdaHome - lambdaAway) 
                            + Model.Log
                            (
                                1 + lambda * (Model.Exp(-homeScore[match, h, a]) - Model.Exp(-(1 - Model.Exp(-1)) * lambdaHome))
                                * (Model.Exp(-awayScore[match, h, a]) - Model.Exp(-(1 - Model.Exp(-1)) * lambdaAway))
                            )
                        );

                       return
                       //Model.Exp(-ksi * days[match, h, a] / 365.25) *
                       //(
                       //     // opt
                       //     Model.Log(nonopt)
                       //);

                       // superfast
                       timeValues[match, h, a] * fastopt;

                   }, a => awayTeamId[match] == a
                   ), h => homeTeamId[match] == h
                   ))));

            context.CheckModel();

            // solve
            var solution = context.Solve(Directive);
            LastReport = solution.GetReport().ToString();

            context.PropagateDecisions();

            watch.Stop();

            _DixonManager.Rho = 0;
            _DixonManager.Lambda = lambda.GetDouble();
            _DixonManager.Mi = mi.GetDouble();
            _DixonManager.Gamma = gamma.GetDouble();
            _DixonManager.Summary = sum.ToDouble();
            _DixonManager.LastElapsed = watch.Elapsed;

            // navrat
            return _DixonManager.Summary;
        }
    }
}
