using Dixon.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixon.Library.Managers
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
