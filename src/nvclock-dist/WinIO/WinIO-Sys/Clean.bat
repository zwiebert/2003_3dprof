del /q I386\*.* OBJ\I386\*.*

set config=%1
shift

rem the called batch just sets target_paths to directories we need to copy output to
call ..\copy_output.bat %config%

rem remove  files in all projects which are using this drivers
for %%i in (%target_paths%) DO if EXIST %%i\WinIO.sys del %%i\WinIO.sys
