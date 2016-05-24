using BetManager.Core.Utils;
using BetManager.Core.VitiBetExport;
using BetManager.Core.Webs;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;

namespace BetManager.Core.Processors
{
    /// <summary>
    /// Zpracování stránek vitisport.cz
    /// </summary>
    public class VitisportProcessor : BaseProcessor
    {
        #region Fields

        private readonly string SectionNameConst = "sekce";
        private readonly string LeagueNameConst = "liga";

        #endregion

        #region Constructor

        public VitisportProcessor(IWebDownloader webDownloader, string url)
             : base(webDownloader, url)
        { }

        #endregion

        #region Overrided methods

        public override string Process()
        {
            VitiBetRoot root = new VitiBetRoot();
            root.DateGenerated = DateTime.Now;
            // sekce
            ParseSections(root, Url);

            // ligy
            ParseLeagues(root);

            // tipy
            ParseTip(root);

            return XmlHelper.Serializable<VitiBetRoot>(root);
        }

        #endregion

        #region Parse web parts

        private void ParseTip(VitiBetRoot root)
        {
            // tabulkaquick
            foreach (var section in root.Sections)
            {
                foreach (var league in section.Leagues)
                {
                    List<Tip> tips = new List<Tip>();

                    string basePage = WebDownloader.DownloadData(league.Url);

                    Document.LoadHtml(basePage);
                    if (Document.DocumentNode != null)
                    {
                        var body = Document.DocumentNode.SelectSingleNode("//body");
                        if (body != null)
                        {
                            var tableQuick = body.SelectSingleNode("//table[@class='tabulkaquick']");
                            if (tableQuick != null)
                            {
                                //var tableRows = tableQuick.SelectNodes("//tr"); // [@class='standardbunka']
                                var tableRows = tableQuick.Descendants("tr");
                                foreach (var row in tableRows)
                                {
                                    var columns = row.ChildNodes;
                                    if (columns.Count < 13)
                                    {
                                        continue;
                                    }

                                    /*
                                    < tr >
                                    < td class="standardbunka" width="20px">07.05</td><td class="standardbunka" width="20px"></td>
                                    <td class="standardbunka" width="170px"> <a href = "./index.php?clanek=profil&amp;sekce=fotbal&amp;tym=Norwich City&amp;liga=anglie&amp;lang=cs" > Norwich City</a></td>
                                    <td class="standardbunka" width="170px"><a href = "./index.php?clanek=profil&amp;sekce=fotbal&amp;tym=Manchester United&amp;liga=anglie&amp;lang=cs" > Manchester United</a></td>
                                    <td class="standardbunka" width="20px"></td>
                                    <td class="vetsipismo" width="25px">1</td>
                                    <td class="standardbunka" width="5px"> : </td>
                                    <td class="vetsipismo" width="25px">2</td>
                                    <td class="standardbunka" width="30px">26 %</td>
                                    <td class="standardbunka" width="30px">28 %</td>
                                    <td class="standardbunka" width="30px">46 %</td>
                                    <td class="barvapodtipek2" width="30px">2</td>
                                    <td class="standardbunka" width="30px">-1.6</td>
                                    <td class="standardbunkaobr" width="20px"><a href = "index.php?clanek=analyzy&amp;sekce=fotbal&amp;liga=anglie&amp;lang=cs&amp;tab=1&amp;zap=1" rel="nofollow"><img src = "./images/pictures/lupa.gif" border="0" width="18"></a></td>
                                    </tr>

                                    */
                                    var tip = new Tip();
                                    // columns[0].InnerText; // datum
                                    // columns[1].InnerText; // odkaz na ligu

                                    tip.EventDate = DateTime.Parse(columns[0].InnerText + "." + DateTime.Now.Year);

                                    // score
                                    tip.CompetitorHome = columns[2].InnerText;
                                    tip.CompetitorAway = columns[3].InnerText;

                                    // column[3] - mezera

                                    tip.TipHomeScore = columns[5].InnerText == "?" ? (int?)null : int.Parse(columns[5].InnerText); // ?
                                    // 5 je dvojtecka
                                    tip.TipAwayScore = columns[7].InnerText == "?" ? (int?)null : int.Parse(columns[7].InnerText); // ?

                                    // procenta
                                    tip.TipHomePercent = columns[8].InnerText == "x" ? (int?)null : int.Parse(columns[8].InnerText.Replace('%', ' ').Trim()); // home % // x
                                    tip.TipDrawPercent = columns[9].InnerText == "x" ? (int?)null : int.Parse(columns[9].InnerText.Replace('%', ' ').Trim()); // remiza % // x
                                    tip.TipAwayPercent = columns[10].InnerText == "x" ? (int?)null : int.Parse(columns[10].InnerText.Replace('%', ' ').Trim()); // away % // x

                                    // tip za sazku
                                    tip.TipBet = columns[11].InnerText == "-" ? null : columns[11].InnerText; // -
                                    // index
                                    tip.Index = columns[12].InnerText == "-" ? (decimal?)null : decimal.Parse(columns[12].InnerText, CultureInfo.InvariantCulture); // -

                                    var link = columns[13].FirstChild;
                                    var detailLink = HttpUtility.HtmlDecode(link.Attributes["href"].Value);
                                    tip.Url = detailLink;

                                    tip.DisplayName = tip.CompetitorHome + " : " + tip.CompetitorAway;

                                    tips.Add(tip);
                                }
                            }
                        }
                    }

                    league.Tips = tips.ToArray();
                }
            }
        }

        private void ParseLeagues(VitiBetRoot root)
        {
            foreach (var section in root.Sections)
            {
                List<League> leagues = new List<League>();

                string basePage = WebDownloader.DownloadData(section.Url);

                Document.LoadHtml(basePage);
                if (Document.DocumentNode != null)
                {
                    var body = Document.DocumentNode.SelectSingleNode("//body");
                    if (body != null)
                    {
                        var leagueMenu = body.SelectSingleNode("//ul[@id='primarne']");
                        if (leagueMenu != null)
                        {
                            var leagueMenuItems = leagueMenu.SelectNodes("//li");
                            foreach (var leagueMenuItem in leagueMenuItems)
                            {
                                var league = new League();
                                var link = leagueMenuItem.FirstChild;
                                var leagueLink = link.Attributes["href"].Value;
                                string leagueDescription = null;
                                if (link.Attributes.Contains("title"))
                                    leagueDescription = link.Attributes["title"].Value;
                                league.Url = HttpUtility.HtmlDecode(leagueLink);
                                league.Key = UrlHelper.ParseUrl(league.Url, LeagueNameConst);
                                league.DisplayName = link.InnerText;
                                league.Description = leagueDescription;

                                if (league.Key != null)
                                    leagues.Add(league);
                            }
                        }
                    }
                }

                section.Leagues = leagues.ToArray();
            }
        }

        private void ParseSections(VitiBetRoot root, string url)
        {
            HashSet<Section> sections = new HashSet<Section>();

            string basePage = WebDownloader.DownloadData(url);

            Document.LoadHtml(basePage);
            if (Document.DocumentNode != null)
            {
                var body = Document.DocumentNode.SelectSingleNode("//body");
                if (body != null)
                {
                    var sectionMenu = body.SelectSingleNode("//div[@id='vodorovne_menu']//ul");
                    if (sectionMenu != null)
                    {
                        var sectionMenuItems = sectionMenu.SelectNodes("//li");
                        foreach (var sectionMenuItem in sectionMenuItems)
                        {
                            var section = new Section();
                            var link = sectionMenuItem.FirstChild;
                            var sectionUrl = link.Attributes["href"].Value;
                            string sectionDescription = null;
                            if (link.Attributes.Contains("title"))
                                sectionDescription = link.Attributes["title"].Value;
                            section.Url = HttpUtility.HtmlDecode(sectionUrl);
                            section.Key = UrlHelper.ParseUrl(section.Url, SectionNameConst);
                            section.DisplayName = link.InnerText;
                            section.Description = sectionDescription;

                            if (section.Key != null)
                                sections.Add(section);
                        }
                    }
                }
            }

            root.Sections = sections.ToArray();
        }

        #endregion
    }
}
