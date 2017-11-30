using Prediction.Core.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Prediction.Core.Services
{
    public class GameMatchService : BaseService, IGameMatchService
    {
        public GameMatchService(string connectionString = null) : base(connectionString)
        {
        }

        public void Prepare()
        {
            string query =
@"
CREATE TABLE GameTeam(
    Id INTEGER PRIMARY KEY,
    DisplayName TEXT
);
";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.ExecuteNonQuery();
            }

            // Id, HomeTeamId, AwayTeamId, HomeScore, AwayScore, DateStart
            query =
@"
CREATE TABLE GameMatch(
    Id INTEGER PRIMARY KEY,
    HomeTeamId INTEGER,
    AwayTeamId INTEGER,
    HomeScore INTEGER,
    AwayScore INTEGER,
    DateStart INTEGER,
    FOREIGN KEY (HomeTeamId) REFERENCES GameTeam(Id)
    FOREIGN KEY (AwayTeamId) REFERENCES GameTeam(Id)
);
";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.ExecuteNonQuery();
            }
        }

        public ICollection<DataInput> GetAll()
        {
            ICollection<DataInput> result = new List<DataInput>();
            string query = @"
select 
    GameMatch.Id,
    GameMatch.HomeTeamId,
    HomeTeam.DisplayName as HomeTeam,
    GameMatch.AwayTeamId,
    AwayTeam.DisplayName as AwayTeam,
    GameMatch.HomeScore,
    GameMatch.AwayScore,
    GameMatch.DateStart
from GameMatch
    inner join GameTeam as HomeTeam on HomeTeam.Id=GameMatch.HomeTeamId
    inner join GameTeam as AwayTeam on AwayTeam.Id=GameMatch.AwayTeamId
";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                var reader = q.ExecuteReader();
                while (reader.Read())
                {
                    var team = new DataInput
                    {
                        Id = reader.GetInt32(0),
                        HomeTeamId = reader.GetInt32(1),
                        HomeTeam = reader.GetString(2),
                        AwayTeamId = reader.GetInt32(3),
                        AwayTeam = reader.GetString(4),
                        HomeScore = reader.GetInt32(5),
                        AwayScore = reader.GetInt32(6),
                        DateStart = reader.GetDateTime(7)
                    };
                    result.Add(team);
                }
            }

            return result;
        }

        public ICollection<GameMatch> Bulk(ICollection<DataInput> matches)
        {
            var result = new List<GameMatch>();

            var teams = new HashSet<GameTeam>();
            foreach (var match in matches)
            {
                // home team
                teams.Add(new GameTeam
                {
                    Id = match.HomeTeamId,
                    DisplayName = match.HomeTeam
                });

                teams.Add(new GameTeam
                {
                    Id = match.AwayTeamId,
                    DisplayName = match.AwayTeam
                });

                var gameMatch = new GameMatch
                {
                    Id = match.Id,
                    HomeTeamId = match.HomeTeamId,
                    AwayTeamId = match.AwayTeamId,
                    HomeScore = match.HomeScore,
                    AwayScore = match.AwayScore,
                    DateStart = match.DateStart
                };

                gameMatch.HomeTeam = teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
                gameMatch.AwayTeam = teams.FirstOrDefault(x => x.Id == match.AwayTeamId);
                // gameMatch.Days = (dateActual - match.DateStart).Days;
                result.Add(gameMatch);
            }

            using (var trans = GetConnection.BeginTransaction())
            {
                string query = @"insert into GameTeam (Id, DisplayName) values (@Id, @DisplayName)";

                using (var cmd = new SQLiteCommand(query, GetConnection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("Id", DbType.Int32));
                    cmd.Parameters.Add(new SQLiteParameter("DisplayName", DbType.String));

                    foreach (var team in teams)
                    {
                        cmd.Parameters["Id"].Value = team.Id;
                        cmd.Parameters["DisplayName"].Value = team.DisplayName;
                        cmd.ExecuteNonQuery();
                    }
                }

                query = @"
insert into GameMatch (Id, HomeTeamId, AwayTeamId, HomeScore, AwayScore, DateStart) 
values (@Id, @HomeTeamId, @AwayTeamId, @HomeScore, @AwayScore, @DateStart)";
                using (var cmd = new SQLiteCommand(query, GetConnection))
                {
                    cmd.Parameters.Add(new SQLiteParameter("Id", DbType.Int32));
                    cmd.Parameters.Add(new SQLiteParameter("HomeTeamId", DbType.Int32));
                    cmd.Parameters.Add(new SQLiteParameter("AwayTeamId", DbType.Int32));
                    cmd.Parameters.Add(new SQLiteParameter("HomeScore", DbType.Int32));
                    cmd.Parameters.Add(new SQLiteParameter("AwayScore", DbType.Int32));
                    cmd.Parameters.Add(new SQLiteParameter("DateStart", DbType.DateTime));

                    foreach (var match in matches)
                    {
                        cmd.Parameters["Id"].Value = match.Id;
                        cmd.Parameters["HomeTeamId"].Value = match.HomeTeamId;
                        cmd.Parameters["AwayTeamId"].Value = match.AwayTeamId;
                        cmd.Parameters["HomeScore"].Value = match.HomeScore;
                        cmd.Parameters["AwayScore"].Value = match.AwayScore;
                        cmd.Parameters["DateStart"].Value = match.DateStart;

                        cmd.ExecuteNonQuery();
                    }
                }

                trans.Commit();
            }

            return result;
        }
    }
}
