using BetManager.Core.DbModels.Predictions;
using BetManager.Core.Domains.Events;
using BetManager.Core.Domains.Predictions;
using BetManager.Solver.Managers;
using BetManager.Solver.Models;
using BetManager.Solver.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.App.Solving
{
    public class DXSolver
    {
        public bool HasSaveToDb { get; internal set; }

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
            .Where(x => x.Days > 0)
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
            .Where(x => x.Days > 0)
            .OrderBy(x => x.DateStart)
            .ToList();

            // prepare dixon
            return new DixonManager(matches, teams);
        }

        protected virtual void SaveToDb(IDixonManager manager, DateTime dateActual, int idTournament, string predictionType)
        {
            IPredictionManager premanager = new PredictionManager();
            var prediction = new Prediction {
                DateCreated = DateTime.Now,
                DatePredict = dateActual,
                Elapsed = manager.LastElapsed.Ticks,
                Gamma = manager.Gamma,
                Ksi = manager.Ksi,
                LikehoodValue = manager.MaximumLikehoodValue,
                ID_Tournament = idTournament,
                Summary = manager.Summary,
                Description = manager.Description,
                ID_PredictionType = predictionType
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
            Solve(ksi, idTournament, 0, 0, dateActual);
        }

        public void Solve(double ksi, int idTournament, int idSeason, int idLastSeason, DateTime dateActual)
        {
            try
            {
                // prepare dixon
                Console.WriteLine("Start solve ksi: {0}", ksi);

                IDixonManager dixonManager = null;
                string predictionType = null;
                if (idSeason == 0 && idLastSeason == 0)
                {
                    dixonManager = PrepareDataAll(idTournament, dateActual);
                    predictionType = "msSolverAllData";
                }
                else
                {
                    dixonManager = PrepareData(idTournament, idSeason, idLastSeason, dateActual);
                    predictionType = "msSolverLastSeason";
                }

                dixonManager.Ksi = ksi;

                // osetreni chyby
                if (dixonManager.Matches.Count == 0 || dixonManager.Teams.Count == 0)
                {
                    Console.WriteLine("Skip cause no data");
                    return;
                }

                IDixonColesSolver solver = new DixonColesSolver(dixonManager);
                Console.WriteLine("Start solving");

                dixonManager.Summary = solver.Solve(dateActual);
                dixonManager.Description = solver.LastReport;
                Console.WriteLine("solved");
                dixonManager.MaximumLikehoodValue = dixonManager.SumMaximumLikehood();
                Console.WriteLine("Maximum likehood counted");

                var result = dixonManager.ToString();
                Console.WriteLine(result);
                string filename = dateActual.ToString("yyyyMMdd-") + "result-ksi-" + ksi + "-t-" + idTournament + "-"+ predictionType + ".csv";
                System.IO.File.WriteAllText(filename, result.Replace(",", "."));

                if (HasSaveToDb)
                {
                    Console.WriteLine("saving to DB");
                    SaveToDb(dixonManager, dateActual, idTournament, predictionType);
                    Console.WriteLine("saved");
                }

                Console.WriteLine(dixonManager.Summary);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err: {0}", ex.Message);
                System.IO.File.AppendAllText("errz.txt", string.Format("idTournament: {3}\nMessage: {0}\nTrace: {1}\nInnerMessage: {2}", 
                    ex.Message, ex.StackTrace, ex.InnerException?.Message, idTournament));
            }
        }

        public void Test(string filepath)
        {
            int idTournament = 1;
            int idSeason = 11733;
            int idLastSeason = 10356;
            DateTime dateActual = new DateTime(2016, 12, 14);

            // prepare dixon
            IDixonManager dixonManager = PrepareData(idTournament, idSeason, idLastSeason, dateActual);
            IDixonColesSolver solver = new DixonColesSolver(dixonManager);

            dixonManager.Gamma = 1.50959228465985;
            dixonManager.Rho = -0.148334085871382;
            dixonManager.Ksi = 0.00185714285714286;

            foreach (var team in dixonManager.Teams)
            {

            }

            //  Burnley; 0.642876871660756; 1.11318950943912
            //Crystal Palace; 0.992914222297631; 1.16944525197482
            //West Bromwich Albion; 0.837599011822103; 0.848324970473902
            //Manchester City; 1.51152826057043; 0.85395831784309
            //Watford; 0.884760198533533; 1.0774333971691
            //Stoke City; 0.83084067241941; 1.04051285260052
            //Leicester City; 1.24383672625791; 0.883005094391995
            //Tottenham; 1.34012388799594; 0.616243308271679
            //Manchester United; 0.960912416830746; 0.662419214774675
            //Middlesbrough; 0.562090832090398; 0.762389283808631
            //West Ham; 1.13724128137898; 1.10841948589809
            //Chelsea; 1.39637501492742; 0.759691114651839
            //Sunderland; 0.840692656134263; 1.13148321122441
            //Arsenal; 1.50261971941139; 0.747390648612331
            //Liverpool; 1.55071458893771; 0.931860143380468
            //Southampton; 0.989312944348403; 0.723817642220095
            //Everton; 1.07466934258972; 0.960892438289372
            //Bournemouth; 0.990667326149388; 1.18735829114802
            //Swansea City; 0.935613432109431; 1.20101904250424
            //Hull City; 0.648803964334445; 1.53431322738814
            //Newcastle United; 0.865343295801357; 1.11597349327305
            //Aston Villa; 0.528048395880155; 1.38507208178721
            //Norwich City; 0.732414947456615; 1.18578798882963
        }
    }
}
