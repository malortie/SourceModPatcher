setlocal enableextensions enabledelayedexpansion
prompt $
echo off
cls

set version=0.3.1

call build.bat %version% win-x64
call build.bat %version% win-x86

endlocal
