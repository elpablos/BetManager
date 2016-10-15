using BetManager.Core.Exports.SofascoreExport;
using BetManager.Core.SofaScoreBasicExport.Teams;
using BetManager.Core.SofaScoreBasicExport.Tournaments;
using BetManager.Core.Utils;
using BetManager.Core.Webs;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetManager.Core.Processors
{
    public class SofascoreBasicDataProcessor : BaseProcessor
    {
        private bool isTest = false;

        public SofascoreBasicDataProcessor(IWebDownloader webDownloader, string url)
            : base(webDownloader, url)
        {
            webDownloader.TryTempData = true;
        }

        public override string Process()
        {
            string ret = null;

            IEnumerable<SofaScoreTeam> teams = new List<SofaScoreTeam>();

            //teams = teams.Union(ParseSportData2("football"));
            teams = teams.Union(ParseSportData2("tennis"));
            //teams = teams.Union(ParseSportData2("ice-hockey"));
            //teams = teams.Union(ParseSportData2("basketball"));

            try
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Save json.."), "Processor");
                // pojistka
                string x = JsonConvert.SerializeObject(teams.ToArray());
                System.IO.File.WriteAllText("Data/SofaScoreBasicData.json", x);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Save json failed {0}", e.Message), "Processor");
            }

            System.Diagnostics.Trace.WriteLine(string.Format("Save XML.."), "Processor");
            ret = XmlHelper.Serializable<SofaScoreTeam[]>(teams.ToArray());
            return ret;
        }

        public IEnumerable<SofaScoreTeam> ParseSportData2(string sportSlug)
        {
            List<SofaScoreTeam> teams = new List<SofaScoreTeam>();
            List<SofaScoreTournament> tournamentLists = new List<SofaScoreTournament>();

            string categoryUrl = string.Format("http://www.sofascore.com/esi/categories/{0}", sportSlug);

            var categories = ParseCategories(categoryUrl);
            foreach (var category in categories)
            {
                // Href - např. /football/albania
                string tournamentUrl = string.Format("http://www.sofascore.com/list/category{0}/tournaments/json", category.Href);
                var tournament = ParseTournament(tournamentUrl);
                tournamentLists.Add(tournament);
            }

            foreach (var tournamentList in tournamentLists)
            {
                // http://www.sofascore.com/tournament/2891//standings/tables/json
                // http://www.sofascore.com/u-tournament/720/season//json

                IEnumerable<SofaScoreBasicExport.Tournaments.Tournament> list = new List<SofaScoreBasicExport.Tournaments.Tournament>();
                if (tournamentList.tournaments.list != null)
                    list = list.Union(tournamentList.tournaments.list);
                if (tournamentList.tournaments.Sections != null)
                    list = list.Union(tournamentList.tournaments.Sections.SelectMany(x => x.Value.tournaments));

                foreach (var tournament in list)
                {
                    string teamsUrl = string.Format("http://www.sofascore.com/u-tournament/{0}/season/{1}/json", tournament.uniqueId, string.Empty);
                    try
                    {
                        var team = ParseTeam(teamsUrl);
                        teams.Add(team);

                        if (isTest)
                            break;
                    }
                    catch (Exception)
                    {
                        System.Diagnostics.Trace.WriteLine(string.Format("Parse problem", teamsUrl), "Processor");
                        System.Threading.Thread.Sleep(2000);
                    }
                }
            }

            return teams;
        }

        public IEnumerable<SofaScoreTeam> ParseSportData(string sportSlug)
        {
            List<SofaScoreTeam> teams = new List<SofaScoreTeam>();

            //teams.Add(ParseTeam("http://www.sofascore.com/tournament/846//standings/tables/json"));
            //return teams;

            List<SofaScoreTournament> tournamentLists = new List<SofaScoreTournament>();

            //string sportUrl = string.Format("http://www.sofascore.com/{0}///json", sportSlug);
            //string sportJson = WebDownloader.DownloadData(sportUrl);
            //var sportData = JsonConvert.DeserializeObject<SofaSport>(sportJson);

            //Sport sport = sportData.sportItem.sport;

            string categoryUrl = string.Format("http://www.sofascore.com/esi/categories/{0}", sportSlug);

            var categories = ParseCategories(categoryUrl);
            foreach (var category in categories)
            {
                // Href - např. /football/albania
                string tournamentUrl = string.Format("http://www.sofascore.com/list/category{0}/tournaments/json", category.Href);
                var tournament = ParseTournament(tournamentUrl);
                tournamentLists.Add(tournament);

                if (isTest)
                    break;
            }

            foreach (var tournamentList in tournamentLists)
            {
                // http://www.sofascore.com/tournament/2891//standings/tables/json
                // http://www.sofascore.com/u-tournament/720/season//json

                if (tournamentList.tournaments.list != null)
                {
                    foreach (var tournament in tournamentList.tournaments.list)
                    {
                        string teamsUrl = string.Format("http://www.sofascore.com/tournament/{0}//standings/tables/json", tournament.uniqueId);
                        var team = ParseTeam(teamsUrl);
                        teams.Add(team);

                        if (isTest)
                            break;
                    }
                }
            }

            return teams;
        }

        public IEnumerable<SofaScoreCategory> ParseCategories(string categoryUrl)
        {
            List<SofaScoreCategory> categories = new List<SofaScoreCategory>();
            string basePage = WebDownloader.DownloadData(categoryUrl);

            Document.LoadHtml(basePage);
            if (Document.DocumentNode != null)
            {
                var body = Document.DocumentNode.SelectSingleNode("//div[@class='leagues']");
                if (body != null)
                {
                    var ulNodes = body.Descendants("ul");
                    foreach (var ulNode in ulNodes)
                    {
                        var liNodes = ulNode.Descendants("li");
                        foreach (HtmlNode liNode in liNodes)
                        {
                            var aNode = liNode.Descendants("a").FirstOrDefault();
                            if (aNode != null)
                            {
                                if (!aNode.Attributes.Contains("data-id")) continue;

                                int id = int.Parse(aNode.Attributes["data-id"].Value);

                                categories.Add(new SofaScoreCategory()
                                {
                                    ID = id,
                                    DisplayName = aNode.InnerText.Trim(),
                                    Href = aNode.Attributes["href"].Value
                                });

                                if (isTest)
                                    break;
                            }
                        }
                    }
                }
            }

            return categories;
        }

        public SofaScoreTeam ParseTeam(string teamUrl)
        {
            string teamJson = WebDownloader.DownloadData(teamUrl);
            return JsonConvert.DeserializeObject<SofaScoreTeam>(teamJson);
        }


        public SofaScoreTournament ParseTournament(string tournamentUrl)
        {
            string tournamentJson = WebDownloader.DownloadData(tournamentUrl);
            return JsonConvert.DeserializeObject<SofaScoreTournament>(tournamentJson);
        }
    }

    public class SofaScoreCategory
    {
        public int ID { get; set; }

        public string DisplayName { get; set; }

        public string Href { get; set; }

    }
}

namespace BetManager.Core.SofaScoreBasicExport.Teams
{

    public class SofaScoreTeam
    {
        #region Help

        private string EmptyJsonArray = "[]";
        private TeamEventPair[] teamEventsList;

        [XmlIgnore]
        [JsonProperty(PropertyName = "teamEvents")]
        public object teamEventsJson { get; set; }

        [XmlArray("TeamEvents")]
        public TeamEventPair[] TeamEventsList
        {
            get
            {


                return TeamEvents.Select(x => new TeamEventPair()
                {
                    ID = x.Key,
                    Event = x.Value
                }).ToArray();
            }
            set { teamEventsList = value; }
        }

        #endregion

        public Standingstable[] standingsTables { get; set; }

        [XmlIgnore]
        public Dictionary<int, Teamevent> TeamEvents
        {
            get
            {
                var json = this.teamEventsJson.ToString();
                if (json.Trim() == EmptyJsonArray)
                {
                    return new Dictionary<int, Teamevent>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<int, Teamevent>>(json);
                }
            }
        }
    }

    public class TeamEventPair
    {
        public int? ID { get; set; }

        public Teamevent Event { get; set; }
    }

    public class Teamevent
    {
        public EventData[] total { get; set; }
        public EventData[] home { get; set; }
        public EventData[] away { get; set; }
    }

    public class EventData
    {
        public int? id { get; set; }
        public string customId { get; set; }
        public Team homeTeam { get; set; }
        public Team awayTeam { get; set; }
        public Score homeScore { get; set; }
        public Score awayScore { get; set; }
        public int? winnerCode { get; set; }
        public string slug { get; set; }
        public int? startTimestamp { get; set; }
    }

    public class Score
    {
        public int? current { get; set; }
    }

    public class Standingstable
    {
        private string EmptyJsonArray = "[]";

        public Sport sport { get; set; }
        public Category category { get; set; }
        public Season season { get; set; }
        public Tournament tournament { get; set; }
        public string name { get; set; }
        public int? round { get; set; }
        //public object[] descriptions { get; set; }
        public int? id { get; set; }
        public string[] totalPointsFields { get; set; }
        public string[] homePointsFields { get; set; }
        public string[] awayPointsFields { get; set; }
        public bool hasTotalPointsFields { get; set; }
        public bool hasHomePointsFields { get; set; }
        public bool hasAwayPointsFields { get; set; }
        public Tablerow[] tableRows { get; set; }
        public Totaltablekeys totalTableKeys { get; set; }
        public Hometablekeys homeTableKeys { get; set; }
        public Awaytablekeys awayTableKeys { get; set; }
        public string updatedAtFormated { get; set; }
        public int? updatedAtTimestamp { get; set; }

        [XmlIgnore]
        public Dictionary<int, Promotion> Promotions
        {
            get
            {
                var json = this.promotionsJson.ToString();
                if (json.Trim() == EmptyJsonArray)
                {
                    return new Dictionary<int, Promotion>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<int, Promotion>>(json);
                }
            }
        }

        [XmlIgnore]
        [JsonProperty(PropertyName = "promotions")]
        public object promotionsJson { get; set; }

        [XmlIgnore]
        public Dictionary<int, Promotionscoloring> PromotionsColoring
        {
            get
            {
                var json = this.promotionsJson.ToString();
                if (json.Trim() == EmptyJsonArray)
                {
                    return new Dictionary<int, Promotionscoloring>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<int, Promotionscoloring>>(json);
                }
            }
        }

        [XmlIgnore]
        [JsonProperty(PropertyName = "promotionsColoring")]
        public object promotionsColoringJson { get; set; }


        public bool isLive { get; set; }

        public PromotionPair[] promotionsList;
        public PromotionPair[] PromotionsList
        {
            get
            {
                return Promotions.Select(x => new PromotionPair()
                {
                    ID = x.Key,
                    Promotion = x.Value
                }).ToArray();
            }
            set { promotionsList = value; }
        }

        public PromotionColoringPair[] promotionsColoringList;
        public PromotionColoringPair[] PromotionsColoringList
        {
            get
            {
                return PromotionsColoring.Select(x => new PromotionColoringPair()
                {
                    ID = x.Key,
                    Promotionscoloring = x.Value
                }).ToArray();
            }
            set { promotionsColoringList = value; }
        }

    }

    public class PromotionColoringPair
    {
        public int? ID { get; set; }
        public Promotionscoloring Promotionscoloring { get; set; }
    }

    public class PromotionPair
    {
        public int? ID { get; set; }
        public Promotion Promotion { get; set; }
    }

    public class Sport
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int? id { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int? priority { get; set; }
        public int? id { get; set; }
        public string flag { get; set; }
    }

    public class Season
    {
        public string name { get; set; }
        public string year { get; set; }
        public int? id { get; set; }
    }

    public class Tournament
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int? id { get; set; }
        public int? uniqueId { get; set; }
        public string uniqueName { get; set; }
        public bool? hasEventPlayerStatistics { get; set; }
        public bool? hasEventPlayerHeatMap { get; set; }
    }

    public class Totaltablekeys
    {
        public string matchesTotal { get; set; }
        public string winTotal { get; set; }
        public string drawTotal { get; set; }
        public string lossTotal { get; set; }
        public string goalsTotal { get; set; }
        public string pointsTotal { get; set; }
    }

    public class Hometablekeys
    {
        public string matchesHome { get; set; }
        public string winHome { get; set; }
        public string drawHome { get; set; }
        public string lossHome { get; set; }
        public string goalsTotalHome { get; set; }
        public string pointsHome { get; set; }
    }

    public class Awaytablekeys
    {
        public string matchesAway { get; set; }
        public string winAway { get; set; }
        public string drawAway { get; set; }
        public string lossAway { get; set; }
        public string goalsTotalAway { get; set; }
        public string pointsAway { get; set; }
    }

    public class Promotion
    {
        private string EmptyJsonArray = "[]";

        public int? id { get; set; }
        public string name { get; set; }
        // public object[] nameTranslations { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> NameTranslations
        {
            get
            {
                if (nameTranslationsJson == null) return new Dictionary<string, string>();
                var json = this.nameTranslationsJson.ToString();
                if (json.Trim() == EmptyJsonArray)
                {
                    return new Dictionary<string, string>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                }
            }
        }

        [XmlIgnore]
        [JsonProperty(PropertyName = "nameTranslations")]
        public object nameTranslationsJson { get; set; }

        public TranslationPair[] nameTranslationList;
        public TranslationPair[] NameTranslationList
        {
            get
            {
                return NameTranslations.Select(x => new TranslationPair()
                {
                    Value = x.Value,
                    Lang = x.Key
                }).ToArray();
            }
            set { nameTranslationList = value; }
        }
    }

    public class TranslationPair
    {
        public string Lang { get; set; }

        public string Value { get; set; }
    }

    public class Promotionscoloring
    {
        public string _class { get; set; }
    }

    public class Tablerow
    {
        public Team team { get; set; }
        public bool isLive { get; set; }
        //public object[] descriptions { get; set; }
        public int? id { get; set; }
        public Promotion promotion { get; set; }
        public string position { get; set; }
        public string homePosition { get; set; }
        public string awayPosition { get; set; }
        public string points { get; set; }
        public string homePoints { get; set; }
        public string awayPoints { get; set; }
        public Totalfields totalFields { get; set; }

        public Standingstable[] standingsTables { get; set; }
        public Homefields[] HomeFields
        {
            get
            {
                var json = this.homeFieldsJson.ToString();

                if (json.StartsWith("["))
                {
                    return JsonConvert.DeserializeObject<Homefields[]>(json);
                }
                else
                {
                    var simpleObj = JsonConvert.DeserializeObject<Homefields>(json);
                    return new Homefields[] { simpleObj };
                }
            }
        }

        [XmlIgnore]
        [JsonProperty(PropertyName = "homeFields")]
        public object homeFieldsJson { get; set; }

        public Awayfields[] AwayFields
        {
            get
            {
                var json = this.awayFieldsJson.ToString();

                if (json.StartsWith("["))
                {
                    return JsonConvert.DeserializeObject<Awayfields[]>(json);
                }
                else
                {
                    var simpleObj = JsonConvert.DeserializeObject<Awayfields>(json);
                    return new Awayfields[] { simpleObj };
                }
            }
        }

        [XmlIgnore]
        [JsonProperty(PropertyName = "awayFields")]
        public object awayFieldsJson { get; set; }
    }

    public class Team
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string gender { get; set; }
        public int? id { get; set; }
        public string shortName { get; set; }
    }

    public class Totalfields
    {
        public string matchesTotal { get; set; }
        public string winTotal { get; set; }
        public string drawTotal { get; set; }
        public string lossTotal { get; set; }
        public string goalsTotal { get; set; }
        public string goalDiffTotal { get; set; }
        public string pointsTotal { get; set; }
    }

    public class Homefields
    {
        public string matchesHome { get; set; }
        public string winHome { get; set; }
        public string drawHome { get; set; }
        public string lossHome { get; set; }
        public string goalsTotalHome { get; set; }
        public string goalDiffHome { get; set; }
        public string pointsHome { get; set; }
    }

    public class Awayfields
    {
        public string matchesAway { get; set; }
        public string winAway { get; set; }
        public string drawAway { get; set; }
        public string lossAway { get; set; }
        public string goalsTotalAway { get; set; }
        public string goalDiffAway { get; set; }
        public string pointsAway { get; set; }
    }

}


namespace BetManager.Core.SofaScoreBasicExport.Tournaments
{
    public class SofaScoreTournament
    {
        public Sport sport { get; set; }
        public Category category { get; set; }
        public Tournaments tournaments { get; set; }
    }

    public class Sport
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int priority { get; set; }
        public int id { get; set; }
        public string flag { get; set; }
    }

    public class Tournaments
    {
        private string EmptyJsonArray = "[]";

        public string type { get; set; }
        public int? goToItem { get; set; }

        public Tournament[] list { get; set; }

        [XmlIgnore]
        public Dictionary<int, Section> Sections
        {
            get
            {
                if (sectionsJson == null) return new Dictionary<int, Section>();
                var json = this.sectionsJson.ToString();
                if (json.Trim() == EmptyJsonArray)
                {
                    return new Dictionary<int, Section>();
                }
                else
                {
                    return JsonConvert.DeserializeObject<Dictionary<int, Section>>(json);
                }
            }
        }

        [XmlIgnore]
        [JsonProperty(PropertyName = "sections")]
        public object sectionsJson { get; set; }

        private SectionPair[] sectionsList;
        [XmlArray("Sections")]
        public SectionPair[] SectionsList
        {
            get
            {


                return Sections.Select(x => new SectionPair()
                {
                    ID = x.Key,
                    Section = x.Value
                }).ToArray();
            }
            set { sectionsList = value; }
        }
    }

    public class SectionPair
    {
        public int ID { get; set; }
        public Section Section { get; set; }
    }

    public class Section
    {
        public string title { get; set; }
        public Tournament[] tournaments { get; set; }
    }

    public class Tournament
    {
        public int id { get; set; }
        public int? order { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string uniqueName { get; set; }
        public bool? hasEventPlayerStatistics { get; set; }
        public bool? hasEventPlayerHeatMap { get; set; }
        public bool? isActive { get; set; }
        public int uniqueId { get; set; }
    }
}

namespace xxx
{

    public class Rootobject
    {
        public object[] standingsTables { get; set; }
        public Category category { get; set; }
        public Sport sport { get; set; }
        public Uniquetournament uniqueTournament { get; set; }
        public object[] teamEvents { get; set; }
        public object season { get; set; }
        public Events events { get; set; }
    }

    public class Category
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int priority { get; set; }
        public int id { get; set; }
        public string flag { get; set; }
    }

    public class Sport
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
    }

    public class Uniquetournament
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
    }

    public class Events
    {
        public Week[] weeks { get; set; }
        public Weekmatches weekMatches { get; set; }
        public bool hasRounds { get; set; }
        public Round[] rounds { get; set; }
        public Roundmatches roundMatches { get; set; }
    }

    public class Weekmatches
    {
        public Data data { get; set; }
        public Tournament[] tournaments { get; set; }
        public Sport1 sport { get; set; }
    }

    public class Data
    {
        public int index { get; set; }
        public int weekStartDate { get; set; }
        public int weekEndDate { get; set; }
    }

    public class Sport1
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
    }

    public class Tournament
    {
        public Tournament1 tournament { get; set; }
        public Category1 category { get; set; }
        public Season season { get; set; }
        public bool? hasEventPlayerStatistics { get; set; }
        public bool? hasEventPlayerHeatMap { get; set; }
        public Event[] events { get; set; }
    }

    public class Tournament1
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
        public int uniqueId { get; set; }
        public string uniqueName { get; set; }
        public bool? hasEventPlayerStatistics { get; set; }
        public bool? hasEventPlayerHeatMap { get; set; }
    }

    public class Category1
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int priority { get; set; }
        public int id { get; set; }
        public string flag { get; set; }
    }

    public class Season
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string year { get; set; }
        public int id { get; set; }
    }

    public class Event
    {
        public Sport2 sport { get; set; }
        public Odds odds { get; set; }
        public Roundinfo roundInfo { get; set; }
        public string customId { get; set; }
        public Status status { get; set; }
        public int winnerCode { get; set; }
        public Hometeam homeTeam { get; set; }
        public Awayteam awayTeam { get; set; }
        public Homescore homeScore { get; set; }
        public Awayscore awayScore { get; set; }
        public object[] time { get; set; }
        public Changes changes { get; set; }
        public bool hasHighlights { get; set; }
        public bool hasHighlightsStream { get; set; }
        public int id { get; set; }
        public bool hasDraw { get; set; }
        public bool hasStatistics { get; set; }
        public Periods periods { get; set; }
        public string lastPeriod { get; set; }
        public string name { get; set; }
        public string startTime { get; set; }
        public string formatedStartDate { get; set; }
        public int startTimestamp { get; set; }
        public string statusDescription { get; set; }
        public string slug { get; set; }
        public int uniqueTournamentId { get; set; }
        public bool hasLineups { get; set; }
        public bool hasLineupsList { get; set; }
        public bool hasOdds { get; set; }
        public bool hasLiveOdds { get; set; }
        public bool hasSubScore { get; set; }
        public bool hasAggregatedScore { get; set; }
        public bool hasFirstToServe { get; set; }
        public bool votingEnabled { get; set; }
        public bool hasTime { get; set; }
        public bool isSyncable { get; set; }
    }

    public class Sport2
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
    }

    public class Odds
    {
        public int id { get; set; }
        public Fulltimeodds fullTimeOdds { get; set; }
    }

    public class Fulltimeodds
    {
        public Regular regular { get; set; }
        public int type { get; set; }
        public string sourceId { get; set; }
        public Live live { get; set; }
    }

    public class Regular
    {
        public _1 _1 { get; set; }
        public _2 _2 { get; set; }
    }

    public class _1
    {
        public string initialDecimalValue { get; set; }
        public string initialFractionalValue { get; set; }
        public string initialAmericanValue { get; set; }
        public string decimalValue { get; set; }
        public string fractionalValue { get; set; }
        public string americanValue { get; set; }
        public string betSlipLink { get; set; }
        public int change { get; set; }
        public string sourceId { get; set; }
        public bool winning { get; set; }
    }

    public class _2
    {
        public string initialDecimalValue { get; set; }
        public string initialFractionalValue { get; set; }
        public string initialAmericanValue { get; set; }
        public string decimalValue { get; set; }
        public string fractionalValue { get; set; }
        public string americanValue { get; set; }
        public string betSlipLink { get; set; }
        public int change { get; set; }
        public string sourceId { get; set; }
        public bool winning { get; set; }
    }

    public class Live
    {
        public _11 _1 { get; set; }
        public _21 _2 { get; set; }
    }

    public class _11
    {
        public string decimalValue { get; set; }
        public string fractionalValue { get; set; }
        public string americanValue { get; set; }
        public string betSlipLink { get; set; }
        public string sourceId { get; set; }
    }

    public class _21
    {
        public string decimalValue { get; set; }
        public string fractionalValue { get; set; }
        public string americanValue { get; set; }
        public string betSlipLink { get; set; }
        public string sourceId { get; set; }
    }

    public class Roundinfo
    {
        public int round { get; set; }
        public string name { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string type { get; set; }
    }

    public class Hometeam
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string gender { get; set; }
        public int id { get; set; }
    }

    public class Awayteam
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string gender { get; set; }
        public int id { get; set; }
    }

    public class Homescore
    {
        public int current { get; set; }
        public int period1 { get; set; }
        public int period2 { get; set; }
        public int period3 { get; set; }
        public int normaltime { get; set; }
        public int overtime { get; set; }
        public int penalties { get; set; }
    }

    public class Awayscore
    {
        public int current { get; set; }
        public int period1 { get; set; }
        public int period2 { get; set; }
        public int period3 { get; set; }
        public int normaltime { get; set; }
        public int overtime { get; set; }
        public int penalties { get; set; }
    }

    public class Changes
    {
        public DateTime changeDate { get; set; }
        public string[] changes { get; set; }
        public int changeTimestamp { get; set; }
        public bool hasExpired { get; set; }
        public bool hasHomeChanges { get; set; }
        public bool hasAwayChanges { get; set; }
    }

    public class Periods
    {
        public string current { get; set; }
        public string period1 { get; set; }
        public string period2 { get; set; }
        public string period3 { get; set; }
        public string overtime { get; set; }
        public string penalties { get; set; }
    }

    public class Roundmatches
    {
        public Data1 data { get; set; }
        public Tournament2[] tournaments { get; set; }
        public Sport3 sport { get; set; }
    }

    public class Data1
    {
        public int index { get; set; }
        public string roundName { get; set; }
        public int roundPositionIndex { get; set; }
    }

    public class Sport3
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
    }

    public class Tournament2
    {
        public Tournament3 tournament { get; set; }
        public Category2 category { get; set; }
        public Season1 season { get; set; }
        public bool? hasEventPlayerStatistics { get; set; }
        public bool? hasEventPlayerHeatMap { get; set; }
        public Event1[] events { get; set; }
    }

    public class Tournament3
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
        public int uniqueId { get; set; }
        public string uniqueName { get; set; }
        public bool? hasEventPlayerStatistics { get; set; }
        public bool? hasEventPlayerHeatMap { get; set; }
    }

    public class Category2
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int priority { get; set; }
        public int id { get; set; }
        public string flag { get; set; }
    }

    public class Season1
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string year { get; set; }
        public int id { get; set; }
    }

    public class Event1
    {
        public Sport4 sport { get; set; }
        public Odds1 odds { get; set; }
        public Roundinfo1 roundInfo { get; set; }
        public string customId { get; set; }
        public Status1 status { get; set; }
        public int winnerCode { get; set; }
        public Hometeam1 homeTeam { get; set; }
        public Awayteam1 awayTeam { get; set; }
        public Homescore1 homeScore { get; set; }
        public Awayscore1 awayScore { get; set; }
        public object[] time { get; set; }
        public Changes1 changes { get; set; }
        public bool hasHighlights { get; set; }
        public bool hasHighlightsStream { get; set; }
        public int id { get; set; }
        public bool hasDraw { get; set; }
        public bool hasStatistics { get; set; }
        public Periods1 periods { get; set; }
        public string lastPeriod { get; set; }
        public string name { get; set; }
        public string startTime { get; set; }
        public string formatedStartDate { get; set; }
        public int startTimestamp { get; set; }
        public string statusDescription { get; set; }
        public string slug { get; set; }
        public int uniqueTournamentId { get; set; }
        public bool hasLineups { get; set; }
        public bool hasLineupsList { get; set; }
        public bool hasOdds { get; set; }
        public bool hasLiveOdds { get; set; }
        public bool hasSubScore { get; set; }
        public bool hasAggregatedScore { get; set; }
        public bool hasFirstToServe { get; set; }
        public bool votingEnabled { get; set; }
        public bool hasTime { get; set; }
        public bool isSyncable { get; set; }
    }

    public class Sport4
    {
        public string name { get; set; }
        public string slug { get; set; }
        public int id { get; set; }
    }

    public class Odds1
    {
        public int id { get; set; }
    }

    public class Roundinfo1
    {
        public int round { get; set; }
        public string name { get; set; }
    }

    public class Status1
    {
        public int code { get; set; }
        public string type { get; set; }
    }

    public class Hometeam1
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string gender { get; set; }
        public int id { get; set; }
    }

    public class Awayteam1
    {
        public string name { get; set; }
        public string slug { get; set; }
        public string gender { get; set; }
        public int id { get; set; }
    }

    public class Homescore1
    {
        public int current { get; set; }
        public int period1 { get; set; }
        public int period2 { get; set; }
        public int period3 { get; set; }
        public int normaltime { get; set; }
    }

    public class Awayscore1
    {
        public int current { get; set; }
        public int period1 { get; set; }
        public int period2 { get; set; }
        public int period3 { get; set; }
        public int normaltime { get; set; }
    }

    public class Changes1
    {
        public string[] changes { get; set; }
        public int changeTimestamp { get; set; }
        public bool hasExpired { get; set; }
        public bool hasHomeChanges { get; set; }
        public bool hasAwayChanges { get; set; }
        public DateTime changeDate { get; set; }
    }

    public class Periods1
    {
        public string current { get; set; }
        public string period1 { get; set; }
        public string period2 { get; set; }
        public string period3 { get; set; }
        public string overtime { get; set; }
        public string penalties { get; set; }
    }

    public class Week
    {
        public int weekIndex { get; set; }
        public int weekStartDate { get; set; }
        public int weekEndDate { get; set; }
    }

    public class Round
    {
        public int round { get; set; }
        public string name { get; set; }
    }

}