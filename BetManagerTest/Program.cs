using BetManager.Core.Dbs;
using BetManager.Core.Processors;
using BetManager.Core.Webs;
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
