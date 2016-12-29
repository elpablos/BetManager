using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace BetManager.Web.Controllers.Admins.Jobs.Create
{
    public class CreateJobModel
    {
        [Required]
        public int? TournamentId { get; set; }

        [Required]
        public DateTime? DateActual { get; set; }

        [Required]
        public double? Ksi { get; set; }
    }
}