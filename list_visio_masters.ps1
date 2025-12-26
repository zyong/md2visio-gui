# List all available masters in Basic Shapes stencil
Write-Host "Listing Visio Basic Shapes masters..." -ForegroundColor Cyan
Write-Host ""

Add-Type -AssemblyName "Microsoft.Office.Interop.Visio"
$visio = New-Object -ComObject Visio.Application
$visio.Visible = $false

try {
    # Open Basic Shapes stencil
    $stencil = $visio.Documents.OpenEx("Basic Shapes.vss", 4)  # visOpenDocked = 4

    Write-Host "Found $($stencil.Masters.Count) masters:" -ForegroundColor Green
    Write-Host ""

    # List all masters
    foreach ($master in $stencil.Masters) {
        Write-Host "  - $($master.Name)" -ForegroundColor White
    }

    Write-Host ""
    Write-Host "Looking for connector-related masters..." -ForegroundColor Yellow
    foreach ($master in $stencil.Masters) {
        if ($master.Name -like "*connect*" -or $master.Name -like "*line*" -or $master.Name -like "*arrow*") {
            Write-Host "  ✓ $($master.Name)" -ForegroundColor Green
        }
    }

    $stencil.Close()
} catch {
    Write-Host "Error: $_" -ForegroundColor Red
} finally {
    $visio.Quit()
    [System.Runtime.Interopservices.Marshal]::ReleaseComObject($visio) | Out-Null
}

Write-Host ""
pause
