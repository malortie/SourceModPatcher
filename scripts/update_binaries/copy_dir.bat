setlocal enableextensions enabledelayedexpansion

xcopy /F /I /S /Y /exclude:%~dp0excluded_files.txt "%~1" "%~2"

endlocal
