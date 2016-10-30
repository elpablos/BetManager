using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Tips.Detail
{
    public class DetailTipViewModel
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

        [Display(Name = "Rozdíl forem")]
        public int Form { get; set; }

        [Display(Name = "Kurz")]
        public double Odd { get; set; }

        [Display(Name = "Forma")]
        public int HomeLastForm { get; set; }

        [Display(Name = "Vstřelené góly")]
        public int HomeLastGiven { get; set; }

        [Display(Name = "Obdržené góly")]
        public int HomeLastTaken { get; set; }

        [Display(Name = "Forma")]
        public int AwayLastForm { get; set; }

        [Display(Name = "Střelené góly")]
        public int AwayLastGiven { get; set; }

        [Display(Name = "Obdržené góly")]
        public int AwayLastTaken { get; set; }

        [Display(Name = "Forma")]
        public int HomeSeasonForm { get; set; }

        [Display(Name = "Vstřelené góly")]
        public int HomeSeasonGiven { get; set; }

        [Display(Name = "Obdržené góly")]
        public int HomeSeasonTaken { get; set; }

        [Display(Name = "Kolo")]
        public int HomeSeasonCount { get; set; }

        [Display(Name = "Forma")]
        public int AwaySeasonForm { get; set; }

        [Display(Name = "Vstřelené góly")]
        public int AwaySeasonGiven { get; set; }

        [Display(Name = "Obdržené góly")]
        public int AwaySeasonTaken { get; set; }

        [Display(Name = "Kolo")]
        public int AwaySeasonCount { get; set; }

        public int? FirstId { get; set; }

        [Display(Name = "Kurz - domácí")]
        public decimal FirstValue { get; set; }

        [Display(Name = "Kurz - remíza")]
        public decimal Xvalue { get; set; }

        public int? XId { get; set; }

        [Display(Name = "Kurz - hosté")]
        public decimal SecondValue { get; set; }

        public int? SecondId { get; set; }

        public virtual ICollection<DetailTipPoissonViewModel> Poissons { get; set; }

        public DetailTipViewModel()
        {
            Poissons = new List<DetailTipPoissonViewModel>();
        }
    }
}