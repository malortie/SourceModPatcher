setlocal enableextensions enabledelayedexpansion
prompt $
echo off
cls

echo Content path: %~1
echo Archive path: %~2

pushd "%~1"
zip -r "..\%~2" .
popd

endlocal
