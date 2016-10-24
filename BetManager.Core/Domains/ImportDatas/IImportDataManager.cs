using System.Collections.Generic;
using BetManager.Core.DbModels;

namespace BetManager.Core.Domains.ImportDatas
{
    public interface IImportDataManager
    {
        ImportData GetById(int id);
        int Insert(ImportData data);
        int InsertBulk(IEnumerable<ImportData> rows);
        int Truncate();
        int ImportClear();
        int ImportData();
        ICollection<ImportData> GetAll();
    }
}