using BetManager.Core.DbModels;
using BetManager.Core.Domains.ImportDatas;
using BetManager.Web.ApiControllers.ImportDatas.Import;
using BetManager.Web.ApiControllers.Predictions.Generate;
using BetManager.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace BetManager.Web.ApiControllers
{
    public class ImportDataController : ApiController
    {
        public int Get()
        {
            HttpContext.Current.Server.ScriptTimeout = 300;

            // DateTime date, string category, string sport
            var vm = new ImportDataImportFilterViewModel();
            //if (sport != null)
            //{
            //    vm.Category = category;
            //    vm.Date = date;
            //    vm.Sport = sport;
            //}
            var result = Handler.Get<ImportDataImportHandler>().Handle(vm);

            return (int)result.Data;
        }

        [HttpPost]
        public int Post(ImportDataImportFilterViewModel model)
        {
            HttpContext.Current.Server.ScriptTimeout = 300;
            var result = Handler.Get<ImportDataImportHandler>().Handle(model);

            return (int)result.Data;
        }

        [Authorize]
        [HttpGet]
        [Route("api/ImportData/Tomorrow")]
        public int Tommorow()
        {
            HttpContext.Current.Server.ScriptTimeout = 300;

            var vm = new ImportDataImportFilterViewModel();
            vm.Date = DateTime.Now.AddDays(1);
            var result = Handler.Get<ImportDataImportHandler>().Handle(vm);

            return (int)result.Data;
        }

        [Authorize]
        [HttpGet]
        [Route("api/ImportData/Yesterday")]
        public int Yesterday()
        {
            HttpContext.Current.Server.ScriptTimeout = 300;

            var vm = new ImportDataImportFilterViewModel();
            vm.Date = DateTime.Now.AddDays(-1);
            var result = Handler.Get<ImportDataImportHandler>().Handle(vm);

            return (int)result.Data;
        }

        [Authorize]
        [HttpGet]
        [Route("api/Prediction")]
        public int Prediction()
        {
            HttpContext.Current.Server.ScriptTimeout = 300;

            var vm = new PredictionGenerateFilterViewModel();
            var result = Handler.Get<PredictionGenerateHandler>().Handle(vm);

            return (int)result.Data;
        }
    }
}