setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{hl1mp_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_hl1mp" "${{hl1mp_content_path}}"
)

if exist "${{hl1mp_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_hl1mp_gamedir" "${{hl1mp_gamedir}}"
)

if exist "${{hl1mp_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_hl1mp_maps" "${{hl1mp_maps_content_path}}"
)

endlocal