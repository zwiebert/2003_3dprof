##############################################################################
# $Id: makefile,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
#
# MAKEFILE for dclkinfo sample
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

!include ..\..\..\master.mk

TGTNAME = dclkinfo
TARGET  = $(TGTNAME).exe
DPFLAGS = -CC -D- -I- -L- -Y-

all:    $(TARGET)

$(TGTNAME).pas:

$(TARGET): $(TGTNAME).pas
        $(DCC) $(TGTNAME).pas $(DPFLAGS) -U..\..\..\lib

clean:
        del $(TARGET)