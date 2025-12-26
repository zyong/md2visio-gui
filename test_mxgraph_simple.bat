@echo off
chcp 65001 >nul
setlocal

echo ========================================
echo Testing mxGraph XML to Visio Conversion
echo ========================================
echo.

echo Step 1: Closing Visio processes...
taskkill /F /IM VISIO.EXE 2>nul
if %ERRORLEVEL% EQU 0 (
    echo Visio closed successfully
    timeout /t 2 /nobreak >nul
) else (
    echo No Visio process found
)

echo.
echo Step 2: Building project...
dotnet build md2visio/md2visio.csproj --configuration Release --verbosity quiet

if %ERRORLEVEL% NEQ 0 (
    echo Build failed!
    pause
    exit /b 1
)
echo Build successful!

echo.
echo Step 3: Creating test C# program...
(
echo using System;
echo using System.IO;
echo using md2visio.mxgraph;
echo.
echo class TestProgram
echo {
echo     static void Main^(^)
echo     {
echo         try
echo         {
echo             string xmlContent = File.ReadAllText^("test_mxgraph.md"^);
echo             var converter = new MxGraphConverter^(^);
echo             Console.WriteLine^("Converting mxGraph XML to Visio..."^);
echo             converter.ConvertToVisio^(xmlContent, "test_mxgraph_output.vsdx"^);
echo             Console.WriteLine^("Conversion completed successfully!"^);
echo             Console.WriteLine^("Output file: test_mxgraph_output.vsdx"^);
echo         }
echo         catch ^(Exception ex^)
echo         {
echo             Console.WriteLine^("Error: " + ex.Message^);
echo             Console.WriteLine^(ex.StackTrace^);
echo         }
echo     }
echo }
) > TestMxGraph.cs

echo.
echo Step 4: Compiling test program...
csc /reference:md2visio\bin\Release\net8.0\md2visio.dll /reference:"C:\Windows\Microsoft.NET\assembly\GAC_MSIL\Microsoft.Office.Interop.Visio\v4.0_15.0.0.0__71e9bce111e9429c\Microsoft.Office.Interop.Visio.dll" TestMxGraph.cs 2>nul

if %ERRORLEVEL% NEQ 0 (
    echo Compilation failed. Trying alternative method...
    echo Using dotnet script instead...

    REM Create a dotnet script
    echo using System; > run.csx
    echo using System.IO; >> run.csx
    echo using md2visio.mxgraph; >> run.csx
    echo string xmlContent = File.ReadAllText("test_mxgraph.md"); >> run.csx
    echo var converter = new MxGraphConverter(); >> run.csx
    echo converter.ConvertToVisio(xmlContent, "test_mxgraph_output.vsdx"); >> run.csx
    echo Console.WriteLine("Done!"); >> run.csx

    dotnet script run.csx
) else (
    echo Compilation successful!
    echo.
    echo Step 5: Running test program...
    TestMxGraph.exe

    REM Cleanup
    del TestMxGraph.cs 2>nul
    del TestMxGraph.exe 2>nul
)

echo.
echo ========================================
echo Test Completed!
echo ========================================
echo.
echo Expected results in test_mxgraph_output.vsdx:
echo - Ellipse shapes (blue): Start and End nodes
echo - Rounded rectangles (yellow): Process steps
echo - Diamond (pink): Decision node
echo - Green rectangle: Success action
echo - All shapes with correct colors from XML
echo.
pause
