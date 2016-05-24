using BetManager.Core.Dbs;
using BetManager.Core.Processors;
using BetManager.Core.Webs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.ConsoleApp
{
    public class Program
    {
        private static bool canImportFortuna = true;
        private static bool canImportTipsport = true;
        private static bool canImportViti = true;
        private static bool canImportSofa = true;

        public static void Main(string[] args)
        {
            DbXmlDataImporter importer = new DbXmlDataImporter();
            string filename = null;
            string url = null;

            // postahuju existujici XML
            using (IWebDownloader webDownloader = new WebDownloader())
            {
                if (canImportFortuna)
                {
                    // export
                    string nameExport = DateTime.Now.ToString("yyyy-MM-dd_HH-MM") + "_export.xml";

                    string urlExport = "http://ext.ifortuna.cz/trefik/export.xml";
                    string xmlExport = webDownloader.DownloadData(urlExport);

                    File.WriteAllText("Data/" + nameExport, xmlExport, Encoding.UTF8);
                    filename = Path.GetFileNameWithoutExtension(nameExport);
                    int? id = importer.ImportXmlData("iFortuna", filename, xmlExport);
                    if (id.HasValue)
                    {
                        importer.IFortunaImport(id.Value);
                    }

                    // XmlData
                    string nameXmlData = DateTime.Now.ToString("yyyy-MM-dd_HH-MM") + "_xmlData.xml";

                    string urlXmlData = "http://ext.ifortuna.cz/xmldata?datefilter=all";
                    string xmlXmlData = webDownloader.DownloadData(urlXmlData);

                    File.WriteAllText("Data/" + nameXmlData, xmlXmlData, Encoding.UTF8);
                    filename = Path.GetFileNameWithoutExtension(nameXmlData);
                    importer.ImportXmlData("iFortuna", filename, xmlXmlData);
                }

                if (canImportTipsport)
                {
                    // oddsFeed
                    string nameOddsFeed = DateTime.Now.ToString("yyyy-MM-dd_HH-MM") + "_oddsFeed.xml";

                    string urlOddsFeed = "http://ban.tipsport.cz/f/oddsFeed.xml";
                    string xmlOddsFeed = webDownloader.DownloadData(urlOddsFeed);

                    File.WriteAllText("Data/" + nameOddsFeed, xmlOddsFeed, Encoding.UTF8);
                    filename = Path.GetFileNameWithoutExtension(nameOddsFeed);
                    importer.ImportXmlData("tipsport", filename, xmlOddsFeed);
                }
            }

            if (canImportViti)
            {
                // zpracuju web
                url = "http://www.vitisport.cz/";
                using (IWebDownloader webDownloader = new WebDownloader())
                {
                    using (IProcessor processor = new VitisportProcessor(webDownloader, url))
                    {
                        string name = DateTime.Now.ToString("yyyy-MM-dd_HH-MM") + "_viti.xml";
                        string xml = processor.Process();

                        File.WriteAllText("Data/" + name, xml, Encoding.UTF8);
                        filename = Path.GetFileNameWithoutExtension(name);
                        int? id = importer.ImportXmlData("vitisport", filename, xml);
                        if (id.HasValue)
                        {
                            importer.VitiImport(id.Value);
                        }
                    }
                }
            }

            if (canImportSofa)
            {
                url = "http://www.sofascore.com/";
                using (IWebDownloader webDownloader = new WebDownloader())
                {
                    using (IProcessor processor = new SofascoreProcessor(webDownloader, url))
                    {
                        string name = DateTime.Now.ToString("yyyy-MM-dd_HH-MM") + "_sofa.xml";
                        string xml = processor.Process();

                        File.WriteAllText("Data/" + name, xml, Encoding.UTF8);
                        filename = Path.GetFileNameWithoutExtension(name);
                        int? id = importer.ImportXmlData("sofascore", filename, xml);
                        //if (id.HasValue)
                        //{
                        //    importer.VitiImport(id.Value);
                        //}
                    }
                }
            }


        }
    }
}

