; ############################################################################
; $Id: getver.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; r6probe_getversion function
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
; $Log: getver.asm,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################
                ; --------------------------------------
                ;  Includes
                ; --------------------------------------
                .xlist
                include macros.inc
                include kernel32.inc
                include r6probe.inc
                include r6probe_ioc.inc
                .list
                .nocref

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: r6probe_getversion
                ;  Desc: Returns r6probe version
                ; --------------------------------------

r6probe_getversion proc c
                public  r6probe_getversion

                local   dwRead: dword
                local   tmp: dword

                xor     eax, eax
                mov     tmp, eax
                push    eax
                lea     edx, dwRead
                push    edx
                push    sizeof dword
                lea     edx, tmp
                push    edx
                push    eax
                push    eax
                push    IOCTL_R6_GET_VERSION
                push    r6probe_hProbe
                call    DeviceIoControl
                mov     eax, tmp
                ret

r6probe_getversion endp

; ############################################################################

                endfile