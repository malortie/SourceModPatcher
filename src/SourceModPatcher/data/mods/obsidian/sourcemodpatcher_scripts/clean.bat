setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

rem Clean all sourcemodpatcher_ addon folders.
pushd "..\add-ons"
  for /D %%d in (%_addonprefix%_*) do (
    echo Deleting %%d
    rd /S /Q "%%d"
  )
popd

endlocal