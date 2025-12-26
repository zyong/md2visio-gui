# Convert test_mxgraph_fixed.md to Visio
Write-Host "Converting test_mxgraph_fixed.md to Visio..." -ForegroundColor Cyan
Write-Host ""

# Close Visio
Write-Host "Closing existing Visio processes..." -ForegroundColor Yellow
Get-Process VISIO -ErrorAction SilentlyContinue | Stop-Process -Force
Start-Sleep -Seconds 1

# Delete old output
$outputPath = "test_mxgraph_output.vsdx"
if (Test-Path $outputPath) {
    Remove-Item $outputPath -Force
    Write-Host "Deleted old output file" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Creating temporary C# program..." -ForegroundColor Yellow

# Create a temporary C# program
$programCode = @'
using System;
using System.IO;
using md2visio.mxgraph;

class Program
{
    static void Main()
    {
        try
        {
            string xmlContent = File.ReadAllText("test_mxgraph_fixed.md");
            var converter = new MxGraphConverter();
            converter.ConvertToVisio(xmlContent, "test_mxgraph_output.vsdx");
            Console.WriteLine("✓ Conversion completed successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
'@

$programCode | Out-File -FilePath "temp_convert.cs" -Encoding UTF8

Write-Host "Compiling and running..." -ForegroundColor Yellow
Write-Host ""

# Find the md2visio.dll path
$dllPath = "md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.dll"
if (-not (Test-Path $dllPath)) {
    $dllPath = "md2visio\bin\Release\net8.0\md2visio.dll"
}

if (Test-Path $dllPath) {
    Write-Host "Found DLL at: $dllPath" -ForegroundColor Green

    # Try to compile with CSC
    $cscPath = "C:\Program Files\Microsoft Visual Studio\2022\Community\MSBuild\Current\Bin\Roslyn\csc.exe"
    if (-not (Test-Path $cscPath)) {
        $cscPath = (Get-Command csc -ErrorAction SilentlyContinue).Source
    }

    if ($cscPath) {
        Write-Host "Compiling with CSC..." -ForegroundColor Yellow
        & $cscPath /reference:"$dllPath" /out:temp_convert.exe temp_convert.cs

        if (Test-Path "temp_convert.exe") {
            Write-Host "Running conversion..." -ForegroundColor Yellow
            Write-Host ""
            & .\temp_convert.exe

            # Cleanup
            Remove-Item temp_convert.cs -ErrorAction SilentlyContinue
            Remove-Item temp_convert.exe -ErrorAction SilentlyContinue
        }
    } else {
        Write-Host "CSC not found, using dotnet run instead..." -ForegroundColor Yellow

        # Create a temp project
        $tempDir = "temp_mxgraph_convert"
        if (Test-Path $tempDir) {
            Remove-Item $tempDir -Recurse -Force
        }
        New-Item -ItemType Directory -Path $tempDir | Out-Null

        # Create project file
        @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\md2visio\md2visio.csproj" />
  </ItemGroup>
</Project>
"@ | Out-File "$tempDir\temp.csproj" -Encoding UTF8

        # Copy program
        Copy-Item temp_convert.cs "$tempDir\Program.cs"

        # Run
        dotnet run --project "$tempDir\temp.csproj" --configuration Release

        # Cleanup
        Remove-Item $tempDir -Recurse -Force -ErrorAction SilentlyContinue
        Remove-Item temp_convert.cs -ErrorAction SilentlyContinue
    }
} else {
    Write-Host "✗ ERROR: Could not find md2visio.dll" -ForegroundColor Red
    Write-Host "Please build the project first with:" -ForegroundColor Yellow
    Write-Host "  dotnet build md2visio/md2visio.csproj --configuration ConsoleRelease" -ForegroundColor White
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan

# Check output
if (Test-Path $outputPath) {
    $fileInfo = Get-Item $outputPath
    Write-Host "✓ SUCCESS: Output file created ($([math]::Round($fileInfo.Length/1KB, 2)) KB)" -ForegroundColor Green
    Write-Host ""
    Write-Host "Opening in Visio..." -ForegroundColor Yellow
    Start-Process $outputPath
} else {
    Write-Host "✗ ERROR: Output file not found at $outputPath" -ForegroundColor Red
}

Write-Host ""
pause
