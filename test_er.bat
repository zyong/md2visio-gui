@echo off
echo ========================================
echo Testing ER Diagram Conversion
echo ========================================
echo.
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
echo - Entity relationships with cardinality
echo - Primary keys highlighted
echo - Proper ER diagram layout
echo.
echo Output file: test_er_output.vsdx
echo.
pause
