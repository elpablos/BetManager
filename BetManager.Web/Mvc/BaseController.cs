using BetManager.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Mvc
{
    public abstract class BaseController : Controller
    {
        private const string KEYFORMAT = "{0}_{1}_";
        protected SessionHelper sessionHelper;

        public string Prefix { get; set; } = string.Empty;

        public ClaimsIdentity UserIdentity { get; set; }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            UserIdentity = (ClaimsIdentity)User.Identity;

            Prefix = string.Format(KEYFORMAT,
                filterContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                filterContext.ActionDescriptor.ActionName);

            var factory = new SessionSearchValueProviderFactory();
            sessionHelper = new SessionHelper(new HttpContextSessionWrapper(Session), Prefix);
            base.OnActionExecuting(filterContext);
        }
    }
}