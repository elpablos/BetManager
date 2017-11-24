using System;
using BetManager.Solver.Managers;
using BetManager.Solver.Solvers;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;
using RDotNet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BetManager.Tester.Solving
{
    public class RDixonColesSolver : IDixonColesSolver
    {
        private IDixonManager dixonManager;
        private REngine engine;

        public string LastReport { get; private set; }

        public RDixonColesSolver(IDixonManager dixonManager)
        {
            this.dixonManager = dixonManager;
            //Setup();
            REngine.SetEnvironmentVariables();
            engine = REngine.GetInstance();
        }

        public double Solve(DateTime actualDate)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            // nastartovani enginu
            //using ()
            {
                // inicializace
                engine.Initialize();

                // nactu R skript
                string scriptPath = $"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}/RScript/script.R".Replace("\\", "/");
                engine.Evaluate($"source('{scriptPath}')");

                var names = engine.CreateCharacterVector(new[] { "HomeTeam", "AwayTeam", "FTHG", "FTAG", "Date" });

                var homeTeam = engine.CreateCharacterVector(dixonManager.Matches.Select(x => x.HomeTeam.DisplayName));
                var awayTeam = engine.CreateCharacterVector(dixonManager.Matches.Select(x => x.AwayTeam.DisplayName));
                var FTHG = engine.CreateIntegerVector(dixonManager.Matches.Select(x => x.HomeScore));
                var FTAG = engine.CreateIntegerVector(dixonManager.Matches.Select(x => x.AwayScore));
                var dates = engine.CreateCharacterVector(dixonManager.Matches.Select(x => x.DateStart.ToString("yyyy-MM-dd")));

                engine.SetSymbol("names", names);
                engine.SetSymbol("homeTeam", homeTeam);
                engine.SetSymbol("awayTeam", awayTeam);
                engine.SetSymbol("FTHG", FTHG);
                engine.SetSymbol("FTAG", FTAG);
                engine.SetSymbol("dates", dates);

                var dta = engine.Evaluate("dta <- CreateDataFrame(names, homeTeam, awayTeam, FTHG, FTAG, dates)").AsDataFrame();
                var dcm = engine.Evaluate("dcm <- DCmodelData(dta)").AsDataFrame();

                var teams = engine.Evaluate("dcm$teams").AsList();

                //var res = engine.Evaluate("res <- doPrediction(ksi = 0.0065 / 3.5, currentDate = as.Date('2012-05-13'))").AsDataFrame();
                //engine.Evaluate("predict.result(res, 'Sparta Prague', 'FC Viktoria Plzen')");

                var par = engine.Evaluate("res$par").AsNumericMatrix();

                foreach (var team in dixonManager.Teams)
                {
                    team.HomeAttack = engine.Evaluate($"res$par['Attack.{team.DisplayName}']").AsNumeric().First();
                    // team.AwayAttack = -1 / engine.Evaluate($"res$par['Defence.{team.DisplayName}']").AsNumeric().First(); // -1?
                    team.AwayAttack = engine.Evaluate($"res$par['Defence.{team.DisplayName}']").AsNumeric().First(); // -1?
                }

                dixonManager.Rho = engine.Evaluate("res$par['RHO']").AsNumeric().First();
                // dixonManager.Gamma = 1+ engine.Evaluate("res$par['HOME']").AsNumeric().First(); // 1+?
                dixonManager.Gamma = engine.Evaluate("res$par['HOME']").AsNumeric().First(); // 1+?
                dixonManager.Summary = engine.Evaluate("res$value").AsNumeric().First();
            }

            watch.Stop();

            LastReport = "MyPC - RDixon";
            dixonManager.LastElapsed = watch.Elapsed;

            // navrat
            return dixonManager.Summary;
        }

        private void Setup()
        {
            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\R-core\R"))
            {
                string rPath = (string)registryKey.GetValue("InstallPath");
                string rVersion = (string)registryKey.GetValue("Current Version");

                var envPath = Environment.GetEnvironmentVariable("PATH");
                string rBinPath = $"{rPath}bin\\x64";
                Environment.SetEnvironmentVariable("PATH", envPath + Path.PathSeparator + rBinPath);
            }
        }

        public void Dispose()
        {
            if (engine != null)
                engine.Dispose();
        }
    }

    public static class DateTimeExt
    {
        public static double ToUnixTimeSeconds(this DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }
}