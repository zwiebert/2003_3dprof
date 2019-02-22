#! /bin/sh

## project directory
pd=$1
if [ "X$pd" = "X" ]; then pd="."; fi

solution_file=NVClockW32-Solution/NVClockW32.sln

. ./MakeDepsFunctions.sh



#extract_vc sources ${pd}/nvclock-bindist         ${pd}/nvclock-bindist.vcproj
#extract_vc sources ${pd}/nvlock-dll-example      ${pd}/nvclock-dll-example.vcproj



## NVClock
extract_vc nvc_exe         ${pd}/nvclock-exe         nvclock.vcproj             ${pd}/nvclock-exe/Release/nvclock.exe  '$(bin_nvc_back)'
extract_vc nvc_log         ${pd}/nvclock-log         nvclock-log.vcproj         ${pd}/nvclock-log/Release/nvclock-log.exe
extract_vc nvc_dll         ${pd}/nvclock-dll         nvclock-dll.vcproj         ${pd}/nvclock-dll/Release/nvclock.dll
#extract_vc nvc_dll_bindist ${pd}/nvclock-dll-bindist nvclock-dll-bindist.vcproj ${pd}/nvclock-dll-bindist/Release/nvclock-dll-dist.zip
extract_vc nvc_back        ${pd}/nvclock-backend     backend.vcproj             ${pd}/nvclock-backend/Release/backend.lib
extract_vc nvc_dll_example ${pd}/nvclock-dll-example nvclock-dll-example.vcproj ${pd}/nvclock-dll-example/Release/nvclock.dll

## WinIO
extract_vc wio_dll  ${pd}/WinIO/WinIO-DLL  WinIO.vcproj      ${pd}/WinIO/WinIO-DLL/Release/WinIO.dll  
extract_vc wio_sys  ${pd}/WinIO/WinIO-Sys  WinIO-Sys.vcproj  ${pd}/WinIO/WinIO-Sys/i386/WinIO.sys
extract_vc wio_vxd  ${pd}/WinIO/WinIO-Vxd  WinIO-Vxd.vcproj  ${pd}/WinIO/WinIO-Vxd/WinIO.vxd
extract_vc wio_test ${pd}/WinIO/WinIO-Test WinIOTest.vcproj  ${pd}/WinIO/WinIO-Test/Release/WinIoTest.exe
