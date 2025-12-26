# Direct mxGraph Test using dotnet run
# This avoids CSC compilation issues

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
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Running conversion using MxGraphConverter..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Create a simple C# project for testing
$tempProject = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="md2visio\md2visio.csproj" />
  </ItemGroup>
</Project>
"@

$tempCode = @"
using System;
using System.IO;
using md2visio.mxgraph;

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("Reading test_mxgraph.md...");
            string xmlContent = File.ReadAllText("test_mxgraph.md");

            Console.WriteLine("Creating MxGraphConverter...");
            var converter = new MxGraphConverter();

            Console.WriteLine("Converting mxGraph XML to Visio...");
            converter.ConvertToVisio(xmlContent, "test_mxgraph_output.vsdx");

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✓ Conversion completed successfully!");
            Console.ResetColor();
            Console.WriteLine("Output file: test_mxgraph_output.vsdx");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("✗ Error: " + ex.Message);
            Console.ResetColor();
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }
}
"@

# Create temporary project
New-Item -ItemType Directory -Path "temp_mxgraph_test" -Force | Out-Null
$tempCode | Out-File -FilePath "temp_mxgraph_test\Program.cs" -Encoding UTF8
$tempProject | Out-File -FilePath "temp_mxgraph_test\temp_test.csproj" -Encoding UTF8

Write-Host "Created temporary test project" -ForegroundColor Green
Write-Host ""

# Run the test
Write-Host "Executing conversion..." -ForegroundColor Yellow
Write-Host ""

dotnet run --project temp_mxgraph_test\temp_test.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Test Completed Successfully!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Cyan
} else {
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Test Failed!" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Cyan
}

# Cleanup
Write-Host ""
Write-Host "Cleaning up temporary files..." -ForegroundColor Yellow
Remove-Item -Path "temp_mxgraph_test" -Recurse -Force -ErrorAction SilentlyContinue

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

if (Test-Path "test_mxgraph_output.vsdx") {
    Write-Host "✓ Output file created successfully!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Opening in Visio..." -ForegroundColor Yellow
    Start-Process "test_mxgraph_output.vsdx" -ErrorAction SilentlyContinue
} else {
    Write-Host "✗ Output file not found. Check errors above." -ForegroundColor Red
}

Write-Host ""
pause
