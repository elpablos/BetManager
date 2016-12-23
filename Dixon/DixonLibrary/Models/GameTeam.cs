namespace Dixon.Library.Models
{
    /// <summary>
    /// Herní tým
    /// </summary>
    public class GameTeam
    {
        /// <summary>
        /// Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Název týmu
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Útok doma
        /// </summary>
        public double HomeAttack { get; set; } = 1.0;

        /// <summary>
        /// Útok venku
        /// </summary>
        public double AwayAttack { get; set; } = 1.0;

        #region Overrided methods

        public override bool Equals(object obj)
        {
            if (obj is GameTeam)
            {
                var team = (GameTeam)obj;
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
