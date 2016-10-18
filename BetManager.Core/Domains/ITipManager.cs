using System.Collections.Generic;
using BetManager.Core.DbModels;

namespace BetManager.Core.Domains
{
    public interface ITipManager
    {
        ICollection<Tip> GetAll(object input);
        Tip GetById(int id);
    }
}