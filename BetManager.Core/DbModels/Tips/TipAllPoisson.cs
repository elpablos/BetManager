using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.DbModels.Tips
{
    public class TipAllPoisson
    {
        public double GoalZero { get; set; }

        public double GoalOne { get; set; }

        public double GoalTwo { get; set; }

        public double GoalThree { get; set; }

        public double GoalFour { get; set; }

        public double GoalFive { get; set; }

        public string DisplayName { get; set; }

        public int? Score { get; set; }

        public double Tip { get; set; }
    }
}
