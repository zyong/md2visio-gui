# Simplest mxGraph Test - Uses built-in testmx command
# This is the most reliable method

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "mxGraph Test (Built-in Method)" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Close Visio
Write-Host "Closing Visio processes..." -ForegroundColor Yellow
Get-Process VISIO -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

Write-Host ""
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build md2visio/md2visio.csproj --configuration ConsoleRelease --verbosity quiet

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    pause
    exit 1
}

Write-Host "Build successful!" -ForegroundColor Green
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running mxGraph conversion..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Run the built-in testmx command
dotnet run --project md2visio/md2visio.csproj --configuration ConsoleRelease -- testmx

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if output was created
$outputPath = "d:\temp\test.vsdx"
if (Test-Path $outputPath) {
    Write-Host "✓ Output file created: $outputPath" -ForegroundColor Green
    Write-Host ""
    Write-Host "Opening in Visio..." -ForegroundColor Yellow
    Start-Process $outputPath -ErrorAction SilentlyContinue
} else {
    Write-Host "Note: TestMxGraph creates output at: $outputPath" -ForegroundColor Yellow
    Write-Host "To test with test_mxgraph.md, use test_mxgraph_run.ps1 instead" -ForegroundColor Yellow
}

Write-Host ""
pause
