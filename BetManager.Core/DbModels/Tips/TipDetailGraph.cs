using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.DbModels.Tips
{
    public class TipDetailGraph
    {
        public DateTime DateStart { get; set; }

        public int Total { get; set; }

        public int Correct { get; set; }

        public double Price { get; set; }
    }
}
