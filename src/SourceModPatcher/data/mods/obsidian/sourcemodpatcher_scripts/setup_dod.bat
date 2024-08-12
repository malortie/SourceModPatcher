setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{dod_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_dod" "${{dod_content_path}}"
)

if exist "${{dod_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_dod_gamedir" "${{dod_gamedir}}"
)

if exist "${{dod_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_dod_maps" "${{dod_maps_content_path}}"
)

endlocal