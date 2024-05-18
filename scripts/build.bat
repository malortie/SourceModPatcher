setlocal enableextensions enabledelayedexpansion
prompt $
echo off
cls

echo Version: %1
echo Platform RID: %2

set artifacts_path=..\artifacts
set archive_path=%artifacts_path%\publish\SourceModPatcher\release_%2

call publish.bat "%artifacts_path%" %2
call package.bat "%archive_path%"
call make_zip.bat "%archive_path%" "SourceModPatcher-v%1-%2.zip"

endlocal
