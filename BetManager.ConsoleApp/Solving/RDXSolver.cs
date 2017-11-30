using BetManager.Solver.Managers;
using BetManager.Solver.Models;
using BetManager.Solver.Solvers;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
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

        public string SourcePath { get; set; }

        protected virtual IDixonManager PrepareData(int idTournament, int idSeason, int idLastSeason, DateTime dateActual)
        {
            var teams = new HashSet<GameTeam>();
            var matches = new List<GameMatch>();

            using (StreamReader reader = File.OpenText(SourcePath))
            {
                using (var csv = new CsvReader(reader, new CsvHelper.Configuration.Configuration
                {
                    Delimiter = ";",
                }))
                {
                    while (csv.Read())
                    {
                        // home team
                        teams.Add(new GameTeam
                        {
                            Id = csv.GetField<int>(1),
                            DisplayName = csv.GetField<string>(2)
                        });

                        teams.Add(new GameTeam
                        {
                            Id = csv.GetField<int>(3),
                            DisplayName = csv.GetField<string>(4)
                        });

                        var match = new GameMatch
                        {
                            Id = csv.GetField<int>(0),
                            HomeTeamId = csv.GetField<int>(1),
                            AwayTeamId = csv.GetField<int>(3),
                            HomeScore = csv.GetField<int>(5),
                            AwayScore = csv.GetField<int>(6),
                            DateStart = csv.GetField<DateTime>(7),
                        };

                        match.HomeTeam = teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
                        match.AwayTeam = teams.FirstOrDefault(x => x.Id == match.AwayTeamId);
                        match.Days = (dateActual - match.DateStart).Days;
                        matches.Add(match);
                    }
                }
            }

            matches = matches.Where(x => x.Days > 0)
                .OrderBy(x => x.DateStart)
                .ToList();

            // prepare dixon
            return new DixonManager(matches, teams.ToList());
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
                //if (idSeason == 0 && idLastSeason == 0)
                //{
                //    dixonManager = PrepareDataAll(idTournament, dateActual);
                //    predictionType = "rSolverAllData";
                //}
                //else
                {
                    dixonManager = PrepareData(idTournament, idSeason, idLastSeason, dateActual);
                    dixonManager.PropLength = 15;
                    predictionType = "rSolverLastSeason";
                }

                dixonManager.Ksi = ksi;

                // osetreni chyby
                if (dixonManager.Matches.Count == 0 || dixonManager.Teams.Count == 0)
                {
                    Console.WriteLine("Skip cause no data");
                    return;
                }

                IDixonColesSolver solver = null;
                // solver = new RDixonColesSolver(dixonManager);
                solver = new DPSolver(dixonManager);
                Console.WriteLine("Start solving");

                dixonManager.Summary = solver.Solve(dateActual);
                dixonManager.Description = solver.LastReport;
                Console.WriteLine("solved");
                dixonManager.MaximumLikehoodValue = dixonManager.SumMaximumLikehood();
                Console.WriteLine("Maximum likehood counted");

                var result = dixonManager.ToString();
                Console.WriteLine(result);
                string filename = dateActual.ToString("yyyyMMdd-") + "result-ksi-" + ksi + "-t-" + idTournament + "-" + predictionType + ".csv";
                System.IO.File.WriteAllText(filename, result.Replace(",", "."));

                solver.Dispose();

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
