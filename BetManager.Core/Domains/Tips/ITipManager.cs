using System.Collections.Generic;
using BetManager.Core.DbModels.Tips;

namespace BetManager.Core.Domains.Tips
{
    public interface ITipManager
    {
        ICollection<Tip> GetAll(object input);
        Tip GetById(int id);
        IEnumerable<TipDetailGraph> GetGraph(object input);
        int TipGenerate();
    }
}