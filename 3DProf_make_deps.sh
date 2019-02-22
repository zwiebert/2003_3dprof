#! /bin/sh

## project directory
pd=$1
if [ "X$pd" = "X" ]; then pd="."; fi

solution_file="./3DProf-Solution/3DProf.sln"

. ./MakeDepsFunctions.sh


## TDProf + libraries
extract_cs tdprof_exe      ${pd}/TDProf         TDProf.csproj        ${pd}/bin/${config}/TDProf.exe
extract_cs menu_img_lib    ${pd}/MenuImageLib   MenuImageLib.csproj  ${pd}/MenuImageLib/bin/${config}/MenuImageLib.dll


## TDProf's helper apps
extract_vc free_ram   ${pd}/free_ram      free_ram.vcproj        ${pd}/free_ram/${config}/free_ram.exe
extract_vc det_3dapi  ${pd}/detect_3d_api detect_3d_api.vcproj   ${pd}/detect_3d_api/${config}/detect_3d_api.exe
extract_vc tdprof_gd  ${pd}/TDProfGD      TDProfGD.vcproj        ${pd}/TDProfGD/${config}/TDProfGD.exe
extract_vc repl_icon  ${pd}/ReplaceIcon   ReplaceIcon.vcproj     ${pd}/ReplaceIcon/${config}/ReplaceIcon.exe


## NVClock
extract_vc nvc_exe         ${pd}/nvclock-exe         nvclock.vcproj             ${pd}/nvclock-exe/${config}/nvclock.exe  '$(bin_nvc_back)'
extract_vc nvc_log         ${pd}/nvclock-log         nvclock-log.vcproj         ${pd}/nvclock-log/${config}/nvclock-log.exe
extract_vc nvc_dll         ${pd}/nvclock-dll         nvclock-dll.vcproj         ${pd}/nvclock-dll/${config}/nvclock.dll
extract_vc nvc_dll_bindist ${pd}/nvclock-dll-bindist nvclock-dll-bindist.vcproj ${pd}/nvclock-dll-bindist/${config}/nvclock-dll-dist.zip
extract_vc nvc_back        ${pd}/nvclock-backend     backend.vcproj             ${pd}/nvclock-backend/${config}/backend.lib


## WinIO
extract_vc wio_dll  ${pd}/WinIO/WinIO-DLL  WinIO.vcproj      ${pd}/WinIO/WinIO-DLL/${config}/WinIO.dll  
extract_vc wio_sys  ${pd}/WinIO/WinIO-Sys  WinIO-Sys.vcproj  ${pd}/WinIO/WinIO-Sys/i386/WinIO.sys
extract_vc wio_vxd  ${pd}/WinIO/WinIO-Vxd  WinIO-Vxd.vcproj  ${pd}/WinIO/WinIO-Vxd/WinIO.vxd


## Installer
extract_vd 3dp_msi  ${pd}/3DProf-Bininstaller       BinInstaller.vdproj          ${pd}/3DProf-Bininstaller/${config}/3DProf.msi  '$(sources) $(binaries) version.txt'
extract_vd r6p_msi  ${pd}/R6Probe-Sys-Installer     R6ProbeInstall.vdproj        ${pd}/R6Probe-Sys-Installer/${config}/R6ProbeInstall.msi


## R6Probe Drivers
extract_vc r6_sys     ${pd}/r6probe/r6probe-sys      r6probe-sys_DDK.vcproj    ${pd}/r6probe/r6probe-sys/i386/r6probe.sys
extract_vc r6_vxd     ${pd}/r6probe/r6probe-vxd      r6probe-vxd_DDK.vcproj    ${pd}/r6probe/r6probe-vxd/i386/r6probe.vxd
## R6Probe Static Library
extract_vc r6_lib     ${pd}/r6probe-lib              r6probe-lib.vcproj        ${pd}/r6probe-lib/${config}/r6probe.lib
## R6Clock Static Library, DLL, commandline clocker, GUI clocker
extract_vc r6clk_lib  ${pd}/r6clock-r6setclk-lib setclk-lib.vcproj     ${pd}/r6clock-r6setclk-lib/${config}/setclk.lib
extract_vc r6clk_exe  ${pd}/r6clock-r6setclk-exe setclk.vcproj         ${pd}/r6clock-r6setclk-exe/${config}/radeon_setclk.exe '$(bin_r6_lib) $(bin_r6clk_lib)'

config="Release-WinIO"
extract_vc r6clk_dll  ${pd}/r6clock-dll          r6clock-dll.vcproj    ${pd}/r6clock-dll/${config}/r6clock.dll
extract_vc clocker    ${pd}/clocker              clocker.vcproj        ${pd}/clocker/${config}/clocker.exe
