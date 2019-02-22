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
rem mkdir %inc_dir%

copy /b ..\nvclock\%config%\nvclock.exe %dll_dir%
copy /b ..\WinIO\WinIO-DLL\%config%\WinIO.dll %dll_dir%
copy /b ..\WinIO\WinIO-Sys\i386\WinIO.sys %dll_dir%

mkdir %config%
del "%target_file%"
%create_zip% "%target_file%" %dist_dir%\*

