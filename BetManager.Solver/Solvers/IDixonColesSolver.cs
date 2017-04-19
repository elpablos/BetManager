using System;

namespace BetManager.Solver.Solvers
{
    public interface IDixonColesSolver : IDisposable
    {
        string LastReport { get; }
        double Solve(DateTime actualDate);
    }
}
