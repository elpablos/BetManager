using BetManager.Core.DbModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Processors
{
    public class ImportDataProcessor : IImportDataProcessor
    {
        private const string URLPATTERN = "http://www.sofascore.com/{0}/{1}/{2}/json";
        private const string USERAGENT = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.143 Safari/537.36";

        public string DownloadData(string sport, string category, DateTime date)
        {
            string ret = string.Empty;

            string url = string.Format(URLPATTERN, sport, category, date.ToString("yyyy-MM-dd"));

            using (var client = new WebClient())
            {
                client.Headers[HttpRequestHeader.UserAgent] = USERAGENT;
                client.Encoding = Encoding.UTF8;

                ret = client.DownloadString(url);
            }

            return ret;
        }

        public IEnumerable<ImportData> ProcessData(string stringData)
        {
            dynamic data = JObject.Parse(stringData);
            var colls = new List<ImportData>();
            foreach (var i in data.sportItem.tournaments)
            {
                foreach (var e in i.events)
                {
                    var item = new ImportData();

                    // pridam vlastnosti
                    item.Date = data.@params.date;
                    item.SportName = data.sportItem.sport.name;
                    item.SportSlug = data.sportItem.sport.slug;
                    item.SportId = data.sportItem.sport.id;

                    item.TournamentName = i.tournament.name;
                    item.TournamentSlug = i.tournament.slug;
                    item.TournamentId = i.tournament.id;
                    item.TournamentUniqueId = i.tournament.uniqueId;

                    item.CategoryName = i.category.name;
                    item.CategorySlug = i.category.slug;
                    item.CategoryId = i.category.id;
                    if (i.season is JObject)
                    {
                        item.SeasonName = i.season.name;
                        item.SeasonSlug = i.season.slug;
                        item.SeasonId = i.season.id;
                        item.SeasonYear = i.season.year;
                    }

                    // event
                    item.EventId = e.id;
                    item.EventCustomId = e.customId;
                    item.EventFirstToServe = e.firstToServe;
                    item.EventHasDraw = e.hasDraw;
                    item.EventWinnerCode = e.winnerCode;
                    item.EventName = e.name;
                    item.EventSlug = e.slug;
                    item.EventStartDate = e.formatedStartDate;
                    item.EventStartTime = e.startTime;
                    item.EventChanges = e.changes.changeDate;

                    item.StatusCode = e.status.code;
                    item.StatusType = e.status.type;
                    item.StatusDescription = e.statusDescription;

                    // team
                    item.HomeTeamId = e.homeTeam.id;
                    item.HomeTeamName = e.homeTeam.name;
                    item.HomeTeamSlug = e.homeTeam.slug;
                    item.HomeTeamGender = e.homeTeam.gender;
                    item.HomeScoreCurrent = e.homeScore.current;
                    item.HomeScorePeriod1 = e.homeScore.period1;
                    item.HomeScorePeriod2 = e.homeScore.period2;
                    item.HomeScorePeriod3 = e.homeScore.period3;
                    item.HomeScoreNormaltime = e.homeScore.normaltime;
                    item.HomeScoreOvertime = e.homeScore.overtime;
                    item.HomeScorePenalties = e.homeScore.penalties;

                    item.AwayTeamId = e.awayTeam.id;
                    item.AwayTeamName = e.awayTeam.name;
                    item.AwayTeamSlug = e.awayTeam.slug;
                    item.AwayTeamGender = e.awayTeam.gender;
                    item.AwayScoreCurrent = e.awayScore.current;
                    item.AwayScorePeriod1 = e.awayScore.period1;
                    item.AwayScorePeriod2 = e.awayScore.period2;
                    item.AwayScorePeriod3 = e.awayScore.period3;
                    item.AwayScoreNormaltime = e.awayScore.normaltime;
                    item.AwayScoreOvertime = e.awayScore.overtime;
                    item.AwayScorePenalties = e.awayScore.penalties;

                    if (e.odds?.fullTimeOdds?.regular != null)
                    {
                        foreach (var odd in e.odds.fullTimeOdds.regular)
                        {
                            //  fullTimeOdds
                            if (((JProperty)odd).Name == "1")
                            {
                                dynamic oddz = ((dynamic)((JProperty)odd).Value);
                                item.OddsRegularFirstSourceId = oddz.sourceId;
                                item.OddsRegularFirstValue = oddz.decimalValue;
                                item.OddsRegularFirstWining = oddz.winning;
                            }
                            //
                            else if (((JProperty)odd).Name == "X")
                            {
                                dynamic oddz = ((dynamic)((JProperty)odd).Value);
                                item.OddsRegularXSourceId = oddz.sourceId;
                                item.OddsRegularXValue = oddz.decimalValue;
                                item.OddsRegularXWining = oddz.winning;
                            }
                            else if (((JProperty)odd).Name == "2")
                            {
                                dynamic oddz = ((dynamic)((JProperty)odd).Value);
                                item.OddsRegularSecondSourceId = oddz.sourceId;
                                item.OddsRegularSecondValue = oddz.decimalValue;
                                item.OddsRegularSecondWining = oddz.winning;
                            }
                            //  doubleChanceOdds
                            else if (((JProperty)odd).Name == "1X")
                            {
                                dynamic oddz = ((dynamic)((JProperty)odd).Value);
                                item.OddsDoubleChangeFirstXSourceId = oddz.sourceId;
                                item.OddsDoubleChangeFirstXValue = oddz.decimalValue;
                                item.OddsDoubleChangeFirstXWining = oddz.winning;
                            }
                            else if (((JProperty)odd).Name == "X2")
                            {
                                dynamic oddz = ((dynamic)((JProperty)odd).Value);
                                item.OddsDoubleChangeXSecondSourceId = oddz.sourceId;
                                item.OddsDoubleChangeXSecondValue = oddz.decimalValue;
                                item.OddsDoubleChangeXSecondWining = oddz.winning;
                            }
                            else if (((JProperty)odd).Name == "12")
                            {
                                dynamic oddz = ((dynamic)((JProperty)odd).Value);
                                item.OddsDoubleChangeFirstSecondSourceId = oddz.sourceId;
                                item.OddsDoubleChangeFirstSecondValue = oddz.decimalValue;
                                item.OddsDoubleChangeFirstSecondWining = oddz.winning;
                            }
                        }
                    }

                    colls.Add(item);
                }
            }

            return colls;
        }
    }
}
