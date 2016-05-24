using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BetManager.Core.VitiBetExport
{
    [Serializable]
    [XmlRootAttribute]
    public class VitiBetRoot
    {
        public DateTime DateGenerated { get; set; }

        public Section[] Sections { get; set; }
    }

    [Serializable]
    public class Section
    {
        public string Key { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public League[] Leagues { get; set; }

        public string Url { get; set; }

        public override int GetHashCode()
        {
            return Key != null ? Key.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is Section && ((Section)obj).Key == Key;
        }
    }

    [Serializable]
    public class League
    {
        public string Key { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public Tip[] Tips { get; set; }

        public override int GetHashCode()
        {
            return Key != null ? Key.GetHashCode() : 0;
        }

        public override bool Equals(object obj)
        {
            return obj != null && obj is League && ((League)obj).Key == Key;
        }
    }

    public class Tip
    {
        public string DisplayName { get; set; }

        public string MyProperty { get; set; }

        public DateTime EventDate { get; set; }

        public string CompetitorHome { get; set; }

        public string CompetitorAway { get; set; }

        public int? TipHomeScore { get; set; }

        public int? TipAwayScore { get; set; }

        public int? TipHomePercent { get; set; }

        public int? TipDrawPercent { get; set; }

        public int? TipAwayPercent { get; set; }

        public string TipBet { get; set; }

        public decimal? Index { get; set; }

        public string Url { get; set; }
        
    }
}
