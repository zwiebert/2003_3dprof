set config=%1
shift
set ddk=%1
shift
set TargetDir=%1
shift

echo build.bat: Using ddk directory "%ddk%"
echo build.bat: Using target directory "%TargetDir%"

call %ddk%\bin\setenv.bat %ddk%

rem change back drive, if setenv.bat has changed it
for %%i in (%TargetDir%) do %%~di
rem change back to old dir
if NOT EXIST "%TargetDir%" mkdir "%TargetDir%"
cd "%TargetDir%"
cd ..

mkdir objfre\I386
nmake

call ..\copy_output.bat %config%

rem copy  files to all projects which are using this drivers
for %%i in (%target_paths%) DO if EXIST %%i copy /y i386\WinIO.sys %%i\WinIO.sys
pause
