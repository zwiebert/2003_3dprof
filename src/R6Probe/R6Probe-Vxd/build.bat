set config=%1
shift
rem unqote parameter
for %%i in (%1) do set ddk=%%~i
shift
set TargetDir=%1
shift

if "x%ddk%" == "x" set ddk=%WinDDK%

set target=i386\r6probe.vxd

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


rem set ddk=d:\dev\98ddk


echo build.bat: Using ddk directory "%ddk%"
echo build.bat: Using target directory "%TargetDir%"

set PATH=;%ddk%\bin\Win98\bin16;%ddk%\bin\Win_ME\bin16;%ddk%\bin\Win98;%ddk%\bin\Win_ME\bin;%PATH%

call %ddk%\bin\setenv.bat %ddk%

rem change back drive, if setenv.bat has changed it
for %%i in (%TargetDir%) do %%~di
rem change back to old dir
if NOT EXIST "%TargetDir%" mkdir "%TargetDir%"
cd "%TargetDir%"
cd ..


echo ### DDK's build.exe produces assembler errors about vmm.inc and coff options of ML. Do manually build for now ### 

rem Call DDK's build utility
rem build.exe


set objdir=objfre\i386
if not exist "%objdir%" mkdir "%objdir%"

@echo on
 ml -nologo -c -coff -DIS_32 -DMASM6 -DSTD_CALL -DBLD_COFF -DRETAIL -I.. -I..\include -I"%ddk%\inc\win98" -I"%ddk%\inc\win98\inc16" -I"%ddk%\inc\win_me" -I"%ddk%\inc\win_me\inc16" /Fo%objdir%\main.obj i386\main.asm
 ml -nologo -c -coff -DIS_32 -DMASM6 -DSTD_CALL -DBLD_COFF -DRETAIL -I.. -I..\include -I"%ddk%\inc\win98" -I"%ddk%\inc\win98\inc16" -I"%ddk%\inc\win_me" -I"%ddk%\inc\win_me\inc16" /Fo%objdir%\ioctl.obj i386\ioctl.asm

if exist %ddk%\bin\win98\bin16\rc.exe (
%ddk%\bin\win98\bin16\rc.exe -r -I.. -I..\include -I"%ddk%\inc\win98" -I"%ddk%\inc\win98\inc16" -Fo%objdir%\r6probe.res r6probe.rc
) else (
%ddk%\bin\win_me\bin16\rc.exe -r -I"%ddk%\inc\win_me" -I"%ddk%\inc\win_me\inc16" -Fo%objdir%\r6probe.res r6probe.rc
)

link -nodefaultlib -ignore:4039,4060,4078 -stub:stub\stub.exe -optidata -nologo -incremental:no -fullbuild -vxd -map %objdir%\main.obj %objdir%\ioctl.obj -def:r6probe.def -out:i386\r6probe.vxd
adrc2vxd i386\r6probe.vxd %objdir%\r6probe.res
 



