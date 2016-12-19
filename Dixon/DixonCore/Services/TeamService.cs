using Dapper;
using Dixon.Core.DbModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixon.Core.Services
{
    public class TeamService : ITeamService
    {
        public virtual IEnumerable<Team> GetAll(int tournament, int season)
        {
            IEnumerable<Team> ret = null;
            using (var sqlConnection = new SqlConnection(@"Server =.\sqlexpress;Database=BetManagerDevel;Trusted_Connection=True;"))
            {
                sqlConnection.Open();

                ret = sqlConnection.Query<Team>(
@"
select distinct BM_Team.* from BM_Event
inner join BM_Team on BM_Team.ID=BM_Event.ID_HomeTeam
where BM_Event.ID_Tournament=@ID_Tournament and BM_Event.ID_Season=@ID_Season
",
                    new { ID_Tournament = tournament, ID_Season = season });
                sqlConnection.Close();
            }

            return ret;
        }
    }
}
