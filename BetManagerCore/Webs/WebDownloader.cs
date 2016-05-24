using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BetManager.Core.Webs
{
  public class WebDownloader : IDisposable, IWebDownloader
    {
        #region Fields

        private bool _IsDisposed;
        private string _userAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.102 Safari/537.36";

        #endregion

        #region Properties

        public WebClient Client { get; private set; }

        public Random Random { get; private set; }

        public bool IsDisposed { get; set; }
        
        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        public bool UseDelay { get; set; }

        #endregion

        #region Constructors

        public WebDownloader()
        {
            Client = new WebClient();
            Client.Disposed += Client_Disposed;
            Random = new Random(DateTime.Now.Millisecond);
            UseDelay = true;
        }

        #endregion

        #region Public methods

        public string DownloadData(string url, Encoding encoding = null)
        {
            System.Diagnostics.Trace.WriteLine(string.Format("Start downloading {0}", url), "WebDownloader");
            Client.Headers[HttpRequestHeader.UserAgent] = UserAgent;
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }

            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            byte[] bytes = Client.DownloadData(url);
            string result = Encoding.UTF8.GetString(bytes);

            System.Diagnostics.Trace.WriteLine(string.Format("Data downloaded - {0} bytes", bytes.Length), "WebDownloader");

            if (UseDelay)
            {
                Thread.Sleep(1000 + Random.Next(1, 5) * 200);
            }

            return result;
        }

        #endregion

        #region IDisposable

        private void Client_Disposed(object sender, EventArgs e)
        {
            _IsDisposed = true;
        }

        public void Dispose()
        {
            lock (this)
            {
                if (Client != null && !_IsDisposed)
                {
                    Client.Dispose();

                    Client = null;
                    _IsDisposed = true;
                }

                IsDisposed = true;
            }
        }

        #endregion
    }
}
