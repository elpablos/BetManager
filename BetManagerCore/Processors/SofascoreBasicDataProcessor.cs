using BetManager.Core.Webs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Processors
{
    public class SofascoreBasicDataProcessor : BaseProcessor
    {

        public SofascoreBasicDataProcessor(IWebDownloader webDownloader, string url)
            :base(webDownloader, url)
        { }

        public override string Process()
        {
            string ret = null;



            return ret;
        }
    }
}
