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
    /// How to connect together
    /// https://msdn.microsoft.com/en-us/library/ff847512(v=vs.93).aspx
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

                Decision summ = new Decision(Domain.RealNonnegative, "summ", teams);
                summ.SetBinding(extendTeams, "Summ", "Id");

                model.AddDecisions(attack, defence, summ);

                // constraints

                // 1) podm utok = obrana suma(pro vsechny tymy(alpha)) == suma(pro vsechny tymy(beta))
                model.AddConstraint("homeAttackCount",
                    Model.Sum(Model.ForEach(teams, t => attack[t])) == Model.Sum(Model.ForEach(teams, t => defence[t])));

                // 2) podm alpha pro vsechny tymy(pocet_vstrelenych_golu_tymu / 1 + ksi)
                // * suma(pro vsechny tymy VYJMA aktualniho(beta_vychozi))) == alpha
                model.AddConstraint("alpha",
                    Model.ForEach(teams, team =>
                    (
                        (GoalGiven[team] / ((1.0 + ksi)
                            * Model.Sum(Model.ForEachWhere(teams, t => DefaultAwayAttack[t], x => x != team)))
                        ) == attack[team]
                    )));

                // 3) podm beta pro vsechny tymy(pocet_obdrzenych_golu_tymu / 1 + ksi) 
                //  * suma(pro vsechny tymy VYJMA aktualniho(alpha_vychozi))) == beta
                model.AddConstraint("beta",
                    Model.ForEach(teams, team =>
                    (
                        (GoalTaken[team] / ((1.0 + ksi)
                            * Model.Sum(Model.ForEachWhere(teams, t => DefaultHomeAttack[t], x => x != team)))
                        ) == defence[team]
                    )));

                // 4) podm gamma ~ pomocny soucet
                // alpha * sum(beta vyjma aktualniho tymu) * 2 == summ
                model.AddConstraint("gamma",
                    Model.ForEach(teams, team =>
                    (
                        (attack[team]
                        * Model.Sum(Model.ForEachWhere(teams, t => defence[t], x => x != team))
                        * 2.0
                        ) == summ[team]
                    )));

                // TODO nefunguje
                // model.AddConstraint("delta", Model.Sum(Model.ForEach(teams, t => summ[t])) == homeGoalCount);

                Goal sum = model.AddGoal("sum", GoalKind.Minimize,
                    Model.Abs(Model.Sum(Model.ForEach(teams, teamz => summ[teamz])))); //  - homeGoalCount

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
            _DixonManager.Gamma = _DixonManager.Ksi;
            _DixonManager.Mi = 0;
            _DixonManager.Lambda = _DixonManager.Teams.Sum(x => x.HomeAttack);
            _DixonManager.Rho = _DixonManager.Teams.Sum(x => x.HomeAttack);

            // navrat
            return _DixonManager.Summary;
        }
    }
}
