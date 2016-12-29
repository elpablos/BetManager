using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Predictions.List
{
    public class PredictionListViewModel
    {
        public PredictionListFilterViewModel Filter { get; set; }

        public ICollection<PredictionListItemViewModel> Rows { get; set; }

        public PredictionListViewModel()
        {
            Rows = new List<PredictionListItemViewModel>();
        }
    }
}