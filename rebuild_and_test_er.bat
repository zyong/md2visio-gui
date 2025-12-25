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
echo Build successful! Running ER diagram test...
echo ========================================
echo.
echo Testing with ER diagram
echo Input: test_er.md
echo Output: test_er_output.vsdx
echo.
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I test_er.md /O test_er_output.vsdx /Y

echo.
echo ========================================
echo Test completed!
echo ========================================
echo.
echo Expected results:
echo - Multiple entities with attributes
echo - UML dynamic connectors between entities
echo - Primary keys highlighted (yellow + key icon)
echo - Proper spacing between entities
echo.
echo Output file: test_er_output.vsdx
echo.
pause
