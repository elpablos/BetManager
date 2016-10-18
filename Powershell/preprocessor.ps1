$liga = Import-Csv "D:\Other\Sofascore\pwshell\1-liga-test.csv" -Delimiter ";"

$output = "" + $liga.Count + " 6 3"
Write-Output $output
foreach ($i in $liga) {
    $output = $i.ID_Season +" "+ $i.ID_HomeTeam +" "+ $i.ID_AwayTeam +" "+ $i.WinnerCode+" "+ $i.HomePoint+" "+ $i.AwayPoint
    Write-Output $output
    $output = $i.Home +" "+ $i.Draw+" "+$i.Away
    Write-Output $output
}