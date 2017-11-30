namespace Prediction.Core.Models
{
    public class GamePredictionTeam
    {
        /// <summary>
        /// Id predikce týmu
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id predikce
        /// </summary>
        public int PredictionId { get; set; }

        /// <summary>
        /// Útok doma
        /// </summary>
        public double HomeAttack { get; set; }

        /// <summary>
        /// Útok venku
        /// </summary>
        public double AwayAttack { get; set; }
    }
}
