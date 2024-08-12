setlocal enableextensions enabledelayedexpansion
prompt $
@echo off
cls

set _addonprefix=sourcemodpatcher

if exist "${{source2007_ep2_patches_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_ep2_patches" "${{source2007_ep2_patches_path}}"
)

if exist "${{source2007_episodic_patches_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_episodic_patches" "${{source2007_episodic_patches_path}}"
)

if exist "${{source2007_hl2_patches_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_hl2_patches" "${{source2007_hl2_patches_path}}"
)

if exist "${{source2007_hl2mp_patches_path}}" (
  call makelinks.bat "..\add-ons\%_addonprefix%_source2007_hl2mp_patches" "${{source2007_hl2mp_patches_path}}"
)

endlocal