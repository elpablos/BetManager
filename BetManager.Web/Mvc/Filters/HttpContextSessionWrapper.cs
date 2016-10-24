using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.Mvc.Filters
{
    public class HttpContextSessionWrapper : ISessionWrapper
    {
        private HttpSessionStateBase sessionState;

        public HttpContextSessionWrapper(HttpSessionStateBase sessionState)
        {
            this.sessionState = sessionState;
        }

        public object GetSessionValue(string key)
        {
            if (sessionState == null)
            {
                return null;
            }
            return sessionState[key];
        }

        public void SetSessionValue(string key, object value)
        {
            if (sessionState == null)
            {
                return;
            }
            sessionState[key] = value;
        }

        public bool ContainsValue(string key)
        {
            if (sessionState == null)
            {
                return false;
            }
            return sessionState[key] != null;
        }

        public void DeleteSessionValue(string key)
        {
            if (sessionState == null)
            {
                return;
            }
            sessionState.Remove(key);
        }
    }
}