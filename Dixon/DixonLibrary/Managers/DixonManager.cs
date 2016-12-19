using Dixon.Library.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixon.Library.Managers
{
    /// <summary>
    /// Výpočet zápasů dle Dixon-Coles
    /// </summary>
    public class DixonManager : IDixonManager
    {
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
        public double Epsilon { get; set; } = 0.0065 / 3.5;

        /// <summary>
        /// Parametr závislosti
        /// upravuje ?poissonovu? krivku kolem 0..1 golu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        public double Rho { get; set; } = 0.11;

        /// <summary>
        /// Výhoda domácího
        /// Tým hrající doma má většinou převahu
        /// PARAMETR SE BUDE MENIT DLE MAXIMALIZACNI FCE!
        /// </summary>
        public double Gama { get; set; } = 1.55;

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

        /// <summary>
        /// Suma věrohodnostních funkcí nad zápasem
        /// </summary>
        /// <param name="dateActual"></param>
        /// <returns></returns>
        public virtual double Sum(DateTime dateActual)
        {
            double sum = 0;
            foreach (var match in Matches)
            {
                sum += Calculate(match, dateActual);
            }
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

            return
                // casova fce
                match.TimeFunc(dateActual, Epsilon)
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
        }
    }
}
