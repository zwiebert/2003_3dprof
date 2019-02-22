; ############################################################################
; $Id: kernel32.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Quick and dirty declarations for kernel32.dll to stay compatible with
; Delphi.
;
; ----------------------------------------------------------------------------
; LICENSE
;
; This program is free software; you can redistribute it and/or modify it
; under the terms of the GNU General Public License (GPL) as published by the
; Free Software Foundation; either version 2 of the License, or (at your
; option) any later version.
;
; This program is distributed in the hope that it will be useful, but WITHOUT
; ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
; FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License for
; more details.
;
; To read the license please visit http://www.gnu.org/copyleft/gpl.html
; ----------------------------------------------------------------------------
; $Log: kernel32.inc,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################
ifdef DELPHI

extern pascal DeviceIoControl: near
extern pascal CreateFile: near
extern pascal CloseHandle: near

else

extern stdcall DeviceIoControl@32: near
DeviceIoControl equ DeviceIoControl@32

extern stdcall CreateFileA@28: near
CreateFile equ CreateFileA@28

extern stdcall CloseHandle@4: near
CloseHandle equ CloseHandle@4

endif