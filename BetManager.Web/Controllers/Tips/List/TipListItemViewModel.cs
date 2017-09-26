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

        [Display(Name = "Kategorie")]
        public string Category { get; set; }

        [Display(Name = "Tip")]
        public int PredictTip { get; set; }

        [Display(Name = "Kurz")]
        public double Odd { get; set; }

        [Display(Name = "Kód vítěze")]
        public int WinnerCode { get; set; }

        [Display(Name = "1%")]
        [DisplayFormat(DataFormatString = "{0:00.0}", ApplyFormatInEditMode = true)]
        public decimal Home { get; set; }

        [Display(Name = "X%")]
        [DisplayFormat(DataFormatString = "{0:00.0}", ApplyFormatInEditMode = true)]
        public decimal Draw { get; set; }

        [Display(Name = "2%")]
        [DisplayFormat(DataFormatString = "{0:00.0}", ApplyFormatInEditMode = true)]
        public decimal Away { get; set; }

        [Display(Name = "Kurz: 1")]
        [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)]
        public decimal FirstValue { get; set; }

        [Display(Name = "Kurz: X")]
        [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)]
        public decimal Xvalue { get; set; }

        [Display(Name = "Kurz: 2")]
        [DisplayFormat(DataFormatString = "{0:0.0}", ApplyFormatInEditMode = true)]
        public decimal SecondValue { get; set; }
    }
}