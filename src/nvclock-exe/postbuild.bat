set config=%1

set target_paths=..\..\bin\%config%\etc

for %%i in (%target_paths%) DO if EXIST %%i copy /y %config%\nvclock.exe %%i\nvclock.exe


set WinIO=..\WinIO

copy /b %WinIO%\WinIO-DLL\%config%\WinIO.dll .\%config%\WinIO.dll
copy /b %WinIO%\WinIO-Sys\I386\WinIO.sys .\%config%\WinIO.sys
