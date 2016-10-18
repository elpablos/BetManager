# params
Param 
(
    [switch]$force = $false,
    [string]$trainPath,
    [string]$netPath,
    [string]$testPath,
    [int]$max_epochs = 20000,
    [int]$num_layers = 4,
    [int]$num_neuron_first_hidden = 22,
    [int]$num_neuron_second_hidden = 15,
    [int]$num_input = 10,
    [int]$num_output = 3
 )

 # nacteni FANNSharp libky (C#)
 $dllPath = Resolve-Path "..\betfann\FANNCSharp.dll"
 [System.Reflection.Assembly]::LoadFrom($dllPath)

# resolve full path
if ($trainPath) 
{
    $trainPath = Resolve-Path $trainPath
}
if ($testPath)
{
    $testPath = Resolve-Path $testPath
}

[Environment]::CurrentDirectory = $pwd
$netPath = [string]([IO.Path]::GetFullPath($netPath))
if (!$force)
{
   
   $force = !(Test-Path $netPath)

   Write-Host $("test force {0}" -f $force)
}

# params
$networkType = [FANNCSharp.NetworkType]::LAYER

# zpracovani neuronove site FANN
Function Build-NeuronNetwork($trainPath, $netPath)
{
    #train
    $desired_error = 0
    $epochs_between_reports = 10;

    # zalozim site
    $net = New-Object FANNCSharp.Double.NeuralNet -ArgumentList $networkType, $num_layers, $num_input, $num_neuron_first_hidden, $num_neuron_second_hidden, $num_output
    # nactu trenovaci data
    $data = New-Object FANNCSharp.Double.TrainingData -ArgumentList $trainPath
    $net.ActivationFunctionHidden = [FANNCSharp.ActivationFunction]::SIGMOID_SYMMETRIC
    $net.ActivationFunctionOutput = [FANNCSharp.ActivationFunction]::SIGMOID_SYMMETRIC
    $net.TrainStopFunction = [FANNCSharp.StopFunction]::STOPFUNC_MSE
    $net.TrainingAlgorithm = [FANNCSharp.TrainingAlgorithm]::TRAIN_RPROP
    $net.InitWeights($data)
    Write-Host "training network"
    $net.TrainOnData($data, $max_epochs, $epochs_between_reports, $desired_error)
    Write-Host "testing network"

    $input = $data.Input
    $output = $data.Output

    for($i=0; $i -lt $data.TrainDataLength; $i++) 
    {
        $calc_out = $net.Run($input[$i])
        Write-Host $("XOR test ({0},{1}) -> {2}, should be {3}" -f $input[$i][1], $input[$i][2], $calc_out[0], $output[$i][0])
    }

    Write-Host "saving network"
    $net.Save($netPath)

    $data.Dispose()
    $net.Dispose()
}

# testovani neuronove site FANN
Function Test-NeuronNetwork($netPath, $testPath)
{
    #vytvorim kolekci
    $results = @()

    $net = New-Object FANNCSharp.Double.NeuralNet -ArgumentList $netPath
    $net.PrintConnections()
    $net.PrintParameters()
    Write-Host "Testing network"
    # nactu trenovaci data
    $data = New-Object FANNCSharp.Double.TrainingData
    if (!$data.ReadTrainFromFile($testPath))
    {
        Write-Host "ERROR reading training data -- ABORT"
    }
    else
    {
        for($i=0; $i -lt $data.TrainDataLength; $i++) 
        {
            $net.ResetMSE()
            $calc_out = $net.Test($data.GetTrainInput($i).Array, $data.GetTrainOutput($i).Array)
            Write-Host $("XOR test ({0}, {1}) -> {2}, should be {3}, difference={4}" -f 
                $data.GetTrainInput($i)[1],
                $data.GetTrainInput($i)[2],
                $calc_out[0],
                $data.GetTrainOutput($i)[0],
                ($calc_out[0] - $data.GetTrainOutput($i)[0])
            )

            # zalozim obj
            $item = New-Object System.Object 

            # naplnim polozku

            # vstupy
            for($in=0; $in -lt $num_input; $in++)
            {
                $item | Add-Member -MemberType NoteProperty -Name $("in_{0}" -f $in) -Value  $data.GetTrainInput($i)[$in]
            }

            # vystupy
            for($out=0; $out -lt $num_output; $out++)
            {
                $item | Add-Member -MemberType NoteProperty -Name $("out_{0}" -f $out) -Value  $data.GetTrainOutput($i)[$out]
            }

            # vystupy - vypoctene
            for($out=0; $out -lt $num_output; $out++)
            {
                $item | Add-Member -MemberType NoteProperty -Name $("calc_{0}" -f $out) -Value  $calc_out[$out]
            }

            # pridam do kolekce
            $results += $item 
        } 
    }

    Write-Host "konec"
    # vypisu kolekci
    Write-Output $results

    $data.Dispose()
    $net.Dispose()
}

Function Set-Culture([System.Globalization.CultureInfo] $culture)
{
    [System.Threading.Thread]::CurrentThread.CurrentUICulture = $culture
    [System.Threading.Thread]::CurrentThread.CurrentCulture = $culture
}

# vypis parametru
Write-Host $("testPath: {0}" -f $testPath)
Write-Host $("trainPath: {0}" -f $trainPath)
Write-Host $("netPath: {0}" -f $netPath)

# desetinna mista s desetinnou teckou
Set-Culture ([System.Globalization.CultureInfo]::GetCultureInfo('en-US'))

#neuronNetwork
if ($force)
{
    Build-NeuronNetwork -train $trainPath -net $netPath
}

if ($testPath)
{
    $filename = [io.path]::ChangeExtension($testPath, "net.csv")
    Write-Host $filename
    Test-NeuronNetwork -netPath $netPath -testPath $testPath | Export-Csv -Path $filename -NoTypeInformation -Delimiter ";" # -Append
}
