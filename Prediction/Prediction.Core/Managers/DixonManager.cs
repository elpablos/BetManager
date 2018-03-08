using Prediction.Core.Models;
using Prediction.Core.Solvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Prediction.Core.Managers
{
    /// <summary>
    /// Výpočet zápasů dle Dixon-Coles
    /// </summary>
    public class DixonManager : IDixonManager
    {
        #region Properties

        /// <summary>
        /// Zápasy
        /// </summary>
        public IList<GameMatch> Matches { get; private set; }

        /// <summary>
        /// Týmy
        /// </summary>
        public IList<GameTeam> Teams { get; private set; }

        /// <summary>
        /// Theta
        /// </summary>
        public IList<double> Thetas { get; private set; }

        /// <summary>
        /// Epsilon pro výpočet časové fce
        /// Výchozí hodnota - polotýdny
        /// Dixon-Coles váha / půl týdne
        /// 0.0065 / 3.5
        /// </summary>
        public double Ksi { get; set; } = 0.0065 / 3.5;

        /// <summary>
        /// Parametr závislosti
        /// upravuje ?poissonovu? krivku kolem -0.5..0.5 golu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        public double Rho { get; set; } = -0.148236387553165;

        /// <summary>
        /// Dalsi parametr 
        /// </summary>
        public double Mi { get; set; }

        /// <summary>
        /// Dalsi parametr 
        /// </summary>
        public double Lambda { get; set; }

        /// <summary>
        /// Dalsi parametr 
        /// </summary>
        public double P { get; set; }

        /// <summary>
        /// Výhoda domácího
        /// Tým hrající doma má většinou převahu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// rozsah 1..2
        /// </summary>
        public double Gamma { get; set; } = 1.50951729586059;

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

        /// <summary>
        /// Počet golů do pravděpodobností
        /// </summary>
        public int PropLength { get; set; } = 10;

        /// <summary>
        /// Datum predikce
        /// </summary>
        public DateTime DatePredict { get; set; }

        public SolverTypeEnum Type { get; set; }

        public int Id { get; set; }

        public DateTime? KsiStart { get; set; }

        #endregion

        /// <summary>
        /// Konstruktor
        /// </summary>
        /// <param name="matches">zápasy</param>
        /// <param name="teams">týmy</param>
        public DixonManager(IList<GameMatch> matches, IList<GameTeam> teams)
        {
            Matches = matches;
            Teams = teams;
            Thetas = new List<double>();
        }

        /// <summary>
        /// Poisson
        /// </summary>
        /// <param name="x">pocet golu</param>
        /// <param name="m">utok/obrana tymu</param>
        /// <returns></returns>
        public static double Poisson(int x, double m) { return Math.Exp(-1 * m) * Math.Pow(m, x) / (MethodExtensions.Factorial(x) * 1.0); }

        /// <summary>
        /// Suma všech pravděpodobností v logaritmu
        /// </summary>
        /// <returns></returns>
        public virtual double SumMaximumLikehood()
        {
            var matches = Matches;

            // filtr na ksi
            if (KsiStart.HasValue)
            {
                matches = Matches.Where(x => x.DateStart >= KsiStart).ToList();
            }

            double ret = 0;
            foreach (var match in matches)
            {
                if (match.HomeScore < match.AwayScore)
                {
                    ret += Math.Log(AwayProbability(match));
                }
                else if (match.HomeScore > match.AwayScore)
                {
                    ret += Math.Log(HomeProbability(match));
                }
                else
                {
                    ret += Math.Log(DrawProbability(match));
                }
            }

            MaximumLikehoodValue = ret;
            return ret;
        }

        public virtual double AwayProbability(GameMatch match)
        {
            double ret = 0;
            var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);

            if (homeTeam == null || awayTeam == null)
            {
                throw new Exception(string.Format("match: {0}\t{1}-{2}\t{3}-{4}", match.Id, match.HomeTeamId, match.AwayTeamId, homeTeam == null, awayTeam == null));
            }

            for (int h = 0; h < PropLength; h++)
            {
                for (int a = (h + 1); a < PropLength; a++)
                {
                    ret +=
                        Poisson(h, LambdaHome(homeTeam, awayTeam))
                        * Poisson(a, LambdaAway(homeTeam, awayTeam));
                }
            }

            return ret;
        }

        public virtual double DrawProbability(GameMatch match)
        {
            double ret = 0;
            var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);

            if (homeTeam == null || awayTeam == null)
            {
                throw new Exception(string.Format("match: {0}\t{1}-{2}\t{3}-{4}", match.Id, match.HomeTeamId, match.AwayTeamId, homeTeam == null, awayTeam == null));
            }

            for (int i = 0; i < PropLength; i++)
            {
                ret += Poisson(i, LambdaHome(homeTeam, awayTeam))
                    * Poisson(i, LambdaAway(homeTeam, awayTeam));
            }

            return ret;
        }

        public virtual double HomeProbability(GameMatch match)
        {
            double ret = 0;
            var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);

            if (homeTeam == null || awayTeam == null)
            {
                throw new Exception(string.Format("match: {0}\t{1}-{2}\t{3}-{4}", match.Id, match.HomeTeamId, match.AwayTeamId, homeTeam == null, awayTeam == null));
            }

            for (int a = 0; a < PropLength; a++)
            {
                for (int h = (a + 1); h < PropLength; h++)
                {
                    ret +=
                        Poisson(h, LambdaHome(homeTeam, awayTeam))
                        * Poisson(a, LambdaAway(homeTeam, awayTeam));
                }
            }

            return ret;
        }

        protected virtual double LambdaHome(GameTeam homeTeam, GameTeam awayTeam)
        {
            return homeTeam.HomeAttack * awayTeam.AwayAttack * Gamma;
        }

        protected virtual double LambdaAway(GameTeam homeTeam, GameTeam awayTeam)
        {
            return awayTeam.HomeAttack * homeTeam.AwayAttack;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Ksi;{0}\n", Ksi)
                .AppendFormat("Gamma;{0}\n", Gamma)
                .AppendFormat("Mi;{0}\n", Mi)
                .AppendFormat("P;{0}\n", P)
                .AppendFormat("Lambda;{0}\n", Lambda)
                .AppendFormat("Rho;{0}\n", Rho)
                .AppendFormat("LastElapsed;{0}\n", LastElapsed)
                .AppendFormat("MatchCount;{0}\n", Matches.Count)
                .AppendFormat("Summary;{0}\n", Summary)
                .AppendFormat("MaximumLikehoodValue;{0}\n", MaximumLikehoodValue)
                .AppendLine()
                .AppendLine("Id;DisplayName;HomeAttack;AwayAttack;Gamma");

            foreach (var team in Teams)
            {
                sb.AppendFormat("{3};{0};{1};{2};{4}\n", team.DisplayName, team.HomeAttack, team.AwayAttack, team.Id, team.Gamma);
            }

            if (Thetas.Count > 0)
            {
                sb.AppendLine().AppendLine("n;Theta");
                for (int i = 0; i < Thetas.Count; i++)
                {
                    sb.AppendFormat("{0};{1}\n", i, Thetas[i]);
                }
            }

            return sb.AppendLine().ToString();
        }
    }
}
