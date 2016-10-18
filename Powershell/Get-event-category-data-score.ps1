# params
Param 
(
    [Parameter(Mandatory=$true)][int]$idTournament,
    [int]$idSeason
 )

function Get-Events() {
    Write-Host "--- DB connecting ---"
    # spojení s DB
    $SqlConnection = New-Object System.Data.SqlClient.SqlConnection
    $SqlConnection.ConnectionString = "Server=.\SQLEXPRESS;Database=BetManagerDevel;Integrated Security=True"
    $SqlCmd = New-Object System.Data.SqlClient.SqlCommand
    $SqlCmd.CommandText = "BM_Event_ALL_Tournament_Score"
    $SqlCmd.Connection = $SqlConnection
    $SqlCmd.CommandType = [System.Data.CommandType]::StoredProcedure

    #predani parametrů
    $SqlCmd.Parameters.AddWithValue("@ID_Tournament",[int]$idTournament)

    if ($idSeason -ne 0) {
        $SqlCmd.Parameters.AddWithValue("@ID_Season",[int]$idSeason)
    }

    $SqlAdapter = New-Object System.Data.SqlClient.SqlDataAdapter
    $SqlAdapter.SelectCommand = $SqlCmd
    $DataSet = New-Object System.Data.DataSet
    $SqlAdapter.Fill($DataSet)
    $SqlConnection.Close()
    Write-Host "--- DB returning table ---"
    return $DataSet #| Export-Csv -Path .\tournament-49.csv -Delimiter ";" -NoTypeInformation
}

function Create-fann-file($liga) {
     Write-Host "--- Creating FANN file ---"
    $output = "" + $liga.Length + " 10 2"
    $output += [Environment]::NewLine
    foreach ($i in $liga) {
        $output += $i.ID_Season +" "+ $i.ID_HomeTeam +" "+ $i.ID_AwayTeam + " " + $i.HomeRound
        $output += " " + $i.HomeSeasonPoints + " " + $i.HomeScoreGiven + " " + $i.HomeScoreTaken
        $output += " " + $i.AwaySeasonPoints + " " + $i.AwayScoreGiven + " " + $i.AwayScoreTaken
        $output += [Environment]::NewLine
        $output += $i.HomeScore +" "+$i.AwayScore
        $output += [Environment]::NewLine
    }
    return $output
}

Function Set-Culture([System.Globalization.CultureInfo] $culture)
{
    [System.Threading.Thread]::CurrentThread.CurrentUICulture = $culture
    [System.Threading.Thread]::CurrentThread.CurrentCulture = $culture
}

# desetinna mista s desetinnou teckou
Set-Culture ([System.Globalization.CultureInfo]::GetCultureInfo('en-US'))

# format nazvu souboru
$filename = "events-tournament-"+$idTournament+"-season-"+$idSeason

# vytvorim CSV soubor
$dataset = Get-Events
$table = $dataset.Tables[0] 
$table | Export-Csv -Path $filename".csv" -Delimiter ";" -NoTypeInformation

# vytvorim FANN soubor
$liga = Import-Csv $filename".csv" -Delimiter ";"
$fann = Create-fann-file($liga)
$fann | Out-File -FilePath $filename".train" -Encoding ascii



