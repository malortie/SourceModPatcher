setlocal enableextensions enabledelayedexpansion
prompt $
echo off
cls

echo Package path: %~1

copy ..\setup_files\LICENSE.txt "%~1\LICENSE.txt"
copy ..\setup_files\README.txt "%~1\README.txt"

endlocal
