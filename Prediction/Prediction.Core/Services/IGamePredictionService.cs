using System.Collections.Generic;
using Prediction.Core.Models;
using Prediction.Core.Managers;
using Prediction.Core.Solvers;

namespace Prediction.Core.Services
{
    public interface IGamePredictionService
    {
        ICollection<IDixonManager> GetAll(double ksi, SolverTypeEnum? type = null);
        // void Insert(GamePrediction prediction);
        void Insert(IDixonManager manager);
        void Prepare();
        IDixonManager GetById(int id, IList<GameMatch> matches, IList<GameTeam> teams);
    }
}