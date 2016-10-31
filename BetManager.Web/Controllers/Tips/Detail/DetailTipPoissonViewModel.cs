using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Tips.Detail
{
    public class DetailTipPoissonViewModel
    {
        [Display(Name = "0 gólů")]
        public double GoalZero { get; set; }

        [Display(Name = "1 gól")]
        public double GoalOne { get; set; }

        [Display(Name = "2 góly")]
        public double GoalTwo { get; set; }

        [Display(Name = "3 góly")]
        public double GoalThree { get; set; }

        [Display(Name = "4 góly")]
        public double GoalFour { get; set; }

        [Display(Name = "5 gólů")]
        public double GoalFive { get; set; }

        [Display(Name = "Tým")]
        public string DisplayName { get; set; }

        [Display(Name = "Skóre")]
        public int? Score { get; set; }

        [Display(Name = "Tip")]
        public double Tip { get; set; }
    }
}