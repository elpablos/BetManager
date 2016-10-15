using BetManager.Core.Webs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManagerEuro
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = null;

            using (IWebDownloader webDownloader = new WebDownloader())
            {
                webDownloader.TryTempData = true;
                webDownloader.UseTime = true;

                // FR - RU
                url = "http://www.sofascore.com/event/6964817/json";
                string name = "2016-05-30-sofa-tennis.xml";
                string json = webDownloader.DownloadData(url);
                //var sofaSport = JsonConvert.DeserializeObject<Core.Exports.SofascoreExport.SofaSport>(json);
                //var xml = XmlHelper.Serializable(sofaSport);
                //File.WriteAllText("Data/" + name, xml, Encoding.UTF8);
            }
        }
    }
}
