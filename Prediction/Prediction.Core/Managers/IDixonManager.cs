using Prediction.Core.Models;
using Prediction.Core.Solvers;
using System;
using System.Collections.Generic;

namespace Prediction.Core.Managers
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
        /// Theta
        /// </summary>
        IList<double> Thetas { get; }

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
        double Gamma { get; set; }

        /// <summary>
        /// Parametr závislosti
        /// upravuje ?poissonovu? krivku kolem -0.5..0.5 golu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        double Rho { get; set; }

        /// <summary>
        /// Dalsi parametr
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        double Mi { get; set; }

        /// <summary>
        /// Dalsi parametr 
        /// </summary>
        double Lambda { get; set; }

        /// <summary>
        /// čas posledního doběhu
        /// </summary>
        TimeSpan LastElapsed { get; set; }

        /// <summary>
        /// Součet
        /// </summary>
        double Summary { get; set; }

        /// <summary>
        /// Součet pravděpodobností
        /// </summary>
        double MaximumLikehoodValue { get; set; }

        /// <summary>
        /// Počet golů na ppdst
        /// </summary>
        int PropLength { get; set; }

        /// <summary>
        /// Report ze solveru
        /// </summary>
        string Description { get; set; }

        /// <summary>
        /// Datum predikce
        /// </summary>
        DateTime DatePredict { get; set; }

        int Id { get; set; }

        SolverTypeEnum Type { get; set; }

        double P { get; set; }

        DateTime? KsiStart { get; set; }

        double SumMaximumLikehood();
        double HomeProbability(GameMatch match);
        double DrawProbability(GameMatch match);
        double AwayProbability(GameMatch match);
    }
}
