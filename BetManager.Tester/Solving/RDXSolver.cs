using BetManager.Core.DbModels.Predictions;
using BetManager.Core.Domains.Events;
using BetManager.Core.Domains.Predictions;
using BetManager.Solver.Managers;
using BetManager.Solver.Models;
using BetManager.Solver.Solvers;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BetManager.Tester.Solving
{
    public class RDXSolver
    {
        public bool HasSaveToDb { get; set; }

        public void DoParallel(int count)
        {
            var ksiints = Enumerable.Range(0, count);

            var ksis = ksiints.Select(x => x / 10.0);
            int[] tournaments = new int[] { 1, };
            DateTime dateActual = new DateTime(2016, 12, 25);

            foreach (var tournament in tournaments)
            {
                Parallel.ForEach(ksis, ksi => Solve(ksi, tournament, dateActual));
            }
        }

        protected virtual IDixonManager PrepareDataAll(int idTournament, DateTime dateActual)
        {
            IEventManager eventService = new EventManager();
            ITeamManager teamService = new TeamManager();

            // getting data from Db
            var dbEvents = eventService.GetAll(new { ID_Tournament = idTournament, ID_Season = (int?)null });
            var dbTeams = teamService.GetAll(new { ID_Tournament = idTournament, ID_Season = (int?)null });

            var teams =
            dbTeams
            .Select(x => new GameTeam
            {
                Id = x.Id,
                DisplayName = x.DisplayName
            }).ToList();

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
            .Where(x => x.Days >= 0)
            .OrderBy(x => x.DateStart)
            .ToList();

            // prepare dixon
            return new DixonManager(matches, teams);
        }

        protected virtual IDixonManager PrepareData(int idTournament, int idSeason, int idLastSeason, DateTime dateActual)
        {
            IEventManager eventService = new EventManager();
            ITeamManager teamService = new TeamManager();

            // getting data from Db
            var dbEvents = eventService.GetAll(new { ID_Tournament = idTournament, ID_Season = idSeason });
            var dbLastEvents = eventService.GetAll(new { ID_Tournament = idTournament, ID_Season = idLastSeason });
            var dbLastTeams = teamService.GetAll(new { ID_Tournament = idTournament, ID_Season = idLastSeason });
            var dbTeams = teamService.GetAll(new { ID_Tournament = idTournament, ID_Season = idSeason });

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
            return new DixonManager(matches, teams);
        }

        protected virtual void SaveToDb(IDixonManager manager, DateTime dateActual, int idTournament)
        {
            IPredictionManager premanager = new PredictionManager();
            var prediction = new Prediction
            {
                DateCreated = DateTime.Now,
                DatePredict = dateActual,
                Elapsed = manager.LastElapsed.Ticks,
                Gamma = manager.Gamma,
                Ksi = manager.Ksi,
                LikehoodValue = manager.MaximumLikehoodValue,
                ID_Tournament = idTournament,
                Summary = manager.Summary,
                Description = manager.Description
            };

            var predictionTeams = manager.Teams.Select(t => new PredictionTeam
            {
                ID_Team = t.Id,
                Attack = t.HomeAttack,
                Defence = t.AwayAttack
            }).ToList();

            premanager.Insert(prediction, predictionTeams);
        }

        public void Solve(double ksi, int idTournament, DateTime dateActual)
        {
            try
            {
                // prepare dixon
                Console.WriteLine("Start solve ksi: {0}", ksi);
                IDixonManager dixonManager = PrepareDataAll(idTournament, dateActual);
                dixonManager.Ksi = ksi;

                // osetreni chyby
                if (dixonManager.Matches.Count == 0 || dixonManager.Teams.Count == 0)
                {
                    Console.WriteLine("Skip cause no data");
                    return;
                }

                IDixonColesSolver solver = new RDixonColesSolver(dixonManager);
                Console.WriteLine("Start solving");

                dixonManager.Summary = solver.Solve(dateActual);
                dixonManager.Description = solver.LastReport;
                Console.WriteLine("solved");
                dixonManager.MaximumLikehoodValue = dixonManager.SumMaximumLikehood();
                Console.WriteLine("Maximum likehood counted");

                var result = dixonManager.ToString();
                Console.WriteLine(result);
                string filename = dateActual.ToString("yyyyMMdd-") + "result-ksi-" + ksi + "-t-" + idTournament + ".csv";
                System.IO.File.WriteAllText(@"C:\Develop\BitBucket\betmanager\BetManager.Tester\bin\logs\"+filename, result.Replace(",", "."));

                if (HasSaveToDb)
                {
                    Console.WriteLine("saving to DB");
                    SaveToDb(dixonManager, dateActual, idTournament);
                    Console.WriteLine("saved");
                }

                Console.WriteLine(dixonManager.Summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err: {0}", ex.Message);
                System.IO.File.AppendAllText(@"C:\Develop\BitBucket\betmanager\BetManager.Tester\bin\logs\errz.txt", 
                    string.Format("idTournament: {3}\nMessage: {0}\nTrace: {1}\nInnerMessage: {2}",
                    ex.Message, ex.StackTrace, ex.InnerException?.Message, idTournament));
            }
        }
    }
}
