using BetManager.Web.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace BetManager.Web.Controllers.Tips.List
{
    /// <summary>
    /// Filtr pro tipy
    /// </summary>
    public class TipListFilterViewModel
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
        [Display(Name = "Forma")]
        public int Form { get; set; }

        public TipListFilterViewModel()
        {
        }

        public void SetDefault(ClaimsIdentity identity)
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            Odd = double.Parse(identity.Claims.First(x => x.Type == "Odd").Value);
            Form = int.Parse(identity.Claims.First(x => x.Type == "Form").Value);
        }

        public void SetDefaultDashboard(ClaimsIdentity identity)
        {
            DateFrom = DateTime.Now.AddDays(-8);
            DateTo = DateTime.Now.AddDays(-1);
            Odd = double.Parse(identity.Claims.First(x => x.Type == "Odd").Value);
            Form = int.Parse(identity.Claims.First(x => x.Type == "Form").Value);
        }
    }
}