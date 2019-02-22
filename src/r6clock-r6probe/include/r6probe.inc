; ############################################################################
; $Id: r6probe.inc,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; Include file for R6PROBE library
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
; $Log: r6probe.inc,v $
; Revision 1.1.1.1  2002/01/25 22:14:25  VSi
; Imported to CVS
;
; ----------------------------------------------------------------------------
;                    Copyright (C) 2001-2002 Vahur Sinij„rv
; ############################################################################

EXTERNDEF           r6probe_hProbe: dword

r6probe_open        PROTO C
r6probe_close       PROTO C
r6probe_readbios    PROTO C _addr: dword
r6probe_readmmr     PROTO C _addr: dword
r6probe_readpll     PROTO C _addr: dword
r6probe_writemmr    PROTO C _addr: dword, _mask: dword, _data: dword
r6probe_writepll    PROTO C _addr: dword, _mask: dword, _data: dword
r6probe_readbiosblk PROTO C buf: dword, _addr: dword, _count: dword
r6probe_getversion  PROTO C

r6probe_close_      MACRO
                    mov     eax, r6probe_hProbe
                    cmp     eax, INVALID_HANDLE_VALUE
                    jz      @f
                    push    eax
                    call    CloseHandle
@@:
                    ENDM
