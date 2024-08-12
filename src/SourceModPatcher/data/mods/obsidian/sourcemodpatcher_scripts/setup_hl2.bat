setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{hl2_gamedir}}" (
  call makelinks_maps_only.bat "..\add-ons\%_addonprefix%_hl2_gamedir" "${{hl2_gamedir}}"
)

if exist "${{hl2_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_hl2_maps" "${{hl2_maps_content_path}}"
)

if exist "${{source2007_hl2_patches_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_hl2_patches" "${{source2007_hl2_patches_path}}"
)

endlocal