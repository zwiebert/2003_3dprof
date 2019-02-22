set config=%1

set target_paths=..\..\bin\%config%\etc

rem for %%i in (%target_paths%) DO if EXIST %%i copy /y %config%\radeon_setclk.exe %%i
rem for %%i in (%target_paths%) DO if EXIST %%i copy /y ..\r6probe.sys %%i

