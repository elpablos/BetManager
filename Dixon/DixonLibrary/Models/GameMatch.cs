using System;

namespace Dixon.Library.Models
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

        #endregion
    }
}
