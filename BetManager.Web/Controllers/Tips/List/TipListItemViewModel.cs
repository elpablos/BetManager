using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Tips.List
{
    /// <summary>
    /// Položka seznamu tipů
    /// </summary>
    public class TipListItemViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Název")]
        public string DisplayName { get; set; }

        [Display(Name = "Datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Tip")]
        public int PredictTip { get; set; }

        [Display(Name = "Kurz")]
        public double Odd { get; set; }

        [Display(Name = "Kód vítěze")]
        public int WinnerCode { get; set; }
    }
}