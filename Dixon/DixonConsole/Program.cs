using Dixon.Core.Services;
using Dixon.Library.Managers;
using Dixon.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
            int idTournament = 1;
            int idSeason = 11733;

            IEventService eventService = new EventService();
            ITeamService teamService = new TeamService();

            // getting data from Db
            var dbEvents = eventService.GetAll(idTournament, idSeason);
            var dbTeams = teamService.GetAll(idTournament, idSeason);
            var teams = dbTeams
                .Select(x => new GameTeam
                {
                    Id = x.Id,
                    DisplayName = x.DisplayName
                }).ToList();

            // preparing data
            var matches = dbEvents.OrderBy(x => x.DateStart)
                .Select(x => new GameMatch
                {
                    Id = x.Id,
                    AwayScore = x.AwayScoreCurrent,
                    HomeTeam = teams.FirstOrDefault(y => y.Id == x.ID_HomeTeam),
                    HomeTeamId = x.ID_HomeTeam,
                    DateStart = x.DateStart,
                    HomeScore = x.HomeScoreCurrent,
                    AwayTeam = teams.FirstOrDefault(y => y.Id == x.ID_AwayTeam),
                    AwayTeamId = x.ID_AwayTeam
                }).ToList();

            // prepare dixon
            DateTime dateActual = new DateTime(2016, 12, 14);
            IDixonManager dixonManager = new DixonManager(matches, teams);

            // TODO REPEAT TILL BEST SUM

            // calculate sum!
            double sum = dixonManager.Sum(dateActual);

            Console.WriteLine(sum);
        }
    }
}
