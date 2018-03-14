using Prediction.Core.Managers;
using Prediction.Core.Models;
using Microsoft.SolverFoundation.Services;
using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;

namespace Prediction.Core.Solvers
{
    /// <summary>
    /// Implementace mahera pro hokej!
    /// </summary>
    public class MaherExtSolver : BaseSolver
    {
        public MaherExtSolver(IDixonManager dixonManager)
            : base(dixonManager)
        {
        }

        public override double Solve(DateTime actualDate)
        {
            Stopwatch watch = new Stopwatch();

            var extendTeams = new List<GameTeamExtend>();

            var removed = _DixonManager.Teams.Where(x => x.Id > 14).ToList();

            foreach (var rem in removed)
            {
                _DixonManager.Teams.Remove(rem);
            }

            foreach (var team in _DixonManager.Teams)
            {
                var extendTeam = new GameTeamExtend
                {
                    Id = team.Id,
                    DefaultHomeAttack = 1.0,
                    DefaultAwayAttack = 1.0,
                    HomeAttack = 1.0, // team.HomeAttack,
                    AwayAttack = 1.0, // team.AwayAttack,
                    DisplayName = team.DisplayName,
                    // vstrelene goly
                    GoalGiven = _DixonManager.Matches.Where(x => x.HomeTeamId == team.Id).Sum(x => x.HomeScore)
                    + _DixonManager.Matches.Where(x => x.AwayTeamId == team.Id).Sum(x => x.AwayScore),
                    // obdrzene goly
                    GoalTaken = _DixonManager.Matches.Where(x => x.HomeTeamId == team.Id).Sum(x => x.AwayScore)
                    + _DixonManager.Matches.Where(x => x.AwayTeamId == team.Id).Sum(x => x.HomeScore),
                };

                extendTeams.Add(extendTeam);
            }

            watch.Start();


            // solver init
            SolverContext context = SolverContext.GetContext();
            context.ClearModel();

            Model model = context.CreateModel();

            Set teams = new Set(Domain.Integer, "Teams");

            var homeScore = _DixonManager.Matches.Sum(x => x.HomeScore);
            var awayScore = _DixonManager.Matches.Sum(x => x.AwayScore);
            _DixonManager.Ksi = awayScore * 1.0 / homeScore;

            // parameters

            // EXCEL pocet golu domaci
            Parameter homeGoalCount = new Parameter(Domain.IntegerNonnegative, "homeGoalCount");
            homeGoalCount.SetBinding(homeScore);

            // EXCEL pocet golu hoste
            Parameter awayGoalCount = new Parameter(Domain.IntegerNonnegative, "awayGoalCount");
            awayGoalCount.SetBinding(awayScore);

            // EXCEL k^2
            Parameter ksi = new Parameter(Domain.Real, "ksi");
            ksi.SetBinding(_DixonManager.Ksi);

            // EXCEL vstrelene goly
            Parameter GoalGiven = new Parameter(Domain.IntegerNonnegative, "GoalGiven", teams);
            GoalGiven.SetBinding(extendTeams, "GoalGiven", "Id");

            // EXCEL obdrzene goly
            Parameter GoalTaken = new Parameter(Domain.IntegerNonnegative, "GoalTaken", teams);
            GoalTaken.SetBinding(extendTeams, "GoalTaken", "Id");

            model.AddParameters(homeGoalCount, awayGoalCount, GoalGiven, GoalTaken, ksi);

            // decisions

            // EXCEL - alpha vychozi
            Decision DefaultHomeAttack = new Decision(Domain.RealRange(0, 5), "DefaultHomeAttack", teams);
            DefaultHomeAttack.SetBinding(extendTeams, "DefaultHomeAttack", "Id");

            // EXCEL - beta vychozi
            Decision DefaultAwayAttack = new Decision(Domain.RealRange(0, 5), "DefaultAwayAttack", teams);
            DefaultAwayAttack.SetBinding(extendTeams, "DefaultAwayAttack", "Id");

            model.AddDecisions(DefaultHomeAttack, DefaultAwayAttack);

            // constraints

            var alphaSum = Model.Sum(Model.ForEach(teams, team => ((GoalGiven[team] / ((1.0 + ksi) * Model.Sum(Model.ForEachWhere(teams, t => DefaultAwayAttack[t], x => x != team)) * 2)))));
            var betaSum = Model.Sum(Model.ForEach(teams, team => ((GoalTaken[team] / ((1.0 + ksi) * Model.Sum(Model.ForEachWhere(teams, t => DefaultHomeAttack[t], x => x != team)) * 2)))));

            // 1) podm utok = obrana suma(pro vsechny tymy(alpha)) == suma(pro vsechny tymy(beta))
            model.AddConstraint("const", alphaSum == betaSum);

            Goal sum = model.AddGoal("sum", GoalKind.Minimize,
            Model.Abs(Model.Sum(
                Model.ForEach(teams, team =>
                {
                    var alpha = ((GoalGiven[team] / ((1.0 + ksi) * Model.Sum(Model.ForEachWhere(teams, t => DefaultAwayAttack[t], x => x != team)) * 2)));
                    var beta = ((GoalTaken[team] / ((1.0 + ksi) * Model.Sum(Model.ForEachWhere(teams, t => DefaultHomeAttack[t], x => x != team)) * 2)));
                    return (alpha * betaSum - (alpha * beta)) * 2;
                })
                ) - homeGoalCount));

            context.CheckModel();

            // solve
            var solution = context.Solve(Directive);

            LastReport = solution.GetReport().ToString();

            context.PropagateDecisions();
            _DixonManager.Summary = sum.ToDouble();

            var sumDefaultAwayAttack = extendTeams.Sum(x => x.DefaultAwayAttack);
            var sumDefaultHomeAttack = extendTeams.Sum(x => x.DefaultHomeAttack);

            // dopocitam 
            foreach (var t in extendTeams)
            {
                t.HomeAttack = t.GoalGiven / ((1 + _DixonManager.Ksi) * (sumDefaultAwayAttack - t.DefaultAwayAttack) * 2);
                t.AwayAttack = t.GoalTaken / ((1 + _DixonManager.Ksi) * (sumDefaultHomeAttack - t.DefaultHomeAttack) * 2);
            }

            foreach (var team in extendTeams)
            {
                team.DefaultHomeAttack = team.HomeAttack;
                team.DefaultAwayAttack = team.AwayAttack;
            }

            sumDefaultAwayAttack = extendTeams.Sum(x => x.DefaultAwayAttack);
            sumDefaultHomeAttack = extendTeams.Sum(x => x.DefaultHomeAttack);

            foreach (var t in extendTeams)
            {
                t.HomeAttack = t.GoalGiven / ((1 + _DixonManager.Ksi) * (sumDefaultAwayAttack - t.DefaultAwayAttack) * 2);
                t.AwayAttack = t.GoalTaken / ((1 + _DixonManager.Ksi) * (sumDefaultHomeAttack - t.DefaultHomeAttack) * 2);
            }

            Console.WriteLine("Elapsed: {0}\t{1}\n", watch.Elapsed, _DixonManager.Summary);

            watch.Stop();

            foreach (var team in extendTeams)
            {
                var originalTeam = _DixonManager.Teams.FirstOrDefault(x => x.Id == team.Id);
                originalTeam.HomeAttack = team.HomeAttack;
                originalTeam.AwayAttack = team.AwayAttack;
            }

            _DixonManager.LastElapsed = watch.Elapsed;
            _DixonManager.Gamma = _DixonManager.Ksi;
            _DixonManager.Mi = 0;
            _DixonManager.Lambda = _DixonManager.Teams.Sum(x => x.HomeAttack);
            _DixonManager.Rho = _DixonManager.Teams.Sum(x => x.HomeAttack);

            // navrat
            return _DixonManager.Summary;
        }
    }
}
