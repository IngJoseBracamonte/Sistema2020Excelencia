# Powershell script to check database schema using MySqlConnector
Add-Type -Path "C:\Src\src\Sistema2020Excelencia\packages\MySqlConnector.2.1.10\lib\net45\MySqlConnector.dll" -ErrorAction SilentlyContinue

$legacyConnStr = "server=localhost;database=sistema2020;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None;Allow User Variables=True"
$systemConnStr = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None"

function Check-TableColumns($connStr, $dbName, $tableName) {
    try {
        $conn = New-Object MySqlConnector.MySqlConnection($connStr)
        $conn.Open()
        
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = "SHOW COLUMNS FROM $tableName"
        $reader = $cmd.ExecuteReader()
        
        Write-Host "Columns in $dbName.$tableName`:" -ForegroundColor Cyan
        while ($reader.Read()) {
            Write-Host "- $($reader.GetString(0)) ($($reader.GetString(1)))"
        }
        $reader.Close()
        $conn.Close()
    } catch {
        Write-Host "Error checking $dbName.$tableName : $_" -ForegroundColor Red
    }
}

Write-Host "Checking legacy database..."
Check-TableColumns $legacyConnStr "sistema2020" "datospersonales"

Write-Host "`nChecking system database..."
Check-TableColumns $systemConnStr "SatHospitalario" "PacientesAdmision"
