##############################################################################
# $Id: makefile,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
#
# Main MAKEFILE
#
# ----------------------------------------------------------------------------
# LICENSE
#
# This program is free software; you can redistribute it and/or modify it
# under the terms of the GNU General Public License (GPL) as published by the
# Free Software Foundation; either version 2 of the License, or (at your
# option) any later version.
#
# This program is distributed in the hope that it will be useful, but WITHOUT
# ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
# FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
# more details.
#
# To read the license please visit http://www.gnu.org/copyleft/gpl.html
# ----------------------------------------------------------------------------
# $Log: makefile,v $
# Revision 1.1.1.1  2002/01/25 22:14:25  VSi
# Imported to CVS
#
# ----------------------------------------------------------------------------
#                    Copyright (C) 2001-2002 Vahur Sinij„rv
##############################################################################

!include master.mk

all: r6probe.lib setclk.lib radeon_setclk.exe

r6probe.lib:
    cd r6probe
    $(MAKE) $(MKFLAGS)
	cd ..
setclk.lib:
    cd setclk
    $(MAKE) $(MKFLAGS)
	cd ..

r6probe.sys : FORCE
    cd r6probe\sys
    $(MAKE) /A $(MKFLAGS)
    copy obj\r6probe.sys ..\..
    cd ..\..

r6probe.vxd : FORCE
    cd r6probe\vxd
    $(MAKE) /A $(MKFLAGS)
    copy obj\r6probe.vxd ..\..
    cd ..\..

FORCE :

clean_r6probe.sys :
    cd r6probe\sys
    $(MAKE) $(MKFLAGS) clean
    cd ..\..

clean_r6probe.vxd :
    cd r6probe\vxd
    $(MAKE) $(MKFLAGS) clean
    cd ..\..

radeon_setclk.exe: setclk.lib
	copy setclk\samples\setclk\obj\setclk.exe .\radeon_setclk.exe
	copy r6probe\sys\obj\r6probe.sys .
	copy radeon_setclk.exe ..\bin\Debug\etc\.
	copy radeon_setclk.exe ..\bin\Release\etc\.

clean:
    cd r6probe
    $(MAKE) $(MKFLAGS) clean
    cd ..
    cd setclk
    $(MAKE) $(MKFLAGS) clean
    cd ..
