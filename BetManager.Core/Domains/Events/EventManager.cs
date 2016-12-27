using BetManager.Core.DbModels.Events;
using Dapper;
using System.Collections.Generic;
using System.Linq;

namespace BetManager.Core.Domains.Events
{
    public class EventManager : BaseManager, IEventManager
    {
        public virtual ICollection<Event> GetAll(object input)
        {
            ICollection<Event> tips = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                tips = conn.Query<Event>("select * from BM_Event where ID_Tournament=@ID_Tournament and (@ID_Season is null or ID_Season=@ID_Season)", input).ToList();
                conn.Close();
            }
            return tips;
        }
    }
}
