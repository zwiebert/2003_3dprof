; ############################################################################
; $Id: setmtim.asm,v 1.1.1.1 2002/01/25 22:14:25 VSi Exp $
;
; setclk_setmtim function
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
; $Log: setmtim.asm,v $
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
.const
timings         dd  MEMTIM_SLOW, MEMTIM_MEDIUM, MEMTIM_FAST

; ############################################################################
.code
                ; -----------------------------------------
                ;  Name: setclk_setmtim
                ;  Desc: Set RADEON memory timings
                ; -----------------------------------------

asm_setclk_setmtim  proc c  timidx: dword
                public  asm_setclk_setmtim

                mov     eax, timidx
                cmp     eax, lengthof timings
                jae     @end

                mov     eax, timings[ eax*4 ]
                invoke  r6probe_writemmr, RADEON_MEM_TIMINGS, MEMTIM_MASK, eax
@end:
                ret
asm_setclk_setmtim  endp

; ############################################################################

                endfile
