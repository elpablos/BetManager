//using Microsoft.SolverFoundation.Services;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Pokus
//{
//    public class Match
//    {
//        public int Id { get; set; }
//        public int HomeTeamId { get; set; }
//        public int AwayTeamId { get; set; }
//    }

//    public class Team
//    {
//        public int Id { get; set; }
//        public double Attack { get; set; }
//        public double Defence { get; set; }
//    }

//    class Program
//    {
//        public static void Main()
//        {
//            var mMatches = new Match[]
//            {
//               new Match { Id=1, HomeTeamId = 1, AwayTeamId = 2 },
//               new Match { Id=2, HomeTeamId = 0, AwayTeamId = 2 }
//            };

//            var mTeams = new Team[]
//            {
//                new Team { Id=1, Attack = 1.2, Defence = 0.8 },
//                new Team { Id=1, Attack = 1.1, Defence = 0.9 },
//                new Team { Id=2, Attack = 0.8, Defence = 1.2 },
//                new Team { Id=3, Attack = 1.0, Defence = 1.0 },
//            };

//            // solver init
//            SolverContext context = SolverContext.GetContext();
//            Model model = context.CreateModel();

//            Set matches = new Set(Domain.Integer, "matches");
//            Set teams = new Set(Domain.Integer, "Teams");

//            // params
//            Parameter homeTeamId = new Parameter(Domain.IntegerNonnegative, "homeTeamId", matches);
//            homeTeamId.SetBinding(mMatches, "HomeTeamId", "Id");

//            Parameter awayTeamId = new Parameter(Domain.IntegerNonnegative, "awayTeamId", matches);
//            awayTeamId.SetBinding(mMatches, "AwayTeamId", "Id");

//            model.AddParameters(homeTeamId, awayTeamId);

//            // decisions
//            Decision attack = new Decision(Domain.RealRange(0, 3), "attack", teams);
//            attack.SetBinding(mTeams, "Attack", "Id");

//            Decision defence = new Decision(Domain.RealRange(0, 3), "defence", teams);
//            defence.SetBinding(mTeams, "Defence", "Id");

//            model.AddDecisions(attack, defence);

//            // constraints
//            model.AddConstraint("attackCount",
//                Model.Sum(Model.ForEach(teams, t => attack[t])) == mTeams.Length);
//            model.AddConstraint("defenceCount",
//                Model.Sum(Model.ForEach(teams, t => defence[t])) == mTeams.Length);

//            //Goal sum = model.AddGoal("sum", GoalKind.Maximize,
//            //   Model.Sum(
//            //       Model.ForEach(matches, match =>
//            //       {
//            //           return attack[0] + (-1)* defence[1];
//            //       })));

//            context.CheckModel();

//            // solve
//            context.Solve();
//            context.PropagateDecisions();

//            //Console.WriteLine(string.Format("Sum: {0:f2}", sum.ToDouble()));
//            Console.ReadLine();
//        }
//    }
//}
