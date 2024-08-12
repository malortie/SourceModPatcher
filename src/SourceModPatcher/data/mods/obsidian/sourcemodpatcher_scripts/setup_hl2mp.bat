setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{hl2mp_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_hl2mp_gamedir" "${{hl2mp_gamedir}}"
)

if exist "${{hl2mp_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_hl2mp_maps" "${{hl2mp_maps_content_path}}"
)

endlocal