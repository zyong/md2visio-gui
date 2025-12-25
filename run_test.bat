@echo off
cd /d "%~dp0"
echo ===== Running Sequence Diagram Test =====
echo.
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I test_sequence.md /O test_sequence_output.vsdx /Y > test_output.log 2>&1
type test_output.log
echo.
echo ===== Test Complete =====
echo.
if exist test_sequence_output.vsdx (
    echo SUCCESS: File generated at test_sequence_output.vsdx
    dir test_sequence_output.vsdx
) else (
    echo ERROR: File was not generated
)
pause
