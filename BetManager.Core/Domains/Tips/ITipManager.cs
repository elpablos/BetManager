using System.Collections.Generic;
using BetManager.Core.DbModels.Tips;

namespace BetManager.Core.Domains.Tips
{
    public interface ITipManager
    {
        ICollection<Tip> GetAll(object input);
        Tip GetById(int id);
        ICollection<TipDetailGraph> GetGraph(object input);
        ICollection<TipAllPoisson> GetAllPoisson(object input);
        int TipGenerate();
    }
}