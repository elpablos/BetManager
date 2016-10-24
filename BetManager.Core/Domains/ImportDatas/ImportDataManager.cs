using BetManager.Core.DbModels;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Domains.ImportDatas
{
    public class ImportDataManager : IImportDataManager
    {
        private const string SqlInsert = @"insert into BM_ImportData ([Date], [SportName], [SportSlug], [SportId], [TournamentName], [TournamentSlug], [TournamentId], [TournamentUniqueId], [CategoryName], [CategorySlug], [CategoryId], [SeasonName], [SeasonSlug], [SeasonId], [SeasonYear], [EventId], [EventCustomId], [EventFirstToServe], [EventHasDraw], [EventWinnerCode], [EventName], [EventSlug], [EventStartDate], [EventStartTime], [EventChanges], [StatusCode], [StatusType], [StatusDescription], [HomeTeamId], [HomeTeamName], [HomeTeamSlug], [HomeTeamGender], [HomeScoreCurrent], [HomeScorePeriod1], [HomeScorePeriod2], [HomeScorePeriod3], [HomeScoreNormaltime], [HomeScoreOvertime], [HomeScorePenalties], [AwayTeamId], [AwayTeamName], [AwayTeamSlug], [AwayTeamGender], [AwayScoreCurrent], [AwayScorePeriod1], [AwayScorePeriod2], [AwayScorePeriod3], [AwayScoreNormaltime], [AwayScoreOvertime], [AwayScorePenalties], [OddsRegularFirstSourceId], [OddsRegularFirstValue], [OddsRegularFirstWining], [OddsRegularXSourceId], [OddsRegularXValue], [OddsRegularXWining], [OddsRegularSecondSourceId], [OddsRegularSecondValue], [OddsRegularSecondWining], [OddsDoubleChangeFirstXSourceId], [OddsDoubleChangeFirstXValue], [OddsDoubleChangeFirstXWining], [OddsDoubleChangeXSecondSourceId], [OddsDoubleChangeXSecondValue], [OddsDoubleChangeXSecondWining], [OddsDoubleChangeFirstSecondSourceId], [OddsDoubleChangeFirstSecondValue], [OddsDoubleChangeFirstSecondWining], [IsProcessed])
values(@Date, @SportName, @SportSlug, @SportId, @TournamentName, @TournamentSlug, @TournamentId, @TournamentUniqueId, @CategoryName, @CategorySlug, @CategoryId, @SeasonName, @SeasonSlug, @SeasonId, @SeasonYear, @EventId, @EventCustomId, @EventFirstToServe, @EventHasDraw, @EventWinnerCode, @EventName, @EventSlug, @EventStartDate, @EventStartTime, @EventChanges, @StatusCode, @StatusType, @StatusDescription, @HomeTeamId, @HomeTeamName, @HomeTeamSlug, @HomeTeamGender, @HomeScoreCurrent, @HomeScorePeriod1, @HomeScorePeriod2, @HomeScorePeriod3, @HomeScoreNormaltime, @HomeScoreOvertime, @HomeScorePenalties, @AwayTeamId, @AwayTeamName, @AwayTeamSlug, @AwayTeamGender, @AwayScoreCurrent, @AwayScorePeriod1, @AwayScorePeriod2, @AwayScorePeriod3, @AwayScoreNormaltime, @AwayScoreOvertime, @AwayScorePenalties, @OddsRegularFirstSourceId, @OddsRegularFirstValue, @OddsRegularFirstWining, @OddsRegularXSourceId, @OddsRegularXValue, @OddsRegularXWining, @OddsRegularSecondSourceId, @OddsRegularSecondValue, @OddsRegularSecondWining, @OddsDoubleChangeFirstXSourceId, @OddsDoubleChangeFirstXValue, @OddsDoubleChangeFirstXWining, @OddsDoubleChangeXSecondSourceId, @OddsDoubleChangeXSecondValue, @OddsDoubleChangeXSecondWining, @OddsDoubleChangeFirstSecondSourceId, @OddsDoubleChangeFirstSecondValue, @OddsDoubleChangeFirstSecondWining, @IsProcessed)";

        private const string SqlTruncate = @"truncate table BM_ImportData";

        public virtual int Insert(ImportData data)
        {
            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute(SqlInsert, data, commandType: CommandType.Text);
                conn.Close();
            }
            return ret;
        }

        public virtual int InsertBulk(IEnumerable<ImportData> rows)
        {
            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute(SqlInsert, rows, commandType: CommandType.Text);
                conn.Close();
            }
            return ret;
        }

        public virtual ImportData GetById(int id)
        {
            ImportData data = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                data = conn.Query<ImportData>("select * from BM_ImportData where ID=@id", new { @id = id }, commandType: CommandType.Text).FirstOrDefault();
                conn.Close();
            }

            return data;
        }

        public virtual ICollection<ImportData> GetAll()
        {
            ICollection<ImportData> data = null;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                data = conn.Query<ImportData>("select top 1000 * from BM_ImportData").ToList();
                conn.Close();
            }
            return data;
        }

        public virtual int Truncate()
        {
            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute(SqlTruncate, null, commandType: CommandType.Text);
                conn.Close();
            }
            return ret;
        }

        public virtual int ImportData()
        {
            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute("BM_ImportData_IMPORT", null, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return ret;
        }

        public virtual int ImportClear()
        {
            int ret = -1;
            using (var conn = ConnectionFactory.GetConnection("DbModel"))
            {
                ret = conn.Execute("BM_ImportData_OLD", null, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return ret;
        }
    }
}
