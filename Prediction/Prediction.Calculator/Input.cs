using Prediction.Core.Solvers;
using System;

namespace Prediction.Calculator
{
    public class Input
    {
        public DateTime? KsiStart { get; set; }
        public SolverTypeEnum? Type { get; set; }
        public int PropLength { get; set; }
        public double Ksi { get; set; }
    }
}
