using System;
using System.Globalization;
using System.Web.Mvc;

namespace BetManager.Web.Mvc.Filters
{
    public class SessionSearchValueProvider : IValueProvider
    {
        private ISessionWrapper session;
        private string _prefix;

        public SessionSearchValueProvider(ISessionWrapper session, string prefix)
        {
            this.session = session;
            _prefix = prefix;
        }

        public bool ContainsPrefix(string prefix)
        {
            return session.ContainsValue(_prefix + prefix);
        }

        public ValueProviderResult GetValue(string key)
        {
            object value = session.GetSessionValue(_prefix + key);
            if (value == null)
            {
                return null;
            }
            return new ValueProviderResult(value, value.ToString(), CultureInfo.CurrentCulture);
        }
    }
}