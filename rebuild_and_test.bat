@echo off
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
echo Build successful! Running test...
echo ========================================
echo.
echo Testing with complex sequence diagram (39 messages, 9 participants)
echo Input: test_complex_sequence.md
echo Output: test_sequence_output_complex_fixed.vsdx
echo.
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I test_complex_sequence.md /O test_sequence_output_complex_fixed.vsdx /Y

echo.
echo ========================================
echo Test completed!
echo ========================================
echo.
echo Expected results:
echo - 9 participants (no duplicates)
echo - 39 messages with arrows
echo - Dynamic lifeline length (approx 20 inches for 39 messages)
echo - All messages connected to lifelines
echo.
echo Output file: test_sequence_output_complex_fixed.vsdx
echo.
pause
