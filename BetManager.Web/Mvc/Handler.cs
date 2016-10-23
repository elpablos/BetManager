using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BetManager.Web.Mvc
{
    public static class Handler
    {
        public static THandler Get<THandler>() where THandler : class
        {
            return DependencyResolver.Current.GetService<THandler>();
        }
    }
}