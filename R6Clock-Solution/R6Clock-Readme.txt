Requirements
============

Visual Studio .NET 2003 (this is a must)
Windows DDK (only if you want to modify the drivers)

Important: Keep this source distribution on your harddisk on a place
where the path is not too long and doesn't contain any
spaces. Otherwise some tools which build the VXD driver may refuse to
work.


Device Driver Projects
======================

The driver source contains prebuilt binaries, so you have not to
remake them.  Remaking the drivers is disabled automatically if there
is no path to the DDK given.  If you have the DDK on your machine
installed, then define an environment variable "WinDDK" containig the
path of the DDK directory (e.g. WinDDK=C:\DEV\WinDDK)

Configuration
=============

The Debug-WinIO and Release-WinIO configurations produce different
r6clock-dll output. The configuration differs in the use of the
WinIO.DLL backend instead of R6Probe.lib.

The WinIO.DLL backend configuration is using a more generic memory
mapping driver.


Advantages and Disadvantages of WinIO Configuration
---------------------------------------------------

+ No assembler code in driver.

+ The WinIO driver is a generic PCI card finder and memory mapper. So
most of the code is now in the library itself, which results in much
easier programming and debugging.

- Currently no working Win9x/ME driver. The winio-vxd project just
  provides some headers, but does not produce any binary output.

- The generic memory mapping driver may be not as safe as the r6probe
  driver, because it is not strictly restricted to mapping Radeon card
  registers like r6probe.



