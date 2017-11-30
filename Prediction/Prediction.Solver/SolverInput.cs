using Prediction.Core.Solvers;
using System;

namespace Prediction.Solver
{
    public class Input
    {
        public SolverInput[] Inputs { get; set; }

        public string Path { get; set; }

        public bool CanSaveToDb { get; set; }

        public DateTime? KsiStart { get; set; }

        public int PropLength { get; set; }
    }

    public class SolverInput
    {
        public DateTime DateStart { get; set; }

        public SolverTypeEnum Type { get; set; }

        public double Ksi { get; set; }

        public SolverInput()
        { }

        public SolverInput(double ksi, DateTime dateStart, SolverTypeEnum type)
        {
            Ksi = ksi;
            DateStart = dateStart;
            Type = type;
        }
    }
}
