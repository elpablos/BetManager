using BetManager.Web.Mvc.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace BetManager.Web.Mvc
{
    public class ModelBuilderResult<TModel> : IModelBuilderResponse
    {
        public string Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public TModel Model { get; set; }
    }
}