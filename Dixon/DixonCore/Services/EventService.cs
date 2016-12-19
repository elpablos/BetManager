using Dapper;
using Dixon.Core.DbModels;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Dixon.Core.Services
{
    public class EventService : IEventService
    {
        public virtual IEnumerable<Event> GetAll(int tournament, int season)
        {
            IEnumerable<Event> ret = null;
            using (var sqlConnection = new SqlConnection(@"Server =.\sqlexpress;Database=BetManagerDevel;Trusted_Connection=True;"))
            {
                sqlConnection.Open();

                ret = sqlConnection.Query<Event>("select * from BM_Event where ID_Tournament=@ID_Tournament and ID_Season=@ID_Season",
                    new { ID_Tournament = tournament, ID_Season = season });
                sqlConnection.Close();
            }

            return ret;
        }
    }
}
