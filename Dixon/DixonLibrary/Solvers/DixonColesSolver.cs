using Dixon.Library.Managers;
using Microsoft.SolverFoundation.Services;
using System;
using System.Diagnostics;

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
            Stopwatch watch = new Stopwatch();

            watch.Start();
            double result = 0;

            // solver init
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            Set matches = new Set(Domain.Integer, "matches");
            Set teams = new Set(Domain.Integer, "Teams");

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

            Parameter ksi = new Parameter(Domain.Real, "ksi");
            ksi.SetBinding(_DixonManager.Ksi);

            model.AddParameters(homeScore, awayScore, days, homeTeamId, awayTeamId, ksi);

            // decisions
            Decision attack = new Decision(Domain.RealRange(0, 2), "attack", teams);
            attack.SetBinding(_DixonManager.Teams, "HomeAttack", "Id");

            Decision defence = new Decision(Domain.RealRange(0, 2), "defence", teams);
            defence.SetBinding(_DixonManager.Teams, "AwayAttack", "Id");

            Decision rho = new Decision(Domain.RealRange(-1, 1), "rho");
            Decision gama = new Decision(Domain.RealRange(1, 2), "gama");

            model.AddDecisions(attack, defence, rho, gama);

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
                       var lambda = attack[h] * defence[a] * gama;

                       // away-attack
                       var mu = attack[a] * defence[h];

                       // funkce tau
                       var tau = Model.If(Model.And(homeScore[match, h, a] == 0, awayScore[match, h, a] == 0),
                           1 - lambda * mu * rho,
                           Model.If(Model.And(homeScore[match, h, a] == 0, awayScore[match, h, a] == 1),
                           1 + lambda * rho,
                           Model.If(Model.And(homeScore[match, h, a] == 1, awayScore[match, h, a] == 0),
                           1 + mu * rho,
                           Model.If(Model.And(homeScore[match, h, a] == 1, awayScore[match, h, a] == 1),
                           1 - rho,
                           1))));

                       return
                           // casova fce
                           Model.Exp(-ksi * days[match, h, a])
                           // ln fce zavislosti tau
                           * (Model.Log(tau)
                           // pocet golu domaciho + ln predikce golu domaciho
                           + homeScore[match, h, a] * Model.Log(lambda)
                           // minus predikce golu domaciho
                           - lambda
                           // pocet golu hosti + ln predikce golu hosti
                           + awayScore[match, h, a] * Model.Log(mu)
                           // minus predikce golu hosti
                           - mu);
                   }, a => awayTeamId[match] == a
                   ), h => homeTeamId[match] == h
                   ))));

            context.CheckModel();

            // solve
            context.Solve();

            context.PropagateDecisions();

            watch.Stop();

            _DixonManager.Rho = rho.GetDouble();
            _DixonManager.Gama = gama.GetDouble();
            _DixonManager.Summary = sum.ToDouble();
            _DixonManager.LastElapsed = watch.Elapsed;

            // navrat
            return _DixonManager.Summary;
        }
    }
}
