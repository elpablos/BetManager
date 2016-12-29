using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Predictions.Detail
{
    /// <summary>
    /// Detail odhadu
    /// </summary>
    public class PredictionDetailTipViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Kód vítěze")]
        public int WinnerCode { get; set; }

        [Display(Name = "Název")]
        public string DisplayName { get; set; }

        [Display(Name = "Sezóna")]
        public string Season { get; set; }

        [Display(Name = "Kategorie")]
        public string Category { get; set; }

        [Display(Name = "Domácí")]
        public string HomeTeam { get; set; }

        [Display(Name = "Hosté")]
        public string AwayTeam { get; set; }

        [Display(Name = "Datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }
    }
}