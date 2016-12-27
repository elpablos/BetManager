namespace BetManager.Core.DbModels.Predictions
{
    public class PredictionTeam
    {
        public int ID { get; set; }
        public int ID_Team { get; set; }
        public double Attack { get; set; }
        public double Defence { get; set; }
        public int ID_Prediction { get; set; }
    }
}
