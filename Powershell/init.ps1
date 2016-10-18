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
$dateActual = Get-Date $today -Day 1 -Month 1

if (Test-Path $filename)
{
    Remove-Item $filename
}

while ($dateActual -lt $today -or $dateActual -eq $today)
{
    $date = (Get-Date $dateActual -Format "yyyy-MM-dd")
    Write-Host "Processing.." + $date

    $args = @()
    $args += ("-date", $date)
    Invoke-Expression "$cmd $args" | Export-Csv -Path $filename -NoTypeInformation -Delimiter ";" -Append

    Start-Sleep -Seconds 2
    $dateActual = (Get-Date $dateActual).AddDays(1)
}


#    $args = @()
#    $args += ("-date", "2016-09-07")
#    Invoke-Expression "$cmd $args" | Export-Csv -Path $filename -NoTypeInformation -Delimiter ";"

Write-Host "--- Init-Hotovo ---"
