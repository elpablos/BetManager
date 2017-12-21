﻿using Prediction.Core.Managers;
using Prediction.Core.Models;
using Microsoft.SolverFoundation.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Prediction.Core.Solvers
{
    public class GameTeamExtend : GameTeam
    {
        public double DefaultHomeAttack { get; set; }

        public double DefaultAwayAttack { get; set; }

        public int GoalGiven { get; set; }

        public int GoalTaken { get; set; }

        public double Summ { get; set; }
    }

    /// <summary>
    /// How to connect together
    /// https://msdn.microsoft.com/en-us/library/ff847512(v=vs.93).aspx
    /// </summary>
    public class MaherSolver : BaseSolver
    {
        public MaherSolver(IDixonManager dixonManager)
            :base(dixonManager)
        {
        }

        public override double Solve(DateTime actualDate)
        {
            Stopwatch watch = new Stopwatch();

            var extendTeams = new List<GameTeamExtend>();
            foreach (var team in _DixonManager.Teams)
            {
                if (team.Id > 14) continue;
                var extendTeam = new GameTeamExtend
                {
                    Id = team.Id,
                    DefaultHomeAttack = 1.0,
                    DefaultAwayAttack = 1.0,
                    HomeAttack = 1.0, // team.HomeAttack,
                    AwayAttack = 1.0, // team.AwayAttack,
                    DisplayName = team.DisplayName,
                    GoalGiven = _DixonManager.Matches.Where(x => x.HomeTeamId == team.Id).Sum(x => x.HomeScore)
                    + _DixonManager.Matches.Where(x => x.AwayTeamId == team.Id).Sum(x => x.AwayScore),
                    GoalTaken = _DixonManager.Matches.Where(x => x.HomeTeamId == team.Id).Sum(x => x.AwayScore)
                    + _DixonManager.Matches.Where(x => x.AwayTeamId == team.Id).Sum(x => x.HomeScore),
                };

                extendTeams.Add(extendTeam);
            }

            watch.Start();

            // 10 iteraci!
            for (int i = 0; i < 10; i++)
            {
                // solver init
                SolverContext context = SolverContext.GetContext();
                context.ClearModel();

                Model model = context.CreateModel();

                foreach (var team in extendTeams)
                {
                    team.DefaultHomeAttack = team.HomeAttack;
                    team.DefaultAwayAttack = team.AwayAttack;
                }

                Set teams = new Set(Domain.Integer, "Teams");

                var homeScore = _DixonManager.Matches.Sum(x => x.HomeScore);
                var awayScore = _DixonManager.Matches.Sum(x => x.AwayScore);
                _DixonManager.Ksi = awayScore * 1.0 / homeScore;

                // parameters
                Parameter homeGoalCount = new Parameter(Domain.IntegerNonnegative, "homeGoalCount");
                homeGoalCount.SetBinding(homeScore);

                Parameter awayGoalCount = new Parameter(Domain.IntegerNonnegative, "awayGoalCount");
                awayGoalCount.SetBinding(awayScore);

                Parameter ksi = new Parameter(Domain.Real, "ksi");
                ksi.SetBinding(_DixonManager.Ksi);

                Parameter DefaultHomeAttack = new Parameter(Domain.RealNonnegative, "DefaultHomeAttack", teams);
                DefaultHomeAttack.SetBinding(extendTeams, "DefaultHomeAttack", "Id");

                Parameter DefaultAwayAttack = new Parameter(Domain.RealNonnegative, "DefaultAwayAttack", teams);
                DefaultAwayAttack.SetBinding(extendTeams, "DefaultAwayAttack", "Id");

                Parameter GoalGiven = new Parameter(Domain.IntegerNonnegative, "GoalGiven", teams);
                GoalGiven.SetBinding(extendTeams, "GoalGiven", "Id");

                Parameter GoalTaken = new Parameter(Domain.IntegerNonnegative, "GoalTaken", teams);
                GoalTaken.SetBinding(extendTeams, "GoalTaken", "Id");

                model.AddParameters(homeGoalCount, awayGoalCount, GoalGiven, GoalTaken, DefaultHomeAttack, DefaultAwayAttack, ksi);

                // decisions
                Decision attack = new Decision(Domain.RealNonnegative, "attack", teams);
                attack.SetBinding(extendTeams, "HomeAttack", "Id");

                Decision defence = new Decision(Domain.RealNonnegative, "defence", teams);
                defence.SetBinding(extendTeams, "AwayAttack", "Id");

                model.AddDecisions(attack, defence);

                // constraints
                model.AddConstraint("homeAttackCount",
                    Model.Sum(Model.ForEach(teams, t => attack[t])) == Model.Sum(Model.ForEach(teams, t => defence[t])));

                model.AddConstraint("alpha",
                    Model.ForEach(teams, team =>
                        ((GoalGiven[team] / ((1.0 + ksi) * Model.Sum(Model.ForEachWhere(teams, t => DefaultAwayAttack[t], x => x != team)))) == attack[team]
                        )));

                model.AddConstraint("beta",
                    Model.ForEach(teams, team =>
                        ((GoalTaken[team] / ((1.0 + ksi) * Model.Sum(Model.ForEachWhere(teams, t => DefaultHomeAttack[t], x => x != team)))) == defence[team]
                        )));


                Goal sum = model.AddGoal("sum", GoalKind.Minimize,
                Model.Abs(Model.Sum(
                     Model.ForEach(teams, teamz =>
                     {
                         // soucet vsech bet_odhad ALE bez pocitaneho tymu
                         var suma = Model.Sum(Model.ForEachWhere(teams, team =>
                         {
                             // suma alpha_vychozi bez pocitaneho tymu
                             var sumaDefaultHome = Model.Sum(Model.ForEachWhere(teams, t => DefaultHomeAttack[t], x => x != team));

                             // pocet obdrzenych golu tymu / ((1+ksi) * suma alpha_vychozi BEZ aktualniho tymu
                             return (GoalTaken[team] / ((1.0 + ksi) * sumaDefaultHome));
                         }, x => x != teamz)); // suma vsech vyjma aktualniho pocitanyho tymu

                         return Model.ForEachWhere(teams, team =>
                         {
                             var sumaDefaultAway = Model.Sum(Model.ForEachWhere(teams, t => DefaultAwayAttack[t], x => x != team));
                             var alpha_odhad = (GoalGiven[team] / ((1.0 + ksi) * sumaDefaultAway));

                             return (alpha_odhad * suma) * 2.0; // 
                         }, x => x == teamz); // pouze aktualni pocitany tym
                     })) - homeGoalCount)); // 

                context.CheckModel();

                // solve
                var solution = context.Solve(Directive);
                LastReport = solution.GetReport().ToString();

                context.PropagateDecisions();

                _DixonManager.Summary = sum.ToDouble();

                Console.WriteLine("Round {0} - {1}\t{2}\n", i + 1, watch.Elapsed, _DixonManager.Summary);
            }

            watch.Stop();

            foreach (var team in extendTeams)
            {
                var originalTeam = _DixonManager.Teams.FirstOrDefault(x => x.Id == team.Id);
                originalTeam.HomeAttack = team.HomeAttack;
                originalTeam.AwayAttack = team.AwayAttack;
            }

            _DixonManager.LastElapsed = watch.Elapsed;
            _DixonManager.Gamma = 1.0;
            _DixonManager.Mi = 0;
            _DixonManager.Lambda = _DixonManager.Teams.Sum(x => x.HomeAttack);
            _DixonManager.Rho = _DixonManager.Teams.Sum(x => x.HomeAttack);

            // navrat
            return _DixonManager.Summary;
        }
    }
}