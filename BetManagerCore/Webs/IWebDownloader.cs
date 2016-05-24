using System;
using System.Text;

namespace BetManager.Core.Webs
{
    public interface IWebDownloader : IDisposable
    {
        bool IsDisposed { get; set; }

        string DownloadData(string url, Encoding encoding = null);
    }
}