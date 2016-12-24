using Dixon.Core.Services;
using Dixon.Library.Managers;
using Dixon.Library.Models;
using Dixon.Library.Solvers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Dixon.ConsoleApp
{
    public class DXSolver
    {
        public void DoParallel(int count)
        {
            var ksiints = Enumerable.Range(0, count);

            var ksis = ksiints.Select(x => x / 10.0);
            Parallel.ForEach(ksis, ksi => Solve(ksi));
        }

        public void Solve(double ksi)
        {
            int idTournament = 1;
            int idSeason = 11733;
            int idLastSeason = 10356;
            DateTime dateActual = new DateTime(2016, 12, 14);

            IEventService eventService = new EventService();
            ITeamService teamService = new TeamService();

            // getting data from Db
            var dbEvents = eventService.GetAll(idTournament, idSeason);
            var dbLastEvents = eventService.GetAll(idTournament, idLastSeason);
            var dbLastTeams = teamService.GetAll(idTournament, idLastSeason);
            var dbTeams = teamService.GetAll(idTournament, idSeason);

            var teams =
            dbTeams
            .Select(x => new GameTeam
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            })
            .Union(
            dbLastTeams
            .Select(x => new GameTeam
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            })
            ).ToList();

            // preparing data
            var matches =
            dbEvents
            .Select(x => new GameMatch
            {
                Id = x.Id,
                AwayScore = x.AwayScoreCurrent,
                HomeTeam = teams.FirstOrDefault(y => y.Id == x.ID_HomeTeam),
                HomeTeamId = x.ID_HomeTeam,
                DateStart = x.DateStart,
                HomeScore = x.HomeScoreCurrent,
                AwayTeam = teams.FirstOrDefault(y => y.Id == x.ID_AwayTeam),
                AwayTeamId = x.ID_AwayTeam,
                Days = (dateActual - x.DateStart).Days
            })
            .Union(
            dbLastEvents
            .Select(x => new GameMatch
            {
                Id = x.Id,
                AwayScore = x.AwayScoreCurrent,
                HomeTeam = teams.FirstOrDefault(y => y.Id == x.ID_HomeTeam),
                HomeTeamId = x.ID_HomeTeam,
                DateStart = x.DateStart,
                HomeScore = x.HomeScoreCurrent,
                AwayTeam = teams.FirstOrDefault(y => y.Id == x.ID_AwayTeam),
                AwayTeamId = x.ID_AwayTeam,
                Days = (dateActual - x.DateStart).Days
            })
            )
            .Where(x => x.Days >= 0)
            .OrderBy(x => x.DateStart)
            .ToList();

            // prepare dixon
            Console.WriteLine("Start solve ksi: {0}", ksi);
            IDixonManager dixonManager = new DixonManager(matches, teams);
            dixonManager.Ksi = ksi;
            IDixonColesSolver solver = new DixonColesSolver(dixonManager);
            dixonManager.Summary = solver.Solve(dateActual);

            var result = dixonManager.ToString();
            Console.WriteLine(result);
            string filename = "result-" + ksi + ".csv";
            System.IO.File.WriteAllText(filename, result.Replace(",", "."));

            Console.WriteLine(dixonManager.Summary);
        }
    }
}
