using System;
using System.Text;

namespace Prediction.Core.Models
{
    /// <summary>
    /// Model pro zápas
    /// </summary>
    public class GameMatch
    {
        /// <summary>
        /// Id zápasu
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Domácí - Id týmu
        /// </summary>
        public int HomeTeamId { get; set; }

        /// <summary>
        /// Domácí - tým
        /// </summary>
        public GameTeam HomeTeam { get; set; }

        /// <summary>
        /// Hosté - Id týmu
        /// </summary>
        public int AwayTeamId { get; set; }

        /// <summary>
        /// Hosté - tým
        /// </summary>
        public GameTeam AwayTeam { get; set; }

        /// <summary>
        /// Domácí - skóre
        /// </summary>
        public int HomeScore { get; set; }

        /// <summary>
        /// Hosté - skóre
        /// </summary>
        public int AwayScore { get; set; }

        /// <summary>
        /// Datum zápasu
        /// </summary>
        public DateTime DateStart { get; set; }

        /// <summary>
        /// Dny
        /// </summary>
        public int Days { get; set; }

        /// <summary>
        /// Hodnota funkce času
        /// </summary>
        public double TimeValue { get; set; }

        public double HomeProp { get; set; }

        public double DrawProp { get; set; }

        public double AwayProp { get; set; }

        #region Overrided methods

        public override bool Equals(object obj)
        {
            if (obj is GameMatch)
            {
                var team = (GameMatch)obj;
                if (team.Id == Id) return true;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Id).Append(";")
                .Append(HomeTeamId).Append(";")
                .Append(AwayTeamId).Append(";")
                .Append(HomeScore).Append(";")
                .Append(AwayScore).Append(";")
                .Append(DateStart).Append(";")
                .Append(HomeProp).Append(";")
                .Append(DrawProp).Append(";")
                .Append(AwayProp).Append(";");

            return sb.ToString();
        }

        #endregion
    }
}
