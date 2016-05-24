using BetManager.Core.Webs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BetManager.Core.Exports.SofascoreExport;
using Newtonsoft.Json;

namespace BetManager.Core.Processors
{
    /// <summary>
    /// Zpracování stránek sofascore.com
    /// </summary>
    public class SofascoreProcessor : BaseProcessor
    {
        //private SofaScore SofaScore;

        public SofascoreProcessor(IWebDownloader webDownloader, string url)
            :base(webDownloader, url)
        { }

        public override string Process()
        {
            //string json = null;
            string url = null;
            var sports = new List<SofaSport>();

            // fotbal
            //json = System.IO.File.ReadAllText("Data/sofascore/-football---json.json", Encoding.UTF8);
            //sports.Add(JsonConvert.DeserializeObject<SofaSport>(json));

            //// tenis
            //json = System.IO.File.ReadAllText("Data/sofascore/-tennis---json.json", Encoding.UTF8);
            //sports.Add(JsonConvert.DeserializeObject<SofaSport>(json));

            //// ice-hockey
            //json = System.IO.File.ReadAllText("Data/sofascore/-ice-hockey---json.json", Encoding.UTF8);
            //sports.Add(JsonConvert.DeserializeObject<SofaSport>(json));

            // data
            url = "http://www.sofascore.com/football///json";
            ProcessData(sports, url);
            url = "http://www.sofascore.com/tennis///json";
            ProcessData(sports, url);
            url = "http://www.sofascore.com/ice-hockey///json";
            ProcessData(sports, url);

            // vysledky
            return Utils.XmlHelper.Serializable<Sofascore>(new Sofascore()
            {
                DateGenerated = DateTime.Now,
                Sports = sports.ToArray()
            });
        }

        private void ProcessData(ICollection<SofaSport> sports, string url)
        {
            string json = WebDownloader.DownloadData(url);
            sports.Add(JsonConvert.DeserializeObject<SofaSport>(json));
        }
    }
}
