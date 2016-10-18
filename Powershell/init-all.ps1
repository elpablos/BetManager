# params
Param 
(
    [Parameter(Mandatory=$true)][string]$filename
 )

#variables
$output = ""

Function Print-Help()
{
    Write-Host "--------------------------------------"
    Write-Host "-- Zpracovani vysledku ze Sofascore --"
    Write-Host "--------------------------------------"
}

Print-Help
Write-Host "--- Init-Start ---"

$cmd = ".\sofascore-sportItem-parser.ps1"

$today = Get-Date
$yesterday = $today.AddDays(-1)
$nextWeek = $today.AddDays(7)
$dateActual = Get-Date $today -Day 13 -Month 10 -Year 2016 # 2016-09-24

if (Test-Path $filename)
{
    Remove-Item $filename
}

$sports = ("tennis", "ice-hockey", "football")
while ($dateActual -le $nextWeek)
{
    $date = (Get-Date $dateActual -Format "yyyy-MM-dd")
    Write-Host "Processing.." + $date

    foreach ($sport in $sports)
    { 
        $path = $filename + "-" + $sport + ".csv"

        $args = @()
        $args += ("-date", $date)
        $args += ("-sport", $sport)

        if ($dateActual -ge $yesterday -and $dateActual -le $nextWeek) {
            $args += ("-force")
        }

        Invoke-Expression "$cmd $args" | Export-Csv -Path $path -NoTypeInformation -Delimiter ";" -Append
   
        Start-Sleep -Seconds 2
    }
    $dateActual = (Get-Date $dateActual).AddDays(1)
}


#    $args = @()
#    $args += ("-date", "2016-09-07")
#    Invoke-Expression "$cmd $args" | Export-Csv -Path $filename -NoTypeInformation -Delimiter ";"

Write-Host "--- Init-Hotovo ---"
