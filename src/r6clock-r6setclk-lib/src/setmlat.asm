; ############################################################################
; $Id: setmlat.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; setclk_setmlat function
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
; $Log: setmlat.asm,v $
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
                include r6reg.inc
                include r6probe.inc
                include setclk.inc

                .list
                .nocref

; ############################################################################
.const
latencies       dd  LATENCY_FAST, LATENCY_MEDIUM

; ############################################################################
.code
                ; -----------------------------------------
                ;  Name: setclk_setmlat
                ;  Desc: Set RADEON memory latency timers
                ; -----------------------------------------

asm_setclk_setmlat  proc c  latidx: dword
                public  asm_setclk_setmlat

                mov     eax, latidx
                cmp     eax, lengthof latencies
                jae     @end

                mov     eax, latencies[ eax*4 ]
                invoke  r6probe_writemmr, RADEON_MEM_INIT_LAT_TIMER, 0ffffffffh, eax
@end:
                ret
asm_setclk_setmlat  endp

; ############################################################################

                endfile
