using Microsoft.SolverFoundation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pokus
{
    public class Match
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
    }

    public class Team
    {
        public int Id { get; set; }
        public double Attack { get; set; }
        public double Defence { get; set; }
    }

    class Program
    {
        public static void Main()
        {
            var mMatches = new Match[]
            {
               new Match { Id=1, HomeTeamId = 1, AwayTeamId = 2, HomeScore=2, AwayScore = 0 },
               new Match { Id=2, HomeTeamId = 3, AwayTeamId = 2, HomeScore=0, AwayScore = 1 },
               //new Match { Id=1, HomeTeamId = 1, AwayTeamId = 3, HomeScore=0, AwayScore = 1 },
               //new Match { Id=1, HomeTeamId = 2, AwayTeamId = 1, HomeScore=0, AwayScore = 1 },
               //new Match { Id=1, HomeTeamId = 2, AwayTeamId = 1, HomeScore=0, AwayScore = 1 },
            };

            var mTeams = new Team[]
            {
                new Team { Id=1, Attack = 1.2, Defence = 0.8 },
                new Team { Id=2, Attack = 0.8, Defence = 1.2 },
                new Team { Id=3, Attack = 1.0, Defence = 1.0 },
            };

            // solver init
            SolverContext context = SolverContext.GetContext();
            Model model = context.CreateModel();

            Set matches = new Set(Domain.Integer, "matches");
            Set teams = new Set(Domain.Integer, "teams");

            Parameter homeTeamId = new Parameter(Domain.IntegerNonnegative, "homeTeamId", matches);
            homeTeamId.SetBinding(mMatches, "HomeTeamId", "Id");

            Parameter awayTeamId = new Parameter(Domain.IntegerNonnegative, "awayTeamId", matches);
            awayTeamId.SetBinding(mMatches, "AwayTeamId", "Id");

            Parameter homeScore = new Parameter(Domain.IntegerNonnegative, "homeScore", matches, teams, teams);
            homeScore.SetBinding(mMatches, "HomeScore", "Id", "HomeTeamId", "AwayTeamId");

            Parameter awayScore = new Parameter(Domain.IntegerNonnegative, "awayScore", matches, teams, teams);
            awayScore.SetBinding(mMatches, "AwayScore", "Id", "HomeTeamId", "AwayTeamId");
            model.AddParameters(homeTeamId, awayTeamId, homeScore, awayScore);

            // decisions
            Decision attack = new Decision(Domain.RealNonnegative, "attack", teams);
            attack.SetBinding(mTeams, "Attack", "Id");

            Decision defence = new Decision(Domain.RealNonnegative, "defence", teams);
            defence.SetBinding(mTeams, "Defence", "Id");

            model.AddDecisions(attack, defence);

            // constraints
            model.AddConstraint("attackCount",
                Model.Sum(Model.ForEach(teams, t => attack[t]) == mTeams.Length));
            model.AddConstraint("defenceCount",
                Model.Sum(Model.ForEach(teams, t => defence[t]) == mTeams.Length));

            //Goal sum = model.AddGoal("sum", Model.ForEach(team))

            Goal sum = model.AddGoal("sum", GoalKind.Minimize,
               Model.Sum(
                   Model.ForEach(matches, match =>
                   Model.ForEachWhere(teams, h =>
                   Model.ForEachWhere(teams, a =>
                   attack[h] * defence[a] - homeScore[match, h, a] + attack[a] * defence[h] - awayScore[match, h, a], a => awayTeamId[match] == a),
                   h => homeTeamId[match] == h
                   ))));

            context.CheckModel();

            // solve
            context.Solve();
            context.PropagateDecisions();

            //Console.WriteLine(string.Format("Sum: {0:f2}", sum.ToDouble()));
            Console.ReadLine();
        }
    }
}
