# Quick test for mxGraph conversion with detailed output
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "mxGraph Quick Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Close Visio
Write-Host "Closing Visio..." -ForegroundColor Yellow
Get-Process VISIO -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

Write-Host "Running conversion with testmx command..." -ForegroundColor Yellow
Write-Host ""

# Run the built-in test
dotnet run --project md2visio/md2visio.csproj --configuration ConsoleRelease --no-build -- testmx

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

# Check output
$outputPath = "d:\temp\test.vsdx"
if (Test-Path $outputPath) {
    $fileInfo = Get-Item $outputPath
    if ($fileInfo.Length -gt 10KB) {
        Write-Host "✓ SUCCESS: Output file created ($([math]::Round($fileInfo.Length/1KB, 2)) KB)" -ForegroundColor Green
        Write-Host ""
        Write-Host "Opening in Visio..." -ForegroundColor Yellow
        Start-Process $outputPath
    } else {
        Write-Host "✗ WARNING: File created but seems too small ($($fileInfo.Length) bytes)" -ForegroundColor Yellow
        Write-Host "The file might be empty or incomplete." -ForegroundColor Yellow
    }
} else {
    Write-Host "✗ ERROR: Output file not found at $outputPath" -ForegroundColor Red
}

Write-Host ""
pause
