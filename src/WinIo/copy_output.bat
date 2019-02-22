rem set directories where this project copies it ouput to at build time and removes it at cleaning time
set target_paths=..\WinIO-Test\%config% ..\..\..\radeon_setclk\clocker\%config%
 
echo Using target directories:  %target_paths%