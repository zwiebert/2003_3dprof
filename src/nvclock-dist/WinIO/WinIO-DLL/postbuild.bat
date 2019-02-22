set config=%1
shift


call ..\copy_output.bat %config%

rem copy  files to all projects which are using this drivers
for %%i in (%target_paths%) DO if EXIST %%i copy /y %config%\WinIO.dll %%i\WinIO.dll
pause
