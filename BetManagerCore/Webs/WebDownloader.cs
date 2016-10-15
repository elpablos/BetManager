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

        public bool TryTempData { get; set; }

        public string UserAgent
        {
            get { return _userAgent; }
            set { _userAgent = value; }
        }

        public bool UseDelay { get; set; }

        public bool UseTime { get; set; }

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
            // generate filename
            string path = url.ToLower().Replace("http://", string.Empty).Replace("https://", string.Empty).Replace("/", "-").Replace("?", "-");
            if (path.EndsWith("json")) { path += ".json"; }
            else if (path.EndsWith("xml")) { path += ".xml"; }
            else { path += ".htm"; }

            if (UseTime)
            {
                path = string.Format("{0}-{1:00}_{2}", DateTime.Now.ToString("yyyy-MM-dd_hh"), (DateTime.Now.Minute / 15) * 15, path);
            }

            path = "Data/Temp/" + path;

            // encoding
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            // reading
            string result = null;
            if (TryTempData && System.IO.File.Exists(path))
            {
                System.Diagnostics.Trace.WriteLine(string.Format(" {0}", url), "WebDownloader");
                result = System.IO.File.ReadAllText(path, encoding);
            }
            else
            {
                System.Diagnostics.Trace.WriteLine(string.Format("Start downloading {0}", url), "WebDownloader");
                Client.Headers[HttpRequestHeader.UserAgent] = UserAgent;
                if (url == null)
                {
                    throw new ArgumentNullException("url");
                }

                try
                {
                    byte[] bytes = Client.DownloadData(url);
                    result = Encoding.UTF8.GetString(bytes);

                    System.Diagnostics.Trace.WriteLine(string.Format("Data downloaded - {0} bytes", bytes.Length), "WebDownloader");
                }
                catch (Exception)
                {
                    result = string.Empty;
                    System.Diagnostics.Trace.WriteLine(string.Format("Data downloaded failed"), "WebDownloader");
                }

                if (TryTempData)
                {
                    System.IO.File.WriteAllText(path, result, encoding);
                }

                // delay
                if (UseDelay)
                {
                    Thread.Sleep(1000 + Random.Next(1, 5) * 200);
                }

                
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
