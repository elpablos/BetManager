using Prediction.Core.Solvers;
using System;
using System.Collections.Generic;

namespace Prediction.Core.Models
{
    public class GamePrediction
    {
        /// <summary>
        /// Id predikce
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Typ
        /// </summary>
        public SolverTypeEnum Type { get; set; }

        /// <summary>
        /// Datum predikce
        /// </summary>
        public DateTime DatePredict { get; set; }

        /// <summary>
        /// Ksi - Epsilon pro výpočet časové fce
        /// </summary>
        public double Ksi { get; set; }

        /// <summary>
        /// Parametr závislosti
        /// upravuje ?poissonovu? krivku kolem -0.5..0.5 golu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        public double Rho { get; set; }

        /// <summary>
        /// Dalsi parametr 
        /// </summary>
        public double Mi { get; set; }

        /// <summary>
        /// Výhoda domácího
        /// Tým hrající doma má většinou převahu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// rozsah 1..2
        /// </summary>
        public double Gamma { get; set; }

        /// <summary>
        /// čas posledního doběhu
        /// </summary>
        public TimeSpan LastElapsed { get; set; }

        /// <summary>
        /// Součet
        /// </summary>
        public double Summary { get; set; }

        /// <summary>
        /// Součet pravděpodobností
        /// </summary>
        public double MaximumLikehoodValue { get; set; }

        /// <summary>
        /// Report ze solveru
        /// </summary>
        public string Description { get; set; }

        public ICollection<GameTeam> Teams { get; set; }

        public ICollection<GameMatch> Matches { get; set; }

        public GamePrediction()
        {
            Teams = new List<GameTeam>();
            Matches = new List<GameMatch>();
        }
    }
}


