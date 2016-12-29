using System.Collections.Generic;
using BetManager.Core.DbModels.Predictions;

namespace BetManager.Core.Domains.Predictions
{
    public interface ISpecificPredicitionManager
    {
        ICollection<SpecificPredicition> GetAll(object input);
        SpecificPredicition GetById(int id);
    }
}