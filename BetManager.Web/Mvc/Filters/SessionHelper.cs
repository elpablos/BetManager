using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Mvc.Filters
{
    /// <summary>
    /// TODO vylepšit tak, aby uměl i strukturu pod!
    /// </summary>
    public class SessionHelper
    {
        private ISessionWrapper _sessionWrapper;
        private string _prefix;

        public SessionHelper(ISessionWrapper sessionWrapper, string prefix)
        {
            _sessionWrapper = sessionWrapper;
            _prefix = prefix;
        }

        public void Remember(object model)
        {
           
            foreach (var prop in model.GetType().GetProperties())
            {
                var persistAttr = prop.GetCustomAttribute(typeof(PersistInSessionAttribute));
                string propName = _prefix + prop.Name;
                if (persistAttr != null)
                {
                    _sessionWrapper.SetSessionValue(propName, prop.GetValue(model, null));
                }
            }
        }

        public void Forget(Type modelType)
        {
            foreach (var prop in modelType.GetProperties())
            {
                string propName = _prefix + prop.Name;
                var persistAttr = prop.GetCustomAttribute(typeof(PersistInSessionAttribute));
                if (persistAttr != null)
                {
                    _sessionWrapper.DeleteSessionValue(propName);
                }
            }
        }
    }
}