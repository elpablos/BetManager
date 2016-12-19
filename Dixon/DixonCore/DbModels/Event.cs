using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dixon.Core.DbModels
{
    public class Event
    {
        public int Id { get; set; }

        public string DisplayName { get; set; }

        public int ID_HomeTeam { get; set; }

        public int ID_AwayTeam { get; set; }

        public Team HomeTeam { get; set; }

        public Team AwayTeam { get; set; }

        public int HomeScoreCurrent { get; set; }

        public int AwayScoreCurrent { get; set; }

        public DateTime DateStart { get; set; }
    }
}
