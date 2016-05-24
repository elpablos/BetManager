using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetManager.Core.Exports.SofascoreExport
{
    [Serializable]
    public class Sofascore
    {
        public DateTime DateGenerated { get; set; }
        public SofaSport[] Sports { get; set; }
    }

    #region Matches

    public class SofaSport
    {
        public Sportitem sportItem { get; set; }
        public Params _params { get; set; }
        public bool isShortDate { get; set; }
    }

    public class Sportitem
    {
        public Sport sport { get; set; }
        public int rows { get; set; }
        public Tournament[] tournaments { get; set; }
    }

    public class Sport
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
    }

    public class Tournament
    {
        public TournamentDetail tournament { get; set; }
        public Category category { get; set; }
        public Season season { get; set; }
        public bool hasEventPlayerStatistics { get; set; }
        public bool hasEventPlayerHeatMap { get; set; }
        public Event[] events { get; set; }
    }

    public class TournamentDetail
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int priority { get; set; }
        public int order { get; set; }
        public int? uniqueId { get; set; }
        public string uniqueName { get; set; }
    }

    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public int? priority { get; set; }
        public object[] mcc { get; set; }
        public string flag { get; set; }
    }

    public class Season
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string year { get; set; }
    }

    public class Event
    {
        public int id { get; set; }
        public string customId { get; set; }
        public Score homeScore { get; set; }
        public Score awayScore { get; set; }
        public Status status { get; set; }
        public int winnerCode { get; set; }
        public Changes changes { get; set; }
        public Roundinfo roundInfo { get; set; }
        public Sport sport { get; set; }
        public Team homeTeam { get; set; }
        public Team awayTeam { get; set; }
        public Odds odds { get; set; }
        public bool hasHighlights { get; set; }
        public bool hasHighlightsStream { get; set; }
        public bool hasDraw { get; set; }
        public bool hasFirstToServe { get; set; }
        public int? uniqueTournamentId { get; set; }
        public string name { get; set; }
        public string startTime { get; set; }
        public string formatedStartDate { get; set; }
        public int startTimestamp { get; set; }
        public string statusDescription { get; set; }
        public string slug { get; set; }
        public Periods periods { get; set; }
        public bool hasLineupsList { get; set; }
        public bool hasOdds { get; set; }
        public bool hasLiveOdds { get; set; }
        public bool isSyncable { get; set; }
        public int firstToServe { get; set; }
        public string lastPeriod { get; set; }
        public Changesdata changesData { get; set; }

        // fotbal
        public bool hasEventPlayerStatistics { get; set; }
        public bool hasEventPlayerHeatMap { get; set; }
        public int aggregatedWinnerCode { get; set; }
    }

    public class Score
    {
        public int current { get; set; }
        public int period1 { get; set; }
        public int period2 { get; set; }
        public int normaltime { get; set; }
        public int period3 { get; set; }
        public string point { get; set; }
        public int period1TieBreak { get; set; }
        public int period2TieBreak { get; set; }
        public int period3TieBreak { get; set; }

        // fotbal
        public int aggregated { get; set; }
        public int overtime { get; set; }
        public int penalties { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string type { get; set; }
    }

    public class Changes
    {
        public DateTime changeDate { get; set; }
        public string[] changes { get; set; }
        public int changeTimestamp { get; set; }
        public bool hasExpired { get; set; }
        public bool hasHomeChanges { get; set; }
        public bool hasAwayChanges { get; set; }
    }

    public class Roundinfo
    {
        public int round { get; set; }
        public string name { get; set; }
    }

    public class Team
    {
        public int id { get; set; }
        public string name { get; set; }
        public string slug { get; set; }
        public string gender { get; set; }
        public Subteam[] subTeams { get; set; }
    }

    public class Subteam
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Odds
    {
        public int id { get; set; }
        public Fulltimeodds fullTimeOdds { get; set; }

        // fotbal
        public Doublechanceodds doubleChanceOdds { get; set; }
    }

    public class Fulltimeodds
    {
        public Regular regular { get; set; }
        public int type { get; set; }
        public string sourceId { get; set; }
        public Live live { get; set; }
    }

    public class Regular
    {
        public Bet _1 { get; set; }
        public Bet _2 { get; set; }

        // fotbal
        public Bet X { get; set; }
    }

    public class Bet
    {
        public string initialDecimalValue { get; set; }
        public string initialFractionalValue { get; set; }
        public string initialAmericanValue { get; set; }
        public string decimalValue { get; set; }
        public string fractionalValue { get; set; }
        public string americanValue { get; set; }
        public string betSlipLink { get; set; }
        public int change { get; set; }
        public string sourceId { get; set; }
        public bool winning { get; set; }
    }

    public class Live
    {
        public BetLive _1 { get; set; }
        public BetLive _2 { get; set; }

        // fotbal
        public BetLive X { get; set; }
    }

    public class BetLive
    {
        public string decimalValue { get; set; }
        public string fractionalValue { get; set; }
        public string americanValue { get; set; }
        public string betSlipLink { get; set; }
        public string sourceId { get; set; }
    }

    public class Doublechanceodds
    {
        public string sourceId { get; set; }
        public int type { get; set; }
        public Regular regular { get; set; }
        public Live live { get; set; }
    }

    public class Periods
    {
        public string point { get; set; }
        public string current { get; set; }
        public string period1 { get; set; }
        public string period2 { get; set; }
        public string period3 { get; set; }
        public string period4 { get; set; }
        public string period5 { get; set; }
    }

    public class Changesdata
    {
        public State home { get; set; }
        public State away { get; set; }
        public bool score { get; set; }
        public bool status { get; set; }
        public bool firstToServe { get; set; }
        public bool notify { get; set; }
    }

    public class State
    {
        public bool team { get; set; }
        public bool score { get; set; }
        public bool point { get; set; }
        public bool pointTieBreak { get; set; }
        public bool current { get; set; }
        public bool currentTieBreak { get; set; }
        public bool period1 { get; set; }
        public bool period1TieBreak { get; set; }
        public bool period2 { get; set; }
        public bool period2TieBreak { get; set; }
        public bool period3 { get; set; }
        public bool period3TieBreak { get; set; }
        public bool period4 { get; set; }
        public bool period4TieBreak { get; set; }
        public bool period5 { get; set; }
        public bool period5TieBreak { get; set; }
    }

    public class Params
    {
        public string sport { get; set; }
        public object category { get; set; }
        public object date { get; set; }
    }


    #endregion
}
