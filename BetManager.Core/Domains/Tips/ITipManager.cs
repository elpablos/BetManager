using System.Collections.Generic;
using BetManager.Core.DbModels.Tips;

namespace BetManager.Core.Domains.Tips
{
    public interface ITipManager
    {
        ICollection<Tip> GetAll(object input);
        Tip GetById(int id);
        ICollection<TipDetailGraph> GetGraph(object input);
        ICollection<TipDetailPoisson> GetAllPoisson(object input);
        ICollection<TipDetailGoal> GetAllGoal(object input);
        ICollection<TipDetailPoisson> GetAllPoissonHistory(object input);
        ICollection<TipDetailProgress> GetAllProgress(object input);

        int TipGenerate();
        int PredictionGenerate();
    }
}