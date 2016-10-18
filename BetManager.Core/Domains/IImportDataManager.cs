using System.Collections.Generic;
using BetManager.Core.DbModels;

namespace BetManager.Core.Domains
{
    public interface IImportDataManager
    {
        ImportData GetById(int id);
        int Insert(ImportData data);
        ICollection<ImportData> GetAll();
    }
}