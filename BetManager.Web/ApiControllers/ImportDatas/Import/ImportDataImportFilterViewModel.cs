using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.ApiControllers.ImportDatas.Import
{
    public class ImportDataImportFilterViewModel
    {
        public string Sport { get; set; }

        public string Category { get; set; }

        public DateTime Date { get; set; }

        public ImportDataImportFilterViewModel()
        {
            Date = DateTime.Now;
            Sport = "football";
            Category = string.Empty;
        }
    }
}