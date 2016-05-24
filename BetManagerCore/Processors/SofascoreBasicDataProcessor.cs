using BetManager.Core.Exports.SofascoreExport;
using xxx;
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

        public SofascoreBasicDataProcessor(IWebDownloader webDownloader, string url)
            : base(webDownloader, url)
        { }

        public override string Process()
        {
            string ret = null;

            IEnumerable<SofaScoreTeam> teams = new List<SofaScoreTeam>();

            teams = teams.Union(ParseSportData("football"));
            teams = teams.Union(ParseSportData("tennis"));
            teams = teams.Union(ParseSportData("ice-hockey"));
            teams = teams.Union(ParseSportData("basketball"));

            try
            {
                // pojistka
                string x = JsonConvert.SerializeObject(teams.ToArray());
                System.IO.File.WriteAllText("Data/SofaScoreBasicData.json", x);
            }
            catch (Exception)
            {
            }

            ret = XmlHelper.Serializable<SofaScoreTeam[]>(teams.ToArray());

            return ret;
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
            }

            foreach (var tournamentList in tournamentLists)
            {
                // http://www.sofascore.com/tournament/2891//standings/tables/json
                // http://www.sofascore.com/u-tournament/720/season//json

                foreach (var tournament in tournamentList.tournaments.list)
                {
                    string teamsUrl = string.Format("http://www.sofascore.com/tournament/{0}//standings/tables/json", tournament.uniqueId);
                    var team = ParseTeam(teamsUrl);
                    teams.Add(team);
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
                    var ulNode = body.SelectSingleNode("//ul");
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

    #region Tournament

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
        public string type { get; set; }
        public List[] list { get; set; }
    }

    public class List
    {
        public int id { get; set; }
        public int order { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string uniqueName { get; set; }
        public bool hasEventPlayerStatistics { get; set; }
        public bool hasEventPlayerHeatMap { get; set; }
        public bool isActive { get; set; }
        public int uniqueId { get; set; }
    }


    #endregion


}

namespace xxx
{

    public class SofaScoreTeam
    {
        private string EmptyJsonArray = "[]";
        private TeamEventPair[] teamEventsList;

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
        public bool hasEventPlayerStatistics { get; set; }
        public bool hasEventPlayerHeatMap { get; set; }
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
