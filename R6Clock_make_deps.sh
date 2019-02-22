#! /bin/sh

## project directory
pd=$1
if [ "X$pd" = "X" ]; then pd="."; fi

solution_file=R6Clock-Solution/R6Clock.sln

. ./MakeDepsFunctions.sh


rm -f temp_rules.txt


## R6Probe Drivers
extract_vc r6_sys     ${pd}/r6probe/r6probe-sys      r6probe-sys_DDK.vcproj    ${pd}/r6probe/r6probe-sys/i386/r6probe.sys
extract_vc r6_vxd     ${pd}/r6probe/r6probe-vxd      r6probe-vxd_DDK.vcproj    ${pd}/r6probe/r6probe-vxd/i386/r6probe.vxd
## R6Probe Static Library
extract_vc r6_lib     ${pd}/r6probe-lib              r6probe-lib.vcproj        ${pd}/r6probe-lib/Release/r6probe.lib
## R6Clock Static Library, DLL, commandline clocker, GUI clocker
extract_vc r6clk_lib  ${pd}/r6clock-r6setclk-lib setclk-lib.vcproj     ${pd}/r6clock-r6setclk-lib/Release/setclk.lib
extract_vc r6clk_dll  ${pd}/r6clock-dll          r6clock-dll.vcproj    ${pd}/r6clock-dll/Release/r6clock.dll '$(bin_r6_lib) $(bin_r6clk_lib)'
extract_vc r6clk_exe  ${pd}/r6clock-r6setclk-exe setclk.vcproj         ${pd}/r6clock-r6setclk-exe/Release/radeon_setclk.exe '$(bin_r6_lib) $(bin_r6clk_lib)'
extract_vc clocker    ${pd}/clocker              clocker.vcproj        ${pd}/clocker/Release/clocker.exe  '$(bin_r6clock-dll)'

extract_vc r6clock-dll-example-C  ${pd}/r6clock-dll/examples/C r6clock-dll-example-C.vcproj   ${pd}/r6clock-dll/examples/C/Release/r6clock-dll-example-C.exe  '$(bin_r6clock-dll)'

extract_vd r6clock-dll-bin ${pd}/r6clock-dll-bin r6clock-dll-bin.vdproj   ${pd}/r6clock-dll-bin/Release/r6clock-dll-bin.CAB  '$(bin_r6_lib) $(bin_r6clk_lib)'

extract_vd r6probe-sys-install ${pd}/R6Probe-Sys-Installer R6ProbeInstall.vdproj  ${pd}/R6Probe-Sys-Installer/Release/R6ProbeInstall.msi

## WinIO
extract_vc wio_dll  ${pd}/WinIO/WinIO-DLL  WinIO.vcproj      ${pd}/WinIO/WinIO-DLL/Release/WinIO.dll  
extract_vc wio_sys  ${pd}/WinIO/WinIO-Sys  WinIO-Sys.vcproj  ${pd}/WinIO/WinIO-Sys/i386/WinIO.sys
extract_vc wio_vxd  ${pd}/WinIO/WinIO-Vxd  WinIO-Vxd.vcproj  ${pd}/WinIO/WinIO-Vxd/i386/WinIO.vxd


