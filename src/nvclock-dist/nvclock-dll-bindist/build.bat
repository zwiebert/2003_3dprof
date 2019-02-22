@ECHO OFF

call buildcfg.bat %1 %2 %3 %4 %5

rem subdirectories
set dll_dir=%dist_dir%
set inc_dir=%dist_dir%\include

rem Utilites
set create_zip=c:\bin\7zan a -tzip -bd -r
set extract_zip=c:\bin\7zan x -tzip -bd

rem create empty dist directory
if EXIST %dist_dir% del /s/q %dist_dir%
mkdir %dist_dir%


mkdir %dll_dir%
mkdir %inc_dir%

copy /b ..\nvclock-dll\%config%\nvclock.dll %dll_dir%
copy /b ..\nvclock-dll\%config%\nvclock-dll.lib %dll_dir%
copy /b ..\nvclock-dll\nvidia_hardware.h %inc_dir%
copy /b ..\WinIO\WinIO-DLL\%config%\WinIO.dll %dll_dir%
copy /b ..\WinIO\WinIO-Sys\i386\WinIO.sys %dll_dir%

copy /b ..\win_nvclock\%config%\test_nvclock_dll.exe %dll_dir%
copy /b ..\win_nvclock\log_clocks*.bat %dll_dir%

mkdir %config%
del "%target_file%"
%create_zip% "%target_file%" nvcdll-bin\*

if not exist G:\home\bertw\proj\www\tn\html\proj\nvclock_w32\download goto end_www
 copy /b "%target_file%" G:\home\bertw\proj\www\tn\html\proj\nvclock_w32\download
 set PATH=%PATH%;e:\cygwin\bin;e:\cygwin\usr\local\bin
 sitecopy -u tn
:end_www

