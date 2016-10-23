using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.ApiControllers.Tips.Graph
{
    public class GraphTipFilterViewModel
    {
        public DateTime? DateFrom { get; set; }

        public DateTime? DateTo { get; set; }

        public double Odd { get; set; }

        public int Form { get; set; }
    }
}