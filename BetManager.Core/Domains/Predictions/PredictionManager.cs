using BetManager.Core.DbModels.Predictions;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace BetManager.Core.Domains.Predictions
{
    public class PredictionManager : BaseManager, IPredictionManager
    {
        private readonly string SqlInsert = 
@"
    insert into BM_Prediction ([ID_Tournament], [DatePredict], [Ksi], [Gamma], [Summary], [LikehoodValue], [DateCreated], [Elapsed])
    values (@ID_Tournament, @DatePredict, @Ksi, @Gamma, @Summary, @LikehoodValue, @DateCreated, @Elapsed)
    select @@identity
";

        private readonly string SqlInsertTeams =
@"
    insert into BM_PredictionTeam ([ID_Team], [Attack], [Defence], [ID_Prediction])
    values (@ID_Team, @Attack, @Defence, @ID_Prediction)
";

        public int Insert(Prediction prediction, IList<PredictionTeam> teams)
        {
            int identity = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                identity = conn.Query<int>(SqlInsert, prediction).Single();

                foreach (var team in teams)
                {
                    team.ID_Prediction = identity;
                }

                int ret = conn.Execute(SqlInsertTeams, teams);

                conn.Close();
            }
            return identity;
        }
    }
}
