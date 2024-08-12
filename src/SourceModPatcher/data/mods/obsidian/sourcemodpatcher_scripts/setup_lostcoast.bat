setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{lostcoast_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_lostcoast" "${{lostcoast_content_path}}"
)

if exist "${{lostcoast_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_lostcoast_gamedir" "${{lostcoast_gamedir}}"
)

if exist "${{lostcoast_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_lostcoast_maps" "${{lostcoast_maps_content_path}}"
)

endlocal