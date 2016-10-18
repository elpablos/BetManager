namespace BetManager.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BM_Score
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID_Event { get; set; }

        public int HomeScoreCurrent { get; set; }

        public int? HomeScorePeriod1 { get; set; }

        public int? HomeScorePeriod2 { get; set; }

        public int? HomeScorePeriod3 { get; set; }

        public int? HomeScoreNormaltime { get; set; }

        public int? HomeScoreOvertime { get; set; }

        public int? HomeScorePenalties { get; set; }

        public int AwayScoreCurrent { get; set; }

        public int? AwayScorePeriod1 { get; set; }

        public int? AwayScorePeriod2 { get; set; }

        public int? AwayScorePeriod3 { get; set; }

        public int? AwayScoreNormaltime { get; set; }

        public int? AwayScoreOvertime { get; set; }

        public int? AwayScorePenalties { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public virtual BM_Event BM_Event { get; set; }
    }
}
