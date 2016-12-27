using System.Collections.Generic;
using BetManager.Core.DbModels.Predictions;

namespace BetManager.Core.Domains.Predictions
{
    public interface IPredictionManager
    {
        int Insert(Prediction prediction, IList<PredictionTeam> teams);
    }
}