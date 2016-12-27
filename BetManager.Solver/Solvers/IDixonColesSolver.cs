using System;

namespace BetManager.Solver.Solvers
{
    public interface IDixonColesSolver
    {
        double Solve(DateTime actualDate);
    }
}
