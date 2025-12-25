@echo off
cd /d "%~dp0"
echo Running sequence diagram conversion test...
echo.

set MD2VISIO_EXE=md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe
set INPUT_FILE=test_sequence.md
set OUTPUT_FILE=test_sequence_output.vsdx

if not exist "%MD2VISIO_EXE%" (
    echo Error: md2visio.exe not found at %MD2VISIO_EXE%
    echo Please build the project first with: dotnet build md2visio/md2visio.csproj --configuration ConsoleRelease
    pause
    exit /b 1
)

if not exist "%INPUT_FILE%" (
    echo Error: Input file %INPUT_FILE% not found
    pause
    exit /b 1
)

echo Input: %INPUT_FILE%
echo Output: %OUTPUT_FILE%
echo.

"%MD2VISIO_EXE%" /I "%INPUT_FILE%" /O "%OUTPUT_FILE%" /Y /V /D

echo.
if exist "%OUTPUT_FILE%" (
    echo SUCCESS: Visio file generated at %OUTPUT_FILE%
) else (
    echo ERROR: Failed to generate Visio file
)

pause
