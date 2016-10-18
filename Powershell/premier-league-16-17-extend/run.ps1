# data
$idTournament=1
$trainSeason=0
$testSeason=11733

# fann
$max_epochs = 2000
$num_layers = 4
$num_neuron_first_hidden = 48 #22
$num_neuron_second_hidden = 27 #15
$num_input = 16
$num_output = 3

# filename
$trainFilename = "events-tournament-"+$idTournament+"-season-"+$trainSeason+".train"
$netFilename = "events-tournament-"+$idTournament+"-season-"+$trainSeason+".net"
$testFilename = "events-tournament-"+$idTournament+"-season-"+$testSeason+".train"
$calcFilename = "events-tournament-"+$idTournament+"-season-"+$testSeason+".net.csv"
$testResultFilename = "events-tournament-"+$idTournament+"-season-"+$testSeason+".csv"

if (!(Test-Path $trainFilename)) {
..\Get-event-category-data-extend.ps1 -idTournament $idTournament -idSeason $testSeason -inverse # inverse!
}

if (!(Test-Path $testFilename)) {
..\Get-event-category-data-extend.ps1 -idTournament $idTournament -idSeason $testSeason 
}
..\fann-processor.ps1 -trainPath $trainFilename -testPath $testFilename -netPath $netFilename -max_epochs $max_epochs -num_layers $num_layers -num_neuron_first_hidden $num_neuron_first_hidden -num_neuron_second_hidden $num_neuron_first_hidden -num_input $num_input -num_output $num_output


$input = Import-Csv -Path $testResultFilename -Delimiter ';'
$calc = Import-Csv -Path $calcFilename -Delimiter ';'

for ($i=0; $i -lt $input.Count; $i++)
{
    $obj = $input[$i]
    $obj | Add-Member -MemberType NoteProperty -Name "calc_0" -Value  $calc[$i].calc_0
    $obj | Add-Member -MemberType NoteProperty -Name "calc_1" -Value  $calc[$i].calc_1
    $obj | Add-Member -MemberType NoteProperty -Name "calc_2" -Value  $calc[$i].calc_2
}


$filename = "result-" +$max_epochs + "-" + $num_neuron_first_hidden + "-" + $num_neuron_second_hidden + ".csv"
$input | Export-Csv -Path $filename -NoTypeInformation -Delimiter ';' | Format-Table # -Append