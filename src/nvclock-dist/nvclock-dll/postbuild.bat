set config=%1

set target_paths=..\win_nvclock\%config%

for %%i in (%target_paths%) DO if EXIST %%i copy /y %config%\nvclock.dll %%i\nvclock.dll
