using System;
using System.ComponentModel.DataAnnotations;

namespace BetManager.Web.Controllers.Predictions.List
{
    /// <summary>
    /// Položka seznamu odhadů
    /// </summary>
    public class PredictionListItemViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Název")]
        public string DisplayName { get; set; }

        [Display(Name = "Datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Rozdíl forem")]
        public int Form { get; set; }

        [Display(Name = "Kurz")]
        public double Odd { get; set; }

        [Display(Name = "Kód vítěze")]
        public int WinnerCode { get; set; }
    }
}