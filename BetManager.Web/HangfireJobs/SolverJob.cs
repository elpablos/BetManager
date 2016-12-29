using BetManager.Core.DbModels.Predictions;
using BetManager.Core.Domains.Events;
using BetManager.Core.Domains.Predictions;
using BetManager.Solver.Managers;
using BetManager.Solver.Models;
using BetManager.Solver.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.HangfireJobs
{
    public class SolverJob 
    {
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
                Summary = manager.Summary
            };

            var predictionTeams = manager.Teams.Select(t => new PredictionTeam
            {
                ID_Team = t.Id,
                Attack = t.HomeAttack,
                Defence = t.AwayAttack
            }).ToList();

            premanager.Insert(prediction, predictionTeams);
        }

        public void Solve(int idTournament, DateTime dateActual, double ksi)
        {
            // prepare dixon
            IDixonManager dixonManager = PrepareDataAll(idTournament, dateActual); // PrepareData(idTournament, idSeason, idLastSeason, dateActual);
            dixonManager.Ksi = ksi;
            IDixonColesSolver solver = new DixonColesSolver(dixonManager);
            dixonManager.Summary = solver.Solve(dateActual);
            dixonManager.MaximumLikehoodValue = dixonManager.SumMaximumLikehood();
            // save to DB
            SaveToDb(dixonManager, dateActual, idTournament);
        }
    }
}