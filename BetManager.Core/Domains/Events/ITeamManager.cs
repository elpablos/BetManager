using System.Collections.Generic;
using BetManager.Core.DbModels.Events;

namespace BetManager.Core.Domains.Events
{
    public interface ITeamManager
    {
        ICollection<Team> GetAll(object input);
    }
}