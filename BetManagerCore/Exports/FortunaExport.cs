using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.FortunaExport
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class EBetOdds
    {

        private EBetOddsCategory[] categoryField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Category")]
        public EBetOddsCategory[] Category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategory
    {

        private EBetOddsCategoryText[] textsField;

        private EBetOddsCategoryCategory categoryField;

        private short idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Text", IsNullable = false)]
        public EBetOddsCategoryText[] Texts
        {
            get
            {
                return this.textsField;
            }
            set
            {
                this.textsField = value;
            }
        }

        /// <remarks/>
        public EBetOddsCategoryCategory Category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public short Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryText
    {

        private string typeField;

        private string languageField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategory
    {

        private object[] itemsField;

        private short idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Competition", typeof(EBetOddsCategoryCategoryCompetition))]
        [System.Xml.Serialization.XmlElementAttribute("Texts", typeof(EBetOddsCategoryCategoryTexts))]
        [System.Xml.Serialization.XmlElementAttribute("Tournament", typeof(EBetOddsCategoryCategoryTournament))]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public short Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetition
    {

        private EBetOddsCategoryCategoryCompetitionFixture fixtureField;

        private EBetOddsCategoryCategoryCompetitionCompetitionOdds competitionOddsField;

        private ushort idField;

        /// <remarks/>
        public EBetOddsCategoryCategoryCompetitionFixture Fixture
        {
            get
            {
                return this.fixtureField;
            }
            set
            {
                this.fixtureField = value;
            }
        }

        /// <remarks/>
        public EBetOddsCategoryCategoryCompetitionCompetitionOdds CompetitionOdds
        {
            get
            {
                return this.competitionOddsField;
            }
            set
            {
                this.competitionOddsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionFixture
    {

        private EBetOddsCategoryCategoryCompetitionFixtureEventInfo eventInfoField;

        private EBetOddsCategoryCategoryCompetitionFixtureCompetitor[] competitorsField;

        /// <remarks/>
        public EBetOddsCategoryCategoryCompetitionFixtureEventInfo EventInfo
        {
            get
            {
                return this.eventInfoField;
            }
            set
            {
                this.eventInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Competitor", IsNullable = false)]
        public EBetOddsCategoryCategoryCompetitionFixtureCompetitor[] Competitors
        {
            get
            {
                return this.competitorsField;
            }
            set
            {
                this.competitorsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionFixtureEventInfo
    {

        private string eventDateField;

        private EBetOddsCategoryCategoryCompetitionFixtureEventInfoText[] textsField;

        private string statusField;

        /// <remarks/>
        public string EventDate
        {
            get
            {
                return this.eventDateField;
            }
            set
            {
                this.eventDateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Text", IsNullable = false)]
        public EBetOddsCategoryCategoryCompetitionFixtureEventInfoText[] Texts
        {
            get
            {
                return this.textsField;
            }
            set
            {
                this.textsField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionFixtureEventInfoText
    {

        private string typeField;

        private string languageField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionFixtureCompetitor
    {

        private EBetOddsCategoryCategoryCompetitionFixtureCompetitorText[] textsField;

        private uint idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Text", IsNullable = false)]
        public EBetOddsCategoryCategoryCompetitionFixtureCompetitorText[] Texts
        {
            get
            {
                return this.textsField;
            }
            set
            {
                this.textsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionFixtureCompetitorText
    {

        private string typeField;

        private string languageField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionCompetitionOdds
    {

        private EBetOddsCategoryCategoryCompetitionCompetitionOddsBet betField;

        /// <remarks/>
        public EBetOddsCategoryCategoryCompetitionCompetitionOddsBet Bet
        {
            get
            {
                return this.betField;
            }
            set
            {
                this.betField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionCompetitionOddsBet
    {

        private EBetOddsCategoryCategoryCompetitionCompetitionOddsBetOdds[] oddsField;

        private string oddsTypeField;

        private uint idField;

        private ushort infoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Odds")]
        public EBetOddsCategoryCategoryCompetitionCompetitionOddsBetOdds[] Odds
        {
            get
            {
                return this.oddsField;
            }
            set
            {
                this.oddsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OddsType
        {
            get
            {
                return this.oddsTypeField;
            }
            set
            {
                this.oddsTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public ushort Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryCompetitionCompetitionOddsBetOdds
    {

        private uint competitorIdField;

        private string specialBetValueField;

        private uint idField;

        private bool idFieldSpecified;

        private string closingField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint CompetitorId
        {
            get
            {
                return this.competitorIdField;
            }
            set
            {
                this.competitorIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SpecialBetValue
        {
            get
            {
                return this.specialBetValueField;
            }
            set
            {
                this.specialBetValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool IDSpecified
        {
            get
            {
                return this.idFieldSpecified;
            }
            set
            {
                this.idFieldSpecified = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Closing
        {
            get
            {
                return this.closingField;
            }
            set
            {
                this.closingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTexts
    {

        private EBetOddsCategoryCategoryTextsText[] textField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Text")]
        public EBetOddsCategoryCategoryTextsText[] Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTextsText
    {

        private string typeField;

        private string languageField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournament
    {

        private EBetOddsCategoryCategoryTournamentText[] textsField;

        private EBetOddsCategoryCategoryTournamentMatch[] matchField;

        private short idField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Text", IsNullable = false)]
        public EBetOddsCategoryCategoryTournamentText[] Texts
        {
            get
            {
                return this.textsField;
            }
            set
            {
                this.textsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Match")]
        public EBetOddsCategoryCategoryTournamentMatch[] Match
        {
            get
            {
                return this.matchField;
            }
            set
            {
                this.matchField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public short Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentText
    {

        private string typeField;

        private string languageField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatch
    {

        private EBetOddsCategoryCategoryTournamentMatchFixture fixtureField;

        private EBetOddsCategoryCategoryTournamentMatchBet[] matchOddsField;

        private uint idField;

        private int infoField;

        /// <remarks/>
        public EBetOddsCategoryCategoryTournamentMatchFixture Fixture
        {
            get
            {
                return this.fixtureField;
            }
            set
            {
                this.fixtureField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Bet", IsNullable = false)]
        public EBetOddsCategoryCategoryTournamentMatchBet[] MatchOdds
        {
            get
            {
                return this.matchOddsField;
            }
            set
            {
                this.matchOddsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchFixture
    {

        private EBetOddsCategoryCategoryTournamentMatchFixtureEventInfo eventInfoField;

        private EBetOddsCategoryCategoryTournamentMatchFixtureCompetitor[] competitorsField;

        private uint[] correlationField;

        /// <remarks/>
        public EBetOddsCategoryCategoryTournamentMatchFixtureEventInfo EventInfo
        {
            get
            {
                return this.eventInfoField;
            }
            set
            {
                this.eventInfoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Competitor", IsNullable = false)]
        public EBetOddsCategoryCategoryTournamentMatchFixtureCompetitor[] Competitors
        {
            get
            {
                return this.competitorsField;
            }
            set
            {
                this.competitorsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("CorrelationID", IsNullable = false)]
        public uint[] Correlation
        {
            get
            {
                return this.correlationField;
            }
            set
            {
                this.correlationField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchFixtureEventInfo
    {

        private string eventDateField;

        private EBetOddsCategoryCategoryTournamentMatchFixtureEventInfoTexts textsField;

        private string statusField;

        /// <remarks/>
        public string EventDate
        {
            get
            {
                return this.eventDateField;
            }
            set
            {
                this.eventDateField = value;
            }
        }

        /// <remarks/>
        public EBetOddsCategoryCategoryTournamentMatchFixtureEventInfoTexts Texts
        {
            get
            {
                return this.textsField;
            }
            set
            {
                this.textsField = value;
            }
        }

        /// <remarks/>
        public string Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchFixtureEventInfoTexts
    {

        private EBetOddsCategoryCategoryTournamentMatchFixtureEventInfoTextsText textField;

        /// <remarks/>
        public EBetOddsCategoryCategoryTournamentMatchFixtureEventInfoTextsText Text
        {
            get
            {
                return this.textField;
            }
            set
            {
                this.textField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchFixtureEventInfoTextsText
    {

        private string typeField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchFixtureCompetitor
    {

        private EBetOddsCategoryCategoryTournamentMatchFixtureCompetitorText[] textsField;

        private uint idField;

        private byte typeField;

        private bool typeFieldSpecified;

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Text", IsNullable = false)]
        public EBetOddsCategoryCategoryTournamentMatchFixtureCompetitorText[] Texts
        {
            get
            {
                return this.textsField;
            }
            set
            {
                this.textsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public byte Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool TypeSpecified
        {
            get
            {
                return this.typeFieldSpecified;
            }
            set
            {
                this.typeFieldSpecified = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchFixtureCompetitorText
    {

        private string typeField;

        private string languageField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Type
        {
            get
            {
                return this.typeField;
            }
            set
            {
                this.typeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchBet
    {

        private EBetOddsCategoryCategoryTournamentMatchBetOdds[] oddsField;

        private string oddsTypeField;

        private uint idField;

        private int infoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Odds")]
        public EBetOddsCategoryCategoryTournamentMatchBetOdds[] Odds
        {
            get
            {
                return this.oddsField;
            }
            set
            {
                this.oddsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OddsType
        {
            get
            {
                return this.oddsTypeField;
            }
            set
            {
                this.oddsTypeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int Info
        {
            get
            {
                return this.infoField;
            }
            set
            {
                this.infoField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class EBetOddsCategoryCategoryTournamentMatchBetOdds
    {

        private string outComeField;

        private uint idField;

        private string specialBetValueField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string OutCome
        {
            get
            {
                return this.outComeField;
            }
            set
            {
                this.outComeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public uint ID
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SpecialBetValue
        {
            get
            {
                return this.specialBetValueField;
            }
            set
            {
                this.specialBetValueField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }


}
