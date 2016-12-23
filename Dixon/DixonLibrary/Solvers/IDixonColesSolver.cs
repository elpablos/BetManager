using System;

namespace Dixon.Library.Solvers
{
    public interface IDixonColesSolver
    {
        double Solve(DateTime actualDate);
    }
}