set config=%1

if not exist .\%config% mkdir .\%config%
rem copy /b ..\WinIo\Source\Dll\%config%\WinIO.DLL .\%config%\WinIO.DLL
rem copy /b ..\WinIo\Source\Drv\NT\I386\WinIO.sys .\%config%\WinIO.sys

