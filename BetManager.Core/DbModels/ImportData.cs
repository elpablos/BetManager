using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.DbModels
{
    public partial class ImportData
    {
        public DateTime? Date { get; set; }

        [StringLength(255)]
        public string SportName { get; set; }

        [StringLength(255)]
        public string SportSlug { get; set; }

        public int? SportId { get; set; }

        [StringLength(255)]
        public string TournamentName { get; set; }

        [StringLength(255)]
        public string TournamentSlug { get; set; }

        public int? TournamentId { get; set; }

        public int? TournamentUniqueId { get; set; }

        [StringLength(255)]
        public string CategoryName { get; set; }

        [StringLength(255)]
        public string CategorySlug { get; set; }

        public int? CategoryId { get; set; }

        [StringLength(255)]
        public string SeasonName { get; set; }

        [StringLength(255)]
        public string SeasonSlug { get; set; }

        public long? SeasonId { get; set; }

        [StringLength(255)]
        public string SeasonYear { get; set; }

        public long? EventId { get; set; }

        [StringLength(255)]
        public string EventCustomId { get; set; }

        public int? EventFirstToServe { get; set; }

        public bool? EventHasDraw { get; set; }

        public int? EventWinnerCode { get; set; }

        [StringLength(255)]
        public string EventName { get; set; }

        [StringLength(255)]
        public string EventSlug { get; set; }

        [StringLength(255)]
        public string EventStartDate { get; set; }

        public TimeSpan? EventStartTime { get; set; }

        [StringLength(50)]
        public string EventChanges { get; set; }

        public int? StatusCode { get; set; }

        [StringLength(255)]
        public string StatusType { get; set; }

        [StringLength(255)]
        public string StatusDescription { get; set; }

        public long? HomeTeamId { get; set; }

        [StringLength(255)]
        public string HomeTeamName { get; set; }

        [StringLength(255)]
        public string HomeTeamSlug { get; set; }

        [StringLength(255)]
        public string HomeTeamGender { get; set; }

        public int? HomeScoreCurrent { get; set; }

        public int? HomeScorePeriod1 { get; set; }

        public int? HomeScorePeriod2 { get; set; }

        public int? HomeScorePeriod3 { get; set; }

        public int? HomeScoreNormaltime { get; set; }

        public int? HomeScoreOvertime { get; set; }

        public int? HomeScorePenalties { get; set; }

        public long? AwayTeamId { get; set; }

        [StringLength(255)]
        public string AwayTeamName { get; set; }

        [StringLength(255)]
        public string AwayTeamSlug { get; set; }

        [StringLength(255)]
        public string AwayTeamGender { get; set; }

        public int? AwayScoreCurrent { get; set; }

        public int? AwayScorePeriod1 { get; set; }

        public int? AwayScorePeriod2 { get; set; }

        public int? AwayScorePeriod3 { get; set; }

        public int? AwayScoreNormaltime { get; set; }

        public int? AwayScoreOvertime { get; set; }

        public int? AwayScorePenalties { get; set; }

        public long? OddsRegularFirstSourceId { get; set; }

        [StringLength(255)]
        public string OddsRegularFirstValue { get; set; }

        public bool? OddsRegularFirstWining { get; set; }

        public long? OddsRegularXSourceId { get; set; }

        [StringLength(255)]
        public string OddsRegularXValue { get; set; }

        public bool? OddsRegularXWining { get; set; }

        public long? OddsRegularSecondSourceId { get; set; }

        [StringLength(255)]
        public string OddsRegularSecondValue { get; set; }

        public bool? OddsRegularSecondWining { get; set; }

        public long? OddsDoubleChangeFirstXSourceId { get; set; }

        [StringLength(255)]
        public string OddsDoubleChangeFirstXValue { get; set; }

        public bool? OddsDoubleChangeFirstXWining { get; set; }

        public long? OddsDoubleChangeXSecondSourceId { get; set; }

        [StringLength(255)]
        public string OddsDoubleChangeXSecondValue { get; set; }

        public bool? OddsDoubleChangeXSecondWining { get; set; }

        public long? OddsDoubleChangeFirstSecondSourceId { get; set; }

        [StringLength(255)]
        public string OddsDoubleChangeFirstSecondValue { get; set; }

        public bool? OddsDoubleChangeFirstSecondWining { get; set; }

        [Key]
        [Column(Order = 0)]
        public int ID { get; set; }

        [Key]
        [Column(Order = 1)]
        public bool IsProcessed { get; set; }
    }
}
