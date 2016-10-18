$from = 475054
$to = 477738

$resource = "http://localhost:54897"
$resource2 = "http://betmanager.lorenzo.cz"

$start_time = Get-Date

for($id=$from; $id -le $to; $id++) 
{

    $url = [string]::Format("$resource/api/importdata/{0}", $id) 
    Write-host $url
    $body = Invoke-RestMethod -Method Get -Uri $url -ContentType 'application/json; charset="UTF-8'
    $result = Invoke-RestMethod -Method Post -Uri "$resource2/api/importdata" -Body (ConvertTo-Json $body) -ContentType 'application/json;charset=UTF-8'
}

Write-Host "Time taken: $((Get-Date).Subtract($start_time).Seconds) second(s)"