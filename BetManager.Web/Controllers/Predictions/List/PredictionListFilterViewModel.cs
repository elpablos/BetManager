using BetManager.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace BetManager.Web.Controllers.Predictions.List
{
    public class PredictionListFilterViewModel
    {
        [PersistInSession]
        [Display(Name = "Datum od")]
        public DateTime? DateFrom { get; set; }

        [PersistInSession]
        [Display(Name = "Datum do")]
        public DateTime? DateTo { get; set; }

        public PredictionListFilterViewModel()
        {

        }

        public void SetDefault(ClaimsIdentity identity)
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
        }
    }
}