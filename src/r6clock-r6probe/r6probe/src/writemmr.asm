; ############################################################################
; $Id: writemmr.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; r6probe_writemmr function
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
; $Log: writemmr.asm,v $
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
                include windows.inc
                include kernel32.inc
                include r6probe.inc
                include r6probe_ioc.inc
                .list
                .nocref

; ############################################################################

                ; --------------------------------------
                ;  Publics
                ; --------------------------------------
                public r6probe_writemmr

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: r6probe_writemmr
                ;  Desc: Writes to memory-mapped reg
                ; --------------------------------------

r6probe_writemmr PROC   C _addr: dword, _mask: dword, _data: dword

                local   dwRead: dword

                xor     edx, edx
                push    edx
                lea     eax, dwRead
                push    eax
                push    edx
                push    edx
                push    sizeof MASKED_WRITE_PARAMS
                lea     eax, _addr
                push    eax
                push    IOCTL_R6_WRITE_MMR
                push    r6probe_hProbe
                call    DeviceIoControl
                ret
r6probe_writemmr ENDP

; ############################################################################

                endfile
