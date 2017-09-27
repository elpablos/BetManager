using System;
using System.ComponentModel.DataAnnotations;

namespace BetManager.Web.Controllers.Tips.UnderUpper
{
    public class TipUnderUpperItemViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Název")]
        public string DisplayName { get; set; }

        [Display(Name = "Datum")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DateStart { get; set; }

        [Display(Name = "Kategorie")]
        public string Category { get; set; }

        [Display(Name = "1.5-")]
        [DisplayFormat(DataFormatString = "{0:00.0}", ApplyFormatInEditMode = true)]
        public decimal OneAndHalfMinus { get; set; }

        [Display(Name = "1.5+")]
        [DisplayFormat(DataFormatString = "{0:00.0}", ApplyFormatInEditMode = true)]
        public decimal OneAndHalfPlus { get; set; }

        [Display(Name = "2.5-")]
        [DisplayFormat(DataFormatString = "{0:00.0}", ApplyFormatInEditMode = true)]
        public decimal TwoAndHalfMinus { get; set; }

        [Display(Name = "2.5+")]
        [DisplayFormat(DataFormatString = "{0:00.0}", ApplyFormatInEditMode = true)]
        public decimal TwoAndHalfPlus { get; set; }

        [Display(Name = "Góly")]
        public int? Goals { get; set; }

        [Display(Name = "Kód vítěze")]
        public int WinnerCode { get; set; }
    }
}