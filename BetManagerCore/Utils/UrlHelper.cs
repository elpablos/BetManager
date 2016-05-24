using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BetManager.Core.Utils
{
    public static class UrlHelper
    {

        public static string ParseUrl(string url, string param)
        {
            return HttpUtility.ParseQueryString(url).Get(param);
        }
    }
}
