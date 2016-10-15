using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.DbModels
{
    public class Tip
    {
        public int ID { get; set; }

        [Display(Name = "Název")]
        public string DisplayName { get; set; }

        [Display(Name = "Datum")]
        public DateTime DateStart { get; set; }

        [Display(Name = "Url")]
        public string Url { get; set; }

        [Display(Name = "Forma")]
        public int Form { get; set; }

        [Display(Name = "Kurz")]
        public decimal Odd { get; set; }

        [Display(Name = "Domácí - forma")]
        public int HomeForm { get; set; }

        [Display(Name = "Domácí - Střelené góly")]
        public int HomeGiven { get; set; }

        [Display(Name = "Domácí - Obdržené góly")]
        public int HomeTaken { get; set; }

        [Display(Name = "Hosté - forma")]
        public int AwayForm { get; set; }

        [Display(Name = "Hosté - Střelené góly")]
        public int AwayGiven { get; set; }

        [Display(Name = "Hosté - Obdržené góly")]
        public int AwayTaken { get; set; }

        [Display(Name = "Kurz - domácí")]
        public decimal FirstValue { get; set; }

        [Display(Name = "Kurz - remíza")]
        public decimal Xvalue { get; set; }

        [Display(Name = "Kurz - hosté")]
        public decimal SecondValue { get; set; }
    }
}
