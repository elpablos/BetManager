namespace BetManager.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BM_Event
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID { get; set; }

        [Required]
        [StringLength(255)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(255)]
        public string Slug { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(255)]
        public string CustomId { get; set; }

        public bool? FirstToServe { get; set; }

        public bool? HasDraw { get; set; }

        public int? WinnerCode { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime? Changes { get; set; }

        public long ID_HomeTeam { get; set; }

        public long ID_AwayTeam { get; set; }

        public long ID_Tournament { get; set; }

        public long? ID_Season { get; set; }

        public long ID_Category { get; set; }

        public int ID_Status { get; set; }

        [StringLength(255)]
        public string StatusDescription { get; set; }

        public int? HomeScoreCurrent { get; set; }

        public int? AwayScoreCurrent { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public virtual BM_Category BM_Category { get; set; }

        public virtual BM_Season BM_Season { get; set; }

        public virtual BM_Status BM_Status { get; set; }

        public virtual BM_Team BM_Team { get; set; }

        public virtual BM_Team BM_Team1 { get; set; }

        public virtual BM_Tournament BM_Tournament { get; set; }

        public virtual BM_Score BM_Score { get; set; }

        public virtual BM_OddsRegular BM_OddsRegular { get; set; }
    }
}
