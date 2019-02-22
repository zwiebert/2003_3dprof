; ############################################################################
; $Id: close.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; r6probe_close function
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
; $Log: close.asm,v $
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
                .list
                .nocref

; ############################################################################
.code
                ; --------------------------------------
                ;  Name: r6probe_close
                ;  Desc: Closes connection to r6probe
                ; --------------------------------------

r6probe_close   proc c
                public r6probe_close

                mov     eax, r6probe_hProbe
                cmp     eax, INVALID_HANDLE_VALUE
                jz      @f
                push    eax
                call    CloseHandle
@@:
                ret
r6probe_close   endp

; ############################################################################

                endfile
