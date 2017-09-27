using BetManager.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Tips.List
{
    public class TipListViewModel
    {
        public TipListFilterViewModel Filter { get; set; }

        public ICollection<TipListItemViewModel> Rows { get; set; }

        public TipListViewModel()
        {
            Rows = new List<TipListItemViewModel>();
        }
    }
}