setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{hl1_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_hl1" "${{hl1_content_path}}"
)

if exist "${{hl1_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_hl1_gamedir" "${{hl1_gamedir}}"
)

if exist "${{hl1_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_hl1_maps" "${{hl1_maps_content_path}}"
)

endlocal