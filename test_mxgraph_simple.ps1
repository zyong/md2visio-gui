# Simple PowerShell test for mxGraph conversion
# This avoids batch file encoding issues

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "mxGraph XML to Visio Conversion Test" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Close Visio
Write-Host "Closing Visio processes..." -ForegroundColor Yellow
Get-Process VISIO -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Build project
Write-Host "Building project..." -ForegroundColor Yellow
dotnet build md2visio/md2visio.csproj --configuration ConsoleRelease

if ($LASTEXITCODE -ne 0) {
    Write-Host "ConsoleRelease build failed! Trying Release..." -ForegroundColor Yellow
    dotnet build md2visio/md2visio.csproj --configuration Release
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Build failed!" -ForegroundColor Red
        pause
        exit 1
    }
}

Write-Host ""
Write-Host "Detecting DLL path..." -ForegroundColor Yellow
$dllPath = ""
if (Test-Path "md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.dll") {
    $dllPath = Resolve-Path "md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.dll"
    Write-Host "Found: ConsoleRelease (win-x64)" -ForegroundColor Green
} elseif (Test-Path "md2visio\bin\Release\net8.0\md2visio.dll") {
    $dllPath = Resolve-Path "md2visio\bin\Release\net8.0\md2visio.dll"
    Write-Host "Found: Release" -ForegroundColor Green
} else {
    Write-Host "ERROR: md2visio.dll not found!" -ForegroundColor Red
    Write-Host "Expected locations:" -ForegroundColor Yellow
    Write-Host "  - md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.dll"
    Write-Host "  - md2visio\bin\Release\net8.0\md2visio.dll"
    pause
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Creating test program..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan

# Create inline C# test code
$code = @"
using System;
using System.IO;
using md2visio.mxgraph;

class MxGraphTest
{
    static void Main()
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
            Environment.Exit(1);
        }
    }
}
"@

# Save test program
$code | Out-File -FilePath "test_mxgraph_program.cs" -Encoding UTF8
Write-Host "Test program created" -ForegroundColor Green

Write-Host ""
Write-Host "Compiling test program..." -ForegroundColor Yellow
csc /reference:"$dllPath" test_mxgraph_program.cs 2>&1 | Out-Null

if ($LASTEXITCODE -eq 0) {
    Write-Host "Compilation successful!" -ForegroundColor Green
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Running conversion..." -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""

    .\test_mxgraph_program.exe

    Write-Host ""
    Write-Host "Cleaning up..." -ForegroundColor Yellow
    Remove-Item test_mxgraph_program.cs -ErrorAction SilentlyContinue
    Remove-Item test_mxgraph_program.exe -ErrorAction SilentlyContinue
} else {
    Write-Host "CSC compilation failed!" -ForegroundColor Red
    Write-Host "Please ensure CSC is in your PATH or use convert_mxgraph.ps1" -ForegroundColor Yellow
    Remove-Item test_mxgraph_program.cs -ErrorAction SilentlyContinue
    exit 1
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Test Completed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Expected results in test_mxgraph_output.vsdx:" -ForegroundColor White
Write-Host "- 开始访问网站 (ellipse, blue)" -ForegroundColor White
Write-Host "- 浏览首页内容 (rounded rect, yellow)" -ForegroundColor White
Write-Host "- 寻找需要的信息 (rounded rect, yellow)" -ForegroundColor White
Write-Host "- 是否找到需要的内容？ (diamond, pink)" -ForegroundColor White
Write-Host "- 继续浏览其他页面 (rounded rect, yellow)" -ForegroundColor White
Write-Host "- 执行目标行动 (rounded rect, green)" -ForegroundColor White
Write-Host "- 离开网站 (ellipse, blue)" -ForegroundColor White
Write-Host "- All connected with arrows" -ForegroundColor White
Write-Host ""
pause
