using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.DbModels.Tips
{
    public class TipDetailProgress
    {
        public int ID { get; set; }
        public DateTime DatePredict { get; set; }
        public double Home { get; set; }
        public double Away { get; set; }
    }
}
