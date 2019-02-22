set config=%1

@echo on

set proj=..\..

set WinIO=%proj%\WinIO

if not exist etc mkdir etc

copy %WinIO%\WinIO-DLL\%config%\WinIO.dll                   .
copy %WinIO%\WinIO-DLL\%config%\WinIO.dll                   .\etc

copy %WinIO%\WinIO-Sys\I386\WinIO.sys                       .
copy %WinIO%\WinIO-Sys\I386\WinIO.sys                       .\etc

copy %proj%\r6probe\r6probe-sys\i386\r6probe.sys            .\etc
copy %proj%\r6probe\r6probe-sys\i386\r6probe.sys            .
copy %proj%\r6probe\r6probe-vxd\i386\r6probe.vxd            .\etc
copy %proj%\r6clock-dll\%config%\r6clock.dll                .\etc
copy %proj%\r6clock-dll\%config%\r6clock.dll                .
copy %proj%\r6clock-r6setclk-exe\%config%\radeon_setclk.exe .\etc

copy %proj%\nvclock-exe\%config%\nvclock.exe                .\etc
copy %proj%\nvclock-dll\%config%\nvclock.dll                .\etc
copy %proj%\nvclock-log\%config%\nvclock-log.exe            .\etc

copy %proj%\TDProfGD\%config%\TDProfGD.exe                  .\etc
copy %proj%\ReplaceIcon\%config%\ReplaceIcon.exe            .\etc
copy %proj%\free_ram\%config%\free_ram.exe                  .\etc
copy %proj%\detect_3d_api\%config%\detect_3d_api.exe        .\etc


if NOT EXIST specfiles mkdir specfiles 
if NOT EXIST usr-specfiles mkdir usr-specfiles
copy %proj%\TDProf\specfiles\*.cfg                           specfiles
copy %proj%\TDProf\specfiles\README.txt                      specfiles
copy %proj%\TDProf\usr-specfiles\README.txt                  usr-specfiles
copy %proj%\TDProf\tdprof.exe.manifest                       .




