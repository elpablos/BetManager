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
    public class DPDISolver : BaseSolver
    {
        public DPDISolver(IDixonManager dixonManager)
            :base(dixonManager)
        { }

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
            Set thetas = new Set(Domain.Integer, "thetas");

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

            Decision p = new Decision(Domain.RealRange(-1, 1), "p");
            Decision gamma = new Decision(Domain.RealRange(0, 3), "gamma");
            Decision mi = new Decision(Domain.RealRange(0, 3), "mi");

            var maxThetas = _DixonManager.PropLength > 10 ? 5 : 3;
            var thet = new System.Collections.Generic.List<Factorial>();
            for (int i = 0; i <= maxThetas; i++)
            {
                thet.Add(new Factorial
                {
                    Id = i,
                    Value = 0.16
                });
            }

            Decision theta = new Decision(Domain.RealNonnegative, "theta", thetas);
            theta.SetBinding(thet, "Value", "Id");

            model.AddDecisions(attack, defence, gamma, mi, theta, p);

            // constraints
            model.AddConstraint("homeAttackCount",
                Model.Sum(Model.ForEach(teams, t => attack[t])) == _DixonManager.Teams.Count);
            model.AddConstraint("awayAttackCount",
                Model.Sum(Model.ForEach(teams, t => defence[t])) == _DixonManager.Teams.Count);

            //double dif = 1.1;
            if (maxThetas == 5)
            {
                model.AddConstraint("limits", Model.Sum(theta[0], theta[1], theta[2], theta[3], theta[4], theta[5]) == 1);
            }
            else
            {
                model.AddConstraint("limits", Model.Sum(theta[0], theta[1], theta[2], theta[3]) == 1);
            }

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

                       // casova fce -nonopt (znelogaritmovano)
                       //var nonopt =
                       //(((Model.Power(lambda, homeScore[match, h, a]) * Model.Exp(-lambda))
                       // / factorial[homeScore[match, h, a]])
                       // *
                       // ((Model.Power(mu, awayScore[match, h, a]) * Model.Exp(-mu))
                       // / factorial[awayScore[match, h, a]]));

                   return
                   //Model.Exp(-ksi * days[match, h, a] / 365.25) *
                   //Model.Log
                   //(
                   //    (1 - p) * nonopt
                   //    +
                   //      Model.If
                   //      (Model.And(homeScore[match, h, a] == awayScore[match, h, a], homeScore[match, h, a] <= maxThetas),
                   //          (p * theta[homeScore[match, h, a]]),
                   //          0)
                   //);

                   timeValues[match, h, a] *
                   Model.If
                   (Model.And(homeScore[match, h, a] == awayScore[match, h, a], homeScore[match, h, a] <= maxThetas),
                   Model.Log
                       (
                           (1 - p) *
                           (
                           (((Model.Power(lambda, homeScore[match, h, a]) * Model.Exp(-lambda)))
                            *
                            ((Model.Power(mu, awayScore[match, h, a]) * Model.Exp(-mu))
                            ))
                           )
                           + (p * theta[homeScore[match, h, a]])
                       ),
                   (Model.Log(1 - p) +
                   (
                        -lambda + (homeScore[match, h, a] * Model.Log(lambda))
                        - mu + (awayScore[match, h, a] * Model.Log(mu))
                        )));
                   }, a => awayTeamId[match] == a
                   ), h => homeTeamId[match] == h
                   ))));

            context.CheckModel();

            // solve
            var solution = context.Solve(Directive);
            LastReport = solution.GetReport().ToString();

            context.PropagateDecisions();

            watch.Stop();

            _DixonManager.Thetas.Clear();
            foreach (var t in thet)
            {
                _DixonManager.Thetas.Add(t.Value);
            }

            _DixonManager.Rho = 0;
            _DixonManager.Mi = mi.GetDouble();
            _DixonManager.Gamma = gamma.GetDouble();
            _DixonManager.Summary = sum.ToDouble();
            _DixonManager.P = p.ToDouble();
            _DixonManager.LastElapsed = watch.Elapsed;

            // navrat
            return _DixonManager.Summary;
        }
    }
}
