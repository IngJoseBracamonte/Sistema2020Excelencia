Add-Type -Path "C:\Src\src\Sistema2020Excelencia\packages\MySqlConnector.2.1.10\lib\net45\MySqlConnector.dll" -ErrorAction SilentlyContinue

$systemConnStr = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None"

try {
    $conn = New-Object MySqlConnector.MySqlConnection($systemConnStr)
    $conn.Open()
    
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT Id, NumeroRecibo, TotalFacturadoUSD, Estado, FechaEmision FROM RecibosFacturas ORDER BY FechaEmision DESC LIMIT 10"
    $reader = $cmd.ExecuteReader()
    
    Write-Host "Recent Recibos:" -ForegroundColor Cyan
    while ($reader.Read()) {
        $id = $reader.GetValue(0)
        $num = $reader.GetValue(1)
        $total = $reader.GetValue(2)
        $estado = $reader.GetValue(3)
        $fecha = $reader.GetValue(4)
        Write-Host "$id | $num | $total | $estado | $fecha"
    }
    $reader.Close()
    $conn.Close()
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
}
