using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Accounts.Detail
{
    public class DetailAccountViewModel
    {
        public int ID { get; set; }

        [Display(Name = "Login")]
        public string UserName { get; set; }

        [Display(Name = "Datum posledního přihlášení")]
        public DateTime? LastLogin { get; set; }

        [Display(Name = "Výchozí kurz")]
        public double Odd { get; set; }

        [Display(Name = "Výchozí forma")]
        public int Form { get; set; }

    }
}