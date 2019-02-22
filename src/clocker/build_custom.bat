set config=%1
set config2=%1

if %config2%==Debug-WinIO   set config2=Debug
if %config2%==Release-WinIO set config2=Release

@echo on
set R6Probe=..\r6probe
copy /y %R6Probe%\r6probe-sys\i386\r6probe.sys  .\%config%\r6probe.sys
copy /y %R6Probe%\r6probe-vxd\i386\r6probe.vxd  .\%config%\r6probe.vxd


set WinIO=..\WinIO
copy /b %WinIO%\WinIO-DLL\%config2%\WinIO.dll   .\%config%\WinIO.dll
copy /b %WinIO%\WinIO-DLL\%config2%\WinIO.dll   .\%config%\WinIO.pdb
copy /b %WinIO%\WinIO-Sys\I386\WinIO.sys        .\%config%\WinIO.sys


copy /b ..\r6clock-dll\%config%\r6clock.dll     .\%config%\r6clock.dll
copy /b ..\r6clock-dll\%config%\r6clock-dll.pdb .\%config%\r6clock-dll.pdb