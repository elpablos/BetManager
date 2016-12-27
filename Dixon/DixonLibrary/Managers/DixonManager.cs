using Dixon.Library.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Dixon.Library.Managers
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
        /// Výhoda domácího
        /// Tým hrající doma má většinou převahu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// rozsah 1..2
        /// </summary>
        public double Gama { get; set; } = 1.50951729586059;

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

        public int PropLength { get; set; } = 10;

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
        }

        public static Func<int, int> Factorial = x => x < 0 ? -1 : x == 1 || x == 0 ? 1 : x * Factorial(x - 1);

        /// <summary>
        /// Poisson
        /// </summary>
        /// <param name="x">pocet golu</param>
        /// <param name="m">utok/obrana tymu</param>
        /// <returns></returns>
        public static double Poisson(int x, double m) { return Math.Exp(-1 * m) * Math.Pow(m, x) / (Factorial(x) * 1.0); }

        /// <summary>
        /// Suma všech pravděpodobností v logaritmu
        /// </summary>
        /// <returns></returns>
        public virtual double SumMaximumLikehood()
        {
            double ret = 0;
            foreach (var match in Matches)
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

        public virtual double HomeProbability(GameMatch match)
        {
            double ret = 0;
            var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);

            for (int h = 0; h < PropLength; h++)
            {
                for (int a = (h + 1); a < PropLength; a++)
                {
                    ret +=
                        Poisson(h, homeTeam.HomeAttack * awayTeam.AwayAttack * Gama)
                        * Poisson(a, awayTeam.HomeAttack * homeTeam.AwayAttack);
                }
            }

            return ret;
        }

        public virtual double DrawProbability(GameMatch match)
        {
            double ret = 0;
            var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);

            for (int i = 0; i < PropLength; i++)
            {
                ret += Poisson(i, homeTeam.HomeAttack * awayTeam.AwayAttack * Gama)
                    * Poisson(i, awayTeam.HomeAttack * homeTeam.AwayAttack);
            }

            return ret;
        }

        public virtual double AwayProbability(GameMatch match)
        {
            double ret = 0;
            var homeTeam = Teams.FirstOrDefault(x => x.Id == match.HomeTeamId);
            var awayTeam = Teams.FirstOrDefault(x => x.Id == match.AwayTeamId);

            for (int a = 0; a < PropLength; a++)
            {
                for (int h = (a+1); h < PropLength; h++)
                {
                    ret += 
                        Poisson(h, homeTeam.HomeAttack * awayTeam.AwayAttack * Gama)
                        * Poisson(a, awayTeam.HomeAttack * homeTeam.AwayAttack);
                }
            }

            return ret;
        }

        /// <summary>
        /// Suma věrohodnostních funkcí nad zápasem
        /// </summary>
        /// <param name="dateActual"></param>
        /// <returns></returns>
        public virtual double Sum(DateTime dateActual)
        {
            double sum = 0;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            foreach (var match in Matches)
            {
                sum += Calculate(match, dateActual);
            }
            watch.Stop();
            LastElapsed = watch.Elapsed;
            return sum;
        }

        /// <summary>
        /// Funkce závislost Tau
        /// </summary>
        /// <param name="match">zapas</param>
        /// <param name="lambda">utok domacich</param>
        /// <param name="mu">utok hosti</param>
        /// <returns>zavislost na tau</returns>
        protected virtual double DependenceTau(GameMatch match, double lambda, double mu)
        {
            double ret = 1;
            if (match.HomeScore == 0 && match.AwayScore == 0)
                ret = 1 - lambda * mu * Rho;
            else if (match.HomeScore == 0 && match.AwayScore == 1)
                ret = 1 + lambda * Rho;
            else if (match.HomeScore == 1 && match.AwayScore == 0)
                ret = 1 + mu * Rho;
            else if (match.HomeScore == 1 && match.AwayScore == 1)
                ret = 1 - Rho;
            return ret;
        }

        /// <summary>
        /// Výpočet logaritmické věrohodnostní funkce
        /// (𝜙(𝑡 − 𝑡𝑘) ∙ (ln 𝜏𝜆𝑘,𝜇𝑘(𝑥𝑘, 𝑦𝑘) + 𝑥𝑘 ∙ ln 𝜆𝑘 − 𝜆𝑘 + 𝑦𝑘 ∙ ln 𝜇𝑘 − 𝜇𝑘))
        /// 𝜆𝑘          parametr určující počet gólů domácích,
        /// 𝜇𝑘          parametr určující počet gólů hostů,
        /// 𝜏           funkce vyjadřující závislost mezi 𝑋𝑖𝑗 a 𝑌𝑖𝑗,
        /// 𝑥𝑘          počet gólů domácího týmu 𝑖 v zápase 𝑘,
        /// 𝑦𝑘          počet gólů hostujícího týmu 𝑗 v zápase 𝑘,
        /// 𝜙(𝑡 − 𝑡𝑘)   funkce času
        /// </summary>
        /// <param name="match"></param>
        /// <param name="dateActual"></param>
        /// <returns></returns>
        protected virtual double Calculate(GameMatch match, DateTime dateActual)
        {
            // home-attack
            var lambda = (match.HomeTeam.HomeAttack * match.AwayTeam.AwayAttack * Gama);
            // away-attack
            var mu = (match.HomeTeam.AwayAttack * match.AwayTeam.HomeAttack);

            var result =
                // casova fce
                match.TimeFunc(dateActual, Ksi)
                // ln fce zavislosti tau
                * (Math.Log(DependenceTau(match, lambda, mu))
                // pocet golu domaciho + ln predikce golu domaciho
                + match.HomeScore * Math.Log(lambda)
                // minus predikce golu domaciho
                - lambda
                // pocet golu hosti + ln predikce golu hosti
                + match.AwayScore * Math.Log(mu)
                // minus predikce golu hosti
                - mu
                );
            return result;
        }


        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendFormat("Gama;{0}\n", Gama)
                .AppendFormat("Rho;{0}\n", Rho)
                .AppendFormat("Ksi;{0}\n", Ksi)
                .AppendFormat("LastElapsed;{0}\n", LastElapsed)
                .AppendFormat("MatchCount;{0}\n", Matches.Count)
                .AppendFormat("Summary;{0}\n", Summary)
                .AppendFormat("MaximumLikehoodValue;{0}\n", MaximumLikehoodValue)
                .AppendLine()
                .AppendLine("DisplayName;HomeAttack;AwayAttack");

            foreach (var team in Teams)
            {
                sb.AppendFormat("{0};{1};{2}\n", team.DisplayName, team.HomeAttack, team.AwayAttack);
            }

            return sb.AppendLine().ToString();
        }
    }
}
