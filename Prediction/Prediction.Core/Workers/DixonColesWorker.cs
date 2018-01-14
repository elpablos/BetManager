using Prediction.Core.Managers;
using Prediction.Core.Models;
using Prediction.Core.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using Prediction.Core.Services;

namespace Prediction.Core.Workers
{
    public class DixonColesWorker
    {
        private IGamePredictionService predictionService;

        public bool CanSaveToDb { get; set; }

        public DateTime? KsiStart { get; set; }

        public int PropLength { get; set; }

        public double EqualityTolerance { get; set; }

        public int TimeLimit { get; set; }

        public DixonColesWorker(IGamePredictionService predictionService)
        {
            this.predictionService = predictionService;
        }

        public IDixonManager PrepareData(ICollection<DataInput> inputs, DateTime dateActual, SolverTypeEnum type)
        {
            var teams = new HashSet<GameTeam>();
            var matches = new List<GameMatch>();

            foreach (var input in inputs)
            {
                if ((dateActual - input.DateStart).Days <= 0) continue;

                // home team
                teams.Add(new GameTeam
                {
                    Id = input.HomeTeamId,
                    DisplayName = input.HomeTeam
                });

                teams.Add(new GameTeam
                {
                    Id = input.AwayTeamId,
                    DisplayName = input.AwayTeam
                });

                var match = new GameMatch
                {
                    Id = input.Id,
                    HomeTeamId = input.HomeTeamId,
                    AwayTeamId = input.AwayTeamId,
                    HomeScore = input.HomeScore,
                    AwayScore = input.AwayScore,
                    DateStart = input.DateStart,
                };

                match.HomeTeam = teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
                match.AwayTeam = teams.FirstOrDefault(x => x.Id == match.AwayTeamId);
                match.Days = (dateActual - match.DateStart).Days;
                matches.Add(match);
            }

            matches = matches.Where(x => x.Days > 0)
                .OrderBy(x => x.DateStart)
                .ToList();

            // prepare dixon

            // prediction.Teams = new List<GameTeam>();

            IDixonManager manager = null;
            switch (type)
            {
                case SolverTypeEnum.DP:
                    manager = new DPManager(matches, teams.ToList());
                    break;
                case SolverTypeEnum.BP:
                    manager = new BPManager(matches, teams.ToList());
                    break;
                case SolverTypeEnum.DPDI:
                    manager = new DPDIManager(matches, teams.ToList());
                    break;
                case SolverTypeEnum.BPDI:
                    manager = new BPDIManager(matches, teams.ToList());
                    break;
                case SolverTypeEnum.Maher:
                    manager = new DixonManager(matches, teams.ToList());
                    break;
                default:
                    throw new NotImplementedException("Nebyl vybrán typ solveru!");
            }
            manager.Type = type;
            return manager;
        }

        public void Solve(double ksi, ICollection<DataInput> inputs, DateTime dateActual, SolverTypeEnum type)
        {
            try
            {
                // prepare dixon
                Console.WriteLine("Start solve ksi: {0}", ksi);

                IDixonManager dixonManager = null;
                string predictionType = null;

                dixonManager = PrepareData(inputs, dateActual, type);

                // TODO donacteni posledni verze
                // dixonManager = predictionService.GetById(9, dixonManager.Matches, dixonManager.Teams);

                dixonManager.PropLength = PropLength;
                dixonManager.KsiStart = KsiStart;
                dixonManager.Ksi = ksi;
                dixonManager.DatePredict = dateActual;

                // osetreni chyby
                if (dixonManager.Matches.Count == 0 || dixonManager.Teams.Count == 0)
                {
                    Console.WriteLine("Skip cause no data");
                    return;
                }

                IDixonColesSolver solver = null;

                switch (type)
                {
                    case SolverTypeEnum.DP:
                        solver = new DPSolver(dixonManager);
                        break;
                    case SolverTypeEnum.BP:
                        solver = new BPSolver(dixonManager);
                        break;
                    case SolverTypeEnum.DPDI:
                        solver = new DPDISolver(dixonManager);
                        break;
                    case SolverTypeEnum.BPDI:
                        solver = new BPDISolver(dixonManager);
                        break;
                    case SolverTypeEnum.Maher:
                        solver = new MaherExtSolver(dixonManager);
                        break;
                    default:
                        throw new NotImplementedException("Nebyl vybrán typ solveru!");
                }

                predictionType = type.ToString();

                Console.WriteLine("Start solving");

                // tolerance solveru
                solver.Directive.EqualityTolerance = EqualityTolerance;
                solver.Directive.Arithmetic = Microsoft.SolverFoundation.Services.Arithmetic.Double;
                solver.Directive.TimeLimit = TimeLimit;

                dixonManager.Summary = solver.Solve(dateActual);
                dixonManager.Description = solver.LastReport;
                Console.WriteLine("solved");
                dixonManager.MaximumLikehoodValue = 0;
                try
                {
                    dixonManager.MaximumLikehoodValue = dixonManager.SumMaximumLikehood();
                    Console.WriteLine("Maximum likehood counted");

                }
                catch (Exception ex)
                {
                    Console.WriteLine("Maximum likehood err: "+ ex.Message);
                    System.IO.File.AppendAllText(@"Data\errz.txt",
                        string.Format("Message: {0}\nTrace: {1}\nInnerMessage: {2}",
                        ex.Message, ex.StackTrace, ex.InnerException?.Message));
                }
            
                var result = dixonManager.ToString();
                Console.WriteLine(result);
                string filename = @"Data\"+ dateActual.ToString("yyyyMMdd-") + "result-ksi-" + ksi + "-" + predictionType + ".csv";
                System.IO.File.WriteAllText(filename, result.Replace(",", "."), System.Text.Encoding.UTF8);

                solver.Dispose();

                Console.WriteLine(dixonManager.Summary);

                if (CanSaveToDb)
                {
                    predictionService.Insert(dixonManager);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Err: {0}", ex.Message);
                System.IO.File.AppendAllText(@"Data\errz.txt",
                    string.Format("Message: {0}\nTrace: {1}\nInnerMessage: {2}",
                    ex.Message, ex.StackTrace, ex.InnerException?.Message));
            }
        }
    }
}
