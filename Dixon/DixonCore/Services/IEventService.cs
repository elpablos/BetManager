using Dixon.Core.DbModels;
using System.Collections.Generic;

namespace Dixon.Core.Services
{
    public interface IEventService
    {
        IEnumerable<Event> GetAll(int tournament, int season);
    }
}