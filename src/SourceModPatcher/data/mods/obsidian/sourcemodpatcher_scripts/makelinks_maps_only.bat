setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonfolder=%~1
set _targetfolder=%~2

echo Addon folder: %_addonfolder%
echo Target folder: %_targetfolder%

rem Create the sourcemodpatcher addon folder if it doesn't already exist.
if not exist "%_addonfolder%" mkdir "%_addonfolder%"

pushd "%_addonfolder%"
  rem Create an empty info.txt file for the addon.
  type nul >info.txt

  rem Create a junction for map folder in the content directory.
  mklink /j "maps" "%_targetfolder%\maps"
popd

endlocal