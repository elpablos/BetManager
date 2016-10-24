using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BetManager.Web.Mvc;
using BetManager.Core.Domains.ImportDatas;
using BetManager.Core.Domains.Tips;
using BetManager.Core.Processors;

namespace BetManager.Web.ApiControllers.ImportDatas.Import
{
    public class ImportDataImportHandler : IModelHandler<ImportDataImportFilterViewModel>
    {
        private readonly IImportDataManager _importManager;
        private readonly ITipManager _tipManager;
        private readonly IImportDataProcessor _processor;

        public ImportDataImportHandler()
        {
            _importManager = new ImportDataManager();
            _tipManager = new TipManager();
            _processor = new ImportDataProcessor();
        }

        public ModelHandlerResult Handle(ImportDataImportFilterViewModel model)
        {
            var result = new ModelHandlerResult();
            string json = _processor.DownloadData(model.Sport, model.Category, model.Date);
            var collection = _processor.ProcessData(json);

            _importManager.ImportClear();
            _importManager.InsertBulk(collection);
            _importManager.ImportData();
            _tipManager.TipGenerate();

            result.Data = collection.Count();
            return result;
        }
    }
}