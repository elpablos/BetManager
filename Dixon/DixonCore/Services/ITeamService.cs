using Dixon.Core.DbModels;
using System.Collections.Generic;

namespace Dixon.Core.Services
{
    public interface ITeamService
    {
        IEnumerable<Team> GetAll(int tournament, int season);
    }
}