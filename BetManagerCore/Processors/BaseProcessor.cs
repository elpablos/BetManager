using BetManager.Core.Webs;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Processors
{
    public abstract class BaseProcessor : IProcessor
    {
        #region Properties

        public string Url { get; private set; }

        public HtmlDocument Document { get; private set; }

        public IWebDownloader WebDownloader { get; private set; }

        #endregion

        #region Constructor

        public BaseProcessor(IWebDownloader webDownloader, string url)
        {
            Url = url;
            Document = new HtmlDocument();
            WebDownloader = webDownloader;
        }

        #endregion

        #region Abstract

        public abstract string Process();

        #endregion

        #region IDisposable

        public void Dispose()
        {
            if (WebDownloader != null && !WebDownloader.IsDisposed)
            {
                WebDownloader.Dispose();
                WebDownloader = null;
            }

            if (Document != null)
            {
                Document = null;
            }
        }

        #endregion
    }
}
