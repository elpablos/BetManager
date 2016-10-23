﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetManager.Web.ApiControllers.Tips.Graph
{
    public class GraphTipViewModel
    {
        public DateTime DateStart { get; set; }

        public int Total { get; set; }

        public int Correct { get; set; }

        public double Price { get; set; }

    }
}