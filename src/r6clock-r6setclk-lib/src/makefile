##############################################################################
# $Id: makefile,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
#
# MAKEFILE for setclk library.
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

!include ..\..\master.mk

TGTNAME = setclk
INCLUDE = -I$(MASM32)\include -I..\..\include
TARGET  = ..\..\lib\$(TGTNAME).lib
BLD     = obj

!ifdef DCU
TARGET  = ..\..\lib\$(TGTNAME).dcu
BLD     = obji
AFLAGS  = -DDELPHI
!endif

!ifdef DEBUG
AFLAGS  = -nologo -c -DIS_32 -DMASM6 -DDEBUG -H32 $(AFLAGS)
LIBFLAGS= -nologo
!else
AFLAGS  = -nologo -c -DIS_32 -DMASM6 -DSTD_CALL -DRETAIL -H32 $(AFLAGS)
LIBFLAGS= -nologo
!endif

##############################################################################
OBJ = $(BLD)\open.obj $(BLD)\setclk.obj $(BLD)\getclk.obj \
      $(BLD)\getmtim.obj $(BLD)\setmtim.obj $(BLD)\getmlat.obj \
      $(BLD)\setmlat.obj $(BLD)\getclkstep.obj $(BLD)\close.obj \
      $(BLD)\c_setclk.obj $(BLD)\c_getclk.obj $(BLD)\c_getclkstep.obj \
      $(BLD)\c_open.obj $(BLD)\c_close.obj \
      $(BLD)\c_getmlat.obj $(BLD)\c_setmlat.obj $(BLD)\c_getmtim.obj $(BLD)\c_setmtim.obj \
      $(BLD)\wrapper.obj
##############################################################################

.asm{$(BLD)}.obj:
        if not exist obj md obj
        $(AS) $(AFLAGS) $(INCLUDE) /Fo$@ $<

!ifdef DEBUG
CFLAGS = -c -nologo -Zi -DDEBUG=1
LFLAGS = -ignore:4060 -debug -opt:ref -subsystem:console -incremental:no -opt:nowin98 -libpath:$(PSDK)\lib -libpath:"$(MSVCDIR)\lib"
!endif

.c{obj}.obj:
    if not exist obj md obj
    $(CC) $(CFLAGS) $(INCLUDE) -Fo$@ $<


all: $(TARGET)

!ifdef DCU
$(TARGET): $(OBJ) $(TGTNAME).pas
        $(DCC) $(TGTNAME).pas -O$(BLD) -N..\..\lib -U..\..\lib
!else
$(TARGET): $(OBJ)
        $(LIB) $(LIBFLAGS) $(OBJ) -out:$(TARGET)
!endif

clean:
        del $(BLD)\*.obj
        del $(TARGET)
