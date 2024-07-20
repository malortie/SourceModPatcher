setlocal enableextensions enabledelayedexpansion
prompt $
echo off
cls

echo SOURCE_SDK_2004_REPO_PATH: %SOURCE_SDK_2004_REPO_PATH%
echo SOURCE_SDK_2006_REPO_PATH: %SOURCE_SDK_2006_REPO_PATH%
echo SOURCE_SDK_2007_REPO_PATH: %SOURCE_SDK_2007_REPO_PATH%

set _binaries_path=%~dp0\..\..\src\SourceContentInstaller\data\binaries
echo Binaries destination path: %_binaries_path%

call %~dp0copy_dir.bat "%SOURCE_SDK_2004_REPO_PATH%\game\hl2\bin" "%_binaries_path%\source2004\hl2\bin"

call %~dp0copy_dir.bat "%SOURCE_SDK_2006_REPO_PATH%\game\episodic\bin" "%_binaries_path%\source2006\episodic\bin"
call %~dp0copy_dir.bat "%SOURCE_SDK_2006_REPO_PATH%\game\hl2\bin" "%_binaries_path%\source2006\hl2\bin"
call %~dp0copy_dir.bat "%SOURCE_SDK_2006_REPO_PATH%\game\hl2mp\bin" "%_binaries_path%\source2006\hl2mp\bin"

call %~dp0copy_dir.bat "%SOURCE_SDK_2007_REPO_PATH%\game\ep2\bin" "%_binaries_path%\source2007\ep2\bin"
call %~dp0copy_dir.bat "%SOURCE_SDK_2007_REPO_PATH%\game\episodic\bin" "%_binaries_path%\source2007\episodic\bin"
call %~dp0copy_dir.bat "%SOURCE_SDK_2007_REPO_PATH%\game\hl2\bin" "%_binaries_path%\source2007\hl2\bin"
call %~dp0copy_dir.bat "%SOURCE_SDK_2007_REPO_PATH%\game\hl2mp\bin" "%_binaries_path%\source2007\hl2mp\bin"

endlocal
