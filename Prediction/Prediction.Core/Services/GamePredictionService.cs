using Prediction.Core.Managers;
using Prediction.Core.Models;
using Prediction.Core.Solvers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace Prediction.Core.Services
{
    public class GamePredictionService : BaseService, IGamePredictionService
    {
        public GamePredictionService(string connectionString = null) : base(connectionString)
        {
        }

        public void Prepare()
        {
            string query =
@"
CREATE TABLE GamePrediction
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Type INTEGER,
    DatePredict NUMERIC,
    Ksi NUMERIC,
    Rho NUMERIC,
    Mi NUMERIC,
    Gamma NUMERIC,
    LastElapsed NUMERIC,
    Summary NUMERIC,
    MaximumLikehoodValue NUMERIC,
    Description TEXT,
    P NUMERIC, 
    Lambda NUMERIC
);
";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.ExecuteNonQuery();
            }

            query =
@"
CREATE TABLE GamePredictionTeam
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PredictionId INTEGER,
    TeamId INTEGER,
    HomeAttack NUMERIC,
    AwayAttack NUMERIC,
    FOREIGN KEY (TeamId) REFERENCES GameTeam(Id),
    FOREIGN KEY (PredictionId) REFERENCES GamePrediction(Id)
);
";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.ExecuteNonQuery();
            }

            query =
@"
CREATE TABLE GamePredictionTheta
(
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    PredictionId INTEGER,
    Ind INTEGER,
    Value NUMERIC,
    FOREIGN KEY (PredictionId) REFERENCES GamePrediction(Id)
);
";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.ExecuteNonQuery();
            }
        }

        public ICollection<IDixonManager> GetAll(double ksi, SolverTypeEnum? type = null)
        {
            ICollection<IDixonManager> result = new List<IDixonManager>();
            string query = @"
select Id, Type, DatePredict, Ksi, Rho, Mi, Gamma, LastElapsed, Summary, MaximumLikehoodValue, Description, P, Lambda
from GamePrediction
where Ksi=@Ksi
    and Type is null or Type=@Type
order by DatePredict asc";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.Parameters.Add(new SQLiteParameter("Type", DbType.Int32));
                q.Parameters.Add(new SQLiteParameter("Ksi", DbType.Double));
                q.Parameters["Type"].Value = type;
                q.Parameters["Ksi"].Value = ksi;

                var reader = q.ExecuteReader();
                while (reader.Read())
                {
                    IDixonManager manager = null;
                    manager = GetManager(reader, null, null);
                    result.Add(manager);
                }
            }

            return result;
        }

//        public void Insert(GamePrediction prediction)
//        {
//            string query = 
//                @"
//insert into GamePrediction (Type, DatePredict, Ksi, Rho, Mi, Gamma, LastElapsed, Summary, MaximumLikehoodValue, Description) 
//values (@Type, @DatePredict, @Ksi, @Rho, @Mi, @Gamma, @LastElapsed, @Summary, @MaximumLikehoodValue, @Description)";
//            using (var trans = GetConnection.BeginTransaction())
//            {
//                try
//                {
//                    using (var cmd = new SQLiteCommand(query, GetConnection))
//                    {
//                        cmd.Parameters.Add(new SQLiteParameter("Type", DbType.Int32));
//                        cmd.Parameters.Add(new SQLiteParameter("DatePredict", DbType.DateTime));
//                        cmd.Parameters.Add(new SQLiteParameter("Ksi", DbType.Double));
//                        cmd.Parameters.Add(new SQLiteParameter("Rho", DbType.Double));
//                        cmd.Parameters.Add(new SQLiteParameter("Mi", DbType.Double));
//                        cmd.Parameters.Add(new SQLiteParameter("Gamma", DbType.Double));
//                        cmd.Parameters.Add(new SQLiteParameter("LastElapsed", DbType.DateTimeOffset));
//                        cmd.Parameters.Add(new SQLiteParameter("Summary", DbType.Double));
//                        cmd.Parameters.Add(new SQLiteParameter("MaximumLikehoodValue", DbType.Double));
//                        cmd.Parameters.Add(new SQLiteParameter("Description", DbType.String));

//                        cmd.Parameters["Type"].Value = prediction.Type;
//                        cmd.Parameters["DatePredict"].Value = prediction.DatePredict;
//                        cmd.Parameters["Ksi"].Value = prediction.Ksi;
//                        cmd.Parameters["Rho"].Value = prediction.Rho;
//                        cmd.Parameters["Mi"].Value = prediction.Mi;
//                        cmd.Parameters["Gamma"].Value = prediction.Gamma;
//                        cmd.Parameters["LastElapsed"].Value = prediction.LastElapsed;
//                        cmd.Parameters["Summary"].Value = prediction.Summary;
//                        cmd.Parameters["MaximumLikehoodValue"].Value = prediction.MaximumLikehoodValue;
//                        cmd.Parameters["Description"].Value = prediction.Description;

//                        cmd.ExecuteNonQuery();
//                    }

//                    query = "select last_insert_rowid()";
//                    using (var cmd = new SQLiteCommand(query, GetConnection))
//                    {
//                        prediction.Id = (int)cmd.ExecuteScalar();
//                    }

//                    query =
//                        @"
//insert into GamePredictionTeam (PredictionId, TeamId, HomeAttack, AwayAttack) 
//values (@PredictionId, @TeamId, @HomeAttack, @AwayAttack)";

//                    foreach (var team in prediction.Teams)
//                    {
//                        using (var cmd = new SQLiteCommand(query, GetConnection))
//                        {
//                            cmd.Parameters.Add(new SQLiteParameter("PredictionId", DbType.Int32));
//                            cmd.Parameters.Add(new SQLiteParameter("TeamId", DbType.Int32));
//                            cmd.Parameters.Add(new SQLiteParameter("HomeAttack", DbType.Double));
//                            cmd.Parameters.Add(new SQLiteParameter("AwayAttack", DbType.Double));

//                            cmd.Parameters["PredictionId"].Value = prediction.Id;
//                            cmd.Parameters["TeamId"].Value = team.Id;
//                            cmd.Parameters["HomeAttack"].Value = team.HomeAttack;
//                            cmd.Parameters["AwayAttack"].Value = team.AwayAttack;

//                            cmd.ExecuteNonQuery();
//                        }
//                    }

//                    trans.Commit();
//                }
//                catch (System.Exception ex)
//                {
//                    trans.Rollback();
//                    throw ex;
//                }
//            }
//        }

        public void Insert(IDixonManager manager)
        {
            string query =
         @"
insert into GamePrediction (Type, DatePredict, Ksi, Rho, Mi, Gamma, LastElapsed, Summary, MaximumLikehoodValue, Description, P, Lambda) 
values (@Type, @DatePredict, @Ksi, @Rho, @Mi, @Gamma, @LastElapsed, @Summary, @MaximumLikehoodValue, @Description)";
            using (var trans = GetConnection.BeginTransaction())
            {
                try
                {
                    using (var cmd = new SQLiteCommand(query, GetConnection))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("Type", DbType.Int32));
                        cmd.Parameters.Add(new SQLiteParameter("DatePredict", DbType.DateTime));
                        cmd.Parameters.Add(new SQLiteParameter("Ksi", DbType.Double));
                        cmd.Parameters.Add(new SQLiteParameter("Rho", DbType.Double));
                        cmd.Parameters.Add(new SQLiteParameter("Mi", DbType.Double));
                        cmd.Parameters.Add(new SQLiteParameter("Gamma", DbType.Double));
                        cmd.Parameters.Add(new SQLiteParameter("LastElapsed", DbType.DateTimeOffset));
                        cmd.Parameters.Add(new SQLiteParameter("Summary", DbType.Double));
                        cmd.Parameters.Add(new SQLiteParameter("MaximumLikehoodValue", DbType.Double));
                        cmd.Parameters.Add(new SQLiteParameter("Description", DbType.String));
                        cmd.Parameters.Add(new SQLiteParameter("P", DbType.Double));
                        cmd.Parameters.Add(new SQLiteParameter("Lambda", DbType.Double));

                        cmd.Parameters["Type"].Value = manager.Type;
                        cmd.Parameters["DatePredict"].Value = manager.DatePredict;
                        cmd.Parameters["Ksi"].Value = manager.Ksi;
                        cmd.Parameters["Rho"].Value = manager.Rho;
                        cmd.Parameters["Mi"].Value = manager.Mi;
                        cmd.Parameters["Gamma"].Value = manager.Gamma;
                        cmd.Parameters["LastElapsed"].Value = manager.LastElapsed;
                        cmd.Parameters["Summary"].Value = manager.Summary;
                        cmd.Parameters["MaximumLikehoodValue"].Value = manager.MaximumLikehoodValue;
                        cmd.Parameters["Description"].Value = manager.Description;
                        cmd.Parameters["P"].Value = manager.P;
                        cmd.Parameters["Lambda"].Value = manager.Lambda;

                        cmd.ExecuteNonQuery();
                    }

                    query = "select last_insert_rowid()";
                    using (var cmd = new SQLiteCommand(query, GetConnection))
                    {
                        manager.Id = (int)(long)cmd.ExecuteScalar();
                    }

                    query =
                        @"
insert into GamePredictionTeam (PredictionId, TeamId, HomeAttack, AwayAttack) 
values (@PredictionId, @TeamId, @HomeAttack, @AwayAttack)";

                    foreach (var team in manager.Teams)
                    {
                        using (var cmd = new SQLiteCommand(query, GetConnection))
                        {
                            cmd.Parameters.Add(new SQLiteParameter("PredictionId", DbType.Int32));
                            cmd.Parameters.Add(new SQLiteParameter("TeamId", DbType.Int32));
                            cmd.Parameters.Add(new SQLiteParameter("HomeAttack", DbType.Double));
                            cmd.Parameters.Add(new SQLiteParameter("AwayAttack", DbType.Double));

                            cmd.Parameters["PredictionId"].Value = manager.Id;
                            cmd.Parameters["TeamId"].Value = team.Id;
                            cmd.Parameters["HomeAttack"].Value = team.HomeAttack;
                            cmd.Parameters["AwayAttack"].Value = team.AwayAttack;

                            cmd.ExecuteNonQuery();
                        }
                    }

                    if (manager.Thetas != null)
                    {
                        query =
    @"
insert into GamePredictionTheta (PredictionId, Ind, Value) 
values (@PredictionId, @Index, @Value)";

                        // foreach (var theta in manager.Thetas)
                        for (int i = 0; i < manager.Thetas.Count; i++)
                        {
                            using (var cmd = new SQLiteCommand(query, GetConnection))
                            {
                                cmd.Parameters.Add(new SQLiteParameter("PredictionId", DbType.Int32));
                                cmd.Parameters.Add(new SQLiteParameter("Index", DbType.Int32));
                                cmd.Parameters.Add(new SQLiteParameter("Value", DbType.Double));

                                cmd.Parameters["PredictionId"].Value = manager.Id;
                                cmd.Parameters["Index"].Value = i;
                                cmd.Parameters["Value"].Value = manager.Thetas[i];

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw ex;
                }
            }
        }

        public IDixonManager GetById(int id, IList<GameMatch> matches, IList<GameTeam> teams) 
        {
            IDixonManager manager = null;

            string query = @"
select Id, Type, DatePredict, Ksi, Rho, Mi, Gamma, LastElapsed, Summary, MaximumLikehoodValue, Description, P, Lambda
from GamePrediction where Id = @Id";
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.Parameters.Add(new SQLiteParameter("Id", DbType.Int32));
                q.Parameters["Id"].Value = id;
                var reader = q.ExecuteReader();
                if (reader.Read())
                {
                    manager = GetManager(reader, matches, teams);
                }
            }

            query = @"
select Id, PredictionId, TeamId, HomeAttack, AwayAttack 
from GamePredictionTeam
where PredictionId = @PredictionId";

            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.Parameters.Add(new SQLiteParameter("PredictionId", DbType.Int32));
                q.Parameters["PredictionId"].Value = id;

                var reader = q.ExecuteReader();
                while (reader.Read())
                {
                    var team = manager.Teams.FirstOrDefault(x => x.Id == reader.GetInt32(2));
                    team.HomeAttack = reader.GetDouble(3);
                    team.AwayAttack = reader.GetDouble(4);
                }
            }

            query = @"
select Id, PredictionId, Ind, Value
from GamePredictionTheta
where PredictionId = @PredictionId
order by Ind";

            manager.Thetas.Clear();
            using (var q = new SQLiteCommand(query, GetConnection))
            {
                q.Parameters.Add(new SQLiteParameter("PredictionId", DbType.Int32));
                q.Parameters["PredictionId"].Value = id;

                var reader = q.ExecuteReader();
                while (reader.Read())
                {
                    manager.Thetas.Add(reader.GetDouble(3));
                }
            }

            return manager;
        }

        private IDixonManager GetManager(SQLiteDataReader reader, IList<GameMatch> matches, IList<GameTeam> teams)
        {
            IDixonManager manager = null;

            var type = (SolverTypeEnum)reader.GetInt32(1);
            switch (type)
            {
                case SolverTypeEnum.DP:
                    manager = new DPManager(matches, teams);
                    break;
                case SolverTypeEnum.BP:
                    manager = new BPManager(matches, teams);
                    break;
                case SolverTypeEnum.DPDI:
                    manager = new DPDIManager(matches, teams);
                    break;
                case SolverTypeEnum.BPDI:
                    manager = new BPDIManager(matches, teams);
                    break;
                default:
                    throw new NotImplementedException("Nebyl vybrán typ solveru!");
            }

            manager.Id = reader.GetInt32(0);
            manager.DatePredict = reader.GetDateTime(2);
            manager.Ksi = reader.GetDouble(3);
            manager.Rho = reader.GetDouble(4);
            manager.Mi = reader.GetDouble(5);
            manager.Gamma = reader.GetDouble(6);
            // manager.LastElapsed = new TimeSpan((long)reader.GetDecimal(7));
            manager.Summary = reader.GetDouble(8);
            manager.MaximumLikehoodValue = reader.GetDouble(9);
            manager.Description = reader.GetString(10);
            manager.P = reader.GetDouble(11);
            manager.Lambda = reader.GetDouble(12);

            return manager;
        }
    }
}
