set config=%1

exit

set target_paths=..\..\bin\%config% ..\clocker\%config% examples\C\%config%

for %%i in (%target_paths%) DO if EXIST %%i copy /y %config%\r6clock.dll %%i\r6clock.dll
for %%i in (%target_paths%) DO if EXIST %%i copy /y %config%\r6clock-dll.pdb %%i\r6clock-dll.pdb
