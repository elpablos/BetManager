using System;

namespace BetManager.Core.DbModels.Predictions
{
    public class Prediction
    {
        public int ID { get; set; }
        public int ID_Tournament { get; set; }
        public DateTime DatePredict { get; set; }
        public double Ksi { get; set; }
        public double Gamma { get; set; }
        public double Summary { get; set; }
        public double LikehoodValue { get; set; }
        public DateTime DateCreated { get; set; }
        public long Elapsed { get; set; }
        public string Description { get; set; }
    }
}
