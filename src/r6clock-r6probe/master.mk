##############################################################################
# $Id: master.mk,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
#
# Configuration for make.
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
# $Log: master.mk,v $
# Revision 1.1.1.1  2002/01/25 22:14:25  VSi
# Imported to CVS
#
# ----------------------------------------------------------------------------
#                    Copyright (C) 2001-2002 Vahur Sinij„rv
##############################################################################

MSVCDIR = j:\Programme\Microsoft Visual Studio .NET 2003\Vc7
PSDK    = j:\Programme\Microsoft Visual Studio .NET 2003\Vc7\PlatformSDK
NTDDK   = d:\dev\winddk
DDKDIR  = d:\dev\98ddk
MASM32  = d:\masm32
DELPHI  = d:\borland\delphi4\bin

AS      = ml
AFLAGS  = -coff -DBLD_COFF

!IFDEF DEBUG
AFLAGS = $(AFLAGS) -Zd -Zi
!ENDIF

CC      = cl
CFLAGS  = -c -Ox -Gf -nologo

MAKE    = nmake
MKFLAGS = /nologo /c

LIB     = lib

LD      = link

RC      = rc

DCC     = $(DELPHI)\dcc32
