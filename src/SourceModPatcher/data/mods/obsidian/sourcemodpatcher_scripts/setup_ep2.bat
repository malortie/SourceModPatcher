setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{source2007_ep2_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_ep2" "${{source2007_ep2_content_path}}"
)

if exist "${{source2007_ep2_maps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_ep2_maps" "${{source2007_ep2_maps_content_path}}"
)

if exist "${{source2007_ep2_patches_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_ep2_patches" "${{source2007_ep2_patches_path}}"
)

endlocal