; ############################################################################
; $Id: getmlat.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; setclk_getmlat function
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
; $Log: getmlat.asm,v $
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
.code
                ; -----------------------------------------
                ;  Name: setclk_getmlat
                ;  Desc: Get RADEON memory latency timers
                ; -----------------------------------------

asm_setclk_getmlat  proc c
                public  asm_setclk_getmlat

                invoke  r6probe_readmmr, RADEON_MEM_INIT_LAT_TIMER
                mov     ecx, eax
                xor     eax, eax
                cmp     ecx, LATENCY_FAST
                jz      @end
                inc     eax
                cmp     ecx, LATENCY_MEDIUM
                jz      @end
                mov     eax, ecx
@end:
                ret
asm_setclk_getmlat  endp

; ############################################################################

                endfile
