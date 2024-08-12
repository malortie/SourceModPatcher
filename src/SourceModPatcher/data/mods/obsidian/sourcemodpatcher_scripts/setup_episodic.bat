setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{source2007_episodic_maps_bsps_content_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_episodic_maps_bsps" "${{source2007_episodic_maps_bsps_content_path}}"
)

endlocal