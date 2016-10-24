using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.Mvc.Filters
{
    public interface ISessionWrapper
    {
        object GetSessionValue(string key);
        void SetSessionValue(string key, object value);
        void DeleteSessionValue(string key);
        bool ContainsValue(string key);
    }
}