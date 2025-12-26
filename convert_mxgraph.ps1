# PowerShell script to convert mxGraph XML to Visio

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "mxGraph XML to Visio Converter" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Close Visio
Write-Host "Closing Visio processes..." -ForegroundColor Yellow
Get-Process VISIO -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build md2visio/md2visio.csproj --configuration Release

if ($LASTEXITCODE -ne 0) {
    Write-Host "Build failed!" -ForegroundColor Red
    pause
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running conversion..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create inline C# script
$code = @"
using System;
using System.IO;
using md2visio.mxgraph;

public class MxGraphTest
{
    public static void Main()
    {
        try
        {
            string xmlContent = File.ReadAllText("test_mxgraph.md");
            var converter = new MxGraphConverter();

            Console.WriteLine("Converting mxGraph XML to Visio...");
            converter.ConvertToVisio(xmlContent, "test_mxgraph_output.vsdx");

            Console.WriteLine("✓ Conversion completed successfully!");
            Console.WriteLine("Output file: test_mxgraph_output.vsdx");
        }
        catch (Exception ex)
        {
            Console.WriteLine("✗ Error: " + ex.Message);
            Console.WriteLine(ex.StackTrace);
        }
    }
}
"@

# Save C# code
$code | Out-File -FilePath "temp_mxgraph_test.cs" -Encoding UTF8

# Compile and run
Write-Host "Compiling test program..." -ForegroundColor Yellow

$dllPath = Resolve-Path "md2visio\bin\Release\net8.0\md2visio.dll"

csc /reference:"$dllPath" temp_mxgraph_test.cs 2>&1

if ($LASTEXITCODE -eq 0) {
    Write-Host "Running conversion..." -ForegroundColor Green
    .\temp_mxgraph_test.exe

    # Cleanup
    Remove-Item temp_mxgraph_test.cs -ErrorAction SilentlyContinue
    Remove-Item temp_mxgraph_test.exe -ErrorAction SilentlyContinue
} else {
    Write-Host "Compilation failed. Check errors above." -ForegroundColor Red
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Expected results:" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "- Ellipse shapes (blue): 开始访问网站, 离开网站" -ForegroundColor White
Write-Host "- Rounded rectangles (yellow): 浏览首页内容, 寻找需要的信息, 继续浏览其他页面" -ForegroundColor White
Write-Host "- Diamond (pink): 是否找到需要的内容？" -ForegroundColor White
Write-Host "- Rounded rectangle (green): 执行目标行动" -ForegroundColor White
Write-Host "- All connected with arrows" -ForegroundColor White
Write-Host ""
pause
