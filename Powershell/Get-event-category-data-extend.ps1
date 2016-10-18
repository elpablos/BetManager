# params
Param 
(
    [Parameter(Mandatory=$true)][int]$idTournament,
    [int]$idSeason,
    [switch]$inverse = $false
 )

function Get-Events() {
    Write-Host "--- DB connecting ---"
    # spojení s DB
    $SqlConnection = New-Object System.Data.SqlClient.SqlConnection
    $SqlConnection.ConnectionString = "Server=.\SQLEXPRESS;Database=BetManagerDevel;Integrated Security=True"
    $SqlCmd = New-Object System.Data.SqlClient.SqlCommand
    $SqlCmd.CommandText = "BM_Event_ALL_Tournament_Extend"
    $SqlCmd.Connection = $SqlConnection
    $SqlCmd.CommandType = [System.Data.CommandType]::StoredProcedure
    $SqlCmd.CommandTimeout = 600000

    #predani parametrů
    $SqlCmd.Parameters.AddWithValue("@ID_Tournament",[int]$idTournament)
    $SqlCmd.Parameters.AddWithValue("@ID_Season",[int]$idSeason)
    $SqlCmd.Parameters.AddWithValue("@Inverse", [bool]$inverse)

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
    $output = "" + $liga.Length + " 16 3"
    $output += [Environment]::NewLine
    foreach ($i in $liga) {
        $output += $i.ID_Season +" "+ $i.ID_HomeTeam +" "+ $i.ID_AwayTeam + " " + $i.HomeRound
        $output += " " + $i.HomeSeasonPoints + " " + $i.HomeScoreGiven + " " + $i.HomeScoreTaken
        $output += " " + $i.HomeForm + " " + $i.HomeLastGiven+ " " + $i.HomeLastTaken
        $output += " " + $i.AwaySeasonPoints + " " + $i.AwayScoreGiven + " " + $i.AwayScoreTaken
        $output += " " + $i.AwayForm + " " + $i.AwayLastGiven+ " " + $i.AwayLastTaken
        $output += [Environment]::NewLine
        $output += $i.Home +" "+ $i.Draw+" "+$i.Away
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
if ($inverse) {
    $filename = "events-tournament-"+$idTournament+"-season-0"
} else {
    $filename = "events-tournament-"+$idTournament+"-season-"+$idSeason
}
Write-Host "filename - "+ $filename

# vytvorim CSV soubor
$dataset = Get-Events
$table = $dataset.Tables[0] 
$table | Export-Csv -Path $filename".csv" -Delimiter ";" -NoTypeInformation

# vytvorim FANN soubor
$liga = Import-Csv $filename".csv" -Delimiter ";"
$fann = Create-fann-file($liga)
$fann | Out-File -FilePath $filename".train" -Encoding ascii



