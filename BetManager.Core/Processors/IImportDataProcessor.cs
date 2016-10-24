using System;
using System.Collections.Generic;
using BetManager.Core.DbModels;

namespace BetManager.Core.Processors
{
    public interface IImportDataProcessor
    {
        string DownloadData(string sport, string category, DateTime date);
        IEnumerable<ImportData> ProcessData(string stringData);
    }
}