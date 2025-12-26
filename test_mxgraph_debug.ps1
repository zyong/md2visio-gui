# Debug test for mxGraph conversion
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "mxGraph Debug Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Close Visio
Write-Host "Closing Visio..." -ForegroundColor Yellow
Get-Process VISIO -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 2

# Delete old output if exists
$outputPath = "d:\temp\test.vsdx"
if (Test-Path $outputPath) {
    Write-Host "Deleting old output file..." -ForegroundColor Yellow
    Remove-Item $outputPath -Force
}

Write-Host ""
Write-Host "Running conversion with full output..." -ForegroundColor Yellow
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Run with full output
dotnet run --project md2visio/md2visio.csproj --configuration ConsoleRelease --no-build -- testmx 2>&1

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Checking output..." -ForegroundColor Yellow
Write-Host ""

# Check output
if (Test-Path $outputPath) {
    $fileInfo = Get-Item $outputPath
    Write-Host "File exists: $outputPath" -ForegroundColor Green
    Write-Host "File size: $($fileInfo.Length) bytes" -ForegroundColor Green

    if ($fileInfo.Length -gt 10KB) {
        Write-Host "✓ File size looks good!" -ForegroundColor Green
    } elseif ($fileInfo.Length -gt 0) {
        Write-Host "⚠ File is very small - might be empty or incomplete" -ForegroundColor Yellow
    } else {
        Write-Host "✗ File is 0 bytes - conversion failed" -ForegroundColor Red
    }

    Write-Host ""
    Write-Host "Opening file..." -ForegroundColor Yellow
    Start-Process $outputPath
} else {
    Write-Host "✗ File NOT created at $outputPath" -ForegroundColor Red
    Write-Host ""
    Write-Host "Possible issues:" -ForegroundColor Yellow
    Write-Host "1. d:\temp directory doesn't exist" -ForegroundColor White
    Write-Host "2. Permission denied" -ForegroundColor White
    Write-Host "3. Visio COM exception" -ForegroundColor White
}

Write-Host ""
pause
