//using Prediction.Core.Managers;
//using Prediction.Core.Models;
//using Microsoft.SolverFoundation.Services;
//using System;
//using System.Diagnostics;
//using System.Linq;
//using System.Collections.Generic;

//namespace Prediction.Core.Solvers
//{
//    public class GameTeamExtend : GameTeam
//    {
//        public double DefaultHomeAttack { get; set; }

//        public double DefaultAwayAttack { get; set; }

//        public int GoalGiven { get; set; }

//        public int GoalTaken { get; set; }
//    }

//    /// <summary>
//    /// How to connect together
//    /// https://msdn.microsoft.com/en-us/library/ff847512(v=vs.93).aspx
//    /// </summary>
//    public class MaherSolver : IDixonColesSolver
//    {
//        private readonly IDixonManager _DixonManager;
//        public string LastReport { get; private set; }

//        public MaherSolver(IDixonManager dixonManager)
//        {
//            _DixonManager = dixonManager;
//        }

//        public double Solve(DateTime actualDate)
//        {
//            Stopwatch watch = new Stopwatch();

//            watch.Start();

//            // TODO 10 iteraci!
//            for (int i = 0; i < 1; i++)
//            {
//                // solver init
//                SolverContext context = SolverContext.GetContext();
//                context.ClearModel();

//                Model model = context.CreateModel();

//                var extendTeams = new List<GameTeamExtend>();
//                foreach (var team in _DixonManager.Teams)
//                {
//                    var extendTeam = new GameTeamExtend
//                    {
//                        Id = team.Id,
//                        DefaultHomeAttack = team.HomeAttack,
//                        DefaultAwayAttack = team.AwayAttack,
//                        HomeAttack = 1.0,
//                        AwayAttack = 1.0,
//                        DisplayName = team.DisplayName,
//                        GoalGiven = _DixonManager.Matches.Where(x => x.HomeTeamId == team.Id).Sum(x => x.HomeScore) 
//                        + _DixonManager.Matches.Where(x => x.AwayTeamId == team.Id).Sum(x => x.AwayScore),
//                        GoalTaken = _DixonManager.Matches.Where(x => x.HomeTeamId == team.Id).Sum(x => x.AwayScore) 
//                        + _DixonManager.Matches.Where(x => x.AwayTeamId == team.Id).Sum(x => x.HomeScore),
//                    };

//                    extendTeams.Add(extendTeam);
//                }

//                Set teams = new Set(Domain.Integer, "Teams");

//                var homeScore = _DixonManager.Matches.Sum(x => x.HomeScore);
//                var awayScore = _DixonManager.Matches.Sum(x => x.AwayScore);
//                _DixonManager.Ksi = homeScore / awayScore;

//                // parameters
//                Parameter homeGoalCount = new Parameter(Domain.IntegerNonnegative, "homeGoalCount");
//                homeGoalCount.SetBinding(homeScore);

//                Parameter awayGoalCount = new Parameter(Domain.IntegerNonnegative, "awayGoalCount");
//                awayGoalCount.SetBinding(awayScore);

//                Parameter ksi = new Parameter(Domain.Real, "ksi");
//                ksi.SetBinding(_DixonManager.Ksi);

//                Parameter DefaultHomeAttack = new Parameter(Domain.IntegerNonnegative, "DefaultHomeAttack", teams);
//                DefaultHomeAttack.SetBinding(_DixonManager.Teams, "DefaultHomeAttack", "Id");

//                Parameter DefaultAwayAttack = new Parameter(Domain.IntegerNonnegative, "DefaultAwayAttack", teams);
//                DefaultAwayAttack.SetBinding(_DixonManager.Teams, "DefaultAwayAttack", "Id");

//                Parameter GoalGiven = new Parameter(Domain.IntegerNonnegative, "GoalGiven", teams);
//                GoalGiven.SetBinding(_DixonManager.Teams, "GoalGiven", "Id");

//                Parameter GoalTaken = new Parameter(Domain.IntegerNonnegative, "GoalTaken", teams);
//                GoalTaken.SetBinding(_DixonManager.Teams, "GoalTaken", "Id");

//                model.AddParameters(homeGoalCount, awayGoalCount, GoalGiven, GoalTaken, DefaultHomeAttack, DefaultAwayAttack, ksi);

//                // decisions
//                Decision attack = new Decision(Domain.RealRange(0, 2), "attack", teams);
//                attack.SetBinding(_DixonManager.Teams, "HomeAttack", "Id");

//                Decision defence = new Decision(Domain.RealRange(0, 2), "defence", teams);
//                defence.SetBinding(_DixonManager.Teams, "AwayAttack", "Id");

//                model.AddDecisions(attack, defence);

//                // constraints
//                model.AddConstraint("homeAttackCount",
//                    Model.Sum(Model.ForEach(teams, t => attack[t])) == Model.Sum(Model.ForEach(teams, t => DefaultHomeAttack[t])));
//                model.AddConstraint("awayAttackCount",
//                    Model.Sum(Model.ForEach(teams, t => defence[t])) == Model.Sum(Model.ForEach(teams, t => DefaultAwayAttack[t])));
//                model.AddConstraint("awayAttackCount",
//                    Model.Sum(Model.ForEach(teams, t => defence[t])) == Model.Sum(Model.ForEach(teams, t => DefaultAwayAttack[t])));

//                Goal sum = model.AddGoal("sum", GoalKind.Minimize,
//                    Model.Sum(

//                        Model.ForEach(teams, team =>
//                        {
//                            // pocet_vstrelenych/((1+ksi)*(suma(beta_vychozi))
//                            attack[team] = GoalGiven[team] / ((1 + ksi) * (Model.Sum(Model.ForEach(teams, t => defence[t])) - );
//                            // pocet_vstrelenych/((1+ksi)*(suma(beta_vychozi))
//                            return 
//                        }
//                        )
//                       //Model.ForEach(matches, match =>
//                       //Model.ForEachWhere(teams, h =>
//                       //Model.ForEachWhere(teams, a =>
//                       //{
//                       //    return
//                       //    // casova fce
//                       //    Model.Exp(-ksi * days[match, h, a] / 365.25)
//                       //    *
//                       //     (
//                       //         (
//                       //             -lambda + (homeScore[match, h, a] * Model.Log(lambda))
//                       //         ) - factorial[homeScore[match, h, a]]
//                       //         +
//                       //         (
//                       //             -mu + (awayScore[match, h, a] * Model.Log(mu))
//                       //         ) - factorial[awayScore[match, h, a]]
//                       //     );
//                       //}, a => awayTeamId[match] == a
//                       //), h => homeTeamId[match] == h
//                       //))

//                       ));

//                context.CheckModel();

//                // solve
//                var solution = context.Solve(new HybridLocalSearchDirective());
//                LastReport = solution.GetReport().ToString();

//                context.PropagateDecisions();
//            }

//            watch.Stop();

//            _DixonManager.LastElapsed = watch.Elapsed;

//            // navrat
//            return _DixonManager.Summary;
//        }

//        public void Dispose()
//        {
//            // throw new NotImplementedException();
//        }
//    }
//}
