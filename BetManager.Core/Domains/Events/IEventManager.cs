using System.Collections.Generic;
using BetManager.Core.DbModels.Events;

namespace BetManager.Core.Domains.Events
{
    public interface IEventManager
    {
        ICollection<Event> GetAll(object input);
    }
}