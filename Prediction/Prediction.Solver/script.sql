select 
	BM_Event.ID as Id,
	TeamHome.ID as HomeTeamId,
	TeamHome.DisplayName as HomeTeam,
	TeamAway.ID as AwayTeamId,
	TeamAway.DisplayName as AwayTeam,
	BM_Event.HomeScoreCurrent as HomeScore,
	BM_Event.AwayScoreCurrent as AwayScore,
	BM_Event.DateStart as [Date],
	BM_Event.WinnerCode,
	BM_OddsRegular.FirstValue,
	BM_OddsRegular.XValue,
	BM_OddsRegular.SecondValue
from BM_Event 
	inner join BM_Team as TeamHome on TeamHome.ID=BM_Event.ID_HomeTeam
	inner join BM_Team as TeamAway on TeamAway.ID=BM_Event.ID_AwayTeam
	left join BM_OddsRegular on BM_OddsRegular.ID_Event=BM_Event.ID
	where BM_Event.ID_Tournament=49 
		and BM_Event.ID_Status=100
order by BM_Event.DateStart

-- SQLLite!
select *,
	cast((JulianDay(GameMatch.DateStart) - JulianDay(GamePrediction.DatePredict)) As Integer) as Dif
from GameMatch
	inner join GameTeam as GameHomeTeam On GameHomeTeam.Id=GameMatch.HomeTeamId
	inner join GameTeam as GameAwayTeam On GameAwayTeam.Id=GameMatch.AwayTeamId
	left join GamePrediction on cast((JulianDay(GameMatch.DateStart) - JulianDay(GamePrediction.DatePredict)) As Integer) between 0 and 6
	left join GamePredictionTeam as GamePredictionHomeTeam on GamePredictionHomeTeam.PredictionId=GamePrediction.Id and GamePredictionHomeTeam.TeamId=GameMatch.HomeTeamId
	left join GamePredictionTeam as GamePredictionAwayTeam on GamePredictionAwayTeam.PredictionId=GamePrediction.Id and GamePredictionAwayTeam.TeamId=GameMatch.AwayTeamId
order by GameMatch.DateStart