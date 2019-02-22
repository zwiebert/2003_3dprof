set config=%1

set target_paths=..\..\bin\%config%\etc

for %%i in (%target_paths%) DO if EXIST %%i copy /y %config%\nvclock.exe %%i\nvclock.exe

