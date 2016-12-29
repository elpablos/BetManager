using System;

namespace BetManager.Solver.Solvers
{
    public interface IDixonColesSolver
    {
        string LastReport { get; }
        double Solve(DateTime actualDate);
    }
}
