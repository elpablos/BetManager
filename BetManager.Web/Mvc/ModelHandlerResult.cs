﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace BetManager.Web.Mvc
{
    public class ModelHandlerResult
    {
        public string Message { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public string Property { get; set; }
        public object Data { get; set; }
    }
}