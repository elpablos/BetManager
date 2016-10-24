using BetManager.Core.DbModels;
using BetManager.Core.Domains.ImportDatas;
using BetManager.Web.ApiControllers.ImportDatas.Import;
using BetManager.Web.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BetManager.Web.ApiControllers
{
    public class ImportDataController : ApiController
    {
        public int Get()
        {
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
    }
}