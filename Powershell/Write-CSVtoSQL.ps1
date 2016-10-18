#################################################### 
# 
# PowerShell CSV to SQL Import Script 
# 
#################################################### 
 
Param 
(
    [Parameter(Mandatory=$true)][string]$csvfile,
    [switch]$truncate
 )

# Database variables 
$sqlserver = ".\SQLEXPRESS" 
$database = "BetManagerDevel" # "BetManager" 
$table = "BM_ImportDataView" # "BM_ImportData" 
 
# CSV variables;  
#$truncate = $true
#$csvfile = "d:\Other\Sofascore\pwshell\export-ice-hockey.csv" 
$csvdelimiter = ";" 
$firstrowcolumnnames = $true 
 
#################################################### 
# 
# No additional changes are required below unless 
# you want to modify your sqlbulkcopy options or 
# your SQL authentication details 
# 
#################################################### 
 
Write-Output "Script started..." 
$elapsed = [System.Diagnostics.Stopwatch]::StartNew() 
 
# 100k worked fastest and kept memory usage to a minimum 
$batchsize = 100000 
 
# Build the sqlbulkcopy connection, and set the timeout to infinite 
$connectionstring = "Data Source=$sqlserver;Integrated Security=true;Initial Catalog=$database;" 

if ($truncate)
{
    $conn = new-object System.Data.SqlClient.SqlConnection $connectionstring
    $conn.Open()
    $cmd = new-object System.Data.SqlClient.SqlCommand(“TRUNCATE TABLE $table”, $conn)
    $cmd.ExecuteNonQuery()
    $conn.Close() 
}

$bulkcopy = new-object ("Data.SqlClient.Sqlbulkcopy") ($connectionstring, [System.Data.SqlClient.SqlBulkCopyOptions]::KeepNulls)
$bulkcopy.DestinationTableName = $table 
$bulkcopy.bulkcopyTimeout = 0 
$bulkcopy.batchsize = $batchsize 
$bulkcopy.EnableStreaming = 1 
  
# Create the datatable, and autogenerate the columns. 
$datatable = New-Object "System.Data.DataTable" 
 
# Open the text file from disk 
$reader = new-object System.IO.StreamReader($csvfile) 
$line = $reader.ReadLine() 
$columns =  $line.Split($csvdelimiter) 
 
    if ($firstrowcolumnnames -eq $false) { 
        foreach ($column in $columns) { 
            $null = $datatable.Columns.Add() 
            } 
        # start reader over 
        $reader.DiscardBufferedData();  
        $reader.BaseStream.Position = 0; 
        } 
    else { 
        foreach ($column in $columns) { 
            $null = $datatable.Columns.Add($column) 
        } 
    } 
 
  # Read in the data, line by line 
    while (($line = $reader.ReadLine()) -ne $null)  { 
        $line = $line -replace '"', ''
        $row = $datatable.NewRow() 
        $row.itemarray = $line.Split($csvdelimiter) 

        $x = 0
        while($x -lt $row.ItemArray.Count)
        {
            if ($row[$x] -eq '')
            {
               $row[$x] = [DBNull]::Value
            }
            $x++
        }

        $datatable.Rows.Add($row)   
        # break;

        # Once you reach your batch size, write to the db,  
        # then clear the datatable from memory 
        $i++; if (($i % $batchsize) -eq 0) { 
        $bulkcopy.WriteToServer($datatable) 
        Write-Output "$i rows have been inserted in $($elapsed.Elapsed.ToString())."; 
        $datatable.Clear() 
        } 
    }  
 
# Close the CSV file 
$reader.Close() 
 
    # Add in all the remaining rows since the last clear 
    if($datatable.Rows.Count -gt 0) { 
        $bulkcopy.WriteToServer($datatable) 
        $datatable.Clear() 
    } 
 
# Sometimes the Garbage Collector takes too long. 
[System.GC]::Collect()     
 
Write-Output "Script complete. $i rows have been inserted into the database." 
Write-Output "Total Elapsed Time: $($elapsed.Elapsed.ToString())"