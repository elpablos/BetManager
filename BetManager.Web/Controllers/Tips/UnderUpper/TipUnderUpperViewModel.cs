using System.Collections.Generic;

namespace BetManager.Web.Controllers.Tips.UnderUpper
{
    public class TipUnderUpperViewModel
    {
        public TipUnderUpperFilterViewModel Filter { get; set; }

        public ICollection<TipUnderUpperItemViewModel> Rows { get; set; }

        public TipUnderUpperViewModel()
        {
            Rows = new List<TipUnderUpperItemViewModel>();
        }
    }
}