setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{cstrike_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_cstrike" "${{cstrike_content_path}}"
)

if exist "${{cstrike_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_cstrike_gamedir" "${{cstrike_gamedir}}"
)

if exist "${{cstrike_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_cstrike_maps" "${{cstrike_maps_content_path}}"
)

endlocal