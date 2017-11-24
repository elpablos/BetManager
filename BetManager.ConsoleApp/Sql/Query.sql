declare @ID_Tournament int = 109, 
	@ID_Season int,
	@ID_LastSeason int

select @ID_Season=MAX(ID_Season) from BM_Event 
where ID_Tournament=@ID_Tournament

select @ID_LastSeason=MAX(ID_Season) from BM_Event 
where ID_Tournament=@ID_Tournament and ID_Season<>@ID_Season

select 
	BM_Event.ID as Id,
	HomeTeam.ID as HomeTeamId,
	HomeTeam.DisplayName as HomeTeam,
	AwayTeam.ID as AwayTeamId,
	AwayTeam.DisplayName as AwayTeam,
	BM_Score.HomeScoreNormaltime as HomeScore,
	BM_Score.AwayScoreNormaltime as AwayScore,
	cast (BM_Event.DateStart as date) as DateStart
from BM_Event
	inner join BM_Score on BM_Score.ID_Event=BM_Event.ID
	inner join BM_Team as HomeTeam on HomeTeam.ID=BM_Event.ID_HomeTeam
	inner join BM_Team as AwayTeam on AwayTeam.ID=BM_Event.ID_AwayTeam
where BM_Event.ID_Tournament=@ID_Tournament
	and BM_Event.ID_Season in (@ID_Season, @ID_LastSeason)
	and BM_Event.ID_Status>=100
order by BM_Event.DateStart desc
