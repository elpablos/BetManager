using Microsoft.SolverFoundation.Services;
using System;

namespace Prediction.Core.Solvers
{
    public interface IDixonColesSolver : IDisposable
    {
        string LastReport { get; }
        double Solve(DateTime actualDate);
        HybridLocalSearchDirective Directive { get; }
    }
}
