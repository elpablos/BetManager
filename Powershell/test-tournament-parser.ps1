Function Process-Data($path)
{
    Write-Host "--- Zpracovávám ---"
    $file = Get-Content -Path ($path)

    $jsonObj = ConvertFrom-Json -InputObject $file
    # $jsonObj.sportItem.tournaments
    # export format
    $sofa = @() #vytvorim kolekci
    foreach ($i in $jsonObj.events.roundMatches.tournaments) 
    {
        # iterace pres eventy/zapasy
        foreach ($e in $i.events) 
        {
            # zalozim obj
            $item = New-Object System.Object 

            #pridam vlastnosti
            $item | Add-Member -MemberType NoteProperty -Name "Date" -Value $jsonObj.params.date
            $item | Add-Member -MemberType NoteProperty -Name "SportName" -Value $jsonObj.sportItem.sport.name
            $item | Add-Member -MemberType NoteProperty -Name "SportSlug" -Value $jsonObj.sportItem.sport.slug
            $item | Add-Member -MemberType NoteProperty -Name "SportId" -Value $jsonObj.sportItem.sport.id -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "TournamentName" -Value $i.tournament.name
            $item | Add-Member -MemberType NoteProperty -Name "TournamentSlug" -Value $i.tournament.slug
            $item | Add-Member -MemberType NoteProperty -Name "TournamentId" -Value $i.tournament.id -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "TournamentUniqueId" -Value $i.tournament.uniqueId -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "CategoryName" -Value $i.category.name
            $item | Add-Member -MemberType NoteProperty -Name "CategorySlug" -Value $i.category.slug
            $item | Add-Member -MemberType NoteProperty -Name "CategoryId" -Value $i.category.id -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "SeasonName" -Value $i.season.name
            $item | Add-Member -MemberType NoteProperty -Name "SeasonSlug" -Value $i.season.slug
            $item | Add-Member -MemberType NoteProperty -Name "SeasonId" -Value $i.season.id -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "SeasonYear" -Value $i.season.year -TypeName int

            #event
            $item | Add-Member -MemberType NoteProperty -Name "EventId" -Value $e.id -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "EventCustomId" -Value $e.customId -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "EventFirstToServe" -Value $e.firstToServe
            $item | Add-Member -MemberType NoteProperty -Name "EventHasDraw" -Value $e.hasDraw
            $item | Add-Member -MemberType NoteProperty -Name "EventWinnerCode" -Value $e.winnerCode -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "EventName" -Value $e.name
            $item | Add-Member -MemberType NoteProperty -Name "EventSlug" -Value $e.slug
            $item | Add-Member -MemberType NoteProperty -Name "EventStartDate" -Value $e.formatedStartDate
            $item | Add-Member -MemberType NoteProperty -Name "EventStartTime" -Value $e.startTime
            $item | Add-Member -MemberType NoteProperty -Name "EventChanges" -Value $e.changes.changeDate

            $item | Add-Member -MemberType NoteProperty -Name "StatusCode" -Value $e.status.code
            $item | Add-Member -MemberType NoteProperty -Name "StatusType" -Value $e.status.type
            $item | Add-Member -MemberType NoteProperty -Name "StatusDescription" -Value $e.statusDescription

            #team
            $item | Add-Member -MemberType NoteProperty -Name "HomeTeamId" -Value $e.homeTeam.id -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "HomeTeamName" -Value $e.homeTeam.name
            $item | Add-Member -MemberType NoteProperty -Name "HomeTeamSlug" -Value $e.homeTeam.slug
            $item | Add-Member -MemberType NoteProperty -Name "HomeTeamGender" -Value $e.homeTeam.gender
            $item | Add-Member -MemberType NoteProperty -Name "HomeScoreCurrent" -Value $e.homeScore.current -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "HomeScorePeriod1" -Value $e.homeScore.period1 -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "HomeScorePeriod2" -Value $e.homeScore.period2 -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "HomeScorePeriod3" -Value $e.homeScore.period3 -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "HomeScoreNormaltime" -Value $e.homeScore.normaltime -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "HomeScoreOvertime" -Value $e.homeScore.overtime -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "HomeScorePenalties" -Value $e.homeScore.penalties -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "AwayTeamId" -Value $e.awayTeam.id -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "AwayTeamName" -Value $e.awayTeam.name
            $item | Add-Member -MemberType NoteProperty -Name "AwayTeamSlug" -Value $e.awayTeam.slug
            $item | Add-Member -MemberType NoteProperty -Name "AwayTeamGender" -Value $e.awayTeam.gender
            $item | Add-Member -MemberType NoteProperty -Name "AwayScoreCurrent" -Value $e.awayScore.current -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "AwayScorePeriod1" -Value $e.awayScore.period1 -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "AwayScorePeriod2" -Value $e.awayScore.period2 -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "AwayScorePeriod3" -Value $e.awayScore.period3 -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "AwayScoreNormaltime" -Value $e.awayScore.normaltime -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "AwayScoreOvertime" -Value $e.awayScore.overtime -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "AwayScorePenalties" -Value $e.awayScore.penalties -TypeName int
            
            # fullTimeOdds
            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularFirstSourceId" -Value $e.odds.fullTimeOdds.regular."1".sourceId -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularFirstValue" -Value $e.odds.fullTimeOdds.regular."1".decimalValue -TypeName decimal
            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularFirstWining" -Value $e.odds.fullTimeOdds.regular."1".winning -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularXSourceId" -Value $e.odds.fullTimeOdds.regular."X".sourceId -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularXValue" -Value $e.odds.fullTimeOdds.regular."X".decimalValue -TypeName decimal
            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularXWining" -Value $e.odds.fullTimeOdds.regular."X".winning -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularSecondSourceId" -Value $e.odds.fullTimeOdds.regular."2".sourceId -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularSecondValue" -Value $e.odds.fullTimeOdds.regular."2".decimalValue -TypeName decimal
            $item | Add-Member -MemberType NoteProperty -Name "OddsRegularSecondWining" -Value $e.odds.fullTimeOdds.regular."2".winning -TypeName int

            # doubleChanceOdds
            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeFirstXSourceId" -Value $e.odds.doubleChanceOdds.regular."1X".sourceId -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeFirstXValue" -Value $e.odds.doubleChanceOdds.regular."1X".decimalValue -TypeName decimal
            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeFirstXWining" -Value $e.odds.doubleChanceOdds.regular."1X".winning -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeXSecondSourceId" -Value $e.odds.doubleChanceOdds.regular."X2".sourceId -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeXSecondValue" -Value $e.odds.doubleChanceOdds.regular."X2".decimalValue -TypeName decimal
            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeXSecondWining" -Value $e.odds.doubleChanceOdds.regular."X2".winning -TypeName int

            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeFirstSecondSourceId" -Value $e.odds.doubleChanceOdds.regular."12".sourceId -TypeName int
            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeFirstSecondValue" -Value $e.odds.doubleChanceOdds.regular."12".decimalValue -TypeName decimal
            $item | Add-Member -MemberType NoteProperty -Name "OddsDoubleChangeFirstSecondWining" -Value $e.odds.doubleChanceOdds.regular."12".winning -TypeName int

            $sofa += $item # pridam do kolekce
        }
    }
    # vypisu kolekci
    Write-Output $sofa
}

$filename = "export-tournament.csv"
$output = "d:\Other\Sofascore\pwshell\tournament-172-all-json.json"
Process-Data($output) | Export-Csv -Path $filename -NoTypeInformation -Delimiter ";" -Append