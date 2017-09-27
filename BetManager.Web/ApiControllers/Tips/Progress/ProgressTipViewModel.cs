using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.ApiControllers.Tips.Progress
{
    public class ProgressTipViewModel
    {
        public int ID { get; set; }
        public DateTime DatePredict { get; set; }
        public double Home { get; set; }
        public double Away { get; set; }

    }
}