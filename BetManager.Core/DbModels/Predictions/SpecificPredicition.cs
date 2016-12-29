using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.DbModels.Predictions
{
    public class SpecificPredicition
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
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd hh:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }
    }
}
