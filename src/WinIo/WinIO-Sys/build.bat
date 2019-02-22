set config=%1
shift
rem unqote parameter
for %%i in (%1) do set ddk=%%~i
shift
set TargetDir=%1
shift


set target=i386\WinIO.sys

if "x%ddk%" == "x" (
echo ## usage build.bat ConfigurationName WinDDK-Path TargetDirectory
echo ## You have not given the path to WinDDK as 2nd parameter."
echo ## Hint: It may help to create an environment variable WinDDK containing the path."

if exist "%target%" (
  echo ### Cannot rebuild "%target%", but a prebuild version exists and will be used ####
  exit 0
) 
exit 1
)


echo build.bat: Using ddk directory "%ddk%"
echo build.bat: Using target directory "%TargetDir%"

call %ddk%\bin\setenv.bat %ddk%

rem change back drive, if setenv.bat has changed it
for %%i in (%TargetDir%) do %%~di
rem change back to old dir
if NOT EXIST "%TargetDir%" mkdir "%TargetDir%"
cd "%TargetDir%"
cd ..

rem Call DDK's build utility
build.exe


