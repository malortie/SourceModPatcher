setlocal enableextensions enabledelayedexpansion
prompt $
echo off
cls

set version=0.4.0

call build.bat %version% win-x64
call build.bat %version% win-x86

endlocal
