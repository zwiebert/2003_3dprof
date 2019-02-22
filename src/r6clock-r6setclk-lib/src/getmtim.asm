; ############################################################################
; $Id: getmtim.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; setclk_getmtim function
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
; $Log: getmtim.asm,v $
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
                include r6.inc
                include r6probe.inc
                include setclk.inc

                .list
                .nocref

; ############################################################################
.code
                ; -----------------------------------------
                ;  Name: r6probe_getmtim
                ;  Desc: Get RADEON memory timings
                ; -----------------------------------------

asm_setclk_getmtim  proc c
                public  asm_setclk_getmtim

                invoke  r6probe_readmmr, RADEON_MEM_TIMINGS
                mov     ecx, eax
                mov     edx, eax
                and     ecx, MEMTIM_MASK
                xor     eax, eax
                cmp     ecx, MEMTIM_SLOW and MEMTIM_MASK
                jz      @end
                inc     eax
                cmp     ecx, MEMTIM_MEDIUM and MEMTIM_MASK
                jz      @end
                inc     eax
                cmp     ecx, MEMTIM_FAST and MEMTIM_MASK
                jz      @end
                mov     eax, edx
@end:
                ret
asm_setclk_getmtim  endp

; ############################################################################

                endfile
