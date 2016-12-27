using BetManager.Core.DbModels.Events;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace BetManager.Core.Domains.Events
{
    public class TeamManager : BaseManager, ITeamManager
    {
        public virtual ICollection<Team> GetAll(object input)
        {
            ICollection<Team> tips = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tips = conn.Query<Team>(
                    @"
                    select distinct BM_Team.* from BM_Event
                    inner join BM_Team on BM_Team.ID=BM_Event.ID_HomeTeam 
                    where ID_Tournament=@ID_Tournament and (@ID_Season is null or ID_Season=@ID_Season)", input).ToList();
                conn.Close();
            }
            return tips;
        }
    }
}
