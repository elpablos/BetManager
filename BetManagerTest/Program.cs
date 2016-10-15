using BetManager.Core.Dbs;
using BetManager.Core.Processors;
using BetManager.Core.Utils;
using BetManager.Core.Webs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            DbXmlDataImporter importer = new DbXmlDataImporter();
            string url = null;

            //DownloadFullSofaData(importer);
            //ReadAndSaveToDb(importer);

            using (IWebDownloader webDownloader = new WebDownloader())
            {
                url = "http://www.sofascore.com/tournament/49/11779/standings/tables/json";

                string name = string.Format("{0}-sofa-cr-prvni-liga.xml", DateTime.Now.ToString("yyyy-MM-dd"));
                string json = webDownloader.DownloadData(url);

                var sofaSport = JsonConvert.DeserializeXmlNode(json, "root");
                var xml = XmlHelper.Serializable(sofaSport);

                File.WriteAllText("Data/" + name, xml, Encoding.UTF8);
            }
        }

        public static void ReadAndSaveToDb(DbXmlDataImporter importer)
        {
            string filename = null;

            filename = "2016-08-16-sofa-cr-prvni-liga";
            string xml = File.ReadAllText("Data/2016-08-16-sofa-cr-prvni-liga.xml");
            int? id = importer.ImportXmlData("sofascoreTournament", filename, xml);
        }

        public static void DownloadFullSofaData(DbXmlDataImporter importer)
        {
            string filename = null;
            string url = null;

            url = "http://www.sofascore.com/";
            using (IWebDownloader webDownloader = new WebDownloader())
            {
                using (IProcessor processor = new SofascoreBasicDataProcessor(webDownloader, url))
                {
                    string name = DateTime.Now.ToString("yyyy-MM-dd_HH-MM") + "_sofaBasicData.xml";
                    string xml = processor.Process();

                    File.WriteAllText("Data/" + name, xml, Encoding.UTF8);
                    filename = Path.GetFileNameWithoutExtension(name);
                    //int? id = importer.ImportXmlData("sofascore", filename, xml);
                    //if (id.HasValue)
                    //{
                    //    importer.VitiImport(id.Value);
                    //}
                }
            }
        }
    }
}
