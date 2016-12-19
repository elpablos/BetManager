using System;
using System.Collections.Generic;
using Dixon.Library.Models;

namespace Dixon.Library.Managers
{
    public interface IDixonManager
    {
        double Epsilon { get; set; }
        double Gama { get; set; }
        IList<GameMatch> Matches { get; }
        double Rho { get; set; }
        IList<GameTeam> Teams { get; }

        double Sum(DateTime dateActual);
    }
}