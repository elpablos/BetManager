using Dixon.ConsoleApp;
using Dixon.Core.Services;
using Dixon.Library.Managers;
using Dixon.Library.Models;
using Dixon.Library.Solvers;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DixonConsole
{
    /// <summary>
    /// Run dixon
    /// TODO: maximization algorithm
    /// https://gist.github.com/trevordixon/9702052
    /// https://msdn.microsoft.com/en-us/library/ff628587(v=vs.93).aspx
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            bool test = true;
            var solver = new DXSolver();
            if (test)
            {
                // solver.Solve(0.0065 / 3.5);
                var ksis = // new double[] { 0, 0.0065 / 3.5, 0.005, 0.01, 0.03 };
                    new double[] { 0.0065 / 3.5 };

                double ksi = 0.0065 / 3.5;
                // premier league, czliga, bundesliga, italie-serie A, spain-primavera, poland-ekstraklasa
                int[] tournaments = new int[] { 2, 38, 54 };
                Parallel.ForEach(tournaments, t => solver.Solve(ksi, t));
            }
            else
            {
                //DixonSolver();
              
                solver.DoParallel(30);
            }
        }

        //static void DixonSolver()
        //{
        //    int idTournament = 1;
        //    int idSeason = 11733;
        //    int idLastSeason = 10356;
        //    DateTime dateActual = new DateTime(2016, 12, 14);

        //    IEventService eventService = new EventService();
        //    ITeamService teamService = new TeamService();

        //    // getting data from Db
        //    var dbEvents = eventService.GetAll(idTournament, idSeason);
        //    var dbLastEvents = eventService.GetAll(idTournament, idLastSeason);
        //    var dbLastTeams = teamService.GetAll(idTournament, idLastSeason);
        //    var dbTeams = teamService.GetAll(idTournament, idSeason);

        //    var teams =
        //    dbTeams
        //    .Select(x => new GameTeam
        //    {
        //        Id = x.Id,
        //        DisplayName = x.DisplayName
        //    })
        //    .Union(
        //    dbLastTeams
        //    .Select(x => new GameTeam
        //    {
        //        Id = x.Id,
        //        DisplayName = x.DisplayName
        //    })
        //    ).ToList();

        //    // preparing data
        //    var matches =
        //    dbEvents
        //    .Select(x => new GameMatch
        //    {
        //        Id = x.Id,
        //        AwayScore = x.AwayScoreCurrent,
        //        HomeTeam = teams.FirstOrDefault(y => y.Id == x.ID_HomeTeam),
        //        HomeTeamId = x.ID_HomeTeam,
        //        DateStart = x.DateStart,
        //        HomeScore = x.HomeScoreCurrent,
        //        AwayTeam = teams.FirstOrDefault(y => y.Id == x.ID_AwayTeam),
        //        AwayTeamId = x.ID_AwayTeam,
        //        Days = (dateActual - x.DateStart).Days
        //    })
        //    .Union(
        //    dbLastEvents
        //    .Select(x => new GameMatch
        //    {
        //        Id = x.Id,
        //        AwayScore = x.AwayScoreCurrent,
        //        HomeTeam = teams.FirstOrDefault(y => y.Id == x.ID_HomeTeam),
        //        HomeTeamId = x.ID_HomeTeam,
        //        DateStart = x.DateStart,
        //        HomeScore = x.HomeScoreCurrent,
        //        AwayTeam = teams.FirstOrDefault(y => y.Id == x.ID_AwayTeam),
        //        AwayTeamId = x.ID_AwayTeam,
        //        Days = (dateActual - x.DateStart).Days
        //    })
        //    )
        //    .Where(x => x.Days >= 0)
        //    .OrderBy(x => x.DateStart)
        //    .ToList();

        //    // prepare dixon
        //    IDixonManager dixonManager = new DixonManager(matches, teams);
        //    IDixonColesSolver solver = new DixonColesSolver(dixonManager);

        //    dixonManager.Summary = solver.Solve(dateActual);

        //    var result = dixonManager.ToString();
        //    Console.WriteLine(result);
        //    System.IO.File.WriteAllText("teams.csv", result.Replace(",", "."));

        //    Console.WriteLine(dixonManager.Summary);
        //}
    }
}
