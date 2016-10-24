using System;
using System.Web.ModelBinding;
using System.Web.Mvc;

namespace BetManager.Web.Mvc.Filters
{
    public class SessionSearchValueProviderFactory : ValueProviderFactory
    {
        private const string KEYFORMAT = "{0}_{1}_";

        public override System.Web.Mvc.IValueProvider GetValueProvider(ControllerContext controllerContext)
        {
            string prefix = string.Format(KEYFORMAT,
                controllerContext.RouteData.Values["controller"].ToString(),
                controllerContext.RouteData.Values["action"].ToString());

            ISessionWrapper sessionWrapper = new HttpContextSessionWrapper(controllerContext.HttpContext.Session);
            return new SessionSearchValueProvider(sessionWrapper, prefix);
        }
    }
}