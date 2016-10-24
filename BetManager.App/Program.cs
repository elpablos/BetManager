using BetManager.Core.Domains.ImportDatas;
using BetManager.Core.Domains.Tips;
using BetManager.Core.Processors;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.App
{
    class Program
    {
        static void Main(string[] args)
        {

            var sw = new Stopwatch();
            sw.Start();
            var processor = new ImportDataProcessor();
            string json = File.ReadAllText(@"d:\Other\Sofascore\pwshell\Cache\2016-10-24-football-.json", Encoding.UTF8);
            var collection = processor.ProcessData(json);

            IImportDataManager importManager = new ImportDataManager();
            ITipManager tipManager = new TipManager();

            Console.WriteLine("Elapsed {0}:{1}.{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
            importManager.ImportClear();
            Console.WriteLine("Elapsed {0}:{1}.{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
            importManager.InsertBulk(collection);
            Console.WriteLine("Elapsed {0}:{1}.{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
            importManager.ImportData();
            Console.WriteLine("Elapsed {0}:{1}.{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
            tipManager.TipGenerate();

            sw.Stop();

            Console.WriteLine("Total Elapsed {0}:{1}.{2}", sw.Elapsed.Minutes, sw.Elapsed.Seconds, sw.Elapsed.Milliseconds);
            Console.ReadKey();
        }
    }
}
