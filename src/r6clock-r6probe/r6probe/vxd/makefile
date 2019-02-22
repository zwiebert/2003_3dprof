##############################################################################
# $Id: makefile,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
#
# MAKEFILE for Windows 9x/ME version of r6probe
#
# Use this file to tune make process for your system. Several paths must be
# set up for compilation to succeed.
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

PATH    = $(DDKDIR)\bin;$(PATH)
AS      = ml
LD      = link
#RC      = $(DDKDIR)\bin\rc16
# -bw/11-Jul-03: rc16.exe is not the correct name from the original DDK
RC      = $(DDKDIR)\bin\win98\bin16\rc
TGTNAME = r6probe
SRCDIR  = \source\radeon\r6probe\vxd
INCLUDE = -I.. -I..\..\include -I$(DDKDIR)\inc\win98 -I$(DDKDIR)\inc\win98\inc16 
TARGET  = obj\$(TGTNAME).vxd
DEFFILE = $(TGTNAME).def
RESFILE = obj\$(TGTNAME).res
STUB    = ..\..\stub\stub.exe

!ifdef DEBUG
AFLAGS  = -nologo -c -coff -DIS_32 -DMASM6 -DDEBUG -DBLD_COFF
LFLAGS  = -vxd -nodefaultlib  -mapinfo:lines -map
RCFLAGS = -r
!else
AFLAGS  = -nologo -c -coff -DIS_32 -DMASM6 -DSTD_CALL -DBLD_COFF -DRETAIL
LFLAGS  = -nodefaultlib
LFLAGS  = $(LFLAGS) -ignore:4039,4060,4078 -stub:$(STUB) -optidata -nologo
LFLAGS  = $(LFLAGS) -incremental:no -fullbuild -vxd -map
RCFLAGS = -r
!endif

##############################################################################
OBJ = obj\main.obj obj\ioctl.obj
##############################################################################

#obj\ioctl.obj: ioctl.asm ..\..\include\r6.inc ..\..\include\r6reg.inc ..\..\include\r6probe_ioc.inc
#        if not exist obj md obj
#        $(AS) $(AFLAGS) $(INCLUDE) /Fo$@ ioctl.asm $<

#obj\main.obj: main.asm ..\..\include\r6.inc ..\..\include\r6reg.inc
#        if not exist obj md obj
#        $(AS) $(AFLAGS) $(INCLUDE) /Fo$@ main.asm $<


.asm{obj}.obj:
        if not exist obj md obj
        $(AS) $(AFLAGS) $(INCLUDE) /Fo$@ $<


.rc{obj}.res:
        $(RC) $(RCFLAGS) $(INCLUDE) -Fo$@ $<

all:    $(TARGET) $(NMSFILE)

$(TARGET): $(OBJ) $(DEFFILE) $(RESFILE)
        $(LD) $(LFLAGS) $(OBJ) -def:$(DEFFILE) -out:$(TARGET)
        $(DDKDIR)\bin\win98\adrc2vxd $(TARGET) $(RESFILE)

clean:
        del obj\*.obj
        del obj\*.lib
        del obj\*.exp
        del obj\*.map
        del obj\*.res
        del obj\*.sym
        del $(TARGET)
