using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Tips.Detail
{
    public class DetailTipTeamViewModel
    {
        [Display(Name = "ID")]
        public int ID_Team { get; set; }

        [Display(Name = "Název")]
        public string DisplayName { get; set; }

        [Display(Name = "Forma")]
        public int Form { get; set; }

        [Display(Name = "Útok")]
        public int Attack { get; set; }

        [Display(Name = "Obrana")]
        public int Defence { get; set; }

        [Display(Name = "Vstřelené góly")]
        public int Given { get; set; }

        [Display(Name = "Obdržené góly")]
        public int Taken { get; set; }

        [Display(Name = "Počet")]
        public int Count { get; set; }

        public virtual ICollection<DetailTipTeamItemViewModel> Events { get; set; }

        public DetailTipTeamViewModel()
        {
            Events = new List<DetailTipTeamItemViewModel>();
        }
    }

    public class DetailTipTeamItemViewModel
    {
        [Display(Name = "ID")]
        public int ID_Team { get; set; }

        [Display(Name = "Název")]
        public string DisplayName { get; set; }

    }
}