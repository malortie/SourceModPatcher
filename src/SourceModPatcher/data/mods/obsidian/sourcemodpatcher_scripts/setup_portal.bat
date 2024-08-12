setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{portal_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_portal" "${{portal_content_path}}"
)

if exist "${{portal_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_portal_gamedir" "${{portal_gamedir}}"
)

if exist "${{portal_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_portal_maps" "${{portal_maps_content_path}}"
)

endlocal