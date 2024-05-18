setlocal enableextensions enabledelayedexpansion
prompt $
echo off
cls

echo Artifacts path: %~1
echo Platform RID: %2

dotnet publish -r %2 -c Release --artifacts-path "%~1" --self-contained true /p:PublishSingleFile=false /p:DebugType=none /p:DebugSymbols=false ..\src\SourceModPatcher\SourceModPatcher.csproj

endlocal
