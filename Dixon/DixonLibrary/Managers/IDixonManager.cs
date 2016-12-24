using System;
using System.Collections.Generic;
using Dixon.Library.Models;

namespace Dixon.Library.Managers
{
    /// <summary>
    /// Interface pro výpočet zápasů dle Dixon-Coles
    /// </summary>
    public interface IDixonManager
    {
        /// <summary>
        /// Zápasy
        /// </summary>
        IList<GameMatch> Matches { get; }

        /// <summary>
        /// Týmy
        /// </summary>
        IList<GameTeam> Teams { get; }

        /// <summary>
        /// Ksi pro výpočet časové fce
        /// Výchozí hodnota - polotýdny
        /// Dixon-Coles váha / půl týdne
        /// 0.0065 / 3.5
        /// </summary>
        double Ksi { get; set; }

        /// <summary>
        /// Výhoda domácího
        /// Tým hrající doma má většinou převahu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        double Gama { get; set; }

        /// <summary>
        /// Parametr závislosti
        /// upravuje ?poissonovu? krivku kolem -0.5..0.5 golu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        double Rho { get; set; }

        /// <summary>
        /// čas posledního doběhu
        /// </summary>
        TimeSpan LastElapsed { get; set; }

        /// <summary>
        /// Součet
        /// </summary>
        double Summary { get; set; }

        /// <summary>
        /// Suma věrohodnostních funkcí nad zápasem
        /// </summary>
        /// <param name="dateActual"></param>
        /// <returns></returns>
        double Sum(DateTime dateActual);
    }
}