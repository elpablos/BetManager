namespace BetManager.EF
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BM_OddsRegular
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long ID_Event { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        public long? FirstId { get; set; }

        public decimal? FirstValue { get; set; }

        public bool? HasFirstWin { get; set; }

        public long? XId { get; set; }

        public decimal? XValue { get; set; }

        public bool? HasXWin { get; set; }

        public long? SecondId { get; set; }

        public decimal? SecondValue { get; set; }

        public bool? HasSecondWin { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public virtual BM_Event BM_Event { get; set; }
    }
}
