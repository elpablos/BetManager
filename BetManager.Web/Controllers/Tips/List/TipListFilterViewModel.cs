using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Tips.List
{
    /// <summary>
    /// Filtr pro tipy
    /// </summary>
    public class TipListFilterViewModel
    {
        [Display(Name = "Datum od")]
        public DateTime? DateFrom { get; set; }

        [Display(Name = "Datum do")]
        public DateTime? DateTo { get; set; }

        [Display(Name = "Kurz")]
        public double Odd { get; set; }

        [Display(Name = "Forma")]
        public int Form { get; set; }

        public TipListFilterViewModel()
        {
        }

        public void SetDefault()
        {
            DateFrom = DateTime.Now;
            DateTo = DateTime.Now;
            Odd = 2.0;
            Form = 30;
        }

        public void SetDefaultDashboard()
        {
            DateFrom = DateTime.Now.AddDays(-8);
            DateTo = DateTime.Now.AddDays(-1);
            Odd = 2.0;
            Form = 30;
        }
    }
}