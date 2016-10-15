using System;
using System.Text;

namespace BetManager.Core.Webs
{
    public interface IWebDownloader : IDisposable
    {
        bool TryTempData { get; set; }

        bool IsDisposed { get; set; }

        string DownloadData(string url, Encoding encoding = null);

        bool UseTime { get; set; }
    }
}