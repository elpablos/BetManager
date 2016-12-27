using BetManager.Solver.Models;
using System;

namespace BetManager.Solver.Managers
{
    public static class MethodExtensions
    {
        /// <summary>
        /// Časová funkce 
        /// e^(-epsilon * t)
        /// </summary>
        /// <param name="match">zápas</param>
        /// <param name="actualDate">aktuální den</param>
        /// <param name="epsilon">epsilon</param>
        /// <returns>časová funkce</returns>
        public static double TimeFunc(this GameMatch match, DateTime actualDate, double epsilon)
        {
            return Math.Exp(-1 * epsilon * (actualDate - match.DateStart).Days);
        }
    }
}
