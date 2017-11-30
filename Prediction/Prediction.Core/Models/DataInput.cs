using System;

namespace Prediction.Core.Models
{
    public class DataInput
    {
        public int Id { get; set; }

        public int HomeTeamId { get; set; }

        public string HomeTeam { get; set; }

        public int AwayTeamId { get; set; }

        public string AwayTeam { get; set; }

        public int HomeScore { get; set; }

        public int AwayScore { get; set; }

        public DateTime DateStart { get; set; }
    }
}
