using BetManager.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace BetManager.Web.Controllers.Tips.UnderUpper
{
    public class TipUnderUpperFilterViewModel
    {
        [PersistInSession]
        [Display(Name = "Datum od")]
        public DateTime? DateFrom { get; set; }

        [PersistInSession]
        [Display(Name = "Datum do")]
        public DateTime? DateTo { get; set; }

        [PersistInSession]
        [Display(Name = "Kurz")]
        public double Odd { get; set; }

        [PersistInSession]
        [Display(Name = "Kategorie")]
        public string Category { get; set; }

        public TipUnderUpperFilterViewModel()
        {
        }

        public void SetDefault(ClaimsIdentity identity)
        {
            DateFrom = DateTime.Now.AddDays(-1);
            DateTo = DateTime.Now.AddDays(1);
            Odd = double.Parse(identity.Claims.First(x => x.Type == "Odd").Value);
            Category = identity.Claims.First(x => x.Type == "Category").Value;
        }
    }
}