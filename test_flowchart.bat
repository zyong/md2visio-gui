@echo off
echo ========================================
echo Testing Flowchart with Colors
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
    echo Build failed!
    pause
    exit /b 1
)

echo.
echo ========================================
echo Build successful! Running flowchart test...
echo ========================================
echo.
echo Testing with flowchart (with colors and styles)
echo Input: test_flowchart.md
echo Output: test_flowchart_output.vsdx
echo.
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I test_flowchart.md /O test_flowchart_output.vsdx /Y /V

echo.
echo ========================================
echo Test completed!
echo ========================================
echo.
echo Expected results:
echo - Nodes with different colors:
echo   * Purple background for start/end nodes (:::startend)
echo   * Blue background for process nodes (:::process)
echo   * Yellow background for decision nodes (:::decision)
echo - Colored borders matching the fill colors
echo - Stadium shapes for start/end
echo - Rectangles for processes
echo - Diamonds for decisions
echo.
echo Output file: test_flowchart_output.vsdx
echo.
pause
