using System.Collections.Generic;
using BetManager.Core.DbModels;

namespace BetManager.Core.Domains
{
    public interface ITipManager
    {
        ICollection<Tip> GetAll();
        Tip GetById(int id);
    }
}