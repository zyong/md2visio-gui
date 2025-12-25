@echo off
cd /d d:\Develop\md2visio-gui
echo Running md2visio for sequence diagram with lifelines...
md2visio\bin\ConsoleRelease\net8.0\win-x64\md2visio.exe /I test_sequence.md /O test_sequence_lifeline.vsdx /Y /V
echo Exit code: %ERRORLEVEL%
pause
