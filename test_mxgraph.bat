@echo off
chcp 65001 >nul
REM ^ Set UTF-8 encoding for Chinese characters

echo ========================================
echo Testing mxGraph XML to Visio Conversion
echo ========================================
echo.

echo Closing Visio processes...
taskkill /F /IM VISIO.EXE 2>nul
if %ERRORLEVEL% EQU 0 (
    echo Visio closed successfully
    timeout /t 2 /nobreak >nul
) else (
    echo No Visio process found
)

echo.
echo Building project...
dotnet build md2visio/md2visio.csproj --configuration ConsoleRelease

if %ERRORLEVEL% NEQ 0 (
    echo ConsoleRelease build failed! Trying Release configuration...
    dotnet build md2visio/md2visio.csproj --configuration Release
    if %ERRORLEVEL% NEQ 0 (
        echo Both builds failed!
        pause
        exit /b 1
    )
)

echo.
echo Detecting DLL path...
if exist "md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.dll" (
    set "DLL_PATH=md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.dll"
    echo Found: ConsoleRelease ^(win-x64^)
) else if exist "md2visio\bin\Release\net8.0\md2visio.dll" (
    set "DLL_PATH=md2visio\bin\Release\net8.0\md2visio.dll"
    echo Found: Release
) else (
    echo ERROR: md2visio.dll not found in expected locations!
    echo Please build the project first.
    pause
    exit /b 1
)

echo.
echo ========================================
echo Creating test program...
echo ========================================
echo.
REM Note: Special characters must be escaped with ^
(
echo using System;
echo using System.IO;
echo using md2visio.mxgraph;
echo.
echo class MxGraphTest
echo {
echo     static void Main^(^)
echo     {
echo         try
echo         {
echo             string xmlContent = File.ReadAllText^("test_mxgraph.md"^);
echo             var converter = new MxGraphConverter^(^);
echo             Console.WriteLine^("Converting mxGraph XML to Visio..."^);
echo             converter.ConvertToVisio^(xmlContent, "test_mxgraph_output.vsdx"^);
echo             Console.WriteLine^("✓ Conversion completed successfully!"^);
echo             Console.WriteLine^("Output file: test_mxgraph_output.vsdx"^);
echo         }
echo         catch ^(Exception ex^)
echo         {
echo             Console.WriteLine^("✗ Error: " + ex.Message^);
echo             Console.WriteLine^(ex.StackTrace^);
echo         }
echo     }
echo }
) > test_mxgraph_program.cs

if not exist test_mxgraph_program.cs (
    echo ERROR: Failed to create test program!
    pause
    exit /b 1
)

echo Test program created successfully
echo.
echo Compiling test program...
csc /reference:"%DLL_PATH%" test_mxgraph_program.cs 2>nul

if %ERRORLEVEL% EQU 0 (
    echo Compilation successful!
    echo.
    echo ========================================
    echo Running conversion...
    echo ========================================
    echo.
    test_mxgraph_program.exe

    echo.
    echo Cleaning up temporary files...
    del test_mxgraph_program.cs 2>nul
    del test_mxgraph_program.exe 2>nul
) else (
    echo CSC compilation failed. Trying direct dotnet run...
    echo.
    echo Note: You may need to use the TestMxGraph class in the console app.
    echo Alternative: Use convert_mxgraph.ps1 for reliable testing
    echo.

    REM Cleanup
    del test_mxgraph_program.cs 2>nul

    echo ========================================
    echo Fallback method not available in this version
    echo ========================================
    echo.
    echo Please use one of these alternatives:
    echo 1. Run: convert_mxgraph.ps1 ^(Recommended^)
    echo 2. Ensure CSC is in your PATH
    echo 3. Add TestMxGraph support to console app
    pause
    exit /b 1
)

echo.
echo ========================================
echo Test Completed!
echo ========================================
echo.
echo Expected results in test_mxgraph_output.vsdx:
echo - 开始访问网站 ^(ellipse, blue^)
echo - 浏览首页内容 ^(rounded rect, yellow^)
echo - 寻找需要的信息 ^(rounded rect, yellow^)
echo - 是否找到需要的内容？ ^(diamond, pink^)
echo - 继续浏览其他页面 ^(rounded rect, yellow^)
echo - 执行目标行动 ^(rounded rect, green^)
echo - 离开网站 ^(ellipse, blue^)
echo - All connected with arrows
echo.
echo ========================================
echo Tip: For easier testing, use convert_mxgraph.ps1
echo ========================================
echo.
pause
